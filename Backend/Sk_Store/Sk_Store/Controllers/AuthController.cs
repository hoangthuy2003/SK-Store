// File: Controllers/AuthController.cs
// Đảm bảo namespace Controllers của bạn là chính xác, ví dụ: Sk_Store.Controllers
using Application.DTOs; // Namespace chứa RegisterDto, LoginDto, AuthResponseDto
using Microsoft.AspNetCore.Mvc;
using Sk_Store.Services.Interfaces; // Namespace chứa IAuthService
using System.Threading.Tasks;

namespace Sk_Store.Controllers // Hoặc namespace Controllers của project API chính của bạn
{
    [Route("api/[controller]")] // Đường dẫn cơ sở cho controller này sẽ là /api/auth
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService )
        {
            _authService = authService;
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
    }
}