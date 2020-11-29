using System.Collections.Generic;
using System.Threading.Tasks;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Interfaces.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IEnumerable<Message>> GetNewestMessagesForUser(int currentUserId);
        Task<IEnumerable<Message>> GetMessagesWithReciever(int senderId, int recieverId, int page, int pageSize);
        Task<bool> CheckIfThereAreNewMessages(int senderId, int reciverId);
    }
}
