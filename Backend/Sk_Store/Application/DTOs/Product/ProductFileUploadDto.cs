using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Product
{
    /// <summary>
    /// DTO dùng ?? t?o s?n ph?m v?i file upload
    /// </summary>
    public class CreateProductWithFilesDto
    {
        [Required(ErrorMessage = "Tên s?n ph?m không ???c ?? tr?ng.")]
        [MaxLength(255, ErrorMessage = "Tên s?n ph?m không ???c v??t quá 255 ký t?.")]
        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá s?n ph?m không ???c ?? tr?ng.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá s?n ph?m ph?i l?n h?n 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "S? l??ng t?n kho không ???c ?? tr?ng.")]
        [Range(0, int.MaxValue, ErrorMessage = "S? l??ng t?n kho không ???c là s? âm.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Vui lòng ch?n danh m?c.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng ch?n th??ng hi?u.")]
        public int BrandId { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Danh sách file ?nh upload
        /// </summary>
        public IFormFile[]? ImageFiles { get; set; }

        /// <summary>
        /// Index c?a ?nh chính trong danh sách ImageFiles (b?t ??u t? 0)
        /// </summary>
        public int PrimaryImageIndex { get; set; } = 0;

        /// <summary>
        /// Danh sách thu?c tính s?n ph?m (JSON string ho?c form data)
        /// </summary>
        public List<CreateProductAttributeDto>? Attributes { get; set; }
    }

    /// <summary>
    /// DTO dùng ?? c?p nh?t s?n ph?m v?i file upload
    /// </summary>
    public class UpdateProductWithFilesDto
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Thương hiệu không được để trống")]
        public int BrandId { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Các file ảnh mới cần upload
        /// </summary>
        public List<IFormFile>? ImageFiles { get; set; }

        /// <summary>
        /// Index của ảnh chính trong danh sách ImageFiles (0-based)
        /// </summary>
        public int PrimaryImageIndex { get; set; } = 0;

        /// <summary>
        /// Có thay thế tất cả ảnh hiện tại hay không
        /// </summary>
        public bool ReplaceAllImages { get; set; } = false;

        /// <summary>
        /// Danh sách ID của các ảnh cần xóa (khi ReplaceAllImages = false)
        /// </summary>
        public List<int>? ImagesToDelete { get; set; }
    }
}