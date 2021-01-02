using System.Threading.Tasks;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Common;

namespace Games4TradeAPI.Interfaces.Services
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