using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Services;
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
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Announcements
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Announcements/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
