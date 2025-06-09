using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Order
{
    public class CreateOrderRequestDto
    {
        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống.")]
        [MaxLength(255, ErrorMessage = "Địa chỉ giao hàng không được vượt quá 255 ký tự.")]
        public string ShippingAddress { get; set; } = null!;

        [Required(ErrorMessage = "Tên người nhận không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên người nhận không được vượt quá 100 ký tự.")]
        public string RecipientName { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại người nhận không được để trống.")]
        [StringLength(10, ErrorMessage = "Số điện thoại người nhận phải có đúng 10 ký tự.")]
        [Phone(ErrorMessage = "Số điện thoại người nhận không hợp lệ.")]
        public string RecipientPhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Phương thức thanh toán không được để trống.")]
        public string PaymentMethod { get; set; } = "COD"; // Mặc định là COD, có thể mở rộng sau

        [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
        public string? Notes { get; set; }

        // Không cần UserId ở đây vì sẽ lấy từ token
        // Thông tin sản phẩm sẽ lấy từ giỏ hàng của người dùng
    }
}
