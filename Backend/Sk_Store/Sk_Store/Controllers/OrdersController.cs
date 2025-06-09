// File: Sk_Store/Controllers/OrdersController.cs
using Application.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces; // Hoặc Sk_Store.Services.Interfaces
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sk_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập cho hầu hết các API đơn hàng
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng từ token.");
            }
            return userId;
        }

        // POST: api/orders
        /// <summary>
        /// Tạo một đơn hàng mới từ giỏ hàng của người dùng.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto orderRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            var (order, errorMessage) = await _orderService.CreateOrderAsync(userId, orderRequestDto);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }
            if (order == null) // Lỗi không mong muốn khác
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi khi tạo đơn hàng." });
            }

            // Trả về 201 Created với thông tin đơn hàng vừa tạo và URL để truy cập nó
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        // GET: api/orders (Lịch sử đơn hàng của người dùng)
        /// <summary>
        /// Lấy lịch sử đơn hàng của người dùng đang đăng nhập (phân trang).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50; // Giới hạn kích thước trang

            var userId = GetCurrentUserId();
            var orders = await _orderService.GetOrdersForUserAsync(userId, pageNumber, pageSize);
            var totalOrders = await _orderService.CountOrdersForUserAsync(userId);

            Response.Headers.Append("X-Total-Count", totalOrders.ToString());
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

            return Ok(orders);
        }

        // GET: api/orders/{id} (Chi tiết đơn hàng cho người dùng hoặc admin)
        /// <summary>
        /// Lấy thông tin chi tiết của một đơn hàng theo ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userId = GetCurrentUserId();
            OrderDto? orderDetails;

            if (User.IsInRole("Admin")) // Nếu là Admin, có thể xem mọi đơn hàng
            {
                orderDetails = await _orderService.GetOrderDetailsAsync(id);
            }
            else // Nếu là user thường, chỉ xem được đơn của mình
            {
                orderDetails = await _orderService.GetOrderDetailsAsync(id, userId);
            }

            if (orderDetails == null)
            {
                return NotFound(new { message = $"Không tìm thấy đơn hàng với ID {id} hoặc bạn không có quyền truy cập." });
            }
            return Ok(orderDetails);
        }


        // === ADMIN ENDPOINTS ===
        // GET: api/orders/admin/all
        /// <summary>
        /// [Admin] Lấy tất cả đơn hàng với tùy chọn lọc và sắp xếp.
        /// </summary>
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrdersForAdmin(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? statusFilter = null,
            [FromQuery] string? sortOrder = "date_desc") // Mặc định: mới nhất trước
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var orders = await _orderService.GetAllOrdersAsync(pageNumber, pageSize, statusFilter, sortOrder);
            var totalOrders = await _orderService.CountAllOrdersAsync(statusFilter);

            Response.Headers.Append("X-Total-Count", totalOrders.ToString());
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

            return Ok(orders);
        }

        // PUT: api/orders/admin/{id}/status
        /// <summary>
        /// [Admin] Cập nhật trạng thái của một đơn hàng.
        /// </summary>
        [HttpPut("admin/{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var adminUserId = GetCurrentUserId(); // Lấy ID admin để ghi log nếu cần
            var success = await _orderService.UpdateOrderStatusAsync(id, statusDto.NewStatus, adminUserId);

            if (!success)
            {
                return BadRequest(new { message = $"Không thể cập nhật trạng thái cho đơn hàng ID {id}. Đơn hàng có thể không tồn tại hoặc trạng thái mới không hợp lệ." });
            }
            return Ok(new { message = $"Trạng thái đơn hàng ID {id} đã được cập nhật thành '{statusDto.NewStatus}'." });
        }
    }
}
