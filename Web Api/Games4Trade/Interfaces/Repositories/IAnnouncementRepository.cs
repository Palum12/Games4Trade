using System.Collections.Generic;
using System.Threading.Tasks;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Interfaces.Repositories
{
    public interface IAnnouncementRepository : IRepository<Announcement>
    {
        Task<Announcement> GetAnnouncementWithAuthor(int id, bool isAdmin);
        Task<IEnumerable<Announcement>> GetAnnouncementsPageWithAuthors(int page, int pageSize, bool isAdmin);
    }
}