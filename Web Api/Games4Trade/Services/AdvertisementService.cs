﻿using System;
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
using Console = Games4Trade.Models.Console;

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
                var advertisement = new Advertisement()
                {
                    UserId = userId,
                    DateCreated = DateTime.Now,
                    Title = ad.Title,
                    ExchangeActive = ad.ExchangeActive,
                    IsActive = true,
                    Price = ad.Price
                };


                switch (ad.Discriminator)
                {
                    case "Game":
                        var game = _mapper.Map<AdvertisementSaveDto, Game>(ad);
                        await _unitOfWork.Games.AddASync(game);
                        advertisement.Item = game;
                        break;
                    case "Accessory":
                        var accessory = _mapper.Map<AdvertisementSaveDto, Accessory>(ad);
                        await _unitOfWork.Accessories.AddASync(accessory);
                        advertisement.Item = accessory;
                        break;
                    case "Console":
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
                    IsSuccessful = false
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
                if (currentAd.Item.GetType() == typeof(Game))
                {
                    var game = currentAd.Item as Game;
                    game.Developer = ad.Developer;
                    game.GameRegionId = ad.RegionId.Value;
                    game.GenreId = ad.GenreId.Value;
                    game.DateReleased = ad.DateReleased;
                    game.StateId = ad.StateId;
                    game.SystemId = ad.SystemId;
                    game.Description = ad.Description;
                }
                else if (currentAd.Item.GetType() == typeof(Accessory))
                {
                    var accessory = currentAd.Item as Accessory;
                    accessory.AccessoryManufacturer = ad.AccessoryManufacturer;
                    accessory.AccessoryModel = ad.AccessoryModel;
                    accessory.DateReleased = ad.DateReleased;
                    accessory.StateId = ad.StateId;
                    accessory.SystemId = ad.SystemId;
                    accessory.Description = ad.Description;
                }
                else if (currentAd.Item.GetType() == typeof(Console))
                {
                    var console = currentAd.Item as Console;
                    console.DateReleased = ad.DateReleased;
                    console.ConsoleRegionId = ad.RegionId.Value;
                    console.StateId = ad.StateId;
                    console.SystemId = ad.SystemId;
                    console.Description = ad.Description;
                }
                
            }
            else
            {
                _unitOfWork.AdvertisementItems.Remove(currentAd.Item);
                switch (ad.Discriminator)
                {
                    case "Game":
                        var game = _mapper.Map<AdvertisementSaveDto, Game>(ad);
                        await _unitOfWork.Games.AddASync(game);
                        currentAd.Item = game;
                        break;
                    case "Accessory":
                        var accessory = _mapper.Map<AdvertisementSaveDto, Accessory>(ad);
                        await _unitOfWork.Accessories.AddASync(accessory);
                        currentAd.Item = accessory;
                        break;
                    case "Console":
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
            
            return new OperationResult(){IsSuccessful = true};
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

            switch (ad.Item.GetType().Name)
            {
                case "Game":
                {
                    var game =  await MapToGame(ad);
                    return new OperationResult()
                    {
                        IsSuccessful = true,
                        Payload = game
                    };
                }
                case "Accessory":
                {
                    var accessory =  await MapToAccessory(ad);
                    return new OperationResult()
                    {
                        IsSuccessful = true,
                        Payload = accessory
                    };
                }
                case "Console":
                {
                    var console = await MapToConsole(ad);
                    return new OperationResult()
                    {
                        IsSuccessful = true,
                        Payload = console
                    };
                }
                default:
                    throw new DataException("Invalid discriminator !");
            }
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

        public async Task<OperationResult> GetAdvetisementsForUser(int userId, int page, bool selfSerive)
        {
            var ads = await _unitOfWork.Advertisements.GetAdsForUser(userId, page, DefaultPageSize, selfSerive);
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
                    return await File.ReadAllBytesAsync(photos.OrderBy(p => p.Id).ElementAt(0).Path);
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
            // here delete old photos
            foreach (var oldPhoto in oldPhotos)
            {
                File.Delete(oldPhoto.Path);
                _unitOfWork.Photos.Remove(oldPhoto);
            }

            int repoRes;
            if (photos.Count == 0)
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
            foreach (var photo in photos)
            {
                var fileId = Guid.NewGuid().ToString("N").ToUpper();
                var directory = @"photos/ad" + adId;
                Directory.CreateDirectory(directory);

                var path = Path.Combine(directory, fileId);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
                var newPhoto = new Photo { DateCreated = DateTime.Now, Path = path, AdvertisementId = adId};
                photosAdded.Add(newPhoto);
                await _unitOfWork.Photos.AddASync(newPhoto);
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
            Models.System system;
            State state;
            Region region;
            switch (ad.Discriminator)
            {
                case "Game":
                    region = await _unitOfWork.Regions.GetASync(ad.RegionId.GetValueOrDefault());
                    state = await _unitOfWork.States.GetASync(ad.StateId);
                    var genre = await _unitOfWork.Genres.GetASync(ad.GenreId.GetValueOrDefault());
                    system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                    objects = new List<object> { region, system, state, genre };

                    if (objects.Any(o => o == null))
                    {
                        var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                        return (false, message);
                    }

                    break;
                case "Console":
                    region = await _unitOfWork.Regions.GetASync(ad.RegionId.GetValueOrDefault());
                    state = await _unitOfWork.States.GetASync(ad.StateId);
                    system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                    objects = new List<object> { region, system, state };

                    if (objects.Any(o => o == null))
                    {
                        var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                        return (false, message);
                    }
                    break;
                case "Accessory":
                    state = await _unitOfWork.States.GetASync(ad.StateId);
                    system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                    objects = new List<object> { system, state };

                    if (objects.Any(o => o == null))
                    {
                        var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                        return (false, message);
                    }

                    break;
                default:
                    return (false, "Wrong discriminator!");
            }
            return (true, null);
        }

        private async Task<bool> IsSelfService(int userId, int adId)
        {
            var ad = await _unitOfWork.Advertisements.GetASync(adId);
            return ad.UserId == userId;
        }

        private async Task<AdvertisementGameDto> MapToGame(Advertisement ad)
        {
            Game game = ad.Item as Game;
            var result = new AdvertisementGameDto()
            {
                Id = ad.Id,
                UserId = ad.UserId,
                Discriminator = ad.Item.GetType().Name,
                DateCreated = ad.DateCreated,
                Description = ad.Item.Description,               
                ExchangeActive = ad.ExchangeActive.GetValueOrDefault(),
                Price = ad.Price,
                Title = ad.Title,
                IsActive = ad.IsActive,
                ShowEmail = ad.ShowEmail,
                ShowPhone = ad.ShowPhone,

                DateReleased = game.DateReleased,
                Developer = game.Developer
            };
            var tempGenre = await _unitOfWork.Genres.GetASync(game.GenreId);
            var tempRegion = await _unitOfWork.Regions.GetASync(game.GameRegionId);
            var tempState = await _unitOfWork.States.GetASync(game.StateId);
            var tempSystem = await _unitOfWork.Systems.GetASync(game.SystemId);

            result.Genre = _mapper.Map<Genre, GenreDto>(tempGenre);
            result.Region = _mapper.Map<Region, RegionDto>(tempRegion);
            result.State = _mapper.Map<State, StateDto>(tempState);
            result.System = _mapper.Map<Models.System, SystemDto>(tempSystem);
            result.User = _mapper.Map<User, UserDto>(ad.User);

            result.Photos = new List<PhotoDto>();
            foreach (var photo in ad.Photos)
            {
                result.Photos.Add(_mapper.Map<Photo, PhotoDto>(photo));
            }

            if (result.ShowEmail)
            {
                result.Email = ad.User.Email;
            }

            if (result.ShowPhone && !string.IsNullOrEmpty(ad.User.PhoneNumber))
            {
                result.PhoneNumber = ad.User.PhoneNumber;
            }

            return result;
        }

        private async Task<AdvertisementConsoleDto> MapToConsole(Advertisement ad)
        {
            Console console = ad.Item as Console;
            var result = new AdvertisementConsoleDto()
            {
                Id = ad.Id,
                UserId = ad.UserId,
                Discriminator = ad.Item.GetType().Name,
                DateCreated = ad.DateCreated,
                Description = ad.Item.Description,
                ExchangeActive = ad.ExchangeActive.GetValueOrDefault(),
                Price = ad.Price,
                Title = ad.Title,
                IsActive = ad.IsActive,
                ShowEmail = ad.ShowEmail,
                ShowPhone = ad.ShowPhone,

                DateReleased = console.DateReleased
            };
            var tempRegion = await _unitOfWork.Regions.GetASync(console.ConsoleRegionId);
            var tempState = await _unitOfWork.States.GetASync(console.StateId);
            var tempSystem = await _unitOfWork.Systems.GetASync(console.SystemId);

            result.Region = _mapper.Map<Region, RegionDto>(tempRegion);
            result.State = _mapper.Map<State, StateDto>(tempState);
            result.System = _mapper.Map<Models.System, SystemDto>(tempSystem);
            result.User = _mapper.Map<User, UserDto>(ad.User);

            result.Photos = new List<PhotoDto>();
            foreach (var photo in ad.Photos)
            {
                result.Photos.Add(_mapper.Map<Photo, PhotoDto>(photo));
            }

            if (result.ShowEmail)
            {
                result.Email = ad.User.Email;
            }

            if (result.ShowPhone && !string.IsNullOrEmpty(ad.User.PhoneNumber))
            {
                result.PhoneNumber = ad.User.PhoneNumber;
            }

            return result;
        }

        private async Task<AdvertisementAccessoryDto> MapToAccessory(Advertisement ad)
        {
            Accessory accessory = ad.Item as Accessory;
            var result = new AdvertisementAccessoryDto()
            {
                Id = ad.Id,
                UserId = ad.UserId,
                Discriminator = ad.Item.GetType().Name,
                DateCreated = ad.DateCreated,
                Description = ad.Item.Description,
                ExchangeActive = ad.ExchangeActive.GetValueOrDefault(),
                Price = ad.Price,
                Title = ad.Title,
                IsActive = ad.IsActive,
                ShowEmail = ad.ShowEmail,
                ShowPhone = ad.ShowPhone,

                AccessoryManufacturer = accessory.AccessoryManufacturer,
                AccessoryModel = accessory.AccessoryModel,
                DateReleased = accessory.DateReleased
            };
            var tempState = await _unitOfWork.States.GetASync(accessory.StateId);
            var tempSystem = await _unitOfWork.Systems.GetASync(accessory.SystemId);         

            result.State = _mapper.Map<State, StateDto>(tempState);
            result.System = _mapper.Map<Models.System, SystemDto>(tempSystem);
            result.User = _mapper.Map<User, UserDto>(ad.User);

            result.Photos = new List<PhotoDto>();
            foreach (var photo in ad.Photos)
            {
                result.Photos.Add(_mapper.Map<Photo, PhotoDto>(photo));
            }

            if (result.ShowEmail)
            {
                result.Email = ad.User.Email;
            }

            if (result.ShowPhone && !string.IsNullOrEmpty(ad.User.PhoneNumber))
            {
                result.PhoneNumber = ad.User.PhoneNumber;
            }

            return result;
        }

    }
}
