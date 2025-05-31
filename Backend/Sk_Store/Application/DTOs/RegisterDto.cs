using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = null!;

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
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime DateOfBirth { get; set; }
    }
}
