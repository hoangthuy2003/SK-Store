using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(SkstoreContext context) : base(context)
        {
        }

        public async Task<Product?> GetProductDetailByIdAsync(int id)
        {
            return await _dbSet
                         .Include(p => p.Brand)
                         .Include(p => p.Category)
                         .Include(p => p.ProductImages)
                         .Include(p => p.Reviews)
                         .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(ProductFilterParameters productFilter)
        {
            var query = _dbSet.AsQueryable();

            if (productFilter.CategoryId.HasValue && productFilter.CategoryId >0)
            {
                query = query.Where(c => c.CategoryId == productFilter.CategoryId);
            }

            if (productFilter.BrandId.HasValue && productFilter.BrandId > 0)
            {
                query = query.Where(b => b.BrandId == productFilter.BrandId);
            }

            if (!string.IsNullOrEmpty(productFilter.SearchTerm))
            {
                var searchItemLower = productFilter.SearchTerm.ToLower();
                query = query.Where(s => s.ProductName.ToLower().Contains(searchItemLower));
            }

            query = query.Skip((productFilter.PageNumber - 1) * productFilter.PageSize)
                .Take(productFilter.PageSize);

            return await query.ToListAsync();
        }
    }
}
