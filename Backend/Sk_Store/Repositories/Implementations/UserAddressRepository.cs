using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class UserAddressRepository : GenericRepository<UserAddress>, IUserAddressRepository
    {
        public UserAddressRepository(SkstoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserAddress>> GetAddressesByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.AddressId)
                .ToListAsync();
        }

        public async Task<UserAddress?> GetDefaultAddressAsync(int userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
        }

        public async Task SetDefaultAddressAsync(int userId, int addressId)
        {
            // Xóa tất cả địa chỉ mặc định hiện tại của user
            await ClearDefaultAddressesAsync(userId);

            // Đặt địa chỉ mới làm mặc định
            var address = await _dbSet.FirstOrDefaultAsync(a => a.UserId == userId && a.AddressId == addressId);
            if (address != null)
            {
                address.IsDefault = true;
                _context.Update(address);
            }
        }

        public async Task<bool> IsAddressOwnerAsync(int userId, int addressId)
        {
            return await _dbSet.AnyAsync(a => a.UserId == userId && a.AddressId == addressId);
        }

        public async Task ClearDefaultAddressesAsync(int userId)
        {
            var defaultAddresses = await _dbSet
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();

            foreach (var address in defaultAddresses)
            {
                address.IsDefault = false;
                _context.Update(address);
            }
        }
    }
}