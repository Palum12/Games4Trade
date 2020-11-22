using Games4TradeAPI.Data;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Repositories
{
    public class AdvertisementItemRepository : Repository<AdvertisementItem>
    {
        public AdvertisementItemRepository(ApplicationContext context) : base(context) { }
    }
}
