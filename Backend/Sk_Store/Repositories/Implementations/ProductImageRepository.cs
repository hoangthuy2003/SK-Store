using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    /// <summary>
    /// Repository để quản lý ảnh sản phẩm
    /// </summary>
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly SkstoreContext _context;

        public ProductImageRepository(SkstoreContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy ảnh theo ID
        /// </summary>
        public async Task<ProductImage?> GetByIdAsync(int imageId)
        {
            return await _context.ProductImages
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.ImageId == imageId);
        }

        /// <summary>
        /// Lấy tất cả ảnh của một sản phẩm
        /// </summary>
        public async Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .OrderByDescending(pi => pi.IsPrimary)
                .ThenBy(pi => pi.ImageId)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy ảnh chính của sản phẩm
        /// </summary>
        public async Task<ProductImage?> GetPrimaryImageByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.IsPrimary);
        }

        /// <summary>
        /// Lấy tất cả ảnh phụ của sản phẩm
        /// </summary>
        public async Task<IEnumerable<ProductImage>> GetSecondaryImagesByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId && !pi.IsPrimary)
                .OrderBy(pi => pi.ImageId)
                .ToListAsync();
        }

        /// <summary>
        /// Thêm ảnh mới
        /// </summary>
        public async Task<ProductImage> AddAsync(ProductImage productImage)
        {
            // Nếu ảnh này được đặt làm primary, bỏ primary của các ảnh khác
            if (productImage.IsPrimary)
            {
                await UnsetAllPrimaryImagesAsync(productImage.ProductId);
            }

            await _context.ProductImages.AddAsync(productImage);
            await _context.SaveChangesAsync();

            return productImage;
        }

        /// <summary>
        /// Thêm nhiều ảnh cùng lúc
        /// </summary>
        public async Task<IEnumerable<ProductImage>> AddRangeAsync(IEnumerable<ProductImage> productImages)
        {
            var images = productImages.ToList();
            var primaryImage = images.FirstOrDefault(img => img.IsPrimary);

            if (primaryImage != null)
            {
                // Bỏ primary của tất cả ảnh cũ
                await UnsetAllPrimaryImagesAsync(primaryImage.ProductId);

                // Đảm bảo chỉ có 1 ảnh primary trong batch mới
                foreach (var img in images.Where(i => i != primaryImage))
                {
                    img.IsPrimary = false;
                }
            }

            await _context.ProductImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();

            return images;
        }

        /// <summary>
        /// Cập nhật thông tin ảnh
        /// </summary>
        public async Task<ProductImage> UpdateAsync(ProductImage productImage)
        {
            // Nếu ảnh này được đặt làm primary, bỏ primary của các ảnh khác
            if (productImage.IsPrimary)
            {
                await UnsetAllPrimaryImagesAsync(productImage.ProductId, productImage.ImageId);
            }

            _context.ProductImages.Update(productImage);
            await _context.SaveChangesAsync();

            return productImage;
        }

        /// <summary>
        /// Xóa ảnh theo ID
        /// </summary>
        public async Task<bool> DeleteAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                return false;

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Xóa ảnh theo object
        /// </summary>
        public async Task<bool> DeleteAsync(ProductImage productImage)
        {
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Xóa tất cả ảnh của một sản phẩm
        /// </summary>
        public async Task<int> DeleteAllByProductIdAsync(int productId)
        {
            var images = await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            if (!images.Any())
                return 0;

            _context.ProductImages.RemoveRange(images);
            await _context.SaveChangesAsync();

            return images.Count;
        }

        /// <summary>
        /// Đặt ảnh làm ảnh chính
        /// </summary>
        public async Task<bool> SetPrimaryImageAsync(int imageId, int productId)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.ImageId == imageId && pi.ProductId == productId);

            if (image == null)
                return false;

            // Bỏ primary của tất cả ảnh khác
            await UnsetAllPrimaryImagesAsync(productId, imageId);

            // Đặt ảnh này làm primary
            image.IsPrimary = true;
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Kiểm tra ảnh có thuộc về sản phẩm không
        /// </summary>
        public async Task<bool> ImageBelongsToProductAsync(int imageId, int productId)
        {
            return await _context.ProductImages
                .AnyAsync(pi => pi.ImageId == imageId && pi.ProductId == productId);
        }

        /// <summary>
        /// Đếm số lượng ảnh của sản phẩm
        /// </summary>
        public async Task<int> CountImagesByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .CountAsync(pi => pi.ProductId == productId);
        }

        /// <summary>
        /// Kiểm tra ảnh có tồn tại không
        /// </summary>
        public async Task<bool> ExistsAsync(int imageId)
        {
            return await _context.ProductImages
                .AnyAsync(pi => pi.ImageId == imageId);
        }

        /// <summary>
        /// Helper method: Bỏ primary của tất cả ảnh trong sản phẩm
        /// </summary>
        private async Task UnsetAllPrimaryImagesAsync(int productId, int? excludeImageId = null)
        {
            var query = _context.ProductImages
                .Where(pi => pi.ProductId == productId && pi.IsPrimary);

            if (excludeImageId.HasValue)
            {
                query = query.Where(pi => pi.ImageId != excludeImageId.Value);
            }

            var primaryImages = await query.ToListAsync();

            foreach (var img in primaryImages)
            {
                img.IsPrimary = false;
            }

            if (primaryImages.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}