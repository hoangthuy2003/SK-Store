// File: Controllers/CategoriesController.cs
using Application.DTOs;
using Application.DTOs.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Required for StatusCodes
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Sk_Store.Services.Interfaces; // Namespace for ICategoryService
using System.Collections.Generic;
using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;

namespace Sk_Store.Controllers // Ensure this namespace matches your project
{
    [Route("api/[controller]")] // Base path: /api/categories
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        // private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService /*, ILogger<CategoriesController> logger*/)
        {
            _categoryService = categoryService;
            // _logger = logger;
        }

        // GET: api/categories
        /// <summary>
        /// Lấy tất cả danh mục.
        /// </summary>
        /// <returns>Danh sách các danh mục.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/categories/{id}
        /// <summary>
        /// Lấy thông tin một danh mục theo ID.
        /// </summary>
        /// <param name="id">ID của danh mục.</param>
        /// <returns>Thông tin danh mục hoặc 404 Not Found.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID danh mục không hợp lệ." });
            }
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = $"Danh mục với ID {id} không tồn tại." });
            }
            return Ok(category);
        }

        // POST: api/categories
        /// <summary>
        /// Tạo một danh mục mới (Yêu cầu quyền Admin).
        /// </summary>
        /// <param name="createCategoryDto">DTO chứa thông tin để tạo danh mục.</param>
        /// <returns>Thông tin danh mục vừa tạo hoặc lỗi nếu có.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCategory = await _categoryService.CreateCategoryAsync(createCategoryDto);
            if (createdCategory == null)
            {
                // Lý do có thể là tên danh mục đã tồn tại
                return BadRequest(new { message = $"Không thể tạo danh mục. Tên '{createCategoryDto.CategoryName}' có thể đã tồn tại." });
            }
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryId }, createdCategory);
        }

        // PUT: api/categories/{id}
        /// <summary>
        /// Cập nhật thông tin một danh mục (Yêu cầu quyền Admin).
        /// </summary>
        /// <param name="id">ID của danh mục cần cập nhật.</param>
        /// <param name="updateCategoryDto">DTO chứa thông tin cập nhật.</param>
        /// <returns>204 No Content nếu thành công, hoặc lỗi nếu có.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID danh mục không hợp lệ." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
            if (!success)
            {
                // Kiểm tra xem danh mục có tồn tại không, hoặc tên mới có bị trùng không
                var categoryExists = await _categoryService.GetCategoryByIdAsync(id);
                if (categoryExists == null)
                {
                    return NotFound(new { message = $"Danh mục với ID {id} không tồn tại." });
                }
                return BadRequest(new { message = $"Không thể cập nhật danh mục. Tên danh mục mới có thể đã tồn tại hoặc có lỗi khác xảy ra." });
            }
            return NoContent();
        }

        // DELETE: api/categories/{id}
        /// <summary>
        /// Xóa một danh mục (Yêu cầu quyền Admin).
        /// </summary>
        /// <param name="id">ID của danh mục cần xóa.</param>
        /// <returns>204 No Content nếu thành công, hoặc lỗi nếu có.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // For invalid operations like deleting a category with products
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID danh mục không hợp lệ." });
            }
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
            {
                // Kiểm tra xem danh mục có tồn tại không, hoặc có sản phẩm nào thuộc danh mục không
                var categoryExists = await _categoryService.GetCategoryByIdAsync(id);
                if (categoryExists == null)
                {
                    return NotFound(new { message = $"Danh mục với ID {id} không tồn tại." });
                }
                // CategoryService.DeleteCategoryAsync trả về false nếu có sản phẩm
                return BadRequest(new { message = $"Không thể xóa danh mục với ID {id}. Danh mục này có thể đang chứa sản phẩm." });
            }
            return NoContent();
        }
    }
}
