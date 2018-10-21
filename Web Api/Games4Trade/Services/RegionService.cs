using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;

namespace Games4Trade.Services
{
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RegionDto>> Get()
        {
            var regions = await _unitOfWork.Regions.GetAllASync();
            var dtos = _mapper.Map<IEnumerable<Region>, IEnumerable<RegionDto>>(regions);
            return dtos;
        }
    }
}
