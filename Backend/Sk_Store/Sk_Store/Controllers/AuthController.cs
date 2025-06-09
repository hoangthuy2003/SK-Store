// File: Controllers/AuthController.cs
// Đảm bảo namespace Controllers của bạn là chính xác, ví dụ: Sk_Store.Controllers
using Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.UnitOfWork;
using Services.Implementations;
using Services.Interfaces;
using Sk_Store.Services.Interfaces; // Namespace chứa IAuthService
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sk_Store.Controllers // Hoặc namespace Controllers của project API chính của bạn
{
    [Route("api/[controller]")] // Đường dẫn cơ sở cho controller này sẽ là /api/auth
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(IAuthService authService, IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
                {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message }); // Có thể trả về AuthResponseDto trực tiếp
            }

            return Ok(new { message = result.Message });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }

        // Endpoint để yêu cầu gửi mã reset mật khẩu
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email);
            if (user != null)
            {
                await _emailService.SendOtpAsync(dto.Email, "SK Store - Yêu cầu đặt lại mật khẩu", "password_reset");
            }
            // Luôn trả về OK để tránh kẻ xấu dò email có tồn tại trong hệ thống hay không
            return Ok(new { message = "Nếu email của bạn tồn tại trong hệ thống, chúng tôi đã gửi một mã OTP." });
        }

        // Endpoint để xác thực OTP và đổi mật khẩu
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var isValidOtp = await _emailService.VerifyOtpAsync(dto.Email, "password_reset", dto.Otp);
            if (!isValidOtp)
            {
                return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn." });
            }

            var user = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email);
            if (user == null) return NotFound(); // Trường hợp hi hữu

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Mật khẩu của bạn đã được thay đổi thành công." });
        }

        // Endpoint để yêu cầu gửi lại mã xác thực email (người dùng phải đăng nhập)
        [HttpPost("send-verification-email")]
        [Authorize] // Yêu cầu đăng nhập
        public async Task<IActionResult> SendVerificationEmail()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            await _emailService.SendOtpAsync(email, "SK Store - Xác thực địa chỉ email", "email_verification");
            return Ok(new { message = "Mã OTP xác thực đã được gửi." });
        }

        // Endpoint để xác thực email bằng OTP
        [HttpPost("verify-email")]
        [Authorize] // Yêu cầu đăng nhập
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var isValidOtp = await _emailService.VerifyOtpAsync(email, "email_verification", dto.Otp);
            if (!isValidOtp)
            {
                return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn." });
            }

            var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
            if (user == null) return NotFound();

            user.IsVerified = true;
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Email của bạn đã được xác thực thành công." });
        }
    }
}