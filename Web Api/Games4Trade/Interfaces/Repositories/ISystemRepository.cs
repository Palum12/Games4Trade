using System.Collections.Generic;
using System.Threading.Tasks;

namespace Games4Trade.Interfaces.Repositories
{
    public interface ISystemRepository : IRepository<Models.System>
    {
        Task<Models.System> GetSystemWithItems(int id);
        Task<Models.System> GetSameSystem(Models.System system);
        Task<IList<Models.System>> GetSystemsForUser(int userId);
    }
}