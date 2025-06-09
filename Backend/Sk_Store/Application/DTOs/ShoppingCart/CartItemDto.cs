using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ShoppingCart
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string? ProductImageUrl { get; set; } // Hình ảnh đại diện sản phẩm
        public int StockQuantity { get; set; } // Số lượng tồn kho của sản phẩm
    }
}
