// File: Services/Implementations/UserService.cs
using Application.DTOs;
using Application.DTOs.User;
using BusinessObjects;
using Repositories.UnitOfWork;
using Sk_Store.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;

namespace Services.Implementations // Đảm bảo namespace đúng
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork /*, ILogger<UserService> logger*/)
        {
            _unitOfWork = unitOfWork;
            // _logger = logger;
        }

        public async Task<(IEnumerable<UserDto> Users, int TotalCount)> GetUsersAsync(UserFilterParametersDto filterParams)
        {
            var usersFromRepo = await _unitOfWork.Users.GetUsersWithRolesAsync(filterParams);
            var totalCount = await _unitOfWork.Users.CountUsersAsync(filterParams);

            var userDtos = usersFromRepo.Select(u => new UserDto
            {
                UserId = u.UserId,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                Gender = u.Gender,
                DateOfBirth = u.DateOfBirth,
                IsActive = u.IsActive,
                RegistrationDate = u.RegistrationDate,
                LastLoginDate = u.LastLoginDate,
                IsVerified = u.IsVerified,
                RoleId = u.RoleId,
                RoleName = u.Role?.RoleName ?? "N/A" // Role đã được include từ repository
            }).ToList();

            return (userDtos, totalCount);
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserWithRoleByIdAsync(userId); // Sử dụng phương thức mới
            if (user == null)
            {
                // _logger?.LogWarning($"User with ID {userId} not found.");
                return null;
            }

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                IsActive = user.IsActive,
                RegistrationDate = user.RegistrationDate,
                LastLoginDate = user.LastLoginDate,
                IsVerified = user.IsVerified,
                RoleId = user.RoleId,
                RoleName = user.Role?.RoleName ?? "N/A"
            };
        }

        public async Task<bool> UpdateUserByAdminAsync(int userId, UserForAdminUpdateDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId); // Không cần include Role ở đây vì chỉ cập nhật IsActive và RoleId
            if (user == null)
            {
                // _logger?.LogWarning($"User with ID {userId} not found for update by admin.");
                return false;
            }

            // Kiểm tra xem RoleId mới có hợp lệ không
            var roleExists = await _unitOfWork.Roles.GetByIdAsync(updateUserDto.RoleId);
            if (roleExists == null)
            {
                // _logger?.LogWarning($"Role with ID {updateUserDto.RoleId} not found. Cannot update user {userId}.");
                return false; // RoleId không hợp lệ
            }

            user.IsActive = updateUserDto.IsActive;
            user.RoleId = updateUserDto.RoleId;
            // Không cần gọi _unitOfWork.Users.UpdateAsync(user) vì user đã được track

            try
            {
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                // _logger?.LogError(ex, $"Error updating user {userId} by admin.");
                return false;
            }
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId); // Không cần include Role ở đây
            if (user == null)
            {
                // _logger?.LogWarning($"User profile with ID {userId} not found.");
                return null;
            }

            return new UserProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                RegistrationDate = user.RegistrationDate,
                LastLoginDate = user.LastLoginDate,
                IsVerified = user.IsVerified
            };
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateUserProfileDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                // _logger?.LogWarning($"User with ID {userId} not found for profile update.");
                return false;
            }

            // Kiểm tra xem số điện thoại mới (nếu thay đổi) có bị trùng với người dùng khác không
            if (user.PhoneNumber != updateUserProfileDto.PhoneNumber)
            {
                var existingUserWithPhoneNumber = await _unitOfWork.Users.GetUserByPhoneNumberAsync(updateUserProfileDto.PhoneNumber);
                if (existingUserWithPhoneNumber != null && existingUserWithPhoneNumber.UserId != userId)
                {
                    // _logger?.LogWarning($"Phone number {updateUserProfileDto.PhoneNumber} is already in use by another user.");
                    return false; // Số điện thoại đã được sử dụng bởi người khác
                }
            }

            user.FirstName = updateUserProfileDto.FirstName;
            user.LastName = updateUserProfileDto.LastName;
            user.PhoneNumber = updateUserProfileDto.PhoneNumber;
            user.Gender = updateUserProfileDto.Gender;
            user.DateOfBirth = updateUserProfileDto.DateOfBirth;

            // _unitOfWork.Users.UpdateAsync(user); // Không cần gọi nếu entity được track
            try
            {
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                // _logger?.LogError(ex, $"Error updating user profile for user ID {userId}.");
                return false;
            }
        }

        public async Task<bool> ChangeUserPasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                // _logger?.LogWarning($"User with ID {userId} not found for password change.");
                return false;
            }

            // Xác thực mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.PasswordHash))
            {
                // _logger?.LogWarning($"Invalid old password for user ID {userId}.");
                return false; // Mật khẩu cũ không đúng
            }

            // Hash mật khẩu mới và cập nhật
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

            // _unitOfWork.Users.UpdateAsync(user); // Không cần gọi nếu entity được track
            try
            {
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                // _logger?.LogError(ex, $"Error changing password for user ID {userId}.");
                return false;
            }
        }
    }
}
