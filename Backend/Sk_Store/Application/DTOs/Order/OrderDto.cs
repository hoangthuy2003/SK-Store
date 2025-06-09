using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Order
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = null!; // Ví dụ: Pending, Processing, Shipped, Delivered, Cancelled
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string RecipientName { get; set; } = null!;
        public string RecipientPhoneNumber { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!; // Ví dụ: Unpaid, Paid, Failed, Refunded
        public DateTime? DeliveryDate { get; set; }
        public decimal ShippingFee { get; set; } // Mặc định có thể là 0
        public string? Notes { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public string? UserFullName { get; set; } // Tên đầy đủ của người đặt hàng (nếu cần cho admin)
        public string? UserEmail { get; set; } // Email người đặt hàng (nếu cần cho admin)
    }
}
