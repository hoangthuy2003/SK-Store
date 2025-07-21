using BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUserAddressRepository : IGenericRepository<UserAddress>
    {
        Task<IEnumerable<UserAddress>> GetAddressesByUserIdAsync(int userId);
        Task<UserAddress?> GetDefaultAddressAsync(int userId);
        Task SetDefaultAddressAsync(int userId, int addressId);
        Task<bool> IsAddressOwnerAsync(int userId, int addressId);
        Task ClearDefaultAddressesAsync(int userId);
    }
}