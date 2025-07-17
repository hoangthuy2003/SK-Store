using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Product
{
    /// <summary>
    /// DTO d�ng ?? t?o s?n ph?m v?i file upload
    /// </summary>
    public class CreateProductWithFilesDto
    {
        [Required(ErrorMessage = "T�n s?n ph?m kh�ng ???c ?? tr?ng.")]
        [MaxLength(255, ErrorMessage = "T�n s?n ph?m kh�ng ???c v??t qu� 255 k� t?.")]
        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Gi� s?n ph?m kh�ng ???c ?? tr?ng.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Gi� s?n ph?m ph?i l?n h?n 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "S? l??ng t?n kho kh�ng ???c ?? tr?ng.")]
        [Range(0, int.MaxValue, ErrorMessage = "S? l??ng t?n kho kh�ng ???c l� s? �m.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Vui l�ng ch?n danh m?c.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui l�ng ch?n th??ng hi?u.")]
        public int BrandId { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Danh s�ch file ?nh upload
        /// </summary>
        public IFormFile[]? ImageFiles { get; set; }

        /// <summary>
        /// Index c?a ?nh ch�nh trong danh s�ch ImageFiles (b?t ??u t? 0)
        /// </summary>
        public int PrimaryImageIndex { get; set; } = 0;

        /// <summary>
        /// Danh s�ch thu?c t�nh s?n ph?m (JSON string ho?c form data)
        /// </summary>
        public List<CreateProductAttributeDto>? Attributes { get; set; }
    }

    /// <summary>
    /// DTO d�ng ?? c?p nh?t s?n ph?m v?i file upload
    /// </summary>
    public class UpdateProductWithFilesDto
    {
        [MaxLength(255, ErrorMessage = "T�n s?n ph?m kh�ng ???c v??t qu� 255 k� t?.")]
        public string? ProductName { get; set; }

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Gi� s?n ph?m ph?i l?n h?n 0.")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "S? l??ng t?n kho kh�ng ???c l� s? �m.")]
        public int? StockQuantity { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }

        public bool? IsActive { get; set; }

        /// <summary>
        /// Danh s�ch file ?nh m?i c?n upload
        /// </summary>
        public IFormFile[]? ImageFiles { get; set; }

        /// <summary>
        /// Index c?a ?nh ch�nh trong danh s�ch ImageFiles m?i (b?t ??u t? 0)
        /// </summary>
        public int? PrimaryImageIndex { get; set; }

        /// <summary>
        /// C� x�a t?t c? ?nh c? kh�ng (true = x�a h?t v� thay b?ng ?nh m?i)
        /// </summary>
        public bool ReplaceAllImages { get; set; } = false;

        /// <summary>
        /// Danh s�ch thu?c t�nh s?n ph?m
        /// </summary>
        public List<UpdateProductAttributeDto>? Attributes { get; set; }
    }
}