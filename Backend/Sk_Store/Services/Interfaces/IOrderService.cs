// File: Services/Interfaces/IOrderService.cs
using Application.DTOs.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces 
{
    public interface IOrderService
    {
        
        Task<(OrderDto? Order, string? ErrorMessage)> CreateOrderAsync(int userId, CreateOrderRequestDto orderRequestDto);

        Task<OrderDto?> GetOrderDetailsAsync(int orderId, int? userId = null); // userId là optional, nếu có thì kiểm tra quyền sở hữu
        Task<bool> UpdateOrderPaymentStatusAsync(int orderId, string newPaymentStatus);

        Task<IEnumerable<OrderSummaryDto>> GetOrdersForUserAsync(int userId, int pageNumber, int pageSize);
        Task<int> CountOrdersForUserAsync(int userId);


        Task<(IEnumerable<OrderDto> Orders, int TotalCount)> GetAllOrdersAsync(OrderFilterParametersDto filterParams);


        
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, int adminUserId);
    }
}
