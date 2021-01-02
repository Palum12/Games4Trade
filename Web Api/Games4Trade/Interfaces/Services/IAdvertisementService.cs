using System;
using System.Threading.Tasks;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Common;
using Games4TradeAPI.Models;
using Microsoft.AspNetCore.Http;

namespace Games4TradeAPI.Interfaces.Services
{
    public interface IAdvertisementService
    {
        Task<OperationResult> AddAdvertisement(int userId, AdvertisementSaveDto ad);
        Task<OperationResult> ArchiveAdvertisement(int userId, int adId);
        Task<OperationResult> EditAdvertisement(int userId, int adId, AdvertisementSaveDto ad);
        Task<OperationResult> GetAdvertisement(int id, int? userId = null);
        Task<OperationResult> GetRecommendedAdsForUser(int userId, int page);
        Task<OperationResult> GetAdvetisementsForUser(int userId, int page, bool selfService);
        Task<OperationResult> GetAdvetisements(AdQueryOptions queryOptions);
        Task<OperationResult> DeleteAdvertisement(int userId, int adId, string reason = null);
        Task<Byte[]> GetAdPhoto(int adId, int? photoId = null);
        Task<OperationResult> ChangeAdPhotos(int adId, int userId, IFormFileCollection photos);
    }
}