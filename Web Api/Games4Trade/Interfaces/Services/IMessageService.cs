using System.Collections.Generic;
using System.Threading.Tasks;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Interfaces.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDto>> GetMessagesWithUser(int currentUserId, int selectedUserId, int page);
        Task<OperationResult> AddMessage(int currentUserId, MessagePostDto message);
        Task<IEnumerable<NewestMessageDto>> GetNewestMessages(int currentUserId);
        Task<OperationResult> SetMessagesAsRead(int currentUserId, int selectedUserId);
        Task<bool> CheckIfThereAreNewMessages(int senderId, int reciverId);
    }
}
