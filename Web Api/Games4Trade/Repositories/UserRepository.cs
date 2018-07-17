using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Models;
using Games4Trade.Persistence;

namespace Games4Trade.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext context) : base(context) { }
    }
}
