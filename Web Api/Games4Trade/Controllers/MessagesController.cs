using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Games4Trade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        

        public MessagesController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery]int? otherUserId, [FromQuery]int? page)
        {
            var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
            if (otherUserId.HasValue && page.HasValue)
            {
                page = page > 0 ? page - 1 : 0;
                var converstaion = await _messageService
                    .GetMessagesWithUser(currentUserId.Value, otherUserId.Value, page.Value);
                return Ok(converstaion);
            }
            var messages = await _messageService.GetNewestMessages(currentUserId.Value);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Post(MessagePostDto message)
        {
            var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
            var targetUser = await _userService.GetUserById(message.ReciverId);
            if (targetUser == null)
            {
                return BadRequest("Target user doesnt exist !");
            }

            if (currentUserId.Value == targetUser.Id)
            {
                return BadRequest("Cannot send message to yourself!");
            }

            var result = await _messageService.AddMessage(currentUserId.Value, message);
            if (result.IsSuccessful)
            {
                return Ok();
            }

            return StatusCode(500, result.Message);
        }

        [HttpPatch]
        public async Task<IActionResult> SetMessagesUnActive(int otherUserId)
        {
            var currentUserId = await _userService.GetUserIdByLogin(User.Identity.Name);
            if (otherUserId > 0)
            {
                await _messageService.SetMessagesAsRead(currentUserId.Value, otherUserId);
                return Ok();
            }
            return BadRequest("Incorrect Id");
        }

    }
}
