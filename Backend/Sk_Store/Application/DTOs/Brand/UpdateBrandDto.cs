using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Brand
{
    public class UpdateBrandDto
    {
        [MaxLength(100, ErrorMessage = "Tên thương hiệu không được vượt quá 100 ký tự.")]
        public string? BrandName { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }
    }
}
