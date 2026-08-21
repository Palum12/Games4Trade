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
        private static readonly ConcurrentDictionary<string, string> users = new();

        public static string? TryGetUserConnection(string username)
        {
            users.TryGetValue(username, out var connectionId);
            return connectionId;
        }

        public override Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name;
            if (userName == null)
            {
                Context.Abort();
                return Task.CompletedTask;
            }

            var connectionId = Context.ConnectionId;
            users.AddOrUpdate(userName, connectionId, (key, value) => connectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User?.Identity?.Name;
            if (username != null &&
                users.TryGetValue(username, out var currentConnectionId) &&
                currentConnectionId == Context.ConnectionId)
            {
                users.TryRemove(username, out _);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
