using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationContext context) : base(context) { }

        public async Task<IEnumerable<Message>> GetNewestMessagesForUser(int currentUserId)
        {
            var exisitngTuples = new List<(int, int)>();
            var result = new List<Message>();

            bool wasSelected((int, int) tuple)
            {
                var normal = exisitngTuples.Contains(tuple);
                var reversed = exisitngTuples.Contains((tuple.Item2, tuple.Item1));
                return normal || reversed;
            }

            var messages = await Context.Messages.Include(m=> m.Reciver)
                .Where(m => m.SenderId == currentUserId || m.ReciverId == currentUserId).OrderByDescending(m => m.DateCreated).ToArrayAsync();

            foreach (var message in messages)
            {
                if (!wasSelected((message.SenderId, message.ReciverId)))
                {
                    exisitngTuples.Add((message.SenderId, message.ReciverId));
                    result.Add(message);
                }
            }

            return result;
        }

        public async Task<IEnumerable<Message>> GetMessagesWithReciver(int senderId, int reciverId, int page, int pageSize)
        {
            var skip = page * pageSize;
            return await Context.Messages
                .Where(m => 
                    (m.ReciverId == reciverId && m.SenderId == senderId) || 
                    (m.SenderId == reciverId && m.ReciverId == senderId))
                .OrderByDescending(a => a.DateCreated)
                .Skip(skip).Take(pageSize)
                .ToListAsync();
        }

    }
}
