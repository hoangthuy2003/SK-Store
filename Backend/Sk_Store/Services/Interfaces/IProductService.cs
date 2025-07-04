﻿// File: Services/Interfaces/IProductService.cs
using Application.DTOs;
using Application.DTOs.Product;
using Repositories; // For ProductFilterParameters
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces // Đảm bảo namespace này đúng với project của bạn
{
    public interface IProductService
    {
        /// <summary>
        /// Lấy danh sách sản phẩm dựa trên các tiêu chí lọc và phân trang.
        /// </summary>
        /// <param name="filterParams">Các tham số để lọc và phân trang sản phẩm.</param>
        /// <returns>Danh sách ProductDto.</returns>
        Task<IEnumerable<ProductDto>> GetProductsAsync(ProductFilterParameters filterParams);

        /// <summary>
        /// Lấy thông tin chi tiết của một sản phẩm dựa trên ID.
        /// </summary>
        /// <param name="productId">ID của sản phẩm.</param>
        /// <returns>ProductDetailDto hoặc null nếu không tìm thấy.</returns>
        Task<ProductDetailDto?> GetProductByIdAsync(int productId);

        /// <summary>
        /// Tạo một sản phẩm mới.
        /// </summary>
        /// <param name="createProductDto">DTO chứa thông tin để tạo sản phẩm.</param>
        /// <returns>ProductDetailDto của sản phẩm vừa tạo.</returns>
        Task<ProductDetailDto?> CreateProductAsync(CreateProductDto createProductDto);

        /// <summary>
        /// Cập nhật thông tin một sản phẩm hiện có.
        /// </summary>
        /// <param name="productId">ID của sản phẩm cần cập nhật.</param>
        /// <param name="updateProductDto">DTO chứa thông tin cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại (ví dụ: sản phẩm không tồn tại).</returns>
        Task<bool> UpdateProductAsync(int productId, UpdateProductDto updateProductDto);

        /// <summary>
        /// Xóa một sản phẩm.
        /// </summary>
        /// <param name="productId">ID của sản phẩm cần xóa.</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại.</returns>
        Task<(bool Success, string ErrorMessage)> DeleteProductAsync(int productId);
        Task<int> CountProductsAsync(ProductFilterParameters filterParams);
        // Bạn có thể thêm các phương thức khác ở đây nếu cần, ví dụ:
        // Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync();
        // Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int productId);
    }
}
