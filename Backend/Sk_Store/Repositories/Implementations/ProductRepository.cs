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
        public async Task<int> CountProductsAsync(ProductFilterParameters productFilter)
        {
            var query = _dbSet.AsQueryable();

            if (productFilter.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == productFilter.IsActive.Value);
            }
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

            return await query.CountAsync();
        }
        // Thêm các method này vào ProductImageRepository class

        /// <summary>
        /// Lấy tất cả ảnh của sản phẩm
        /// </summary>
        public async Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }

        /// <summary>
        /// Xóa ảnh theo ID
        /// </summary>
        public async Task DeleteAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
            }
        }
        public async Task<IEnumerable<Product>> GetProductsAsync(ProductFilterParameters productFilter)
        {
            var query = _dbSet
                        .Include(p => p.Category)        // <<< THÊM INCLUDE
                        .Include(p => p.Brand)         // <<< THÊM INCLUDE
                        .Include(p => p.ProductImages) // <<< THÊM INCLUDE (để lấy ảnh đại diện)
                        .Include(p => p.Reviews)       // <<< THÊM INCLUDE (để tính rating)
                        .AsQueryable();
            if (productFilter.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == productFilter.IsActive.Value);
            }
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
            
            // Thêm sorting logic
            if (!string.IsNullOrEmpty(productFilter.SortBy))
            {
                var isDescending = productFilter.SortOrder?.ToLower() == "desc";
                
                Console.WriteLine($"🔄 Applying sort: {productFilter.SortBy} {(isDescending ? "DESC" : "ASC")}");
                
                query = productFilter.SortBy.ToLower() switch
                {
                    "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "name" => isDescending ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName),
                    "created" => isDescending ? query.OrderByDescending(p => p.CreationDate) : query.OrderBy(p => p.CreationDate),
                    "popular" => query.OrderByDescending(p => p.Reviews.Count), // Sắp xếp theo số lượng đánh giá
                    _ => query.OrderBy(p => p.ProductName) // Default sort
                };
            }
            else
            {
                Console.WriteLine("🔄 Using default sorting (ProductName ASC)");
                // Default sorting nếu không có SortBy
                query = query.OrderBy(p => p.ProductName);
            }

            query = query.Skip((productFilter.PageNumber - 1) * productFilter.PageSize)
                         .Take(productFilter.PageSize);

            return await query.ToListAsync();
        }
    }
}
