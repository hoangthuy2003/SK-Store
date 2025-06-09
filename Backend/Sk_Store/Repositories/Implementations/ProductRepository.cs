// File: Repositories/Implementations/ProductRepository.cs
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
                         .Include(p => p.Category)
                         .Include(p => p.Brand)
                         .Include(p => p.ProductImages)
                         .Include(p => p.ProductAttributes) // Đảm bảo đã include ProductAttributes
                         .Include(p => p.Reviews)
                            .ThenInclude(r => r.User) // Include User để lấy thông tin người đánh giá
                         .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(ProductFilterParameters productFilter)
        {
            var query = _dbSet
                        .Include(p => p.Category)        // <<< THÊM INCLUDE
                        .Include(p => p.Brand)         // <<< THÊM INCLUDE
                        .Include(p => p.ProductImages) // <<< THÊM INCLUDE (để lấy ảnh đại diện)
                        .Include(p => p.Reviews)       // <<< THÊM INCLUDE (để tính rating)
                        .AsQueryable();

            if (productFilter.CategoryId.HasValue && productFilter.CategoryId > 0)
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
            
            // Sắp xếp (ví dụ: theo tên sản phẩm, hoặc ngày tạo mới nhất) - tùy chọn
            // query = query.OrderBy(p => p.ProductName);


            query = query.Skip((productFilter.PageNumber - 1) * productFilter.PageSize)
                         .Take(productFilter.PageSize);

            return await query.ToListAsync();
        }
    }
}
