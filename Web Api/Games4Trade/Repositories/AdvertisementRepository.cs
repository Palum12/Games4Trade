using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Repositories
{
    public class AdvertisementRepository : Repository<Advertisement>, IAdvertisementReposiotry
    {
        public AdvertisementRepository(ApplicationContext context) : base(context) { }

        public async Task<Advertisement> GetAdvertisementWithItem(int id, int? userId = null)
        {
            return await Context.Advertisements
                .Where(a => a.IsActive || (userId.HasValue && a.UserId == userId))
                .Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Advertisement>> Get(int page, int pageSize)
        {
            var skip = page * pageSize;
            return await Context.Advertisements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.DateCreated)
                .Skip(skip).Take(pageSize)
                .Include(a => a.Item).ToListAsync();
        }

        public async Task<IEnumerable<Advertisement>> GetRecommendedAdvertisements(int userId)
        {
            var genres = await Context.UserGenreRelationship
                .Where(x => x.UserId == userId).Select(x => x.GenreId).ToArrayAsync();
            var systems = await Context.UserSystemRelationship
                .Where(x => x.UserId == userId).Select(x => x.SystemId).ToArrayAsync();
            var observedUsers = await Context.ObservedUsersRelationship
                .Where(u => u.ObservingUserId == userId).Select(x => x.ObservedUserId).ToArrayAsync();

            var ads = await Context.Advertisements.Include(a => a.Item)
                .Where( a => a.IsActive && (observedUsers.Contains(a.UserId) || systems.Contains(a.Item.SystemId) || 
                            (a.Item is Game && genres.Contains(((Game)a.Item).GenreId)))  )
                .Take(10)
                .OrderByDescending(a => a.DateCreated)
                .ToArrayAsync();

            return ads;
        }

        public async Task<IEnumerable<Advertisement>> GetAdsForUser(int userId, int page, int pageSize)
        {
            var skip = page * pageSize;
            return await Context.Advertisements
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.DateCreated)
                .Skip(skip).Take(pageSize)
                .ToListAsync();
        }
    }
}
