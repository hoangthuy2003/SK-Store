using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ShoppingCart
{
    public class UpdateCartItemQuantityDto
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100.")] // Giới hạn số lượng
        public int Quantity { get; set; }
    }
}
