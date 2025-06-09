// File: Sk_Store/Controllers/ProfileController.cs
using Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sk_Store.Services.Interfaces;
using System.Security.Claims; // Cần để lấy thông tin User từ Token
using System.Threading.Tasks;

namespace Sk_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Tất cả các API trong controller này đều yêu cầu đăng nhập
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        // Hàm tiện ích để lấy UserId của người dùng đang đăng nhập từ Claims
        private int GetCurrentUserId()
        {
            // Lấy giá trị của ClaimTypes.NameIdentifier (thường là UserId đã được set khi tạo token)
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"); // "sub" là một claim chuẩn cho subject (user id)
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                // Trường hợp không tìm thấy hoặc không parse được UserId từ token
                // Điều này không nên xảy ra nếu token được tạo đúng cách và người dùng đã được xác thực.
                // Bạn có thể throw một exception hoặc trả về lỗi cụ thể.
                // Ở đây, tạm thời trả về 0 hoặc một giá trị không hợp lệ để gây lỗi ở các bước sau nếu cần.
                // Hoặc, tốt hơn là kiểm tra và trả về Unauthorized nếu không có claim.
                // throw new UnauthorizedAccessException("User ID not found in token.");
                // Trong thực tế, middleware xác thực sẽ xử lý việc token không hợp lệ trước khi đến đây.
                // Nếu đến được đây mà không có claim, có thể là lỗi cấu hình token.
                return 0; // Hoặc xử lý lỗi phù hợp
            }
            return userId;
        }

        // GET: api/profile
        /// <summary>
        /// Lấy thông tin hồ sơ của người dùng đang đăng nhập.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) // Kiểm tra nếu GetCurrentUserId trả về lỗi
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var userProfile = await _userService.GetUserProfileAsync(userId);
            if (userProfile == null)
            {
                // Điều này không nên xảy ra nếu token hợp lệ và người dùng tồn tại
                return NotFound(new { message = "Không tìm thấy thông tin hồ sơ người dùng." });
            }
            return Ok(userProfile);
        }

        // PUT: api/profile
        /// <summary>
        /// Cập nhật thông tin hồ sơ của người dùng đang đăng nhập.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto updateUserProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var success = await _userService.UpdateUserProfileAsync(userId, updateUserProfileDto);
            if (!success)
            {
                // Lý do thất bại có thể do SĐT mới bị trùng hoặc lỗi server
                return BadRequest(new { message = "Cập nhật hồ sơ thất bại. Số điện thoại có thể đã được sử dụng hoặc có lỗi xảy ra." });
            }

            return Ok(new { message = "Hồ sơ của bạn đã được cập nhật thành công." }); // Trả về 200 OK với thông báo
            // Hoặc có thể trả về NoContent() nếu không muốn gửi message
            // return NoContent();
        }

        // POST: api/profile/change-password
        /// <summary>
        /// Thay đổi mật khẩu của người dùng đang đăng nhập.
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var success = await _userService.ChangeUserPasswordAsync(userId, changePasswordDto);
            if (!success)
            {
                // Lý do thất bại có thể do mật khẩu cũ không đúng
                return BadRequest(new { message = "Thay đổi mật khẩu thất bại. Mật khẩu cũ không chính xác hoặc có lỗi xảy ra." });
            }

            return Ok(new { message = "Mật khẩu của bạn đã được thay đổi thành công." });
        }
    }
}
