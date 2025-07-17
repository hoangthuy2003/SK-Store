// File: Repositories/UnitOfWork/IUnitOfWork.cs
using Repositories.Interfaces;
using System; // For IDisposable
using System.Threading.Tasks; // For Task
using Repositories; // Namespace chứa SkstoreContext

namespace Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Khai báo các interface của Repository ở đây
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IBrandRepository Brands { get; }
        IRoleRepository Roles { get; }
        IOrderRepository Orders { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        IReviewRepository Reviews { get; }
        IUserAddressRepository UserAddresses { get; }

        // <<<<<<< THÊM THUỘC TÍNH NÀY ĐỂ EXPOSE DBCONTEXT >>>>>>>
        /// <summary>
        /// Gets the database context.
        /// </summary>
        SkstoreContext Context { get; } // Thêm dòng này

        // Phương thức quan trọng nhất để lưu tất cả thay đổi
        Task<int> CompleteAsync();
    }
}
