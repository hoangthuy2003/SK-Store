// File: Services/Interfaces/IOrderService.cs
using Application.DTOs.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces // Hoặc Sk_Store.Services.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Tạo một đơn hàng mới từ giỏ hàng của người dùng.
        /// </summary>
        /// <returns>OrderDto của đơn hàng vừa tạo, hoặc null nếu có lỗi.</returns>
        Task<(OrderDto? Order, string? ErrorMessage)> CreateOrderAsync(int userId, CreateOrderRequestDto orderRequestDto);

        /// <summary>
        /// Lấy thông tin chi tiết một đơn hàng theo ID (cho cả người dùng và admin).
        /// </summary>
        Task<OrderDto?> GetOrderDetailsAsync(int orderId, int? userId = null); // userId là optional, nếu có thì kiểm tra quyền sở hữu

        /// <summary>
        /// Lấy lịch sử đơn hàng của một người dùng (phân trang).
        /// </summary>
        Task<IEnumerable<OrderSummaryDto>> GetOrdersForUserAsync(int userId, int pageNumber, int pageSize);
        Task<int> CountOrdersForUserAsync(int userId);


        // Chức năng cho Admin
        /// <summary>
        /// Lấy tất cả đơn hàng (cho admin, có phân trang và lọc).
        /// </summary>
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize, string? statusFilter, string? sortOrder);
        Task<int> CountAllOrdersAsync(string? statusFilter);


        /// <summary>
        /// Cập nhật trạng thái của một đơn hàng (cho admin).
        /// </summary>
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, int adminUserId);
    }
}
