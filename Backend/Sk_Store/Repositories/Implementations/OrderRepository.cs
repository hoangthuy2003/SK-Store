using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return await _dbSet.Include(o=>o.OrderItems)
                .ThenInclude(o=>o.Product).
                FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _dbSet.Where(p => p.UserId == userId).ToListAsync();
        }
    }
}
