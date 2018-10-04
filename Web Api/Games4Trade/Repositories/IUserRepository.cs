using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task AddObsersvedUser(int observingUserId, int observedUserId);
        Task<User> GetUserByLogin(string login);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByRecoveryAddress(string recoveryAddress);
        Task<IList<User>> GetObservedUsersForUser(int userId);
    }
}