// File: Repositories/Implementations/OrderRepository.cs
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(SkstoreContext context) : base(context)
        {
        }

        public async Task<Order?> GetOrderDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.User) // Thêm User để lấy thông tin người đặt nếu cần ở service
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages) // Để lấy hình ảnh sản phẩm cho OrderItemDto
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId, bool includeItems = false)
        {
            var query = _dbSet.Where(o => o.UserId == userId);
            if (includeItems)
            {
                query = query.Include(o => o.OrderItems); // Chỉ include nếu cần thiết
            }
            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }
    }
}
