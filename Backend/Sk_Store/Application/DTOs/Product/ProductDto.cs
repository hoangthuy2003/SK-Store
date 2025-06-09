// File: Application/DTOs/ProductDto.cs
namespace Application.DTOs.Product
{
    /// <summary>
    /// DTO hiển thị thông tin tóm tắt của sản phẩm (dùng cho danh sách).
    /// </summary>
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? PrimaryImageUrl { get; set; } // URL hình ảnh đại diện
        public string CategoryName { get; set; } = null!; // Tên danh mục
        public string BrandName { get; set; } = null!; // Tên thương hiệu
        public double AverageRating { get; set; } // Đánh giá trung bình
        public int ReviewCount { get; set; } // Số lượng đánh giá
    }
}
