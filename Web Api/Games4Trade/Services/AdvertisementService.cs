using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Console = Games4TradeAPI.Models.Console;
using Image = SixLabors.ImageSharp.Image;
using Region = Games4TradeAPI.Models.Region;

namespace Games4TradeAPI.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IAdvertisementReposiotry repository;
        private readonly IUserRepository userRepository;
        private readonly IRepository<Photo> photoRepository;
        private readonly IRepository<State> stateRepository;
        private readonly IGenreRepository genreRepository;
        private readonly IRepository<Region> regionRepository;
        private readonly ISystemRepository systemRepository;
        private readonly IMapper mapper;
        private const int DefaultPageSize = 10;

        public AdvertisementService(
            IAdvertisementReposiotry repository,
            IRepository<Photo> photoRepository,
            IRepository<State> stateRepository,
            IGenreRepository genreRepository,
            IRepository<Region> regionRepository,
            ISystemRepository systemRepository,
            IMapper mapper)
        {
            this.repository = repository;
            this.photoRepository = photoRepository;
            this.stateRepository = stateRepository;
            this.genreRepository = genreRepository;
            this.regionRepository = regionRepository;
            this.systemRepository = systemRepository;
            this.mapper = mapper;
        }
        
        public async Task<OperationResult> AddAdvertisement(int userId, AdvertisementSaveDto ad)
        {
            var isCorrectResultTuple = await CheckIfRelationshipsAreCorrect(ad);
            if (isCorrectResultTuple.Item1)
            {
                var advertisement = mapper.Map<AdvertisementSaveDto, Advertisement>(ad);
                advertisement.UserId = userId;

                switch (ad.Discriminator)
                {
                    case nameof(Game):
                        var game = mapper.Map<AdvertisementSaveDto, Game>(ad);
                        await _unitOfWork.Games.AddAsync(game);
                        advertisement.Item = game;
                        break;
                    case nameof(Accessory):
                        var accessory = mapper.Map<AdvertisementSaveDto, Accessory>(ad);
                        await _unitOfWork.Accessories.AddAsync(accessory);
                        advertisement.Item = accessory;
                        break;
                    case nameof(Console):
                        var console = mapper.Map<AdvertisementSaveDto, Console>(ad);
                        await _unitOfWork.Consoles.AddAsync(console);
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
                _unitOfWork.AdvertisementItems.Remove(currentAd.Item);
                switch (ad.Discriminator)
                {
                    case nameof(Game):
                        var game = mapper.Map<AdvertisementSaveDto, Game>(ad);
                        await _unitOfWork.Games.AddAsync(game);
                        currentAd.Item = game;
                        break;
                    case nameof(Accessory):
                        var accessory = mapper.Map<AdvertisementSaveDto, Accessory>(ad);
                        await _unitOfWork.Accessories.AddAsync(accessory);
                        currentAd.Item = accessory;
                        break;
                    case nameof(Console):
                        var console = mapper.Map<AdvertisementSaveDto, Console>(ad);
                        await _unitOfWork.Consoles.AddAsync(console);
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
            var user = await userRepository.GetAsync(userId);
            var ad = await repository.GetAsync(adId);
            if (! await IsSelfService(userId, adId))
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
                var newPhoto = new Photo {DateCreated = DateTime.Now, Path = path, AdvertisementId = adId};
                photosAdded.Add(newPhoto);
                await photoRepository.AddAsync(newPhoto);

                // here create miniature
                if (i == 0)
                {
                    var newPath = Path.Combine(directory, "miniature");
                    using (var outputStream = new FileStream(newPath, FileMode.Create))
                    using (Stream inputStream = photo.OpenReadStream())
                    {
                        var image = Image.Load(inputStream);
                        image.Mutate(img => img.Resize(new ResizeOptions()
                            {
                                Mode = ResizeMode.Max,
                                Size = new Size(300, 200)
                            }
                        ));
                        image.Save(outputStream, new JpegEncoder());
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

        private async Task<(bool, string)> CheckIfRelationshipsAreCorrect(AdvertisementSaveDto ad)
        {
            IList<Object> objects;
            var system = await repository.GetAsync(ad.SystemId);
            var state = await stateRepository.GetAsync(ad.StateId);
            Region region;
            switch (ad.Discriminator)
            {
                case nameof(Game):
                    var genre = await genreRepository.GetAsync(ad.GenreId.GetValueOrDefault());
                    region = await regionRepository.GetAsync(ad.RegionId.GetValueOrDefault());
                    objects = new List<object> { region, system, state, genre };
                    break;
                case nameof(Console):
                    region = await regionRepository.GetAsync(ad.RegionId.GetValueOrDefault());
                    objects = new List<object> { region, system, state };
                    break;
                case nameof(Accessory):
                    objects = new List<object> { system, state };
                    break;
                default:
                    return (false, "Wrong discriminator!");
            }
            if (objects.Any(o => o == null))
            {
                var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
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
