using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;
using Games4Trade.Interfaces.Repositories;

namespace Games4TradeAPI.Repositories
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

        public async Task<IList<User>> GetObservedUsersForUser(int userId, int? page = null, int? pageSize = null)
        {
            var idsOfObservedUser = await Context.ObservedUsersRelationship.Where(ou => ou.ObservingUserId == userId)
                .Select(ou => ou.ObservedUserId).ToArrayAsync();
            if (page.HasValue && pageSize.HasValue)
            {
                var skip = page * pageSize;
                return await Context.Users
                    .Where(u => idsOfObservedUser.Contains(u.Id))
                    .Skip(skip.Value).Take(pageSize.Value)
                    .ToListAsync();
            }
            return await Context.Users.Where(u => idsOfObservedUser.Contains(u.Id)).ToListAsync();
        }

        public async Task AddObsersvedUser(int observingUserId, int observedUserId)
        {
            await Context.ObservedUsersRelationship.AddAsync(new ObservedUsersRelationship
            {
                ObservingUserId = observingUserId,
                ObservedUserId = observedUserId
            });
        }

        public void DeleteObservedUser(int observingUserId, int observedUserId)
        {
            Context.ObservedUsersRelationship.Remove(new ObservedUsersRelationship
            {
                ObservingUserId = observingUserId,
                ObservedUserId = observedUserId
            });
        }

        public async Task ReplaceGenresForUser(int userId, IList<UserLikedGenre> pairs)
        {
            var listOfCurrentRelationships = await Context.UserGenreRelationship
                .Where(x => x.UserId == userId).ToListAsync();

            Context.UserGenreRelationship.RemoveRange(listOfCurrentRelationships);
            await Context.UserGenreRelationship.AddRangeAsync(pairs);
        }

        public async Task ReplaceSystemsForUser(int userId, IList<UserOwnedSystem> pairs)
        {
            var listOfCurrentRelationships = await Context.UserSystemRelationship
                .Where(x => x.UserId == userId).ToListAsync();

            Context.UserSystemRelationship.RemoveRange(listOfCurrentRelationships);
            await Context.UserSystemRelationship.AddRangeAsync(pairs);
        }
    }
}
