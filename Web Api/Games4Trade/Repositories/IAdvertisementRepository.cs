using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    public interface IAdvertisementReposiotry : IRepository<Advertisement>
    {
        Task<Advertisement> GetAdvertisementWithItem(int id, int? userId = null);
        Task<IEnumerable<Advertisement>> Get(int page, int pageSize);
        Task<IEnumerable<Advertisement>> GetRecommendedAdvertisements(int userId, int howMany);
        Task<IEnumerable<Advertisement>> GetAdsForUser(int userId, int page, int pageSize, bool skipInActive);
    }
}
