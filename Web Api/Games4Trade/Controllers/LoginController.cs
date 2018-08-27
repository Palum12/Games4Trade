using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Games4Trade.Controllers
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
            if (string.IsNullOrEmpty(result.token))
            {
                return BadRequest(result.message);
            }
            return new ObjectResult(result.token);
        }
    }
}