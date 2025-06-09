using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(SkstoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _dbSet
                .Include(r => r.User) // Để lấy thông tin UserName
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<bool> HasUserReviewedProductAsync(int userId, int productId)
        {
            return await _dbSet.AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }
    }
}