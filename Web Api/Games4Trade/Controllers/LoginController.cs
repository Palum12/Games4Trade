using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Games4TradeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(UserLoginDto user)
        {
            var result = await _loginService.LoginUser(user);
            if (result.IsSuccessful)
            {
                return new ObjectResult(result.Payload);
            }
            return BadRequest(result.Message);
        }

        [HttpPost]
        [Route("password/change")]
        public async Task<IActionResult> ChangePassword(UserRecoverDto userRecoverDto)
        {
            var result = await _loginService.ChangePassword(userRecoverDto);
            if (result.IsSuccessful)
            {
                return Ok(result.Message);
            }
            return StatusCode(500, result.Message);
        }

        [HttpPost]
        [Route("password/recover")]
        public async Task<IActionResult> RecoverPassword(string email)
        {
            var result = await _loginService.RecoverPassword(email);
            if (result.IsSuccessful)
            {
                return Ok(result.Message);
            }
            return StatusCode(500, result.Message);
        }

        [HttpHead]
        public async Task<IActionResult> CheckIfLoginIsTaken(string login)
        {
            var result = await _loginService.CheckIfLoginIsTaken(login);
            if (result.IsSuccessful)
            {
                return Conflict();
            }
            return NotFound();
        }
    }
}