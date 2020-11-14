using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;
using Games4Trade.Interfaces.Repositories;

namespace Games4Trade.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationContext context) : base(context) { }

        public async Task<IEnumerable<Message>> GetNewestMessagesForUser(int currentUserId)
        {                
            var query = from m in Context.Messages
                let msgTo = m.ReceiverId == currentUserId
                let msgFrom = m.SenderId == currentUserId
                where msgTo || msgFrom
                group m by msgTo ? m.SenderId : m.ReceiverId into g
                select g.OrderByDescending(x => x.DateCreated).First();
            var result = await query.OrderByDescending(m => m.DateCreated).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Message>> GetMessagesWithReciver(int senderId, int reciverId, int page, int pageSize)
        {
            var skip = page * pageSize;
            return await Context.Messages
                .Where(m => 
                    (m.ReceiverId == reciverId && m.SenderId == senderId) || 
                    (m.SenderId == reciverId && m.ReceiverId == senderId))
                .OrderByDescending(a => a.DateCreated)
                .Skip(skip).Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> CheckIfThereAreNewMessages(int senderId, int reciverId)
        {
            return await Context.Messages
                .AnyAsync(m => m.SenderId == senderId && m.ReceiverId == reciverId && !m.IsDelivered);
        }
    }
}
