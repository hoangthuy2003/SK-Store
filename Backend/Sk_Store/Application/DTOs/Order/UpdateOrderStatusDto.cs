using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "Trạng thái đơn hàng mới không được để trống.")]
        public string NewStatus { get; set; } = null!; // Ví dụ: Processing, Shipped, Delivered, Cancelled
    }
}
