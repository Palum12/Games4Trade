using System.Threading.Tasks;
using Games4Trade.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Games4TradeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StatesController(IStateService stateService)
        {
            _stateService = stateService;
        }

        // GET: api/States
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var states = await _stateService.Get();
            return Ok(states);
        }

    }
}
