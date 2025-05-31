// File: Repositories/UnitOfWork/UnitOfWork.cs
using Repositories.Implementations;
using Repositories.Interfaces;
using System.Threading.Tasks;

namespace Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SkstoreContext _context;

        // Khởi tạo các repository
        public IUserRepository Users { get; private set; }
        public IProductRepository Products { get; private set; }

        public UnitOfWork(SkstoreContext context)
        {
            _context = context;

            // Truyền context cho các repository khi khởi tạo
            Users = new UserRepository(_context);
            Products = new ProductRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            // Gọi SaveChangesAsync của DbContext
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}