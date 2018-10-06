using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade.Services
{
    public interface IUserService
    {
        Task<OperationResult> AddObservedUser(ObservedUsersRelationshipDto pair);
        Task<OperationResult> DeleteObservedUser(ObservedUsersRelationshipDto pair);
        Task<IList<UserDto>> Get();
        Task<UserDto> GetUserById(int id);
        Task<int?> GetUserIdByLogin(string login);
        Task<IList<UserDto>> GetObservedUsersForUser(int userId);
        Task<OperationResult> CreateUser(UserRegisterDto newUser);
        Task<OperationResult> CheckIfEmailExists(string email);
    }
}