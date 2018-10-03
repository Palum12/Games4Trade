using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Data;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Repositories
{
    public class SystemRepository : Repository<Models.System>, ISystemRepository
    {
        public SystemRepository(ApplicationContext context) : base(context) { }

        public async Task<Models.System> GetSystemWithItems(int id)
        {
            return await Context.Systems.Include( s=> s.AdvertisementItems).Where(s => s.Id == id).SingleOrDefaultAsync();
        }

        public async Task<Models.System> GetSameSystem(Models.System system)
        {
            return await Context.Systems
                .Where(s => s.Manufacturer.Equals(system.Manufacturer, StringComparison.OrdinalIgnoreCase) &&
                            s.Model.Equals(system.Model, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefaultAsync();
        }

        public async Task<IList<Models.System>> GetSystemsForUser(int userId)
        {
            var arrayOfIds = await Context.UserSystemRelationship
                .Where(x => x.UserId == userId).Select(x => x.SystemId).ToArrayAsync();
            var systems = await Context.Systems.Where(s => arrayOfIds.Contains(s.Id)).ToListAsync();

            return systems;
        }
    }
}
