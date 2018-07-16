using System;
using Games4Trade.Repositories;

namespace Games4Trade.Core
{
    interface IUnitOfWork : IDisposable
    {
        IDummyRepository Dummies { get; }
        int Complete();
    }
}
