using Games4Trade.Dtos;
using System.Threading.Tasks;

namespace Games4Trade.Hubs
{
    public interface IMessagesClient
    {
        Task Recieve(MessageDto message);
    }
}
