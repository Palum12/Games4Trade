using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Common;
using Games4TradeAPI.Models;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Console = Games4TradeAPI.Models.Console;
using Region = Games4TradeAPI.Models.Region;

namespace Games4TradeAPI.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private const int DefaultPageSize = 10;

        #region Dependencies
        private readonly IAdvertisementReposiotry repository;
        private readonly IUserRepository userRepository;
        private readonly IPhotoRepository photoRepository;
        private readonly IRepository<State> stateRepository;
        private readonly IGenreRepository genreRepository;
        private readonly IRepository<Region> regionRepository;
        private readonly ISystemRepository systemRepository;
        private readonly IRepository<AdvertisementItem> advertisementItemRepository;
        private readonly IImageService imageService;
        private readonly IMapper mapper;
        #endregion

        public AdvertisementService(
            IAdvertisementReposiotry repository,
            IUserRepository userRepository,
            IPhotoRepository photoRepository,
            IRepository<State> stateRepository,
            IGenreRepository genreRepository,
            IRepository<Region> regionRepository,
            ISystemRepository systemRepository,
            IRepository<AdvertisementItem> advertisementItemRepository,
            IImageService imageService,
            IMapper mapper)
        {
            this.repository = repository;
            this.userRepository = userRepository;
            this.photoRepository = photoRepository;
            this.stateRepository = stateRepository;
            this.genreRepository = genreRepository;
            this.regionRepository = regionRepository;
            this.systemRepository = systemRepository;
            this.advertisementItemRepository = advertisementItemRepository;
            this.imageService = imageService;
            this.mapper = mapper;
        }
        
        public async Task<OperationResult> AddAdvertisement(int userId, AdvertisementSaveDto ad)
        {
            var isCorrectResultTuple = await CheckIfRelationshipsAreCorrect(ad);
            if (isCorrectResultTuple.Item1)
            {
                var advertisement = mapper.Map<AdvertisementSaveDto, Advertisement>(ad);
                advertisement.UserId = userId;

                switch (ad.Discriminator) // todo: test this
                {
                    case nameof(Game):
                        var game = mapper.Map<AdvertisementSaveDto, Game>(ad);
                        await advertisementItemRepository.AddAsync(game);
                        advertisement.Item = game;
                        break;
                    case nameof(Accessory):
                        var accessory = mapper.Map<AdvertisementSaveDto, Accessory>(ad);
                        await advertisementItemRepository.AddAsync(accessory);
                        advertisement.Item = accessory;
                        break;
                    case nameof(Console):
                        var console = mapper.Map<AdvertisementSaveDto, Console>(ad);
                        await advertisementItemRepository.AddAsync(console);
                        advertisement.Item = console;
                        break;
                    default:
                        return new OperationResult
                        {
                            IsSuccessful = false,
                            IsClientError = true,
                            Message = "Invalid discriminator!"
                        };
                }
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
                Message = isCorrectResultTuple.Item2
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

            var isCorrectResultTuple = await CheckIfRelationshipsAreCorrect(ad);
            if (!isCorrectResultTuple.Item1)
            {
                return new OperationResult
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = isCorrectResultTuple.Item2
                };
            }

            var currentAd = await repository.GetAdvertisementWithDetails(adId, userId);

            currentAd.ExchangeActive = ad.ExchangeActive;
            currentAd.ShowEmail = ad.ShowEmail;
            currentAd.ShowPhone = ad.ShowPhone;
            currentAd.Price = ad.Price;
            currentAd.Title = ad.Title;

            if (currentAd.Item.GetType().Name.Equals(ad.Discriminator))
            {
                switch (currentAd.Item)
                {
                    case Game game:
                        game.Developer = ad.Developer;
                        game.GameRegionId = ad.RegionId.Value;
                        game.GenreId = ad.GenreId.Value;
                        game.DateReleased = ad.DateReleased;
                        game.StateId = ad.StateId;
                        game.SystemId = ad.SystemId;
                        game.Description = ad.Description;
                        break;
                    case Accessory accessory:
                        accessory.AccessoryManufacturer = ad.AccessoryManufacturer;
                        accessory.AccessoryModel = ad.AccessoryModel;
                        accessory.DateReleased = ad.DateReleased;
                        accessory.StateId = ad.StateId;
                        accessory.SystemId = ad.SystemId;
                        accessory.Description = ad.Description;
                        break;
                    case Console console:
                        console.DateReleased = ad.DateReleased;
                        console.ConsoleRegionId = ad.RegionId.Value;
                        console.StateId = ad.StateId;
                        console.SystemId = ad.SystemId;
                        console.Description = ad.Description;
                        break;
                    default:
                        break;
                }
                
            }
            else
            {
                advertisementItemRepository.Remove(currentAd.Item);
                switch (ad.Discriminator)
                {
                    case nameof(Game):
                        var game = mapper.Map<AdvertisementSaveDto, Game>(ad);
                        await advertisementItemRepository.AddAsync(game);
                        currentAd.Item = game;
                        break;
                    case nameof(Accessory):
                        var accessory = mapper.Map<AdvertisementSaveDto, Accessory>(ad);
                        await advertisementItemRepository.AddAsync(accessory);
                        currentAd.Item = accessory;
                        break;
                    case nameof(Console):
                        var console = mapper.Map<AdvertisementSaveDto, Console>(ad);
                        await advertisementItemRepository.AddAsync(console);
                        currentAd.Item = console;
                        break;
                    default:
                        return new OperationResult
                        {
                            IsSuccessful = false,
                            IsClientError = true,
                            Message = "Invalid discriminator!"
                        };
                }

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
            var result = mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementWithoutItemDto>>(ads);
            return new OperationResult()
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> GetAdvetisementsForUser(int userId, int page, bool selfService)
        {
            var ads = await repository.GetAdsForUser(userId, page, DefaultPageSize, selfService);
            var result = mapper.Map<IEnumerable<Advertisement>, IEnumerable< AdvertisementWithoutItemDto>>(ads);
            return new OperationResult
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> GetAdvetisements(AdQueryOptions queryOptions)
        {
            var ads = await repository.GetQueriedAds(queryOptions);
            var result = mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementWithoutItemDto>>(ads);
            
            return new OperationResult()
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> DeleteAdvertisement(int userId, int adId, string message = null)
        {
            var ad = await repository.GetAsync(adId);
            if (ad.UserId != userId)
            {
                var user = await userRepository.GetAsync(userId);
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

        public async Task<byte[]> GetAdPhoto(int adId, int? photoId = null)
        {
            Photo photo;
            if (photoId.HasValue)
            {
                photo = await photoRepository.GetAsync(photoId.Value);
                if (photo is not null)
                {
                    return photo?.Bytes;
                }
            }
            photo = await photoRepository.FirstOrDefaultAsync(p => p.AdvertisementId == adId);
            return photo?.Bytes;
        }

        public async Task<OperationResult> ChangeAdPhotos(int adId, int userId, IFormFileCollection photos)
        {
            var user = await userRepository.GetAsync(userId);
            var ad = await repository.GetAsync(adId);

            if (!await IsSelfService(userId, adId))
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Nie można edytować zdjęć innego użytkownika"
                };
            }

            var oldPhotos = await photoRepository.FindAsync(p => p.AdvertisementId == adId, ImageDownloadFormat.MetadataOnly);
            await photoRepository.RemoveRangeAsync(oldPhotos);

            var newPhotos = photos.Select(file =>
            (
                new Photo
                {
                    AdvertisementId = adId,
                    FileName = file.FileName,
                    ObjectName = $"{userId}/{adId}/{file.FileName}"
                },
                file
            ));

            var photosAdded = await photoRepository.AddRangeAsync(newPhotos);
            var repoRes = await photoRepository.SaveChangesAsync();
            if (repoRes == oldPhotos.Count() + photos.Count)
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }
            else
            {
                throw new DataException();
            }
        }

        private async Task<int> RemoveAdWithPhotos(Advertisement ad)
        {
            var photos = await photoRepository.FindAsync(p =>  p.AdvertisementId == ad.Id, ImageDownloadFormat.MetadataOnly);
            repository.Remove(ad);
            await photoRepository.RemoveRangeAsync(photos);
            var repoResult = await photoRepository.SaveChangesAsync();

            return repoResult;
        }

        private async Task<(bool, string)> CheckIfRelationshipsAreCorrect(AdvertisementSaveDto ad)
        {
            IList<ModelBase> objects;
            var system = await systemRepository.GetAsync(ad.SystemId);
            var state = await stateRepository.GetAsync(ad.StateId);
            Region region;
            switch (ad.Discriminator)
            {
                case nameof(Game):
                    var genre = await genreRepository.GetAsync(ad.GenreId.GetValueOrDefault());
                    region = await regionRepository.GetAsync(ad.RegionId.GetValueOrDefault());
                    objects = new List<ModelBase> { region, system, state, genre };
                    break;
                case nameof(Console):
                    region = await regionRepository.GetAsync(ad.RegionId.GetValueOrDefault());
                    objects = new List<ModelBase> { region, system, state };
                    break;
                case nameof(Accessory):
                    objects = new List<ModelBase> { system, state };
                    break;
                default:
                    return (false, "Wrong discriminator!");
            }
            if (objects.Any(o => o == null))
            {
                var message = "Invalid data";
                return (false, message);
            }
            return (true, null);
        }

        private async Task<bool> IsSelfService(int userId, int adId)
        {
            var ad = await repository.GetAsync(adId);
            return ad.UserId == userId;
        }

        private async Task<AdvertisementBasicDto> FillAdvertisement(AdvertisementItem source)
        {
            AdvertisementBasicDto result;

            switch (source)
            {
                case Game g:
                    result = mapper.Map<Advertisement, AdvertisementGameDto>(g.Advertisement);
                    mapper.Map(g, result);
                    result.Discriminator = nameof(Game);
                    var tempGenre = await genreRepository.GetAsync(g.GenreId);
                    var tempRegionGame = await regionRepository.GetAsync(g.GameRegionId);
                    ((AdvertisementGameDto)result).Genre = mapper.Map<Genre, GenreDto>(tempGenre);
                    ((AdvertisementGameDto)result).Region = mapper.Map<Region, RegionDto>(tempRegionGame);
                    break;
                case Console c:
                    result = mapper.Map<Advertisement, AdvertisementConsoleDto>(c.Advertisement);
                    mapper.Map(c, result);
                    result.Discriminator = nameof(Console);
                    var tempRegionConsole = await regionRepository.GetAsync(c.ConsoleRegionId);
                    ((AdvertisementConsoleDto)result).Region = mapper.Map<Region, RegionDto>(tempRegionConsole);
                    break;
                case Accessory a:
                    result = mapper.Map<Advertisement, AdvertisementAccessoryDto>(a.Advertisement);
                    mapper.Map(a, result);
                    result.Discriminator = nameof(Accessory);
                    break;
                default:
                    throw new NotSupportedException();
            }

            var state = await stateRepository.GetAsync(source.StateId);
            var system = await systemRepository.GetAsync(source.SystemId);
            result.State = mapper.Map<State, StateDto>(state);
            result.System = mapper.Map<Models.System, SystemDto>(system);

            if (source.Advertisement.ShowEmail)
            {
                result.Email = source.Advertisement.User.Email;
            }

            if (source.Advertisement.ShowPhone && !string.IsNullOrEmpty(source.Advertisement.User.PhoneNumber))
            {
                result.PhoneNumber = source.Advertisement.User.PhoneNumber;
            }
            return result;
        }
    }
}
