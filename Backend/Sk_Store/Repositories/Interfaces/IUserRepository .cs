// File: Repositories/Interfaces/IUserRepository.cs
using Application.DTOs; // Thêm using này nếu UserFilterParametersDto nằm trong Application.DTOs
using Application.DTOs.User;
using BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);

        /// <summary>
        /// Lấy danh sách người dùng với vai trò, có phân trang và lọc.
        /// </summary>
        Task<IEnumerable<User>> GetUsersWithRolesAsync(UserFilterParametersDto filterParams);

        /// <summary>
        /// Lấy thông tin chi tiết người dùng bao gồm vai trò.
        /// </summary>
        Task<User?> GetUserWithRoleByIdAsync(int userId);

        /// <summary>
        /// Đếm tổng số người dùng dựa trên các tiêu chí lọc (dùng cho phân trang).
        /// </summary>
        Task<int> CountUsersAsync(UserFilterParametersDto filterParams);
    }
}
