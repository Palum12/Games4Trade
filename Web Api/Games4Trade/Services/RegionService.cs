using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;

namespace Games4TradeAPI.Services
{
    public class RegionService : IRegionService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Region> repository; 

        public RegionService(IRepository<Region> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<RegionDto>> Get()
        {
            var regions = await repository.GetAllAsync();
            var dtos = mapper.Map<IEnumerable<Region>, IEnumerable<RegionDto>>(regions);
            return dtos;
        }
    }
}
