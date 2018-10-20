using System.Threading.Tasks;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    public interface IAdvertisementReposiotry : IRepository<Advertisement>
    {
        Task<Advertisement> GetAdvertisementWithItem(int id);
    }
}
