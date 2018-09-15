using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade.Services
{
    public interface ISystemService
    {
        Task<IList<SystemGetDto>> GetSystems();
        Task<OperationResult> CreateSystem(SystemCreateOrUpdateDto system);
        Task<OperationResult> EditSystem(int id, SystemCreateOrUpdateDto system);
        Task<OperationResult> DeleteSystem(int id);
    }
}