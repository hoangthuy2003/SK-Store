// File: Application/DTOs/ReviewDto.cs
using System;

namespace Application.DTOs.Product
{
    /// <summary>
    /// DTO cho thông tin đánh giá sản phẩm.
    /// </summary>
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        // public int ProductId { get; set; } // Không cần thiết nếu đã nằm trong ProductDetailDto
        public string? UserName { get; set; } // Tên người dùng đánh giá (nếu có)
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
