using BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    
    public interface IUserAddressRepository : IGenericRepository<UserAddress>
    {
       
        Task<IEnumerable<UserAddress>> GetAddressesByUserIdAsync(int userId);
    }
}