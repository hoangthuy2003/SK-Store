// File: Services/Implementations/ProductService.cs
using Application.DTOs;
using Application.DTOs.Product;
using BusinessObjects;
using Repositories;
using Repositories.UnitOfWork;
using Services.Interfaces;
using Sk_Store.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.Extensions.Logging; // Bỏ comment nếu bạn muốn dùng ILogger

namespace Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork /*, ILogger<ProductService> logger */)
        {
            _unitOfWork = unitOfWork;
            // _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(ProductFilterParameters filterParams)
        {
            var products = await _unitOfWork.Products.GetProductsAsync(filterParams);
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var primaryImage = product.ProductImages?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl;
                double averageRating = 0;
                int reviewCount = 0;

                if (product.Reviews != null && product.Reviews.Any())
                {
                    averageRating = product.Reviews.Average(r => r.Rating);
                    reviewCount = product.Reviews.Count;
                }

                productDtos.Add(new ProductDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    PrimaryImageUrl = primaryImage,
                    CategoryName = product.Category?.CategoryName ?? "N/A",
                    BrandName = product.Brand?.BrandName ?? "N/A",
                    AverageRating = averageRating,
                    ReviewCount = reviewCount,
                    StockQuantity = product.StockQuantity,
                    IsActive = product.IsActive
                });
            }
            return productDtos;
        }

        public async Task<ProductDetailDto?> GetProductByIdAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetProductDetailByIdAsync(productId);
            if (product == null)
            {
                return null;
            }

            var productDetailDto = new ProductDetailDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.CategoryName,
                BrandId = product.BrandId,
                BrandName = product.Brand.BrandName,
                IsActive = product.IsActive,
                CreationDate = product.CreationDate,
                LastUpdatedDate = product.LastUpdatedDate,
                ProductImages = product.ProductImages.Select(img => new ProductImageDto
                {
                    ImageId = img.ImageId,
                    ImageUrl = img.ImageUrl,
                    IsPrimary = img.IsPrimary
                }).ToList(),
                ProductAttributes = product.ProductAttributes.Select(attr => new ProductAttributeDto
                {
                    AttributeId = attr.AttributeId,
                    AttributeName = attr.AttributeName,
                    AttributeValue = attr.AttributeValue
                }).ToList(),
                Reviews = product.Reviews.Select(rev => new ReviewDto
                {
                    ReviewId = rev.ReviewId,
                    UserName = (rev.User != null) ? $"{rev.User.FirstName} {rev.User.LastName}".Trim() : "Anonymous",
                    Rating = rev.Rating,
                    Comment = rev.Comment,
                    ReviewDate = rev.ReviewDate
                }).ToList()
            };

            if (productDetailDto.Reviews.Any())
            {
                productDetailDto.AverageRating = productDetailDto.Reviews.Average(r => r.Rating);
                productDetailDto.ReviewCount = productDetailDto.Reviews.Count;
            }
            else
            {
                productDetailDto.AverageRating = 0;
                productDetailDto.ReviewCount = 0;
            }

            return productDetailDto;
        }

        public async Task<ProductDetailDto?> CreateProductAsync(CreateProductDto createProductDto)
        {
            var categoryExists = await _unitOfWork.Categories.GetByIdAsync(createProductDto.CategoryId);
            if (categoryExists == null)
            {
                // _logger?.LogWarning($"Category with ID {createProductDto.CategoryId} not found during product creation.");
                return null;
            }

            var brandExists = await _unitOfWork.Brands.GetByIdAsync(createProductDto.BrandId);
            if (brandExists == null)
            {
                // _logger?.LogWarning($"Brand with ID {createProductDto.BrandId} not found during product creation.");
                return null;
            }

            var product = new Product
            {
                ProductName = createProductDto.ProductName,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                StockQuantity = createProductDto.StockQuantity,
                CategoryId = createProductDto.CategoryId,
                BrandId = createProductDto.BrandId,
                IsActive = createProductDto.IsActive,
                CreationDate = DateTime.UtcNow
            };

            if (createProductDto.Images != null && createProductDto.Images.Any())
            {
                foreach (var imgDto in createProductDto.Images)
                {
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = imgDto.ImageUrl,
                        IsPrimary = imgDto.IsPrimary
                    });
                }
            }

            if (createProductDto.Attributes != null && createProductDto.Attributes.Any())
            {
                foreach (var attrDto in createProductDto.Attributes)
                {
                    product.ProductAttributes.Add(new ProductAttribute
                    {
                        AttributeName = attrDto.AttributeName,
                        AttributeValue = attrDto.AttributeValue
                    });
                }
            }

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return await GetProductByIdAsync(product.ProductId);
        }

        public async Task<bool> UpdateProductAsync(int productId, UpdateProductDto updateProductDto)
        {
            // Lấy sản phẩm cùng với các collection liên quan để EF Core có thể theo dõi thay đổi trên chúng
            var product = await _unitOfWork.Products.GetProductDetailByIdAsync(productId);
            if (product == null)
            {
                // _logger?.LogWarning($"Product with ID {productId} not found for update.");
                return false;
            }

            if (updateProductDto.ProductName != null)
                product.ProductName = updateProductDto.ProductName;
            if (updateProductDto.Description != null)
                product.Description = updateProductDto.Description;
            if (updateProductDto.Price.HasValue)
                product.Price = updateProductDto.Price.Value;
            if (updateProductDto.StockQuantity.HasValue)
                product.StockQuantity = updateProductDto.StockQuantity.Value;
            if (updateProductDto.IsActive.HasValue)
                product.IsActive = updateProductDto.IsActive.Value;

            if (updateProductDto.CategoryId.HasValue && updateProductDto.CategoryId.Value != product.CategoryId)
            {
                var categoryExists = await _unitOfWork.Categories.GetByIdAsync(updateProductDto.CategoryId.Value);
                if (categoryExists == null) { /* _logger?.LogWarning($"Invalid CategoryId {updateProductDto.CategoryId.Value} during product update."); */ return false; }
                product.CategoryId = updateProductDto.CategoryId.Value;
            }

            if (updateProductDto.BrandId.HasValue && updateProductDto.BrandId.Value != product.BrandId)
            {
                var brandExists = await _unitOfWork.Brands.GetByIdAsync(updateProductDto.BrandId.Value);
                if (brandExists == null) { /* _logger?.LogWarning($"Invalid BrandId {updateProductDto.BrandId.Value} during product update."); */ return false; }
                product.BrandId = updateProductDto.BrandId.Value;
            }

            product.LastUpdatedDate = DateTime.UtcNow;

            // Cập nhật hình ảnh: Xóa cũ, thêm mới (cách đơn giản)
            if (updateProductDto.Images != null)
            {
                // Để EF Core theo dõi việc xóa, cần xóa từng item khỏi collection mà context đang track
                // Hoặc nếu bạn có _context trực tiếp: _context.RemoveRange(product.ProductImages);
                // Cách an toàn hơn là xóa từng cái một khỏi collection của product
                var imagesToRemove = product.ProductImages.ToList(); // Tạo bản copy để duyệt và xóa
                foreach (var img in imagesToRemove)
                {
                    product.ProductImages.Remove(img);
                    // Nếu bạn quản lý file vật lý, đây là lúc để xóa file đó
                }
                // Hoặc: product.ProductImages.Clear(); // Điều này có thể không được EF Core theo dõi đúng cách trong mọi trường hợp
                // nếu không load product với tracking đầy đủ.
                // Với GetProductDetailByIdAsync đã include ProductImages, Clear() nên hoạt động.


                foreach (var imgDto in updateProductDto.Images)
                {
                    product.ProductImages.Add(new ProductImage
                    {
                        // ImageId sẽ tự động nếu là mới, hoặc bạn cần logic để cập nhật ImageId hiện có
                        ImageUrl = imgDto.ImageUrl,
                        IsPrimary = imgDto.IsPrimary
                        // ProductId sẽ được EF gán
                    });
                }
            }

            // Cập nhật thuộc tính: Xóa cũ, thêm mới (cách đơn giản)
            if (updateProductDto.Attributes != null)
            {
                var attributesToRemove = product.ProductAttributes.ToList();
                foreach (var attr in attributesToRemove)
                {
                    product.ProductAttributes.Remove(attr);
                }
                // Hoặc: product.ProductAttributes.Clear();


                foreach (var attrDto in updateProductDto.Attributes)
                {
                    product.ProductAttributes.Add(new ProductAttribute
                    {
                        AttributeName = attrDto.AttributeName,
                        AttributeValue = attrDto.AttributeValue
                        // ProductId sẽ được EF gán
                    });
                }
            }

            // Không cần gọi _unitOfWork.Products.UpdateAsync(product) vì product đã được tracked
            try
            {
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                // _logger?.LogError(ex, $"Error updating product with ID {productId}");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                // _logger?.LogWarning($"Product with ID {productId} not found for deletion.");
                return false;
            }

            // Lưu ý: EF Core sẽ tự động xử lý cascade delete cho ProductImages và ProductAttributes
            // nếu chúng được cấu hình onDelete: Cascade trong OnModelCreating của DbContext.
            // Nếu không, bạn có thể cần xóa chúng thủ công trước khi xóa product.
            // Với cấu hình mặc định (không có ClientSetNull), thì khi xóa Product, các record liên quan
            // trong ProductImages, ProductAttributes, OrderItems, Reviews (có ProductId là FK) sẽ bị xóa theo.
            // Tuy nhiên, Reviews có UserId là nullable và onDelete: SetNull, nên khi User bị xóa, Review.UserId sẽ là null.

            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
