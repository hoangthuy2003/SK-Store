// File: Application/DTOs/ProductAttributeDto.cs
namespace Application.DTOs.Product
{
    /// <summary>
    /// DTO cho thông tin thuộc tính sản phẩm.
    /// </summary>
    public class ProductAttributeDto
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } = null!;
        public string AttributeValue { get; set; } = null!;
    }
}
