// File: Services/Interfaces/IUserService.cs
using Application.DTOs;
using Application.DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sk_Store.Services.Interfaces // Đảm bảo namespace đúng
{
    public interface IUserService
    {
       
        Task<(IEnumerable<UserDto> Users, int TotalCount)> GetUsersAsync(UserFilterParametersDto filterParams);

        
        Task<UserDto?> GetUserByIdAsync(int userId);

        
        Task<bool> UpdateUserByAdminAsync(int userId, UserForAdminUpdateDto updateUserDto);

        
        Task<UserProfileDto?> GetUserProfileAsync(int userId);
        Task<bool> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateUserProfileDto);
        Task<bool> ChangeUserPasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    }
}
