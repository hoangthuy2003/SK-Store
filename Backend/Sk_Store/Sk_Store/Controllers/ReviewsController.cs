// File: Sk_Store/Controllers/ReviewsController.cs
using Application.DTOs.Product; // Namespace của ReviewDto
using Application.DTOs.Review;  // Namespace của CreateReviewDto
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces; // Hoặc Sk_Store.Services.Interfaces
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sk_Store.Controllers
{
    [Route("api")] // Sử dụng route chung
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                // Sẽ không xảy ra nếu [Authorize] hoạt động đúng và token hợp lệ
                // Trả về 0 hoặc một giá trị không hợp lệ để chỉ ra lỗi nếu cần xử lý ở nơi gọi
                return 0;
            }
            return userId;
        }

        // GET: api/products/{productId}/reviews
        /// <summary>
        /// Lấy tất cả đánh giá cho một sản phẩm.
        /// </summary>
        /// <param name="productId">ID của sản phẩm</param>
        [HttpGet("products/{productId:int}/reviews")]
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviews = await _reviewService.GetReviewsForProductAsync(productId);
            if (reviews == null || !reviews.Any())
            {
                return Ok(new List<ReviewDto>()); // Trả về danh sách rỗng nếu không có đánh giá
            }
            return Ok(reviews);
        }

        // POST: api/products/{productId}/reviews
        /// <summary>
        /// Người dùng thêm một đánh giá mới cho sản phẩm (yêu cầu đăng nhập và đã mua sản phẩm).
        /// </summary>
        /// <param name="productId">ID của sản phẩm</param>
        /// <param name="createReviewDto">Thông tin đánh giá</param>
        [HttpPost("products/{productId:int}/reviews")]
        [Authorize] // Yêu cầu người dùng phải đăng nhập
        public async Task<IActionResult> AddReview(int productId, [FromBody] CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == 0) // User ID không hợp lệ từ token
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var (review, errorMessage) = await _reviewService.AddReviewAsync(productId, userId, createReviewDto);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            if (review == null) // Lỗi không mong muốn khác
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi thêm đánh giá." });
            }

            // Trả về 201 Created với thông tin đánh giá vừa tạo
            // và URL để truy cập nó (nếu có endpoint riêng cho GetReviewById)
            // Hiện tại chưa có GetReviewById, nên trả về ReviewDto là đủ.
            return CreatedAtAction(nameof(GetReviewsForProduct), new { productId = productId }, review);
        }

        // DELETE: api/reviews/{reviewId}
        /// <summary>
        /// [Admin] Xóa một đánh giá.
        /// </summary>
        /// <param name="reviewId">ID của đánh giá cần xóa</param>
        [HttpDelete("reviews/{reviewId:int}")]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền xóa
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var (success, errorMessage) = await _reviewService.DeleteReviewAsync(reviewId);
            if (!success)
            {
                return BadRequest(new { message = errorMessage ?? "Không thể xóa đánh giá." });
            }
            return NoContent(); // 204 No Content
        }
    }
}
