using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Repositories
{
    public class AnnouncementRepository : Repository<Announcement>, IAnnouncementReposiotry
    {
        public AnnouncementRepository(ApplicationContext context) : base(context) {}

        public async Task<Announcement> GetAnnouncementWithAuthor(int id)
        {
            return await Context.Announcements.Include(a => a.User).SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Announcement>> GetAnnouncementsWithAuthors()
        {
            return await Context.Announcements.Include(a => a.User).ToListAsync();
        }
    }
}
