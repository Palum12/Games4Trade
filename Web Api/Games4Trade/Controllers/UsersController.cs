using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Microsoft.AspNetCore.Mvc;
using Games4Trade.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.Get();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            if (User.Identity.IsAuthenticated && await IsSelfService(id))
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            else if (User.Identity.IsAuthenticated)
            {
                var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
                var user = await _userService.GetUserProfile(id, currentUserId);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            else
            {
                var user = await _userService.GetUserProfile(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
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

        [HttpGet]
        [Authorize]
        [Route("id")]
        public async Task<IActionResult> GetLoggedUserId()
        {
            var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
            return Ok(currentUserId);
        }

        [HttpPatch]
        [Route("{id}/description")]
        [Authorize]
        public async Task<IActionResult> ChangeUserDescription(int id, [FromBody]string userDescription)
        {
            
            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            var result = await _userService.ChangeUserDescription(id, userDescription);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            return StatusCode(500, result.Message);
        }

        [HttpPatch]
        [Route("{id}/phone")]
        [Authorize]
        public async Task<IActionResult> ChangeUserPhone(int id,
            [FromBody]
            [Phone]
            [MinLength(7)]
            [MaxLength(11)]
            string phoneNumber)
        {

            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            var result = await _userService.ChangeUserPhone(id, phoneNumber);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            return StatusCode(500, result.Message);
        }

        [HttpPatch]
        [Route("{id}/email")]
        [Authorize]
        public async Task<IActionResult> ChangeUserEmail(int id, [FromBody][EmailAddress]string email)
        {

            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            var isEmailTaken = await _userService.CheckIfEmailExists(email);
            if (isEmailTaken.IsSuccessful)
            {
                return Conflict(isEmailTaken.Message);
            }

            var result = await _userService.ChangeUserEmail(id, email);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            return StatusCode(500, result.Message);
        }

        [HttpGet]
        [Route("{id}/photo")]
        public async Task<IActionResult> GetUserProfilePhoto(int id)
        {
            var bytes = await _userService.GetUserPhoto(id);
            if(bytes != null)
            {
                return File(bytes, "image/jpeg");
            }

            return NotFound();
        }

        [HttpPatch]
        [Route("{id}/photo")]
        [Authorize]
        public async Task<IActionResult> ChangeUserProfilePhoto(int id, [FromForm]IFormFile photo)
        {
            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            var acceptedExtensions = new [] { "jpg", "png", "jpeg", "bmp", "svg"};
            if (!acceptedExtensions.Any(e => photo.FileName.EndsWith(e)))
            {
                return BadRequest(
                    "The photo field only accepts files with the following extensions: .jpg, .png, .jpeg, .bmp, .svg");
            }
            if (photo.Length > 3_000_000)
            {
                return BadRequest("Too big file size!");
            }

            var result = await _userService.ChangeUserPhoto(id, photo);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            return StatusCode(500, result.Message);
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
        public async Task<IActionResult> GetObservedUsersForUser(int id, [FromQuery] int? page)
        {
            if (!await IsSelfService(id))
            {
                return Unauthorized();
            }

            IList<ObservedUserDto> users;
            if (page.HasValue)
            {
                page = page > 0 ? page - 1 : 0;
                users = await _userService.GetObservedUsersForUser(id, page);
            }
            else
            {
                users = await _userService.GetObservedUsersForUser(id);
            }
            
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

        private async Task<bool> IsSelfService(int userId)
        {
            var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
            return currentUserId.Value == userId;
        }
      
    }
}