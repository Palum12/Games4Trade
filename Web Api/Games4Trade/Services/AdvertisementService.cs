using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Console = Games4Trade.Models.Console;
using Image = SixLabors.ImageSharp.Image;
using Region = Games4Trade.Models.Region;

namespace Games4Trade.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int DefaultPageSize = 10;

        public AdvertisementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        public async Task<OperationResult> AddAdvertisement(int userId, AdvertisementSaveDto ad)
        {
            var isCorrectResultTuple = await CheckIfRelationshipsAreCorrect(ad);
            if (isCorrectResultTuple.Item1)
            {
                var advertisement = _mapper.Map<AdvertisementSaveDto, Advertisement>(ad);
                advertisement.UserId = userId;


                switch (ad.Discriminator)
                {
                    case nameof(Game):
                        var game = _mapper.Map<AdvertisementSaveDto, Game>(ad);
                        await _unitOfWork.Games.AddASync(game);
                        advertisement.Item = game;
                        break;
                    case nameof(Accessory):
                        var accessory = _mapper.Map<AdvertisementSaveDto, Accessory>(ad);
                        await _unitOfWork.Accessories.AddASync(accessory);
                        advertisement.Item = accessory;
                        break;
                    case nameof(Console):
                        var console = _mapper.Map<AdvertisementSaveDto, Console>(ad);
                        await _unitOfWork.Consoles.AddASync(console);
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
                await _unitOfWork.Advertisements.AddASync(advertisement);

                var result = await _unitOfWork.CompleteASync();
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
            var ad = await _unitOfWork.Advertisements.GetASync(adId);
            ad.IsActive = false;
            await _unitOfWork.CompleteASync();

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

            var currentAd = await _unitOfWork.Advertisements.GetAdvertisementWithDetails(adId, userId);

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
                        var game = _mapper.Map<AdvertisementSaveDto, Game>(ad);
                        await _unitOfWork.Games.AddASync(game);
                        currentAd.Item = game;
                        break;
                    case nameof(Accessory):
                        var accessory = _mapper.Map<AdvertisementSaveDto, Accessory>(ad);
                        await _unitOfWork.Accessories.AddASync(accessory);
                        currentAd.Item = accessory;
                        break;
                    case nameof(Console):
                        var console = _mapper.Map<AdvertisementSaveDto, Console>(ad);
                        await _unitOfWork.Consoles.AddASync(console);
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

            var repoResult = await _unitOfWork.CompleteASync();

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
            var ad =  await _unitOfWork.Advertisements.GetAdvertisementWithDetails(id, userId);
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
            var ads = await _unitOfWork.Advertisements.GetRecommendedAdvertisements(userId, page, DefaultPageSize);
            var result = _mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementWithoutItemDto>>(ads);
            return new OperationResult()
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> GetAdvetisementsForUser(int userId, int page, bool selfService)
        {
            var ads = await _unitOfWork.Advertisements.GetAdsForUser(userId, page, DefaultPageSize, selfService);
            var result = _mapper.Map<IEnumerable<Advertisement>, IEnumerable< AdvertisementWithoutItemDto>>(ads);
            return new OperationResult
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> GetAdvetisements(AdQueryOptions queryOptions)
        {
            var ads = await _unitOfWork.Advertisements.GetQueriedAds(queryOptions);
            var result = _mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementWithoutItemDto>>(ads);
            
            return new OperationResult()
            {
                IsSuccessful = true,
                Payload = result
            };
        }

        public async Task<OperationResult> DeleteAdvertisement(int userId, int adId, string message = null)
        {
            var ad = await _unitOfWork.Advertisements.GetASync(adId);
            if (ad.UserId != userId)
            {
                var user = await _unitOfWork.Users.GetASync(userId);
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
                    
                    var otherUser = await _unitOfWork.Users.GetASync(ad.UserId);
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
                var photo = await _unitOfWork.Photos.GetASync(photoId.Value);
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
                    await _unitOfWork.Photos.FindASync(p => p.AdvertisementId.HasValue && p.AdvertisementId == adId);
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
            var user = await _unitOfWork.Users.GetASync(userId);
            var ad = await _unitOfWork.Advertisements.GetASync(adId);
            if (! await IsSelfService(userId, adId))
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Nie można edytować zdjęć innego użytkownika"
                };
            }
            var temp = await _unitOfWork.Photos
                .FindASync(p => p.AdvertisementId.HasValue && p.AdvertisementId == adId);
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
                _unitOfWork.Photos.Remove(oldPhoto);
            }

            int repoRes;
            if (!photos.Any())
            {
                repoRes = await _unitOfWork.CompleteASync();
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
                await _unitOfWork.Photos.AddASync(newPhoto);

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

            repoRes = await _unitOfWork.CompleteASync();
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
            var photos = await _unitOfWork.Photos
                .FindASync(p => p.AdvertisementId.HasValue && p.AdvertisementId == ad.Id);
            _unitOfWork.Advertisements.Remove(ad);
            var repoResult = await _unitOfWork.CompleteASync();
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
            var system = await _unitOfWork.Systems.GetASync(ad.SystemId);
            var state = await _unitOfWork.States.GetASync(ad.StateId);
            Region region;
            switch (ad.Discriminator)
            {
                case nameof(Game):
                    var genre = await _unitOfWork.Genres.GetASync(ad.GenreId.GetValueOrDefault());
                    region = await _unitOfWork.Regions.GetASync(ad.RegionId.GetValueOrDefault());
                    objects = new List<object> { region, system, state, genre };
                    break;
                case nameof(Console):
                    region = await _unitOfWork.Regions.GetASync(ad.RegionId.GetValueOrDefault());
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
            var ad = await _unitOfWork.Advertisements.GetASync(adId);
            return ad.UserId == userId;
        }

        private async Task<AdvertisementBasicDto> FillAdvertisement(AdvertisementItem source)
        {
            AdvertisementBasicDto result;

            var taskState = _unitOfWork.States.GetASync(source.StateId);
            var taskSystem = _unitOfWork.Systems.GetASync(source.SystemId);

            switch (source)
            {
                case Game g:
                    result = _mapper.Map<Advertisement, AdvertisementGameDto>(g.Advertisement);
                    _mapper.Map(g, result);
                    result.Discriminator = nameof(Game);
                    var tempGenre = await _unitOfWork.Genres.GetASync(g.GenreId);
                    var tempRegionGame = await _unitOfWork.Regions.GetASync(g.GameRegionId);
                    ((AdvertisementGameDto)result).Genre = _mapper.Map<Genre, GenreDto>(tempGenre);
                    ((AdvertisementGameDto)result).Region = _mapper.Map<Region, RegionDto>(tempRegionGame);
                    break;
                case Console c:
                    result = _mapper.Map<Advertisement, AdvertisementConsoleDto>(c.Advertisement);
                    _mapper.Map(c, result);
                    result.Discriminator = nameof(Console);
                    var tempRegionConsole = await _unitOfWork.Regions.GetASync(c.ConsoleRegionId);
                    ((AdvertisementConsoleDto)result).Region = _mapper.Map<Region, RegionDto>(tempRegionConsole);
                    break;
                case Accessory a:
                    result = _mapper.Map<Advertisement, AdvertisementAccessoryDto>(a.Advertisement);
                    _mapper.Map(a, result);
                    result.Discriminator = nameof(Accessory);
                    break;
                default:
                    throw new NotSupportedException();
            }

            await Task.WhenAll(taskState, taskSystem);
            result.State = _mapper.Map<State, StateDto>(taskState.Result);
            result.System = _mapper.Map<Models.System, SystemDto>(taskSystem.Result);

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
