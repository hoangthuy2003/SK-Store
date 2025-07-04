﻿// File: Repositories/UnitOfWork/UnitOfWork.cs
using Repositories.Implementations;
using Repositories.Interfaces;
using System.Threading.Tasks;
using Repositories; // Namespace chứa SkstoreContext

namespace Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SkstoreContext _context; // DbContext của bạn

        // Khởi tạo các repository
        public IUserRepository Users { get; private set; }
        public IProductRepository Products { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IBrandRepository Brands { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IShoppingCartRepository ShoppingCarts { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IUserAddressRepository UserAddresses { get; private set; }

        // <<<<<<< THÊM IMPLEMENTATION CHO THUỘC TÍNH CONTEXT >>>>>>>
        /// <summary>
        /// Gets the database context.
        /// </summary>
        public SkstoreContext Context => _context; // Thêm dòng này

        public UnitOfWork(SkstoreContext context)
        {
            _context = context;

            // Truyền context cho các repository khi khởi tạo
            Users = new UserRepository(_context);
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context);
            Brands = new BrandRepository(_context);
            Roles = new RoleRepository(_context);
            Orders = new OrderRepository(_context); // Đảm bảo OrderRepository được khởi tạo
            ShoppingCarts = new ShoppingCartRepository(_context);
            Reviews = new ReviewRepository(_context);
            UserAddresses = new UserAddressRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
