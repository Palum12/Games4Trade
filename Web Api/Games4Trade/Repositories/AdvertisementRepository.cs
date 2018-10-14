
using Games4Trade.Data;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    public class AdvertisementRepository : Repository<Advertisement>, IAdvertisementReposiotry
    {
        public AdvertisementRepository(ApplicationContext context) : base(context) { }
    }
}
