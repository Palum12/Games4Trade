using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Games4TradeAPI.Core.Mapping;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;

namespace Games4TradeAPI.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRepository<Region> repository; 

        public RegionService(IRepository<Region> repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<RegionDto>> Get()
        {
            var regions = await repository.GetAllAsync();
            return regions.Select(region => region.ToDto());
        }
    }
}
