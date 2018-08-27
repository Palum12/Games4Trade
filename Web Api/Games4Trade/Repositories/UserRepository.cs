using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext context) : base(context) { }

        public async Task<User> GetUserByLogin(string login)
        {
            return await Context.Users.SingleOrDefaultAsync(u => u.Login.Equals(login));
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await Context.Users.SingleOrDefaultAsync(u => u.Email.Equals(email));
        }

        public async Task<User> GetUserByRecoveryAddress(string recoveryAddress)
        {
            return await Context.Users.SingleOrDefaultAsync(u => u.RecoveryAddress.Equals(recoveryAddress));
        }
    }
}
