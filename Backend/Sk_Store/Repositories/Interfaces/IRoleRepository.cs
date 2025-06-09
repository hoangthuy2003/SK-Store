// File: Repositories/Interfaces/IRoleRepository.cs
using BusinessObjects; // Namespace chứa Role

namespace Repositories.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        // Bạn có thể thêm các phương thức đặc thù cho Role ở đây nếu cần trong tương lai
        // Ví dụ: Task<Role?> GetRoleByNameAsync(string roleName);
    }
}
