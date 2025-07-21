using Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace Sk_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Tất cả endpoint đều yêu cầu đăng nhập
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressService _userAddressService;

        public UserAddressController(IUserAddressService userAddressService)
        {
            _userAddressService = userAddressService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return 0;
            }
            return userId;
        }

        /// <summary>
        /// Lấy tất cả địa chỉ của người dùng hiện tại
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var addresses = await _userAddressService.GetUserAddressesAsync(userId);
            return Ok(addresses);
        }

        /// <summary>
        /// Lấy địa chỉ mặc định của người dùng hiện tại
        /// </summary>
        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultAddress()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var address = await _userAddressService.GetDefaultAddressAsync(userId);
            if (address == null)
            {
                return NotFound(new { message = "Chưa có địa chỉ mặc định." });
            }

            return Ok(address);
        }

        /// <summary>
        /// Lấy địa chỉ theo ID
        /// </summary>
        [HttpGet("{addressId:int}")]
        public async Task<IActionResult> GetAddressById(int addressId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var address = await _userAddressService.GetUserAddressByIdAsync(addressId, userId);
            if (address == null)
            {
                return NotFound(new { message = "Không tìm thấy địa chỉ." });
            }

            return Ok(address);
        }

        /// <summary>
        /// Thêm địa chỉ mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] CreateUserAddressDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var (address, errorMessage) = await _userAddressService.AddUserAddressAsync(userId, createDto);
            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return CreatedAtAction(nameof(GetAddressById), new { addressId = address!.AddressId }, address);
        }

        /// <summary>
        /// Cập nhật địa chỉ
        /// </summary>
        [HttpPut("{addressId:int}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] UpdateUserAddressDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var (address, errorMessage) = await _userAddressService.UpdateUserAddressAsync(addressId, userId, updateDto);
            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(address);
        }

        /// <summary>
        /// Xóa địa chỉ
        /// </summary>
        [HttpDelete("{addressId:int}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var (success, errorMessage) = await _userAddressService.DeleteUserAddressAsync(addressId, userId);
            if (!success)
            {
                return BadRequest(new { message = errorMessage });
            }

            return NoContent();
        }

        /// <summary>
        /// Đặt địa chỉ làm mặc định
        /// </summary>
        [HttpPut("{addressId:int}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(int addressId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var (success, errorMessage) = await _userAddressService.SetDefaultAddressAsync(addressId, userId);
            if (!success)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new { message = "Đã đặt địa chỉ làm mặc định thành công." });
        }
    }
}
