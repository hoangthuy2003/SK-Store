namespace Application.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        // KPIs
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int NewCustomersLast30Days { get; set; }
        public int TotalProductsSold { get; set; }

        // Chart Data
        public List<RevenueByDateDto> RevenueOverTime { get; set; } = new();
        public List<CategorySalesDto> CategorySalesDistribution { get; set; } = new();

        // Lists
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
        public List<TopProductDto> TopSellingProducts { get; set; } = new();
    }

    public class RevenueByDateDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class CategorySalesDto
    {
        public string CategoryName { get; set; } = null!;
        public int TotalQuantitySold { get; set; }
    }

    public class RecentOrderDto
    {
        public int OrderId { get; set; }
        public string UserFullName { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = null!;
        public DateTime OrderDate { get; set; }
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? PrimaryImageUrl { get; set; }
        public int TotalQuantitySold { get; set; }
    }
}