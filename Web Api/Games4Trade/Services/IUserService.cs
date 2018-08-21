using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;

namespace Games4Trade.Services
{
    public interface IUserService
    {
        Task<IList<UserDto>> Get();
        Task<bool> CreateUser(UserRegisterDto newUser);
        Task<bool> CheckIfEmailExists(string email);
    }
}