using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(SkstoreContext context) : base(context)
        {
        }

        public async Task<ShoppingCart?> GetCartByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(sc => sc.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(sc => sc.UserId == userId);
        }
    }
}