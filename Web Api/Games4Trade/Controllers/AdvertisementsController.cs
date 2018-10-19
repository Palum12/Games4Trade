using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Services;
using Microsoft.AspNetCore.Authorization;
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
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{id}/photos")]
        public async Task<IActionResult> GetPhotos(int id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        [HttpPatch]
        [Route("{id}/photos")]
        [Authorize]
        public async Task<IActionResult> ChangeAdPhotos(int id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, [FromQuery] string reason)
        {
            if (User.IsInRole("Admin") && reason == null)
            {
                return BadRequest("Reason for deleting must be added !");
            }
            throw new NotImplementedException();
        }
        

        private async Task<int> GetCurrentUserId()
        {
            var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
            return currentUserId.Value;
        }
    }
}
