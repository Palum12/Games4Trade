using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IEnumerable<Message>> GetNewestMessagesForUser(int currentUserId);
        Task<IEnumerable<Message>> GetMessagesWithReciver(int senderId, int reciverId, int page, int pageSize);
    }
}
