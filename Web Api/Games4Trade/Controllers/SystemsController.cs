using System.Threading.Tasks;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Games4TradeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemsController : ControllerBase
    {
        private readonly ISystemService _systemService;

        public SystemsController(ISystemService systemService)
        {
            _systemService = systemService;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _systemService.GetSystems();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] SystemCreateOrUpdateDto system)
        {
            var result = await _systemService.CreateSystem(system);
            if (result.IsSuccessful)
            {
                return Ok(result.Payload);
            }

            if (result.Payload != null)
            {
                return Conflict("Object already exists!");
            }

            return StatusCode(500, result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SystemCreateOrUpdateDto system)
        {
            var result = await _systemService.EditSystem(id, system);
            if (result.IsSuccessful)
            {
                return Ok(result.Payload);
            }

            if (result.Payload == null)
            {
                return NotFound();
            }
            else
            {
                return Conflict();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _systemService.DeleteSystem(id);
            if (result.IsSuccessful)
            {
                return Ok();
            }
            else
            {
                if (result.Payload != null)
                {
                    return StatusCode(500, result.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
        }
    }
}
