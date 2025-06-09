using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Category
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        // Có thể thêm số lượng sản phẩm thuộc danh mục này nếu cần
        // public int ProductCount { get; set; }
    }
}
