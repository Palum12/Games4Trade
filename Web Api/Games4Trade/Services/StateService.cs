using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;

namespace Games4TradeAPI.Services
{
    public class StateService : IStateService
    {
        private readonly IRepository<State> repository;
        private readonly IMapper mapper;

        public StateService(IRepository<State> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<StateDto>> Get()
        {
            var states = await repository.GetAllAsync();
            var dtos = mapper.Map<IEnumerable<State>, IEnumerable<StateDto>>(states);
            return dtos;
        }
    }
}
