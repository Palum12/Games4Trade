using System.Threading.Tasks;
using Games4Trade.Models;
using Microsoft.AspNetCore.SignalR;

namespace Games4Trade.Hub
{
    public interface IMessagesClient
    {
        Task Add(Message message);
    }

    public class MessagesHub : Hub<IMessagesClient>
    {
        public async Task Add(Message message) => await Clients.All.Add(message);
    }
}
