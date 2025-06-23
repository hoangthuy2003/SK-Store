using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBrandRepository:IGenericRepository<Brand>
    {
        Task<IEnumerable<Brand>> GetPagedBrandsAsync(BrandFilterParameters filterParams);
    }
}
