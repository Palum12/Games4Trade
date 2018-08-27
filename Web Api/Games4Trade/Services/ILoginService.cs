using System.Threading.Tasks;
using Games4Trade.Dtos;

namespace Games4Trade.Services
{
    public interface ILoginService
    {
        Task<(string token, string message)> LoginUser(UserLoginDto user);
        Task<bool> CheckIfLoginIsTaken(string login);
        string ComputeHash(string salt, string password);
        string GetSalt();
    }
}