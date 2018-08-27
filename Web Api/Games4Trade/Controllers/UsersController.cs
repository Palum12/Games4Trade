using System.Threading.Tasks;
using Games4Trade.Dtos;
using Microsoft.AspNetCore.Mvc;
using Games4Trade.Services;
using Microsoft.AspNetCore.Authorization;

namespace Games4Trade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var users = await _userService.Get();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserRegisterDto newUser)
        {
            var isEmailTaken = await _userService.CheckIfEmailExists(newUser.Email);
            if (isEmailTaken)
            {
                return Conflict("This email adress is already taken");
            }

            var isSuccessful = await _userService.CreateUser(newUser);
            if (isSuccessful)
            {
                return Ok();
            }
            
            return BadRequest("Something went wrong, please contact administrator.");

        }
      
    }
}