using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;

namespace Games4TradeAPI.Interfaces.Services
{
    public interface IRegionService
    {
        Task<IEnumerable<RegionDto>> Get();
    }
}
