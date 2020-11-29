using System.Collections.Generic;
using System.Threading.Tasks;
using Games4TradeAPI.Dtos;

namespace Games4TradeAPI.Interfaces.Services
{
    public interface IStateService
    {
        Task<IEnumerable<StateDto>> Get();
    }
}
