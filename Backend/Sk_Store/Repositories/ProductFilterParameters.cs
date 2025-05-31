using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProductFilterParameters
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? SearchTerm { get; set; }


        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}
