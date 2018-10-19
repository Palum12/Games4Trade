using System;
using System.Threading.Tasks;
using Games4Trade.Services;
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

        private async Task<int> GetCurrentUserId()
        {
            var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
            return currentUserId.Value;
        }
    }
}
