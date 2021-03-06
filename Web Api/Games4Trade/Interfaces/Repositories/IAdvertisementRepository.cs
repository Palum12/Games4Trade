﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Interfaces.Repositories
{
    public interface IAdvertisementReposiotry : IRepository<Advertisement>
    {
        Task<Advertisement> GetAdvertisementWithDetails(int id, int? userId = null);
        Task<IEnumerable<Advertisement>> Get(int page, int pageSize);
        Task<IEnumerable<Advertisement>> GetQueriedAds(AdQueryOptions options);
        Task<IEnumerable<Advertisement>> GetRecommendedAdvertisements(int userId, int page, int pageSize);
        Task<IEnumerable<Advertisement>> GetAdsForUser(int userId, int page, int pageSize, bool skipInActive);
    }
}
