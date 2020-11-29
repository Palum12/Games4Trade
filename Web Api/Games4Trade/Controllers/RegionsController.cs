using System.Threading.Tasks;
using Games4TradeAPI.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Games4TradeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var regions = await _regionService.Get();
            return Ok(regions);
        }
    }
}
