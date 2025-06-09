using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class UserForAdminUpdateDto
    {
        // Admin có thể không được phép thay đổi các thông tin cá nhân nhạy cảm như Email, tên, SĐT, ngày sinh, giới tính.
        // Nếu muốn cho phép, bạn có thể thêm các trường đó vào đây.
        // Thông thường, admin sẽ quản lý vai trò và trạng thái tài khoản.

        [Required(ErrorMessage = "Trạng thái hoạt động không được để trống.")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID vai trò không hợp lệ.")] // Giả sử RoleId bắt đầu từ 1
        public int RoleId { get; set; }

        // Có thể thêm các trường khác mà Admin được phép cập nhật, ví dụ:
        // public bool IsVerified { get; set; }
    }
}
