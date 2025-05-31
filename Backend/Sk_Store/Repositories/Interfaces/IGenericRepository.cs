using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        //GetAll tra ve kieu IEnumerable (danh sach doi tuong)
        Task<IEnumerable<T>> GetAllAsync();

        //GetById tra ve 1 doi tuong, ? sau T de neu ko thay thi tra ve null
        Task<T?> GetByIdAsync(object id);

        //Tra ve list doi tuong thoa man dieu kien predicate
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        //Them doi tuong moi
        Task AddAsync(T entity);

        //Cap nhat doi tuong co san
        Task UpdateAsync(T entity);

        //Xoa doi tuong co san
        Task DeleteAsync(T entity);

    }
}
