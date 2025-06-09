// File: Services/Implementations/ShoppingCartService.cs
using Application.DTOs.ShoppingCart;
using BusinessObjects;
using Repositories.UnitOfWork;
using Services.Interfaces; // Hoặc Sk_Store.Services.Interfaces
using System;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.Extensions.Logging; // Nếu bạn dùng logger

namespace Services.Implementations // Hoặc Sk_Store.Services.Implementations
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly ILogger<ShoppingCartService> _logger;

        public ShoppingCartService(IUnitOfWork unitOfWork /*, ILogger<ShoppingCartService> logger*/)
        {
            _unitOfWork = unitOfWork;
            // _logger = logger;
        }

        private async Task<ShoppingCart> GetOrCreateCartEntityAsync(int userId)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CreationDate = DateTime.UtcNow
                };
                await _unitOfWork.ShoppingCarts.AddAsync(cart);
                // Quan trọng: Phải CompleteAsync ở đây để cart có CartId cho các thao tác sau
                // Tuy nhiên, để đảm bảo tính nhất quán, thường CompleteAsync sẽ được gọi ở cuối mỗi public method.
                // Trong trường hợp này, nếu cart mới được tạo, nó sẽ được lưu khi AddItem hoặc các thao tác khác gọi CompleteAsync.
                // Nếu không có thao tác nào khác sau đó, cart sẽ không được lưu.
                // -> Giải pháp tốt hơn: Nếu cart mới, lưu ngay để có ID.
                await _unitOfWork.CompleteAsync(); // Lưu để có CartId
                // Sau khi lưu, EF Core sẽ tự động gán CartId cho đối tượng 'cart'
            }
            // Đảm bảo rằng CartItems được nạp. GetCartByUserIdAsync đã include CartItems.
            // Nếu bạn lấy cart bằng cách khác, cần đảm bảo CartItems được load.
            // Ví dụ: await _context.Entry(cart).Collection(c => c.CartItems).LoadAsync();
            return cart;
        }

        private CartDto MapCartToDto(ShoppingCart cart)
        {
            if (cart == null) return new CartDto(); // Trả về giỏ hàng trống nếu cart entity là null

            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CreationDate = cart.CreationDate,
                LastUpdatedDate = cart.LastUpdatedDate,
                Items = cart.CartItems?.Select(ci => new CartItemDto
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.ProductName ?? "N/A", // Product cần được include
                    UnitPrice = ci.Product?.Price ?? 0,
                    Quantity = ci.Quantity,
                    ProductImageUrl = ci.Product?.ProductImages?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl,
                    StockQuantity = ci.Product?.StockQuantity ?? 0
                }).ToList() ?? new List<CartItemDto>()
            };
        }

        public async Task<CartDto?> GetCartAsync(int userId)
        {
            var cartEntity = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            // GetCartByUserIdAsync đã include CartItems và Product.
            if (cartEntity == null)
            {
                // Nếu không có giỏ hàng, trả về một DTO giỏ hàng trống thay vì null
                // để frontend dễ xử lý hơn, không cần kiểm tra null nhiều lần.
                return new CartDto { UserId = userId, Items = new List<CartItemDto>() };
            }
            return MapCartToDto(cartEntity);
        }

        public async Task<(CartDto? Cart, string? ErrorMessage)> AddItemToCartAsync(int userId, AddItemToCartDto itemDto)
        {
            var product = await _unitOfWork.Products.GetProductDetailByIdAsync(itemDto.ProductId); // GetProductDetailByIdAsync bao gồm cả Images
            if (product == null || !product.IsActive)
            {
                return (null, "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                return (null, $"Không đủ số lượng tồn kho cho sản phẩm '{product.ProductName}'. Chỉ còn {product.StockQuantity}.");
            }

            var cartEntity = await GetOrCreateCartEntityAsync(userId);

            var cartItem = cartEntity.CartItems.FirstOrDefault(ci => ci.ProductId == itemDto.ProductId);

            if (cartItem != null)
            {
                // Sản phẩm đã có trong giỏ, cập nhật số lượng
                if (product.StockQuantity < cartItem.Quantity + itemDto.Quantity)
                {
                    return (null, $"Không đủ số lượng tồn kho cho sản phẩm '{product.ProductName}' để thêm. Tổng số lượng yêu cầu ({cartItem.Quantity + itemDto.Quantity}) vượt quá số lượng tồn ({product.StockQuantity}).");
                }
                cartItem.Quantity += itemDto.Quantity;
            }
            else
            {
                // Sản phẩm chưa có trong giỏ, thêm mới
                cartItem = new CartItem
                {
                    CartId = cartEntity.CartId, // Quan trọng: Gán CartId
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    // Product sẽ được EF Core tự động liên kết nếu CartId và ProductId hợp lệ
                    // Hoặc bạn có thể gán: Product = product (nếu product được track bởi cùng context)
                };
                cartEntity.CartItems.Add(cartItem);
            }

            cartEntity.LastUpdatedDate = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();

            // Sau khi lưu, cần lấy lại cartEntity với các thông tin Product đã được include đầy đủ
            var updatedCartEntity = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            return (MapCartToDto(updatedCartEntity!), null);
        }

        public async Task<(CartDto? Cart, string? ErrorMessage)> UpdateItemQuantityAsync(int userId, int productId, UpdateCartItemQuantityDto quantityDto)
        {
            var cartEntity = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            if (cartEntity == null)
            {
                return (null, "Không tìm thấy giỏ hàng.");
            }

            var cartItem = cartEntity.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                return (null, "Sản phẩm không có trong giỏ hàng.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
            {
                // Nếu sản phẩm không còn tồn tại/active, có thể xóa khỏi giỏ
                cartEntity.CartItems.Remove(cartItem);
                await _unitOfWork.CompleteAsync();
                var updatedCartAfterRemoval = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
                return (MapCartToDto(updatedCartAfterRemoval!), "Sản phẩm đã bị xóa khỏi giỏ vì không còn tồn tại.");
            }

            if (product.StockQuantity < quantityDto.Quantity)
            {
                return (null, $"Không đủ số lượng tồn kho cho sản phẩm '{product.ProductName}'. Chỉ còn {product.StockQuantity}.");
            }

            cartItem.Quantity = quantityDto.Quantity;
            cartEntity.LastUpdatedDate = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();

            var updatedCartEntity = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            return (MapCartToDto(updatedCartEntity!), null);
        }

        public async Task<CartDto?> RemoveItemFromCartAsync(int userId, int productId)
        {
            var cartEntity = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            if (cartEntity == null)
            {
                // _logger?.LogWarning($"Cart not found for user ID {userId} when trying to remove item.");
                return null; // Hoặc trả về CartDto trống
            }

            var cartItem = cartEntity.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                // _logger?.LogInformation($"Item with ProductId {productId} not found in cart for user ID {userId}.");
                return MapCartToDto(cartEntity); // Trả về giỏ hàng hiện tại nếu item không có
            }

            cartEntity.CartItems.Remove(cartItem);
            // EF Core sẽ tự xử lý việc xóa CartItem khỏi CSDL khi CompleteAsync được gọi
            // nếu CartItem được load và track bởi context.
            // Để chắc chắn, bạn có thể gọi: _unitOfWork.Context.Set<CartItem>().Remove(cartItem);
            // nhưng với cách lấy cartEntity.CartItems.Remove(cartItem) và CompleteAsync là đủ.

            cartEntity.LastUpdatedDate = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();

            var updatedCartEntity = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            return MapCartToDto(updatedCartEntity!);
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cartEntity = await _unitOfWork.ShoppingCarts.GetCartByUserIdAsync(userId);
            if (cartEntity == null || !cartEntity.CartItems.Any())
            {
                return true; // Giỏ hàng không tồn tại hoặc đã trống
            }

            // Xóa tất cả CartItems liên quan đến CartId này
            // Cách 1: Lặp và xóa (EF Core sẽ theo dõi)
            var itemsToRemove = cartEntity.CartItems.ToList(); // Tạo bản copy để tránh lỗi khi sửa đổi collection đang duyệt
            foreach (var item in itemsToRemove)
            {
                cartEntity.CartItems.Remove(item);
                // Hoặc: _unitOfWork.Context.Set<CartItem>().Remove(item);
            }


            cartEntity.LastUpdatedDate = DateTime.UtcNow;
            // Không cần gọi _unitOfWork.ShoppingCarts.UpdateAsync(cartEntity) nếu nó được track.

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
