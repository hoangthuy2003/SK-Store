// File: Services/Interfaces/IReviewService.cs
using Application.DTOs.Product; // Namespace của ReviewDto
using Application.DTOs.Review;  // Namespace của CreateReviewDto
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces // Hoặc Sk_Store.Services.Interfaces
{
    public interface IReviewService
    {
        /// <summary>
        /// Lấy tất cả đánh giá cho một sản phẩm.
        /// </summary>
        Task<IEnumerable<ReviewDto>> GetReviewsForProductAsync(int productId);

        /// <summary>
        /// Người dùng thêm một đánh giá mới cho sản phẩm.
        /// </summary>
        /// <returns>ReviewDto của đánh giá vừa tạo, hoặc null nếu có lỗi.</returns>
        Task<(ReviewDto? Review, string? ErrorMessage)> AddReviewAsync(int productId, int userId, CreateReviewDto createReviewDto);

        /// <summary>
        /// (Admin) Xóa một đánh giá.
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteReviewAsync(int reviewId);
    }
}
