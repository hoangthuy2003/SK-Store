using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class UserFilterParametersDto
    {
        public string? SearchTerm { get; set; } // Tìm kiếm theo Email, FirstName, LastName, PhoneNumber
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "RegistrationDate"; // Mặc định sắp xếp theo ngày đăng ký
        public string SortDirection { get; set; } = "DESC"; // Mặc định giảm dần (mới nhất trước)
    }
}
