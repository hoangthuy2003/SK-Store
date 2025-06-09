using Application.DTOs.Auth;
using BusinessObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Implementations;
using Repositories.Interfaces;
using Repositories.UnitOfWork;
using Sk_Store.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // 1. Tìm người dùng dựa trên email
            var user = await _unitOfWork.Users.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Email hoặc mật khẩu không chính xác." };
            }

            // 2. Kiểm tra xem tài khoản có bị khóa không
            if (!user.IsActive)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Tài khoản của bạn đã bị khóa." };
            }

            // 3. Xác thực mật khẩu
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Email hoặc mật khẩu không chính xác." };
            }

            // 4. Nếu thông tin hợp lệ, tạo JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Lấy Secret Key từ cấu hình
            var jwtSecret = _configuration["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(jwtSecret))
            {
                // _logger.LogError("JWT Secret key không được cấu hình trong appsettings.json.");
                return new AuthResponseDto { IsSuccess = false, Message = "Lỗi cấu hình hệ thống xác thực. Vui lòng liên hệ quản trị viên." };
            }
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            // Tạo danh sách các Claims cho Token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Subject (thường là ID người dùng)
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID, giúp token là duy nhất mỗi lần tạo
                // Thêm các claim khác nếu cần, ví dụ: FirstName, LastName
                new Claim("firstname", user.FirstName ?? string.Empty), // Đảm bảo không null
                new Claim("lastname", user.LastName ?? string.Empty),   // Đảm bảo không null
                // Quan trọng: Claim cho Role để phân quyền
                // Giả sử user.Role.RoleName được nạp hoặc bạn có cách lấy RoleName
                // Nếu không, dùng RoleId.ToString()
                // Để lấy user.Role.RoleName, bạn cần Ensure IUserRepository.GetUserByEmailAsync() includes Role
                 //new Claim(ClaimTypes.Role, user.RoleId.ToString()) 
                // Ví dụ nếu Role được include:
                 new Claim(ClaimTypes.Role, user.Role?.RoleName ?? user.RoleId.ToString())
            };

            // Lấy thời gian hết hạn từ cấu hình, mặc định là 7 ngày
            var expirationInDays = _configuration.GetValue<double?>("JwtSettings:ExpirationInDays") ?? 7.0;
            var tokenExpires = DateTime.UtcNow.AddDays(expirationInDays);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"], // Lấy từ cấu hình
                Audience = _configuration["JwtSettings:Audience"] // Lấy từ cấu形
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Cập nhật LastLoginDate cho người dùng
            user.LastLoginDate = DateTime.UtcNow;
            try
            {
                await _unitOfWork.CompleteAsync(); // Lưu thay đổi LastLoginDate
            }
            catch (Exception ex)
            {
            }

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Đăng nhập thành công!",
                Token = tokenString,
                TokenExpires = tokenDescriptor.Expires
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Kiểm tra xem email đã tồn tại chưa
            var emailExists = await _unitOfWork.Users.GetUserByEmailAsync(registerDto.Email);
            if (emailExists != null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Địa chỉ email này đã được sử dụng." };
            }

            // 2. THÊM BƯỚC KIỂM TRA SỐ ĐIỆN THOẠI
            var phoneExists = await _unitOfWork.Users.GetUserByPhoneNumberAsync(registerDto.PhoneNumber);
            if (phoneExists != null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Số điện thoại này đã được sử dụng." };
            }

            //hashing password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            //tao doi tuong user moi
            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                Gender = registerDto.Gender,
                DateOfBirth = registerDto.DateOfBirth,
                IsActive = true,
                RegistrationDate = DateTime.UtcNow,
                IsVerified = false,
                RoleId = 2
            };
            try
            {
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                return new AuthResponseDto { IsSuccess = false, Message = $"Đã xảy ra lỗi trong quá trình đăng ký: {e.Message}" };
            }

            return new AuthResponseDto { IsSuccess = true, Message = "Đăng ký tài khoản thành công!" };

        }
    }
}
