using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Order
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; } // Giá tại thời điểm đặt hàng
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string? ProductImageUrl { get; set; }
    }
}
