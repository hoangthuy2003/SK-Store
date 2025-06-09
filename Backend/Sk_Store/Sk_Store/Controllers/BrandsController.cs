// File: Controllers/BrandsController.cs
using Application.DTOs;
using Application.DTOs.Brand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Required for StatusCodes
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Sk_Store.Services.Interfaces; // Namespace for IBrandService
using System.Collections.Generic;
using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;

namespace Sk_Store.Controllers // Ensure this namespace matches your project
{
    [Route("api/[controller]")] // Base path: /api/brands
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        // private readonly ILogger<BrandsController> _logger;

        public BrandsController(IBrandService brandService /*, ILogger<BrandsController> logger*/)
        {
            _brandService = brandService;
            // _logger = logger;
        }

        // GET: api/brands
        /// <summary>
        /// Lấy tất cả thương hiệu.
        /// </summary>
        /// <returns>Danh sách các thương hiệu.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BrandDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
        }

        // GET: api/brands/{id}
        /// <summary>
        /// Lấy thông tin một thương hiệu theo ID.
        /// </summary>
        /// <param name="id">ID của thương hiệu.</param>
        /// <returns>Thông tin thương hiệu hoặc 404 Not Found.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBrandById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID thương hiệu không hợp lệ." });
            }
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
            {
                return NotFound(new { message = $"Thương hiệu với ID {id} không tồn tại." });
            }
            return Ok(brand);
        }

        // POST: api/brands
        /// <summary>
        /// Tạo một thương hiệu mới (Yêu cầu quyền Admin).
        /// </summary>
        /// <param name="createBrandDto">DTO chứa thông tin để tạo thương hiệu.</param>
        /// <returns>Thông tin thương hiệu vừa tạo hoặc lỗi nếu có.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandDto createBrandDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdBrand = await _brandService.CreateBrandAsync(createBrandDto);
            if (createdBrand == null)
            {
                return BadRequest(new { message = $"Không thể tạo thương hiệu. Tên '{createBrandDto.BrandName}' có thể đã tồn tại." });
            }
            return CreatedAtAction(nameof(GetBrandById), new { id = createdBrand.BrandId }, createdBrand);
        }

        // PUT: api/brands/{id}
        /// <summary>
        /// Cập nhật thông tin một thương hiệu (Yêu cầu quyền Admin).
        /// </summary>
        /// <param name="id">ID của thương hiệu cần cập nhật.</param>
        /// <param name="updateBrandDto">DTO chứa thông tin cập nhật.</param>
        /// <returns>204 No Content nếu thành công, hoặc lỗi nếu có.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] UpdateBrandDto updateBrandDto)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID thương hiệu không hợp lệ." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _brandService.UpdateBrandAsync(id, updateBrandDto);
            if (!success)
            {
                var brandExists = await _brandService.GetBrandByIdAsync(id);
                if (brandExists == null)
                {
                    return NotFound(new { message = $"Thương hiệu với ID {id} không tồn tại." });
                }
                return BadRequest(new { message = $"Không thể cập nhật thương hiệu. Tên thương hiệu mới có thể đã tồn tại hoặc có lỗi khác xảy ra." });
            }
            return NoContent();
        }

        // DELETE: api/brands/{id}
        /// <summary>
        /// Xóa một thương hiệu (Yêu cầu quyền Admin).
        /// </summary>
        /// <param name="id">ID của thương hiệu cần xóa.</param>
        /// <returns>204 No Content nếu thành công, hoặc lỗi nếu có.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID thương hiệu không hợp lệ." });
            }
            var success = await _brandService.DeleteBrandAsync(id);
            if (!success)
            {
                var brandExists = await _brandService.GetBrandByIdAsync(id);
                if (brandExists == null)
                {
                    return NotFound(new { message = $"Thương hiệu với ID {id} không tồn tại." });
                }
                return BadRequest(new { message = $"Không thể xóa thương hiệu với ID {id}. Thương hiệu này có thể đang chứa sản phẩm." });
            }
            return NoContent();
        }
    }
}
