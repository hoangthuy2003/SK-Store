using BusinessObjects;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
 
        Task<ShoppingCart?> GetCartByUserIdAsync(int userId);
    }
}