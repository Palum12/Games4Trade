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
    public class StateService : IStateService
    {
        private readonly IRepository<State> repository;

        public StateService(IRepository<State> repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<StateDto>> Get()
        {
            var states = await repository.GetAllAsync();
            return states.Select(state => state.ToDto());
        }
    }
}
