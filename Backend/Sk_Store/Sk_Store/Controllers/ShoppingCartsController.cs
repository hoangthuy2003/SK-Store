// File: Sk_Store/Controllers/ShoppingCartsController.cs
using Application.DTOs.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces; // Hoặc Sk_Store.Services.Interfaces
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sk_Store.Controllers
{
    [Route("api/cart")] // Đổi thành "cart" cho ngắn gọn hơn "shoppingcarts"
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập cho tất cả các API giỏ hàng
    public class ShoppingCartsController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartsController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                // Sẽ không xảy ra nếu [Authorize] hoạt động đúng và token hợp lệ
                throw new UnauthorizedAccessException("Không thể xác định người dùng từ token.");
            }
            return userId;
        }

        // GET: api/cart
        /// <summary>
        /// Lấy giỏ hàng của người dùng hiện tại.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetCurrentUserId();
            var cart = await _shoppingCartService.GetCartAsync(userId);
            // GetCartAsync đã được điều chỉnh để trả về CartDto trống nếu không có giỏ hàng,
            // nên không cần kiểm tra null ở đây nữa.
            return Ok(cart);
        }

        // POST: api/cart/items
        /// <summary>
        /// Thêm một sản phẩm vào giỏ hàng.
        /// </summary>
        [HttpPost("items")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddItemToCartDto addItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            var (cart, errorMessage) = await _shoppingCartService.AddItemToCartAsync(userId, addItemDto);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            if (cart == null) // Trường hợp lỗi không mong muốn khác
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng." });
            }

            return Ok(cart);
        }

        // PUT: api/cart/items/{productId}
        /// <summary>
        /// Cập nhật số lượng của một sản phẩm trong giỏ hàng.
        /// </summary>
        [HttpPut("items/{productId:int}")]
        public async Task<IActionResult> UpdateCartItemQuantity(int productId, [FromBody] UpdateCartItemQuantityDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            var (cart, errorMessage) = await _shoppingCartService.UpdateItemQuantityAsync(userId, productId, updateDto);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            if (cart == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi cập nhật giỏ hàng." });
            }

            return Ok(cart);
        }

        // DELETE: api/cart/items/{productId}
        /// <summary>
        /// Xóa một sản phẩm khỏi giỏ hàng.
        /// </summary>
        [HttpDelete("items/{productId:int}")]
        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            var userId = GetCurrentUserId();
            var cart = await _shoppingCartService.RemoveItemFromCartAsync(userId, productId);
            if (cart == null)
            {
                // Có thể là giỏ hàng không tồn tại, hoặc sản phẩm không có trong giỏ.
                // Trả về giỏ hàng hiện tại (có thể trống) hoặc NotFound nếu logic yêu cầu.
                // Hiện tại service trả về giỏ hàng hiện tại nếu item không có, hoặc null nếu giỏ không tồn tại.
                // Để đơn giản, nếu cart là null (do giỏ không tồn tại), ta có thể lấy lại giỏ trống.
                var currentCart = await _shoppingCartService.GetCartAsync(userId);
                return Ok(currentCart);
            }
            return Ok(cart);
        }

        // DELETE: api/cart
        /// <summary>
        /// Xóa tất cả sản phẩm khỏi giỏ hàng (làm trống giỏ hàng).
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetCurrentUserId();
            var success = await _shoppingCartService.ClearCartAsync(userId);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi làm trống giỏ hàng." });
            }
            // Sau khi xóa, trả về giỏ hàng trống
            var emptyCart = await _shoppingCartService.GetCartAsync(userId);
            return Ok(emptyCart);
        }
    }
}
