using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Repositories
{
    public class AnnouncementRepository : Repository<Announcement>, IAnnouncementReposiotry
    {
        public AnnouncementRepository(ApplicationContext context) : base(context) {}

        public async Task<Announcement> GetAnnouncementWithAuthor(int id, bool isAdmin)
        {
            return await Context.Announcements.Include(a => a.User)
                .SingleOrDefaultAsync(a => a.Id == id && (isAdmin || a.IsActive));
        }

        public async Task<IEnumerable<Announcement>> GetAnnouncementsPageWithAuthors(int page, int pageSize, bool isAdmin)
        {
            var skip = page * pageSize;
            return await Context.Announcements
                .Where(a => isAdmin || a.IsActive)
                .OrderByDescending(a => a.DateCreated)
                .Skip(skip).Take(pageSize)
                .Include(a => a.User).ToListAsync();
        }
    }
}
