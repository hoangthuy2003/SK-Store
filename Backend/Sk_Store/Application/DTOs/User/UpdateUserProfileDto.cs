using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class UpdateUserProfileDto
    {
        [Required(ErrorMessage = "Họ không được để trống")]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Tên không được để trống")]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(10, ErrorMessage = "Số điện thoại phải có đúng 10 ký tự")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Giới tính không được để trống")]
        public string Gender { get; set; } = null!; // Cân nhắc dùng Enum nếu có các giá trị cố định

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime DateOfBirth { get; set; }

        // Email không cho phép cập nhật ở đây để đảm bảo tính duy nhất và quy trình xác thực nếu thay đổi.
        // Nếu muốn cho phép đổi email, cần quy trình xác thực email mới.
    }
}
