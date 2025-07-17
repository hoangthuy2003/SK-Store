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
        [MaxLength(255, ErrorMessage = "Tên s?n ph?m không ???c v??t quá 255 ký t?.")]
        public string? ProductName { get; set; }

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá s?n ph?m ph?i l?n h?n 0.")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "S? l??ng t?n kho không ???c là s? âm.")]
        public int? StockQuantity { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }

        public bool? IsActive { get; set; }

        /// <summary>
        /// Danh sách file ?nh m?i c?n upload
        /// </summary>
        public IFormFile[]? ImageFiles { get; set; }

        /// <summary>
        /// Index c?a ?nh chính trong danh sách ImageFiles m?i (b?t ??u t? 0)
        /// </summary>
        public int? PrimaryImageIndex { get; set; }

        /// <summary>
        /// Có xóa t?t c? ?nh c? không (true = xóa h?t và thay b?ng ?nh m?i)
        /// </summary>
        public bool ReplaceAllImages { get; set; } = false;

        /// <summary>
        /// Danh sách thu?c tính s?n ph?m
        /// </summary>
        public List<UpdateProductAttributeDto>? Attributes { get; set; }
    }
}