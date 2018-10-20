﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Microsoft.AspNetCore.Http;

namespace Games4Trade.Services
{
    public interface IAdvertisementService
    {
        Task<OperationResult> AddAdvertisement(int userId, AdvertisementSaveDto ad);
        Task<AdvertisementGetDto> GetAdvertisement(int id);
        Task<OperationResult> DeleteAdvertisement(int userId, int adId, string reason = null);
        Task<Byte[]> GetAdPhoto(int adId, int photoId);
        Task<OperationResult> ChangeAdPhotos(int adId, int userId, IFormFileCollection photos);
    }
}