using System;
using System.Collections.Generic;
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
            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var genres = await _genreService.GetGenresForUser(id);
            return Ok(genres);
        }

        [HttpPatch]
        [Route("{id}/genres")]
        [Authorize]
        public async Task<IActionResult> ChangeUserLikedGenres(int id, IList<int> likedGenres)
        {
            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            var result =  await _userService.ReplaceGenresForUser(id, likedGenres);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            if (result.IsClientError)
            {
                return BadRequest();
            }

            return StatusCode(500, result.Message);
        }

        [HttpGet]
        [Route("{id}/systems")]
        [Authorize]
        public async Task<IActionResult> GetSystemsForUser(int id)
        {
            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            var genres = await _systemService.GetSystemsForUser(id);
            return Ok(genres);
        }

        [HttpPatch]
        [Route("{id}/systems")]
        [Authorize]
        public async Task<IActionResult> ChangeUserLikedSystems(int id, IList<int> ownedSystems)
        {
            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            var result = await _userService.ReplaceSystemsForUser(id, ownedSystems);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            if (result.IsClientError)
            {
                return BadRequest();
            }

            return StatusCode(500, result.Message);
        }

        [HttpGet]
        [Route("{id}/observed")]
        [Authorize]
        public async Task<IActionResult> GetObservedUsersForUser(int id)
        {
            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            var users = await _userService.GetObservedUsersForUser(id);
            return Ok(users);
        }

        [Authorize]
        [Route("{id}/observed")]
        [HttpPost]
        public async Task<IActionResult> AddObservedUser([FromBody] ObservedUsersRelationshipDto pair)
        {
            if (!await IsSelfService(pair.ObservingUserId))
            {
                return Unauthorized();
            }

            var result = await _userService.AddObservedUser(pair);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            if (result.Payload != null)
            {
                return Conflict();
            }

            if (result.IsClientError)
            {
                return NotFound(result.Message);
            }
            return StatusCode(500, result.Message);
        }


        [Authorize]
        [Route("{id}/observed")]
        [HttpDelete]
        public async Task<IActionResult> DeleteObservedUser([FromBody] ObservedUsersRelationshipDto pair)
        {
            if (!await IsSelfService(pair.ObservingUserId))
            {
                return Unauthorized();
            }

            var result = await _userService.DeleteObservedUser(pair);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            if (result.IsClientError)
            {
                return NotFound();
            }

            return StatusCode(500, result.Message);
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

        private async Task<bool> IsSelfService(int userId)
        {
            var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
            return currentUserId.Value == userId;
        }
      
    }
}