using BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    
    public interface IReviewRepository : IGenericRepository<Review>
    {
        
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
    }
}