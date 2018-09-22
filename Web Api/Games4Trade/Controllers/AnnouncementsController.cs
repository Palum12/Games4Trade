using System;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Games4Trade.Controllers
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

        // GET: api/Announcements
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var announcements = await _announcementService.GetAnnouncements();
            return Ok(announcements);
        }

        // GET: api/Announcements/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            var announcement = await _announcementService.GetAnnouncement(id);
            if (announcement != null)
            {
                return Ok(announcement);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: api/Announcements
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

        // PUT: api/Announcements/5
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
                    return NotFound();
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

        // DELETE: api/ApiWithActions/5
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
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
        }
    }
}
