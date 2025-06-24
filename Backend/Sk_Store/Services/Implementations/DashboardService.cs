using Application.DTOs.Dashboard;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.UnitOfWork;
using Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardStatsDto> GetDashboardStatisticsAsync()
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var allOrders = await _unitOfWork.Context.Orders
                                    .Include(o => o.OrderItems)
                                    .ThenInclude(oi => oi.Product)
                                    .ThenInclude(p => p.Category)
                                    .Include(o => o.User)
                                    .AsNoTracking()
                                    .ToListAsync();

            var stats = new DashboardStatsDto
            {
                // KPIs
                TotalRevenue = allOrders.Where(o => o.PaymentStatus == "Paid").Sum(o => o.TotalAmount),
                TotalOrders = allOrders.Count,
                NewCustomersLast30Days = await _unitOfWork.Context.Users.CountAsync(u => u.RegistrationDate >= thirtyDaysAgo),
                TotalProductsSold = allOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity),

                // Revenue Chart
                RevenueOverTime = allOrders
                    .Where(o => o.PaymentStatus == "Paid" && o.OrderDate >= thirtyDaysAgo)
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new RevenueByDateDto { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount) })
                    .OrderBy(r => r.Date)
                    .ToList(),

                // Category Chart
                CategorySalesDistribution = allOrders
                    .SelectMany(o => o.OrderItems)
                    .GroupBy(oi => oi.Product.Category.CategoryName)
                    .Select(g => new CategorySalesDto { CategoryName = g.Key, TotalQuantitySold = g.Sum(oi => oi.Quantity) })
                    .OrderByDescending(c => c.TotalQuantitySold)
                    .ToList(),

                // Recent Orders List
                RecentOrders = allOrders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Select(o => new RecentOrderDto
                    {
                        OrderId = o.OrderId,
                        UserFullName = $"{o.User.FirstName} {o.User.LastName}",
                        TotalAmount = o.TotalAmount,
                        OrderStatus = o.OrderStatus,
                        OrderDate = o.OrderDate
                    }).ToList(),

                // Top Selling Products List
                TopSellingProducts = allOrders
                    .SelectMany(o => o.OrderItems)
                    .GroupBy(oi => new { oi.ProductId, oi.Product.ProductName, oi.Product.ProductImages })
                    .Select(g => new TopProductDto
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.ProductName,
                        PrimaryImageUrl = g.Key.ProductImages.FirstOrDefault(img => img.IsPrimary)?.ImageUrl,
                        TotalQuantitySold = g.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(p => p.TotalQuantitySold)
                    .Take(5)
                    .ToList()
            };

            return stats;
        }
    }
}