// File: Services/Implementations/CategoryService.cs
using Application.DTOs;
using Application.DTOs.Category;
using BusinessObjects;
using Repositories.UnitOfWork;
using Services.Interfaces;
using Sk_Store.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;

namespace Services.Implementations // Đảm bảo namespace đúng
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork /*, ILogger<CategoryService> logger*/)
        {
            _unitOfWork = unitOfWork;
            // _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description
            });
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null) return null;
            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            // Kiểm tra xem tên danh mục đã tồn tại chưa (tùy chọn, nhưng nên có)
            var existingCategory = await _unitOfWork.Categories.FindAsync(c => c.CategoryName.ToLower() == createCategoryDto.CategoryName.ToLower());
            if (existingCategory.Any())
            {
                // _logger?.LogWarning($"Category with name '{createCategoryDto.CategoryName}' already exists.");
                return null; // Hoặc throw một custom exception / trả về thông báo lỗi cụ thể
            }

            var category = new Category
            {
                CategoryName = createCategoryDto.CategoryName,
                Description = createCategoryDto.Description
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null) return false;

            // Kiểm tra tên mới (nếu có) có trùng với category khác không
            if (!string.IsNullOrEmpty(updateCategoryDto.CategoryName) &&
                updateCategoryDto.CategoryName.ToLower() != category.CategoryName.ToLower())
            {
                var existingCategory = await _unitOfWork.Categories.FindAsync(c => c.CategoryName.ToLower() == updateCategoryDto.CategoryName.ToLower() && c.CategoryId != categoryId);
                if (existingCategory.Any())
                {
                    // _logger?.LogWarning($"Cannot update category. Name '{updateCategoryDto.CategoryName}' already exists for another category.");
                    return false;
                }
                category.CategoryName = updateCategoryDto.CategoryName;
            }

            if (updateCategoryDto.Description != null) // Cho phép xóa description bằng cách truyền ""
            {
                category.Description = updateCategoryDto.Description;
            }


            // _unitOfWork.Categories.UpdateAsync(category); // Không cần thiết nếu entity được track
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null) return false;

            // Kiểm tra xem có sản phẩm nào thuộc danh mục này không trước khi xóa (quan trọng!)
            var productsInCategory = await _unitOfWork.Products.FindAsync(p => p.CategoryId == categoryId);
            if (productsInCategory.Any())
            {
                // _logger?.LogWarning($"Cannot delete category '{category.CategoryName}' (ID: {categoryId}) as it contains products.");
                return false; // Hoặc throw exception / trả về thông báo lỗi
            }

            await _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
