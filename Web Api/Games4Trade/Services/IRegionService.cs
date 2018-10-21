using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;

namespace Games4Trade.Services
{
    public interface IRegionService
    {
        Task<IEnumerable<RegionDto>> Get();
    }
}
