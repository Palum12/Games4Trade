using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;

namespace Games4Trade.Services
{
    public interface IUserService
    {
        Task<IList<UserDto>> Get();
    }
}