using Games4Trade.Dtos;
using System.Threading.Tasks;

namespace Games4TradeAPI.Hubs
{
    public interface IMessagesClient
    {
        Task Recieve(MessageDto message);
    }
}
