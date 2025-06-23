using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsAsync(ProductFilterParameters productFilter);
        Task<Product?> GetProductDetailByIdAsync(int id);
        Task<int> CountProductsAsync(ProductFilterParameters productFilter);
    }
}
