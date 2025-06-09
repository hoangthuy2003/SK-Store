// File: Application/DTOs/ProductDetailDto.cs
using System;
using System.Collections.Generic;

namespace Application.DTOs.Product
{
    /// <summary>
    /// DTO hiển thị thông tin chi tiết đầy đủ của một sản phẩm.
    /// </summary>
    public class ProductDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public int CategoryId { get; set; } // ID để tham chiếu
        public string CategoryName { get; set; } = null!; // Tên để hiển thị

        public int BrandId { get; set; } // ID để tham chiếu
        public string BrandName { get; set; } = null!; // Tên để hiển thị

        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        public ICollection<ProductImageDto> ProductImages { get; set; } = new List<ProductImageDto>();
        public ICollection<ProductAttributeDto> ProductAttributes { get; set; } = new List<ProductAttributeDto>();
        public ICollection<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
