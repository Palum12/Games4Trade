using Games4Trade.Data;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext context) : base(context) { }
    }
}
