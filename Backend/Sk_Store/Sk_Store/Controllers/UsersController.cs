// File: Sk_Store/Controllers/UsersController.cs
using Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sk_Store.Services.Interfaces;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sk_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Quan trọng: Chỉ định rằng toàn bộ controller này chỉ dành cho Admin
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        /// <summary>
        /// Lấy danh sách người dùng với các tùy chọn lọc, sắp xếp và phân trang (Admin only).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilterParametersDto filterParams)
        {
            // Service đã được thiết kế để trả về cả danh sách và tổng số lượng
            var (users, totalCount) = await _userService.GetUsersAsync(filterParams);

            // Thêm header X-Total-Count vào response
            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

            return Ok(users);
        }

        // GET: api/users/{id}
        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng theo ID (Admin only).
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"Không tìm thấy người dùng với ID {id}." });
            }
            return Ok(user);
        }

        // PUT: api/users/{id}
        /// <summary>
        /// Cập nhật vai trò và trạng thái hoạt động của người dùng (Admin only).
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForAdminUpdateDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _userService.UpdateUserByAdminAsync(id, updateUserDto);

            if (!success)
            {
                // Lý do thất bại có thể do user không tồn tại hoặc roleId không hợp lệ.
                return BadRequest(new { message = $"Cập nhật thất bại. Vui lòng kiểm tra lại ID người dùng và ID vai trò." });
            }

            return NoContent(); // HTTP 204 No Content - Chuẩn cho các request Update thành công và không cần trả về dữ liệu.
        }
    }
}