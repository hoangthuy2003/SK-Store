using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(SkstoreContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Brand>> GetPagedBrandsAsync(BrandFilterParameters filterParams)
        {
            return await _dbSet
                .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                .Take(filterParams.PageSize)
                .ToListAsync();
        }
    }
}
