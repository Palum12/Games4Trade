using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Repositories
{
    public class AdvertisementRepository : Repository<Advertisement>, IAdvertisementReposiotry
    {
        public AdvertisementRepository(ApplicationContext context) : base(context) { }

        public async Task<Advertisement> GetAdvertisementWithItem(int id)
        {
            return await Context.Advertisements.Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
