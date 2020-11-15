using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4TradeAPI.Interfaces.Services
{
    public interface ISystemService
    {
        Task<IList<SystemDto>> GetSystems();
        Task<IList<SystemDto>> GetSystemsForUser(int userId);
        Task<OperationResult> CreateSystem(SystemCreateOrUpdateDto system);
        Task<OperationResult> EditSystem(int id, SystemCreateOrUpdateDto system);
        Task<OperationResult> DeleteSystem(int id);
    }
}