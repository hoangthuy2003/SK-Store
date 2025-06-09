// File: Repositories/Implementations/RoleRepository.cs
using BusinessObjects; // Namespace chứa Role
using Repositories.Interfaces; // Namespace chứa IRoleRepository
// using Microsoft.EntityFrameworkCore; // Nếu cần các phương thức LINQ đặc thù
// using System.Threading.Tasks; // Nếu cần các phương thức async đặc thù

namespace Repositories.Implementations
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(SkstoreContext context) : base(context)
        {
        }

        // Triển khai các phương thức đặc thù nếu có trong IRoleRepository
        // Ví dụ:
        // public async Task<Role?> GetRoleByNameAsync(string roleName)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());
        // }
    }
}
