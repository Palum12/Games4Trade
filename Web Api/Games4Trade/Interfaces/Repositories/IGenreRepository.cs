using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Models;

namespace Games4Trade.Interfaces.Repositories
{
    public interface IGenreRepository : IRepository<Genre>
    {
        Task<Genre> GetGenreWithGames(int id);
        Task<IList<Genre>> GetGenresForUser(int userId);
    }
}