using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade.Services
{
    public interface IAdvertisementService
    {
        Task<OperationResult> AddAdvertisement(int userId, AdvertisementSaveDto ad);
    }
}