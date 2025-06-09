using Application.DTOs.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();
        Task<BrandDto?> GetBrandByIdAsync(int brandId);
        Task<BrandDto?> CreateBrandAsync(CreateBrandDto createBrandDto);
        Task<bool> UpdateBrandAsync(int brandId, UpdateBrandDto updateBrandDto);
        Task<bool> DeleteBrandAsync(int brandId);
    }
}
