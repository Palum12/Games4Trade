using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    public interface IAnnouncementReposiotry : IRepository<Announcement>
    {
        Task<Announcement> GetAnnouncementWithAuthor(int id, bool isAdmin);
        Task<IEnumerable<Announcement>> GetAnnouncementsPageWithAuthors(int page, int pageSize, bool isAdmin);
    }
}
