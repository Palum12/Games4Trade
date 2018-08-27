using System.Threading.Tasks;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByLogin(string login);
    }
}