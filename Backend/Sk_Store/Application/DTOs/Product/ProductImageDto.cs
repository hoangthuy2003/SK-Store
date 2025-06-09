// File: Application/DTOs/ProductImageDto.cs
namespace Application.DTOs.Product
{
    /// <summary>
    /// DTO cho thông tin hình ảnh sản phẩm.
    /// </summary>
    public class ProductImageDto
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}
