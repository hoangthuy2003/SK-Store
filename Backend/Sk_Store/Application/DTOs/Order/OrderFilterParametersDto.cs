namespace Application.DTOs.Order
{
    public class OrderFilterParametersDto
    {
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? SearchTerm { get; set; } // Tìm theo mã đơn hàng, tên hoặc email khách hàng
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}