using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Brand
{
    public class BrandDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public string? Description { get; set; }
        // public int ProductCount { get; set; }
    }
}
