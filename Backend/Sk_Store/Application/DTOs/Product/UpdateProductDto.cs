// File: Application/DTOs/UpdateProductDto.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Product
{
    /// <summary>
    /// DTO dùng để cập nhật thông tin một sản phẩm.
    /// Các trường có thể null để cho phép cập nhật một phần.
    /// </summary>
    public class UpdateProductDto
    {
        [MaxLength(255, ErrorMessage = "Tên sản phẩm không được vượt quá 255 ký tự.")]
        public string? ProductName { get; set; }

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được là số âm.")]
        public int? StockQuantity { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }

        public bool? IsActive { get; set; }

        // Quản lý hình ảnh và thuộc tính khi cập nhật có thể phức tạp hơn.
        // Ví dụ: cần ID để xác định hình ảnh/thuộc tính nào cần cập nhật hoặc xóa.
        // Dưới đây là một cách tiếp cận đơn giản, có thể cần điều chỉnh tùy theo logic nghiệp vụ.
        public List<UpdateProductImageDto>? Images { get; set; }
        public List<UpdateProductAttributeDto>? Attributes { get; set; }
    }

    /// <summary>
    /// DTO phụ trợ để cập nhật/thêm hình ảnh khi cập nhật sản phẩm.
    /// </summary>
    public class UpdateProductImageDto
    {
        public int? ImageId { get; set; } // ID của hình ảnh hiện có để cập nhật. Null nếu là hình ảnh mới.

        [Required(ErrorMessage = "URL hình ảnh không được để trống.")]
        [Url(ErrorMessage = "URL hình ảnh không hợp lệ.")]
        public string ImageUrl { get; set; } = null!;
        public bool IsPrimary { get; set; } = false;
    }

    /// <summary>
    /// DTO phụ trợ để cập nhật/thêm thuộc tính khi cập nhật sản phẩm.
    /// </summary>
    public class UpdateProductAttributeDto
    {
        public int? AttributeId { get; set; } // ID của thuộc tính hiện có để cập nhật. Null nếu là thuộc tính mới.

        [Required(ErrorMessage = "Tên thuộc tính không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên thuộc tính không được vượt quá 100 ký tự.")]
        public string AttributeName { get; set; } = null!;

        [Required(ErrorMessage = "Giá trị thuộc tính không được để trống.")]
        [MaxLength(255, ErrorMessage = "Giá trị thuộc tính không được vượt quá 255 ký tự.")]
        public string AttributeValue { get; set; } = null!;
    }
}
