﻿using System.Collections.Generic;
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
        Task<IList<User>> GetObservedUsersForUser(int userId, int? page = null, int? pageSize = null);
        void DeleteObservedUser(int observingUserId, int observedUserId);
        Task ReplaceGenresForUser(int userId, IList<UserLikedGenre> pairs);
        Task ReplaceSystemsForUser(int userId, IList<UserOwnedSystem> pairs);
    }
}