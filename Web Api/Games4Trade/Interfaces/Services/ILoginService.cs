using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade.Interfaces.Services
{
    public interface ILoginService
    {
        Task<OperationResult> LoginUser(UserLoginDto user);
        Task<OperationResult> CheckIfLoginIsTaken(string login);
        Task<OperationResult> ChangePassword(UserRecoverDto recoverDto);
        Task<OperationResult> RecoverPassword(string email);
        string ComputeHash(string salt, string password);
        string GetSalt();
    }
}