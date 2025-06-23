using Application.DTOs.Brand;
using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetAllBrandsAsync(BrandFilterParameters filterParams);
        Task<BrandDto?> GetBrandByIdAsync(int brandId);
        Task<BrandDto?> CreateBrandAsync(CreateBrandDto createBrandDto);
        Task<bool> UpdateBrandAsync(int brandId, UpdateBrandDto updateBrandDto);
        Task<(bool Success, string ErrorMessage)> DeleteBrandAsync(int brandId);
        Task<int> GetTotalBrandsCountAsync();
    }
}
