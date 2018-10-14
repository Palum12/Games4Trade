using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade.Services
{
    public interface IMessageService
    {
        Task<OperationResult> GetMessagesWithUser(int currentUserId, int selectedUserId, int? page = null);
        Task<OperationResult> AddMessage(MessagePostDto message);
        Task<OperationResult> GetNewestMessages(IList<NewestMessageDto> messages);
    }
}
