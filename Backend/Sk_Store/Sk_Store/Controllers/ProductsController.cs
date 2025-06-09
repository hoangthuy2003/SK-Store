// File: Controllers/ProductsController.cs
using Application.DTOs; // Namespace chứa các DTOs như ProductDto, CreateProductDto...
using Application.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories; // Namespace chứa ProductFilterParameters
using Services.Interfaces;
using Sk_Store.Services.Interfaces; // Namespace chứa IProductService
using System.Collections.Generic;
using System.Threading.Tasks;
// using Microsoft.Extensions.Logging; // Bỏ comment nếu bạn muốn dùng ILogger

namespace Sk_Store.Controllers // Đảm bảo namespace này đúng với project của bạn
{
    [Route("api/[controller]")] // Đường dẫn cơ sở: /api/products
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        // private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService /*, ILogger<ProductsController> logger*/)
        {
            _productService = productService;
            // _logger = logger;
        }

        // GET: api/products
        /// <summary>
        /// Lấy danh sách sản phẩm với các tùy chọn lọc và phân trang.
        /// </summary>
        /// <remarks>
        /// Ví dụ request:
        ///
        ///     GET /api/products?SearchTerm=pen&CategoryId=1&BrandId=2&PageNumber=1&PageSize=10
        ///
        /// </remarks>
        /// <param name="filterParams">Đối tượng chứa các tham số lọc và phân trang.</param>
        /// <returns>Danh sách sản phẩm.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilterParameters filterParams)
        {
            // Bạn có thể thêm validation cho filterParams ở đây nếu cần,
            // ví dụ: PageSize không được quá lớn.
            if (filterParams.PageSize > 100) // Ví dụ giới hạn PageSize
            {
                filterParams.PageSize = 100;
            }
            var products = await _productService.GetProductsAsync(filterParams);
            return Ok(products);
        }

        // GET: api/products/{id}
        /// <summary>
        /// Lấy thông tin chi tiết của một sản phẩm theo ID.
        /// </summary>
        /// <param name="id">ID của sản phẩm.</param>
        /// <returns>Thông tin chi tiết sản phẩm hoặc 404 Not Found.</returns>
        [HttpGet("{id:int}")] // Thêm ràng buộc :int để đảm bảo id là số nguyên
        [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID sản phẩm không hợp lệ." });
            }
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                // _logger?.LogInformation($"Product with id {id} not found.");
                return NotFound(new { message = $"Sản phẩm với ID {id} không tồn tại." });
            }
            return Ok(product);
        }

        // POST: api/products
        /// <summary>
        /// Tạo một sản phẩm mới (Yêu cầu quyền Admin).
        /// </summary>
        /// <param name="createProductDto">DTO chứa thông tin để tạo sản phẩm.</param>
        /// <returns>Thông tin sản phẩm vừa tạo hoặc lỗi nếu có.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProduct = await _productService.CreateProductAsync(createProductDto);
            if (createdProduct == null)
            {
                // _logger?.LogWarning("Failed to create product. Service returned null. Input DTO: {@CreateProductDto}", createProductDto);
                return BadRequest(new { message = "Không thể tạo sản phẩm. Vui lòng kiểm tra lại thông tin đầu vào (ví dụ: CategoryId, BrandId có tồn tại không, hoặc các trường bắt buộc đã được cung cấp)." });
            }

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
        }

        // PUT: api/products/{id}
        /// <summary>
        /// Cập nhật thông tin một sản phẩm (Yêu cầu quyền Admin).
        /// </summary>
        /// <param name="id">ID của sản phẩm cần cập nhật.</param>
        /// <param name="updateProductDto">DTO chứa thông tin cập nhật.</param>
        /// <returns>204 No Content nếu thành công, hoặc lỗi nếu có.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID sản phẩm không hợp lệ." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _productService.UpdateProductAsync(id, updateProductDto);
            if (!success)
            {
                // _logger?.LogWarning($"Failed to update product with ID {id}. Service returned false. Input DTO: {@UpdateProductDto}", updateProductDto);
                // Kiểm tra xem sản phẩm có tồn tại không trước khi trả về NotFound,
                // vì service có thể trả về false do lỗi validation nội bộ (ví dụ: CategoryId mới không tồn tại)
                var productExists = await _productService.GetProductByIdAsync(id);
                if (productExists == null)
                {
                    return NotFound(new { message = $"Sản phẩm với ID {id} không tồn tại." });
                }
                return BadRequest(new { message = $"Không thể cập nhật sản phẩm với ID {id}. Vui lòng kiểm tra lại thông tin cập nhật." });
            }

            return NoContent();
        }

        // DELETE: api/products/{id}
        /// <summary>
        /// Xóa một sản phẩm (Yêu cầu quyền Admin).
        /// </summary>
        /// <param name="id">ID của sản phẩm cần xóa.</param>
        /// <returns>204 No Content nếu thành công, hoặc lỗi nếu có.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID sản phẩm không hợp lệ." });
            }

            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                // _logger?.LogWarning($"Failed to delete product with ID {id}. Service returned false (product might not exist).");
                return NotFound(new { message = $"Sản phẩm với ID {id} không tồn tại hoặc không thể xóa." });
            }
            return NoContent();
        }
    }
}
