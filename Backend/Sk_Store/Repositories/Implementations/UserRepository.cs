// File: Repositories/Implementations/UserRepository.cs
using Application.DTOs; // Thêm using này
using Application.DTOs.User;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions; // Cho Expression
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SkstoreContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.Include(u => u.Role)
                               .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            // Nên include Role ở đây nếu cần thông tin Role khi tìm bằng SĐT
            return await _dbSet.Include(u => u.Role)
                               .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetUserWithRoleByIdAsync(int userId)
        {
            return await _dbSet.Include(u => u.Role)
                               .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        private IQueryable<User> ApplyUserFilters(IQueryable<User> query, UserFilterParametersDto filterParams)
        {
            if (!string.IsNullOrEmpty(filterParams.SearchTerm))
            {
                var searchTermLower = filterParams.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(searchTermLower) ||
                    u.FirstName.ToLower().Contains(searchTermLower) ||
                    u.LastName.ToLower().Contains(searchTermLower) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTermLower)) // Kiểm tra null cho PhoneNumber
                );
            }

            if (filterParams.RoleId.HasValue)
            {
                query = query.Where(u => u.RoleId == filterParams.RoleId.Value);
            }

            if (filterParams.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filterParams.IsActive.Value);
            }
            return query;
        }

        public async Task<IEnumerable<User>> GetUsersWithRolesAsync(UserFilterParametersDto filterParams)
        {
            var query = _dbSet.Include(u => u.Role).AsQueryable();

            query = ApplyUserFilters(query, filterParams);

            // Sắp xếp
            if (!string.IsNullOrEmpty(filterParams.SortBy))
            {
                // Cần cẩn thận với việc sắp xếp động để tránh SQL Injection nếu SortBy là chuỗi tùy ý.
                // Cách an toàn hơn là dùng switch-case hoặc dictionary để map SortBy với Expression.
                // Ví dụ đơn giản:
                bool descending = filterParams.SortDirection?.ToLower() == "desc";
                switch (filterParams.SortBy.ToLower())
                {
                    case "email":
                        query = descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                        break;
                    case "firstname":
                        query = descending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName);
                        break;
                    case "lastname":
                        query = descending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName);
                        break;
                    case "registrationdate":
                    default: // Mặc định sắp xếp theo RegistrationDate
                        query = descending ? query.OrderByDescending(u => u.RegistrationDate) : query.OrderBy(u => u.RegistrationDate);
                        break;
                }
            }
            else // Sắp xếp mặc định nếu không có SortBy
            {
                query = query.OrderByDescending(u => u.RegistrationDate);
            }


            // Phân trang
            query = query.Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                         .Take(filterParams.PageSize);

            return await query.ToListAsync();
        }

        public async Task<int> CountUsersAsync(UserFilterParametersDto filterParams)
        {
            var query = _dbSet.AsQueryable();
            query = ApplyUserFilters(query, filterParams);
            return await query.CountAsync();
        }
    }
}
