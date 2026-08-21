using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Games4TradeAPI.Core.Advertisements;
using Games4TradeAPI.Core.Images;
using Games4TradeAPI.Core.Mapping;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Games4TradeAPI.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IAdvertisementReposiotry repository;
        private readonly IUserRepository userRepository;
        private readonly IRepository<Photo> photoRepository;
        private readonly IRepository<AdvertisementItem> advertisementItemRepository;
        private readonly IAdvertisementItemStrategyResolver strategyResolver;
        private readonly IThumbnailGenerator thumbnailGenerator;
        private const int DefaultPageSize = 10;

        public AdvertisementService(
            IAdvertisementReposiotry repository,
            IUserRepository userRepository,
            IRepository<Photo> photoRepository,
            IRepository<AdvertisementItem> advertisementItemRepository,
            IAdvertisementItemStrategyResolver strategyResolver,
            IThumbnailGenerator thumbnailGenerator)
        {
            this.repository = repository;
            this.userRepository = userRepository;
            this.photoRepository = photoRepository;
            this.advertisementItemRepository = advertisementItemRepository;
            this.strategyResolver = strategyResolver;
            this.thumbnailGenerator = thumbnailGenerator;
        }
        
        public async Task<OperationResult> AddAdvertisement(int userId, AdvertisementSaveDto ad)
        {
            if (!strategyResolver.TryResolve(ad.Discriminator, out var strategy))
            {
                return new OperationResult
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Invalid discriminator!"
                };
            }

            var relationshipError = await strategy.ValidateRelationshipsAsync(ad);
            if (relationshipError != null)
            {
                return new OperationResult
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = relationshipError
                };
            }

            var advertisement = ad.ToModel();
            advertisement.UserId = userId;

            var item = strategy.Create(ad);
            await advertisementItemRepository.AddAsync(item);
            advertisement.Item = item;
            await repository.AddAsync(advertisement);

            var result = await repository.SaveChangesAsync();
            if (result > 0)
            {
                return new OperationResult
                {
                    IsSuccessful = true,
                    Payload = advertisement.Id
                };
            }

            return new OperationResult
            {
                IsSuccessful = false,
                IsClientError = false
            };
        }

        public async Task<OperationResult> ArchiveAdvertisement(int userId, int adId)
        {           
            if (!await IsSelfService(userId, adId))
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Nie można archwizować cudzego ogłoszenia!"
                };
            }
            var ad = await repository.GetAsync(adId);
            if (ad == null)
            {
                return AdvertisementNotFoundResult();
            }

            ad.IsActive = false;
            await repository.SaveChangesAsync();

            return new OperationResult(){IsSuccessful = true};
        }

        public async Task<OperationResult> EditAdvertisement(int userId, int adId, AdvertisementSaveDto ad)
        {           
            if (!await IsSelfService(userId, adId))
            {
                return new OperationResult
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Nie można edytować cudzych ogłoszeń"
                };
            }

            if (!strategyResolver.TryResolve(ad.Discriminator, out var strategy))
            {
                return new OperationResult
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Invalid discriminator!"
                };
            }

            var relationshipError = await strategy.ValidateRelationshipsAsync(ad);
            if (relationshipError != null)
            {
                return new OperationResult
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = relationshipError
                };
            }

            var currentAd = await repository.GetAdvertisementWithDetails(adId, userId);
            if (currentAd == null)
            {
                return AdvertisementNotFoundResult();
            }

            currentAd.ExchangeActive = ad.ExchangeActive;
            currentAd.ShowEmail = ad.ShowEmail;
            currentAd.ShowPhone = ad.ShowPhone;
            currentAd.Price = ad.Price;
            currentAd.Title = ad.Title;

            if (strategy.ItemType == currentAd.Item.GetType())
            {
                strategy.Update(currentAd.Item, ad);
            }
            else
            {
                advertisementItemRepository.Remove(currentAd.Item);
                var replacementItem = strategy.Create(ad);
                await advertisementItemRepository.AddAsync(replacementItem);
                currentAd.Item = replacementItem;
            }

            var repoResult = await repository.SaveChangesAsync();

            if (repoResult > 0)
            {
                return new OperationResult{ IsSuccessful = true };
            }
            return new OperationResult()
            {
                IsSuccessful = false,
                IsClientError = false
            };
        }

        public async Task<OperationResult> GetAdvertisement(int id, int? userId = null)
        {
            var ad =  await repository.GetAdvertisementWithDetails(id, userId);
            if (ad == null)
            {
                return new OperationResult()
                {
                    IsSuccessful = false
                };
            }

            var result = await FillAdvertisement(ad.Item);
            return new OperationResult()
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> GetRecommendedAdsForUser(int userId, int page)
        {
            var ads = await repository.GetRecommendedAdvertisements(userId, page, DefaultPageSize);
            var result = ads.Select(advertisement => advertisement.ToSummaryDto()).ToList();
            return new OperationResult()
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> GetAdvetisementsForUser(int userId, int page, bool selfService)
        {
            var ads = await repository.GetAdsForUser(userId, page, DefaultPageSize, selfService);
            var result = ads.Select(advertisement => advertisement.ToSummaryDto()).ToList();
            return new OperationResult
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> GetAdvetisements(AdQueryOptions queryOptions)
        {
            var ads = await repository.GetQueriedAds(queryOptions);
            var result = ads.Select(advertisement => advertisement.ToSummaryDto()).ToList();
            
            return new OperationResult()
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> DeleteAdvertisement(int userId, int adId, string? message = null)
        {
            var ad = await repository.GetAsync(adId);
            if (ad == null)
            {
                return AdvertisementNotFoundResult();
            }

            if (ad.UserId != userId)
            {
                var user = await userRepository.GetAsync(userId);
                if (user == null)
                {
                    return new OperationResult
                    {
                        IsSuccessful = false,
                        IsClientError = true,
                        Message = "Użytkownik nie istnieje"
                    };
                }

                if (user.Role.Equals("Admin"))
                {
                    if (message == null)
                    {
                        return new OperationResult()
                        {
                            IsSuccessful = false,
                            IsClientError = true,
                            Message = "Proszę dodać wiadomość !"
                        };
                    }                   
                    
                    var otherUser = await userRepository.GetAsync(ad.UserId);
                    if (otherUser == null)
                    {
                        return new OperationResult
                        {
                            IsSuccessful = false,
                            IsClientError = true,
                            Message = "Właściciel ogłoszenia nie istnieje"
                        };
                    }

                    var text = string.Format(
                        @"Witaj. </br> Twoje ogłoszenie z serwisu Games4Trade o tytule: '{0}' zostało usunięte. Oto powód usunięcia ogłoszenia:<br>{1}",
                        ad.Title, message);
                    var emailResult = await OtherServices.SendEmail(otherUser.Email, "Wiadomość o usunięciu ogłoszenia.", text);
                    if (emailResult)
                    {
                        var repoResult = await RemoveAdWithPhotos(ad);
                        if (repoResult > 0)
                        {
                            return new OperationResult()
                            {
                                IsSuccessful = true
                            };
                        }

                        return OtherServices.GetIncorrectDatabaseConnectionResult();
                    }
                    return new OperationResult()
                    {
                        IsSuccessful = false,
                        IsClientError = false
                    };
                }

                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Tylko administrator może usunąć cudze ogłoszenie!"
                };
            }

            var result = await RemoveAdWithPhotos(ad);
            if (result > 0)
            {
                return new OperationResult() {IsSuccessful = true};
            }

            return new OperationResult() {IsSuccessful = false, IsClientError = true};
        }

        public async Task<byte[]?> GetAdPhoto(int adId, int? photoId = null)
        {
            if (photoId.HasValue)
            {
                var photo = await photoRepository.GetAsync(photoId.Value);
                if (photo?.AdvertisementId == null || photo.AdvertisementId != adId)
                {
                    return null;
                }
                var bytes = await File.ReadAllBytesAsync(photo.Path);
                return bytes;
            }
            else
            {
                var photos =
                    await photoRepository.FindAsync(p => p.AdvertisementId.HasValue && p.AdvertisementId == adId);
                if (photos.Any())
                {
                    var directory = @"photos/ad" + adId;
                    var path = Path.Combine(directory, "miniature");
                    return await File.ReadAllBytesAsync(path);
                }
                return await File.ReadAllBytesAsync(@"photos/adPhoto.png");
            }
        }

        public async Task<OperationResult> ChangeAdPhotos(int adId, int userId, IFormFileCollection photos)
        {
            var ad = await repository.GetAsync(adId);
            if (ad == null)
            {
                return AdvertisementNotFoundResult();
            }

            if (ad.UserId != userId)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Nie można edytować zdjęć innego użytkownika"
                };
            }
            var temp = await photoRepository
                .FindAsync(p => p.AdvertisementId.HasValue && p.AdvertisementId == adId);
            var oldPhotos = temp.ToArray();
            if (oldPhotos.Any())
            {
                var directory = @"photos/ad" + ad.Id;
                var path = Path.Combine(directory, "miniature");
                File.Delete(path);
            }
            // here delete old photos
            foreach (var oldPhoto in oldPhotos)
            {
                File.Delete(oldPhoto.Path);
                photoRepository.Remove(oldPhoto);
            }

            int repoRes;
            if (!photos.Any())
            {
                repoRes = await repository.SaveChangesAsync();
                if (repoRes == oldPhotos.Length)
                {
                    return new OperationResult { IsSuccessful = true };
                }
                throw new DataException();
            }

            var photosAdded = new List<Photo>();

            // here add new photos
            for (var i = 0; i < photos.Count; i++)
            {
                var photo = photos[i];
                var fileId = Guid.NewGuid().ToString("N").ToUpper();
                var directory = @"photos/ad" + adId;
                Directory.CreateDirectory(directory);

                var path = Path.Combine(directory, fileId);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
                var newPhoto = new Photo {DateCreated = DateTime.UtcNow, Path = path, AdvertisementId = adId};
                photosAdded.Add(newPhoto);
                await photoRepository.AddAsync(newPhoto);

                // here create miniature
                if (i == 0)
                {
                    var newPath = Path.Combine(directory, "miniature");
                    await using (var outputStream = new FileStream(newPath, FileMode.Create))
                    await using (var inputStream = photo.OpenReadStream())
                    {
                        await thumbnailGenerator.WriteJpegAsync(inputStream, outputStream, 300, 200);
                    }
                }

            }

            repoRes = await repository.SaveChangesAsync();
            if (repoRes == oldPhotos.Length + photos.Count)
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }
            else
            {
                var directory = @"photos/ad" + ad.Id;
                var path = Path.Combine(directory, "miniature");
                File.Delete(path);
                foreach (var photo in photosAdded)
                {
                    File.Delete(photo.Path);
                }
                throw new DataException();
            }
        }

        private async Task<int> RemoveAdWithPhotos(Advertisement ad)
        {
            var photos = await photoRepository
                .FindAsync(p => p.AdvertisementId.HasValue && p.AdvertisementId == ad.Id);
            repository.Remove(ad);
            var repoResult = await repository.SaveChangesAsync();
            if (repoResult > 0)
            {
                var directory = @"photos/ad" + ad.Id;
                var path = Path.Combine(directory, "miniature");
                File.Delete(path);
                foreach (var photo in photos)
                {
                    File.Delete(photo.Path);
                }
                return repoResult;
            }

            return 0;
        }

        private async Task<bool> IsSelfService(int userId, int adId)
        {
            var ad = await repository.GetAsync(adId);
            return ad?.UserId == userId;
        }

        private static OperationResult AdvertisementNotFoundResult()
        {
            return new OperationResult
            {
                IsSuccessful = false,
                IsClientError = true,
                Message = "Ogłoszenie nie istnieje"
            };
        }

        private async Task<AdvertisementBasicDto> FillAdvertisement(AdvertisementItem source)
        {
            var strategy = strategyResolver.Resolve(source);
            return await strategy.ToDtoAsync(source);
        }
    }
}
