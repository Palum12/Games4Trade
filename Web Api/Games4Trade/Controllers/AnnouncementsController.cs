using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Interfaces.Services;
using Games4Trade.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Games4TradeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementsController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet("page/{page}")]
        public async Task<IActionResult> GetPage(int page)
        {
            page = page > 0 ? page - 1 : 0;
            IList<AnnouncementGetDto> announcements;
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                announcements = await _announcementService.GetAnnouncementsPage(page, true);
            }
            else
            {
                announcements = await _announcementService.GetAnnouncementsPage(page, false);
            }
            return Ok(announcements);
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            AnnouncementGetDto announcement;
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                announcement = await _announcementService.GetAnnouncement(id, true);
            }
            else
            {
                announcement = await _announcementService.GetAnnouncement(id, false);
            }
            if (announcement != null)
            {
                return Ok(announcement);
            }
            else
            {
                return BadRequest("Resource doesnt exists");
            }
        }

        [HttpPost]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Post([FromBody] AnnouncementSaveDto value)
        {
            if (ModelState.IsValid)
            {
                var currentName = User.Identity.Name;
                var response = await _announcementService.CreateAnnouncement(value, currentName);
                if (response.IsSuccessful)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            else
            {
                return BadRequest(String.Join(" ,", OtherServices.ReturnAllModelErrors(ModelState)));
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeArchive(int id, [FromBody] AnnouncementArchiveDto value)
        {
            var result = await _announcementService.ChangeStatus(id, value);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            if (result.IsClientError)
            {
                return BadRequest("Resource doesnt exists!");
            }

            return StatusCode(500, result.Message);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] AnnouncementSaveDto value)
        {

            if (ModelState.IsValid)
            {
                var response = await _announcementService.EditAnnouncement(id, value);
                if (response.IsSuccessful)
                {
                    return Ok();
                }
                else if (response.IsClientError)
                {
                    if (response.Payload != null)
                    {
                        return Ok();
                    }
                    return BadRequest("Resource doesnt exists!");
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            else
            {
                return BadRequest(String.Join(" ,", OtherServices.ReturnAllModelErrors(ModelState)));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _announcementService.DeleteAnnouncement(id);
            if (response.IsSuccessful)
            {
                return Ok();
            }
            else
            {
                if (response.IsClientError)
                {
                    return BadRequest("Resource doesnt exists!");
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
        }
    }
}
