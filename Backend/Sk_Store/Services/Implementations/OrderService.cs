// File: Services/Implementations/OrderService.cs
using Application.DTOs.Order;
using BusinessObjects;
using Repositories.UnitOfWork;
using Services.Interfaces; // Hoặc Sk_Store.Services.Interfaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Cho ToListAsync và các phương thức EF khác

namespace Services.Implementations // Hoặc Sk_Store.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShoppingCartService _shoppingCartService; // Cần để xóa giỏ hàng

        public OrderService(IUnitOfWork unitOfWork, IShoppingCartService shoppingCartService)
        {
            _unitOfWork = unitOfWork;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<(OrderDto? Order, string? ErrorMessage)> CreateOrderAsync(int userId, CreateOrderRequestDto orderRequestDto)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return (null, "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm vào giỏ trước khi đặt hàng.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return (null, "Không tìm thấy thông tin người dùng."); // Lỗi không mong muốn
            }

            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Pending", // Trạng thái ban đầu
                ShippingAddress = orderRequestDto.ShippingAddress,
                RecipientName = orderRequestDto.RecipientName,
                RecipientPhoneNumber = orderRequestDto.RecipientPhoneNumber,
                PaymentMethod = orderRequestDto.PaymentMethod,
                PaymentStatus = "Unpaid", // Mặc định cho COD
                Notes = orderRequestDto.Notes,
                ShippingFee = 0 // Tạm thời phí ship là 0, có thể tính toán sau
            };

            decimal totalAmount = 0;

            foreach (var cartItem in cart.CartItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                if (product == null || !product.IsActive)
                {
                    return (null, $"Sản phẩm '{cartItem.Product?.ProductName ?? "ID: " + cartItem.ProductId}' không còn tồn tại hoặc đã ngừng kinh doanh.");
                }
                if (product.StockQuantity < cartItem.Quantity)
                {
                    return (null, $"Số lượng tồn kho của sản phẩm '{product.ProductName}' không đủ. Chỉ còn {product.StockQuantity}.");
                }

                var orderItem = new OrderItem
                {
                    // OrderId sẽ được EF gán khi newOrder được lưu
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price // Lấy giá hiện tại của sản phẩm
                };
                newOrder.OrderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.UnitPrice;

                // Giảm số lượng tồn kho
                product.StockQuantity -= cartItem.Quantity;
                // _unitOfWork.Products.UpdateAsync(product); // Không cần nếu product được track
            }

            newOrder.TotalAmount = totalAmount + (newOrder.ShippingFee ?? 0);

            try
            {
                await _unitOfWork.Orders.AddAsync(newOrder);
                // UnitOfWork.CompleteAsync() sẽ lưu tất cả thay đổi (Order, OrderItems, Product stock)
                var result = await _unitOfWork.CompleteAsync();
                if (result <= 0) // Kiểm tra xem có bản ghi nào được lưu không
                {
                    return (null, "Không thể tạo đơn hàng. Vui lòng thử lại.");
                }

                // Xóa giỏ hàng sau khi đặt hàng thành công
                await _shoppingCartService.ClearCartAsync(userId);

                // Lấy lại đơn hàng vừa tạo với đầy đủ thông tin để trả về
                var createdOrderDetails = await GetOrderDetailsAsync(newOrder.OrderId, userId);
                return (createdOrderDetails, null);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Lỗi khi tạo đơn hàng cho UserId: {UserId}", userId);
                return (null, $"Đã xảy ra lỗi trong quá trình tạo đơn hàng: {ex.Message}");
            }
        }

        public async Task<OrderDto?> GetOrderDetailsAsync(int orderId, int? userId = null)
        {
            var order = await _unitOfWork.Orders.GetOrderDetailsAsync(orderId); // GetOrderDetailsAsync cần include OrderItems và Product

            if (order == null) return null;

            // Nếu userId được cung cấp (nghĩa là người dùng đang xem), kiểm tra xem họ có phải chủ đơn hàng không
            if (userId.HasValue && order.UserId != userId.Value)
            {
                // Người dùng không phải chủ sở hữu, không cho xem (trừ khi là admin, nhưng logic đó sẽ ở controller)
                return null;
            }

            var user = await _unitOfWork.Users.GetByIdAsync(order.UserId);

            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                UserFullName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : null,
                UserEmail = user?.Email,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                RecipientName = order.RecipientName,
                RecipientPhoneNumber = order.RecipientPhoneNumber,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                DeliveryDate = order.DeliveryDate,
                ShippingFee = order.ShippingFee ?? 0,
                Notes = order.Notes,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.ProductName ?? "N/A", // Đảm bảo Product được include
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    ProductImageUrl = oi.Product?.ProductImages?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderSummaryDto>> GetOrdersForUserAsync(int userId, int pageNumber, int pageSize)
        {
            // Gọi repository với includeItems = true
            var ordersFromRepo = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId, true);

            return ordersFromRepo
                         .Skip((pageNumber - 1) * pageSize) // Phân trang sau khi đã lấy và sắp xếp
                         .Take(pageSize)
                         .Select(o => new OrderSummaryDto
                         {
                             OrderId = o.OrderId,
                             OrderDate = o.OrderDate,
                             OrderStatus = o.OrderStatus,
                             TotalAmount = o.TotalAmount,
                             TotalItems = o.OrderItems.Sum(oi => oi.Quantity)
                         }).ToList();
        }
        public async Task<int> CountOrdersForUserAsync(int userId)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o => o.UserId == userId);
            return orders.Count();
        }


        // Chức năng Admin
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize, string? statusFilter, string? sortOrder)
        {
            var query = _unitOfWork.Context.Orders
                                     .Include(o => o.OrderItems)
                                        .ThenInclude(oi => oi.Product)
                                     .Include(o => o.User) // Để lấy thông tin người dùng
                                     .AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.OrderStatus.ToLower() == statusFilter.ToLower());
            }

            // Sắp xếp
            switch (sortOrder?.ToLower())
            {
                case "date_asc":
                    query = query.OrderBy(o => o.OrderDate);
                    break;
                case "total_desc":
                    query = query.OrderByDescending(o => o.TotalAmount);
                    break;
                case "total_asc":
                    query = query.OrderBy(o => o.TotalAmount);
                    break;
                default: // date_desc (mặc định)
                    query = query.OrderByDescending(o => o.OrderDate);
                    break;
            }

            var orders = await query.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return orders.Select(order => new OrderDto // Ánh xạ thủ công hoặc dùng AutoMapper
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                UserFullName = order.User != null ? $"{order.User.FirstName} {order.User.LastName}".Trim() : "N/A",
                UserEmail = order.User?.Email,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                RecipientName = order.RecipientName,
                RecipientPhoneNumber = order.RecipientPhoneNumber,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                DeliveryDate = order.DeliveryDate,
                ShippingFee = order.ShippingFee ?? 0,
                Notes = order.Notes,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.ProductName ?? "N/A",
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    ProductImageUrl = oi.Product?.ProductImages?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
                }).ToList()
            }).ToList();
        }

        public async Task<int> CountAllOrdersAsync(string? statusFilter)
        {
            var query = _unitOfWork.Context.Orders.AsQueryable();
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.OrderStatus.ToLower() == statusFilter.ToLower());
            }
            return await query.CountAsync();
        }


        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, int adminUserId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                return false; // Đơn hàng không tồn tại
            }

            // Kiểm tra xem newStatus có hợp lệ không (ví dụ: không cho chuyển từ 'Delivered' về 'Pending')
            // Logic này có thể phức tạp tùy theo quy trình nghiệp vụ.
            // Ví dụ đơn giản:
            var validStatuses = new List<string> { "Pending", "Processing", "Shipped", "Delivered", "Cancelled", "Failed" };
            if (!validStatuses.Contains(newStatus))
            {
                return false; // Trạng thái mới không hợp lệ
            }

            // Nếu trạng thái là "Cancelled", cần xem xét việc hoàn lại số lượng sản phẩm vào kho
            if (newStatus == "Cancelled" && order.OrderStatus != "Cancelled") // Chỉ hoàn kho nếu trước đó chưa bị hủy
            {
                // Lấy lại các OrderItems của đơn hàng này (cần include)
                var orderToCancel = await _unitOfWork.Orders.GetOrderDetailsAsync(orderId);
                if (orderToCancel != null)
                {
                    foreach (var item in orderToCancel.OrderItems)
                    {
                        var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity += item.Quantity;
                        }
                    }
                }
            }


            order.OrderStatus = newStatus;
            // Có thể ghi log ai đã thay đổi trạng thái (adminUserId) nếu cần
            // order.LastUpdatedByAdminId = adminUserId;
            // order.LastUpdatedDate = DateTime.UtcNow;
            if (newStatus.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
            {
                // Chỉ cập nhật nếu DeliveryDate chưa được set, để tránh ghi đè nếu admin lỡ tay bấm lại.
                if (order.DeliveryDate == null)
                {
                    order.DeliveryDate = DateTime.UtcNow;
                }
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
