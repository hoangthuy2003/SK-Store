// File: Services/Implementations/BrandService.cs
using Application.DTOs;
using Application.DTOs.Brand;
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
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly ILogger<BrandService> _logger;

        public BrandService(IUnitOfWork unitOfWork /*, ILogger<BrandService> logger*/)
        {
            _unitOfWork = unitOfWork;
            // _logger = logger;
        }

        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.Brands.GetAllAsync();
            return brands.Select(b => new BrandDto
            {
                BrandId = b.BrandId,
                BrandName = b.BrandName,
                Description = b.Description
            });
        }

        public async Task<BrandDto?> GetBrandByIdAsync(int brandId)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);
            if (brand == null) return null;
            return new BrandDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Description = brand.Description
            };
        }

        public async Task<BrandDto?> CreateBrandAsync(CreateBrandDto createBrandDto)
        {
            var existingBrand = await _unitOfWork.Brands.FindAsync(b => b.BrandName.ToLower() == createBrandDto.BrandName.ToLower());
            if (existingBrand.Any())
            {
                // _logger?.LogWarning($"Brand with name '{createBrandDto.BrandName}' already exists.");
                return null;
            }

            var brand = new Brand
            {
                BrandName = createBrandDto.BrandName,
                Description = createBrandDto.Description
            };

            await _unitOfWork.Brands.AddAsync(brand);
            await _unitOfWork.CompleteAsync();

            return new BrandDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Description = brand.Description
            };
        }

        public async Task<bool> UpdateBrandAsync(int brandId, UpdateBrandDto updateBrandDto)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);
            if (brand == null) return false;

            if (!string.IsNullOrEmpty(updateBrandDto.BrandName) &&
                updateBrandDto.BrandName.ToLower() != brand.BrandName.ToLower())
            {
                var existingBrand = await _unitOfWork.Brands.FindAsync(b => b.BrandName.ToLower() == updateBrandDto.BrandName.ToLower() && b.BrandId != brandId);
                if (existingBrand.Any())
                {
                    // _logger?.LogWarning($"Cannot update brand. Name '{updateBrandDto.BrandName}' already exists for another brand.");
                    return false;
                }
                brand.BrandName = updateBrandDto.BrandName;
            }

            if (updateBrandDto.Description != null)
            {
                brand.Description = updateBrandDto.Description;
            }


            // _unitOfWork.Brands.UpdateAsync(brand);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteBrandAsync(int brandId)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);
            if (brand == null) return false;

            var productsInBrand = await _unitOfWork.Products.FindAsync(p => p.BrandId == brandId);
            if (productsInBrand.Any())
            {
                // _logger?.LogWarning($"Cannot delete brand '{brand.BrandName}' (ID: {brandId}) as it contains products.");
                return false;
            }

            await _unitOfWork.Brands.DeleteAsync(brand);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
