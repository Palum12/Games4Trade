using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Games4TradeAPI.Hubs
{
    [Authorize]
    public class MessagesHub : Hub<IMessagesClient>
    {
        private static readonly ConcurrentDictionary<string, string> users = new ConcurrentDictionary<string, string>();

        public static string TryGetUserConnection (string username)
        {
            users.TryGetValue(username, out string connectionId);
            return connectionId;
        }

        public override Task OnConnectedAsync()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;
            users.AddOrUpdate(userName, connectionId, (key, value) => connectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string username = Context.User.Identity.Name;
            users.TryRemove(username, out _);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
