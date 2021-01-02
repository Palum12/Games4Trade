using System.Collections.Generic;
using System.Threading.Tasks;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Common;

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