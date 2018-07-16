using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    /// <summary>
    /// Dummy generic repo of dummy
    /// </summary>
    public interface IDummyRepository : IRepository<Dummy>
    {
        IEnumerable<Dummy> GetTopDummies(int count);
        IEnumerable<Dummy> GetDummiesWihtSomething(int a, int b);
    }
}
