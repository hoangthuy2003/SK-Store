using Application.DTOs.Category;
using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CategoryFilterParameters filterParams);
        Task<CategoryDto?> GetCategoryByIdAsync(int categoryId);
        Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<bool> UpdateCategoryAsync(int categoryId, UpdateCategoryDto updateCategoryDto);
        Task<(bool Success, string ErrorMessage)> DeleteCategoryAsync(int categoryId);
        Task<int> GetTotalCategoriesCountAsync();
    }
}
