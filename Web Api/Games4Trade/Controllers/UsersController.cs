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
        private readonly IGenreService _genreService;
        private readonly ISystemService _systemService;

        public UsersController(IUserService userService, IGenreService genreService, ISystemService systemService)
        {
            _userService = userService;
            _genreService = genreService;
            _systemService = systemService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var users = await _userService.Get();
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}/genres")]
        [Authorize]
        public async Task<IActionResult> GetGenresForUser(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var genres = await _genreService.GetGenresForUser(id);
            return Ok(genres);
        }

        [HttpGet]
        [Route("{id}/systems")]
        [Authorize]
        public async Task<IActionResult> GetSystemsForUser(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            var genres = await _systemService.GetSystemsForUser(id);
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserRegisterDto newUser)
        {
            var isEmailTaken = await _userService.CheckIfEmailExists(newUser.Email);
            if (isEmailTaken.IsSuccessful)
            {
                return Conflict(isEmailTaken.Message);
            }

            var result = await _userService.CreateUser(newUser);
            if (result.IsSuccessful)
            {
                return Ok();
            }
            return BadRequest(result.Message);

        }
      
    }
}