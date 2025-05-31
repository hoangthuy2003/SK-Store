// File: Repositories/UnitOfWork/IUnitOfWork.cs
using Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Khai báo các interface của Repository ở đây
        IUserRepository Users { get; }
        IProductRepository Products { get; }

        // Phương thức quan trọng nhất để lưu tất cả thay đổi
        Task<int> CompleteAsync();
    }
}