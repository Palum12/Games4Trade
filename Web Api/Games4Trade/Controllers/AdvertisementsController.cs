using System;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Games4Trade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;
        private readonly IUserService _userService;

        public AdvertisementsController(IAdvertisementService advertisementService, IUserService userService)
        {
            _advertisementService = advertisementService;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _advertisementService.GetAdvertisement(id);
            if (result.IsSuccessful)
            {
                return Ok(result.Payload);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("{adId}/photos/{photoId}")]
        public async Task<IActionResult> GetPhotos(int adId, int photoId)
        {
            var bytes = await _advertisementService.GetAdPhoto(adId, photoId);
            if (bytes != null)
            {
                return File(bytes, "image/jpeg");
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(AdvertisementSaveDto ad)
        {
            if (ModelState.IsValid)
            {
                var currentId = await GetCurrentUserId();
                var result = await _advertisementService.AddAdvertisement(currentId, ad);
                if (result.IsSuccessful)
                {
                    return Ok();
                }
                else if (result.IsClientError)
                {
                    return BadRequest(result.Message);
                }

                return StatusCode(500);
            }
            
            return BadRequest(OtherServices.ReturnAllModelErrors(ModelState));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, AdvertisementPutDto ad)
        {
            if (ModelState.IsValid)
            {
                throw new NotImplementedException();
            }
            return BadRequest(OtherServices.ReturnAllModelErrors(ModelState));
        }

        [HttpPatch]
        [Route("{id}/archived")]
        [Authorize]
        public async Task<IActionResult> Archive(int id)
        {
            var userId = await GetCurrentUserId();
            var result = await _advertisementService.ArchiveAdvertisement(userId, id);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            if (result.IsClientError)
            {
                return Forbid();
            }

            return StatusCode(500);
        }

        [HttpPatch]
        [Route("{id}/photos")]
        [Authorize]
        public async Task<IActionResult> ChangeAdPhotos(int id, [FromForm]IFormFileCollection photos)
        {
            var acceptedExtensions = new[] { "jpg", "png", "jpeg", "bmp", "svg" };
            foreach (var photo in photos)
            {
                if (!acceptedExtensions.Any(e => photo.FileName.EndsWith(e)))
                {
                    return BadRequest(
                        "The photo field only accepts files with the following extensions: .jpg, .png, .jpeg, .bmp, .svg");
                }
                if (photo.Length > 3_000_000)
                {
                    return BadRequest("Too big file size!");
                }
            }

            var userId = await GetCurrentUserId();
            var result = await _advertisementService.ChangeAdPhotos(id, userId, photos);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, [FromQuery] string reason)
        {
            var userId = await GetCurrentUserId();

            var result = await _advertisementService.DeleteAdvertisement(userId, id, reason);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            if (result.IsClientError)
            {
                return Forbid();
            }

            return StatusCode(500);
        }
        

        private async Task<int> GetCurrentUserId()
        {
            var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
            return currentUserId.Value;
        }
    }
}
