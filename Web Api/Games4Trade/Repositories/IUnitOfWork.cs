using System;
using System.Threading.Tasks;
using Games4Trade.Models;
using Console = Games4Trade.Models.Console;

namespace Games4Trade.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IGenreRepository Genres { get; }
        ISystemRepository Systems { get; }
        Task<int> CompleteASync();
    }
}