export interface DashboardStats {
  totalRevenue: number;
  totalOrders: number;
  newCustomersLast30Days: number;
  totalProductsSold: number;
  revenueOverTime: RevenueByDate[];
  categorySalesDistribution: CategorySales[];
  recentOrders: RecentOrder[];
  topSellingProducts: TopProduct[];
}

export interface RevenueByDate {
  date: string;
  revenue: number;
}

export interface CategorySales {
  categoryName: string;
  totalQuantitySold: number;
}

export interface RecentOrder {
  orderId: number;
  userFullName: string;
  totalAmount: number;
  orderStatus: string;
  orderDate: string;
}

export interface TopProduct {
  productId: number;
  productName: string;
  primaryImageUrl?: string;
  totalQuantitySold: number;
}