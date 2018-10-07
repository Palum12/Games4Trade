using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Microsoft.AspNetCore.Http;

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
        Task<OperationResult> ChangeUserDescription(int userId, string description);
        Task<Byte[]> GetUserPhoto(int userId);
        Task<OperationResult> ChangeUserPhoto(int userId, IFormFile photo);
        Task<OperationResult> ReplaceGenresForUser(int userId, IList<int> genresIds);
        Task<OperationResult> ReplaceSystemsForUser(int userId, IList<int> systemsIds);
    }
}