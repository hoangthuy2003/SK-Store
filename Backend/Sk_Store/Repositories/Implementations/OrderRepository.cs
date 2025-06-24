// File: Repositories/Implementations/OrderRepository.cs
using Application.DTOs.Order;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
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

        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedOrdersAsync(OrderFilterParametersDto filterParams)
        {
            var query = _dbSet.Include(o => o.User).AsNoTracking().AsQueryable();

            // Áp dụng bộ lọc
            if (!string.IsNullOrEmpty(filterParams.OrderStatus))
            {
                query = query.Where(o => o.OrderStatus == filterParams.OrderStatus);
            }
            if (!string.IsNullOrEmpty(filterParams.PaymentStatus))
            {
                query = query.Where(o => o.PaymentStatus == filterParams.PaymentStatus);
            }
            if (filterParams.FromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= filterParams.FromDate.Value);
            }
            if (filterParams.ToDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= filterParams.ToDate.Value.AddDays(1).AddTicks(-1)); // Đến cuối ngày
            }
            if (!string.IsNullOrEmpty(filterParams.SearchTerm))
            {
                var term = filterParams.SearchTerm.ToLower();
                query = query.Where(o => o.OrderId.ToString() == term ||
                                         o.User.FirstName.ToLower().Contains(term) ||
                                         o.User.LastName.ToLower().Contains(term) ||
                                         o.User.Email.ToLower().Contains(term));
            }

            // Đếm tổng số lượng trước khi phân trang
            var totalCount = await query.CountAsync();

            // Sắp xếp và phân trang
            var orders = await query.OrderByDescending(o => o.OrderDate)
                                    .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                                    .Take(filterParams.PageSize)
                                    .ToListAsync();

            return (orders, totalCount);
        }
    }
}
