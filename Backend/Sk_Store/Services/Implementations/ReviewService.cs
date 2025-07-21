// File: Services/Implementations/ReviewService.cs
using Application.DTOs.Product; // Namespace của ReviewDto
using Application.DTOs.Review;  // Namespace của CreateReviewDto
using BusinessObjects;
using Repositories.UnitOfWork;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Cho AnyAsync

namespace Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsForProductAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByProductIdAsync(productId);
            return reviews.Select(r => new ReviewDto
            {
                ReviewId = r.ReviewId,
                UserName = (r.User != null) ? $"{r.User.FirstName} {r.User.LastName}".Trim() : "Người dùng ẩn danh",
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate
            }).ToList();
        }

        private async Task<bool> CheckIfUserPurchasedProductAsync(int userId, int productId)
        {
            // Kiểm tra xem người dùng có mua sản phẩm này và sản phẩm đó đã được giao/hoàn thành không
            // Chỉ cho phép đánh giá với các trạng thái: Delivered hoặc Completed
            var validStatuses = new[] { "Delivered", "Completed" };
            
            // Debugging: Log thông tin
            Console.WriteLine($"[DEBUG] Checking purchase for User ID: {userId}, Product ID: {productId}");
            
            // Sử dụng một query duy nhất để kiểm tra điều kiện
            var result = await _unitOfWork.Context.Orders
                .Where(o => o.UserId == userId && validStatuses.Contains(o.OrderStatus))
                .AnyAsync(o => o.OrderItems.Any(oi => oi.ProductId == productId));
                               
            Console.WriteLine($"[DEBUG] User has purchased product with valid delivery status: {result}");
            return result;
        }

        public async Task<(ReviewDto? Review, string? ErrorMessage)> AddReviewAsync(int productId, int userId, CreateReviewDto createReviewDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
            {
                return (null, "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
            }

            // 1. Kiểm tra xem người dùng đã mua sản phẩm này và đã được giao/hoàn thành chưa
            var hasPurchased = await CheckIfUserPurchasedProductAsync(userId, productId);
            if (!hasPurchased)
            {
                return (null, "Bạn chỉ có thể đánh giá các sản phẩm bạn đã mua và đã được giao hoặc hoàn thành.");
            }

            // 2. Kiểm tra xem người dùng đã đánh giá sản phẩm này chưa
            var hasReviewed = await _unitOfWork.Reviews.HasUserReviewedProductAsync(userId, productId);
            if (hasReviewed)
            {
                return (null, "Bạn đã đánh giá sản phẩm này rồi.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId); // Lấy thông tin user để hiển thị tên

            var newReview = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment,
                ReviewDate = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(newReview);
            await _unitOfWork.CompleteAsync();

            // Sau khi lưu, newReview sẽ có ReviewId
            return (new ReviewDto
            {
                ReviewId = newReview.ReviewId,
                UserName = (user != null) ? $"{user.FirstName} {user.LastName}".Trim() : "Người dùng ẩn danh",
                Rating = newReview.Rating,
                Comment = newReview.Comment,
                ReviewDate = newReview.ReviewDate
            }, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteReviewAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
            {
                return (false, "Không tìm thấy đánh giá để xóa.");
            }

            await _unitOfWork.Reviews.DeleteAsync(review);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }
    }
}
