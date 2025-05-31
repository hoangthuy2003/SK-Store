using BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
      
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);

        Task<Order?> GetOrderDetailsAsync(int orderId);
    }
}