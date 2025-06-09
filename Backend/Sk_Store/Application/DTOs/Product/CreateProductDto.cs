// File: Application/DTOs/CreateProductDto.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Product
{
    /// <summary>
    /// DTO dùng để tạo mới một sản phẩm.
    /// </summary>
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tên sản phẩm không được vượt quá 255 ký tự.")]
        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho không được để trống.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được là số âm.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thương hiệu.")]
        public int BrandId { get; set; }

        public bool IsActive { get; set; } = true; // Mặc định là active khi tạo mới

        public List<CreateProductImageDto>? Images { get; set; }
        public List<CreateProductAttributeDto>? Attributes { get; set; }
    }

    /// <summary>
    /// DTO phụ trợ để tạo hình ảnh khi tạo sản phẩm.
    /// </summary>
    public class CreateProductImageDto
    {
        [Required(ErrorMessage = "URL hình ảnh không được để trống.")]
        [Url(ErrorMessage = "URL hình ảnh không hợp lệ.")]
        public string ImageUrl { get; set; } = null!;
        public bool IsPrimary { get; set; } = false;
    }

    /// <summary>
    /// DTO phụ trợ để tạo thuộc tính khi tạo sản phẩm.
    /// </summary>
    public class CreateProductAttributeDto
    {
        [Required(ErrorMessage = "Tên thuộc tính không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên thuộc tính không được vượt quá 100 ký tự.")]
        public string AttributeName { get; set; } = null!;

        [Required(ErrorMessage = "Giá trị thuộc tính không được để trống.")]
        [MaxLength(255, ErrorMessage = "Giá trị thuộc tính không được vượt quá 255 ký tự.")]
        public string AttributeValue { get; set; } = null!;
    }
}
