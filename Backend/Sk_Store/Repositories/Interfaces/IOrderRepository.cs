using BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {

        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId, bool includeItems = false); // Thêm tham số includeItems

        /// <summary>
        /// Lấy chi tiết một đơn hàng, bao gồm OrderItems và thông tin Product liên quan.
        /// </summary>
        Task<Order?> GetOrderDetailsAsync(int orderId);
    }
}