using Application.DTOs.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IShoppingCartService
    {
        /// <summary>
        /// Lấy hoặc tạo giỏ hàng cho người dùng.
        /// </summary>
        Task<CartDto?> GetCartAsync(int userId);

        /// <summary>
        /// Thêm một sản phẩm vào giỏ hàng.
        /// </summary>
        /// <returns>Trả về CartDto sau khi thêm, hoặc null nếu có lỗi (ví dụ: sản phẩm không tồn tại, hết hàng).</returns>
        Task<(CartDto? Cart, string? ErrorMessage)> AddItemToCartAsync(int userId, AddItemToCartDto itemDto);

        /// <summary>
        /// Cập nhật số lượng của một sản phẩm trong giỏ hàng.
        /// </summary>
        /// <returns>Trả về CartDto sau khi cập nhật, hoặc null nếu có lỗi.</returns>
        Task<(CartDto? Cart, string? ErrorMessage)> UpdateItemQuantityAsync(int userId, int productId, UpdateCartItemQuantityDto quantityDto);

        /// <summary>
        /// Xóa một sản phẩm khỏi giỏ hàng.
        /// </summary>
        /// <returns>Trả về CartDto sau khi xóa, hoặc null nếu sản phẩm không có trong giỏ.</returns>
        Task<CartDto?> RemoveItemFromCartAsync(int userId, int productId);

        /// <summary>
        /// Xóa tất cả sản phẩm khỏi giỏ hàng.
        /// </summary>
        Task<bool> ClearCartAsync(int userId);
    }
}
