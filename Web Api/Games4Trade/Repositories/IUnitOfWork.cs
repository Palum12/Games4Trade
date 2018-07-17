using System;
using System.Threading.Tasks;

namespace Games4Trade.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        Task<int> CompleteASync();
    }
}