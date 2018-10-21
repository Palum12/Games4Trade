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
using Console = Games4Trade.Models.Console;

namespace Games4Trade.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

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
                    Description = ad.Description,
                    Title = ad.Title,
                    ExchangeActive = ad.ExchangeActive,
                    IsActive = true,
                    Price = ad.Price
                };


                switch (ad.Discriminator)
                {
                    case "Game":
                        var game = new Game();
                        game.DateReleased = ad.DateReleased;
                        game.Developer = ad.Developer;
                        game.GameRegionId = ad.RegionId;
                        game.GenreId = ad.GenreId;
                        game.SystemId = ad.SystemId;
                        game.StateId = ad.StateId;
                        await _unitOfWork.Games.AddASync(game);
                        advertisement.Item = game;
                        break;
                    case "Accessory":
                        var accessory = new Accessory();
                        accessory.AccessoryManufacturer = ad.AccessoryManufacturer;
                        accessory.AccessoryModel = ad.AccessoryModel;
                        accessory.StateId = ad.StateId;
                        accessory.SystemId = ad.SystemId;
                        accessory.DateReleased = ad.DateReleased;
                        await _unitOfWork.Accessories.AddASync(accessory);
                        advertisement.Item = accessory;
                        break;
                    case "Console":
                        var console = new Games4Trade.Models.Console();
                        console.ConsoleRegionId = ad.RegionId;
                        console.DateReleased = ad.DateReleased;
                        console.StateId = ad.StateId;
                        console.SystemId = ad.SystemId;
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
                        IsSuccessful = true
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
            var result = await _unitOfWork.CompleteASync();
            if (result > 0)
            {
                return new OperationResult(){IsSuccessful = true};
            }

            return OtherServices.GetIncorrectDatabaseConnectionResult();
        }

        //todo
        public async Task<OperationResult> EditAdvertisement(int userId, AdvertisementPutDto ad)
        {           
            if (!await IsSelfService(userId, ad.Id))
            {
                return new OperationResult
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Nie można edytować cudzych ogłoszeń"
                };
            }
            var currentAd = await _unitOfWork.Advertisements.GetAdvertisementWithItem(ad.Id);
            if (currentAd.Item.GetType().Name.Equals(ad.Discriminator))
            {
                // here its okey
            }
            else
            {
                // here delete existing and add new maybe ad.Item = new Game() ?
            }
            throw new NotImplementedException();
        }

        public async Task<OperationResult> GetAdvertisement(int id)
        {
            var ad =  await _unitOfWork.Advertisements.GetAdvertisementWithItem(id);
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
                        @"Witaj. </br> Twoje ogłoszenie z serwisu Games4Trade zostało usunięte. Oto powód usunięcia ogłoszenia:<br>{0}",
                        message);
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

        public async Task<byte[]> GetAdPhoto(int adId, int photoId)
        {
            var photo = await _unitOfWork.Photos.GetASync(photoId);
            if (photo?.AdvertisementId == null || photo.AdvertisementId != adId)
            {
                return null;
            }
            var bytes = await File.ReadAllBytesAsync(photo.Path);
            return bytes;
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
                    region = await _unitOfWork.Regions.GetASync(ad.RegionId);
                    state = await _unitOfWork.States.GetASync(ad.StateId);
                    var genre = await _unitOfWork.Genres.GetASync(ad.GenreId);
                    system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                    objects = new List<object> { region, system, state, genre };

                    if (objects.Any(o => o == null))
                    {
                        var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                        return (false, message);
                    }

                    break;
                case "Console":
                    region = await _unitOfWork.Regions.GetASync(ad.RegionId);
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
                Description = ad.Description,               
                ExchangeActive = ad.ExchangeActive.GetValueOrDefault(),
                Price = ad.Price,
                Title = ad.Title,

                DateReleased = game.DateReleased,
                Developer = game.Developer
            };
            var tempGenre = await _unitOfWork.Genres.GetASync(game.GenreId);
            var tempRegion = await _unitOfWork.Regions.GetASync(game.GameRegionId);
            var tempState = await _unitOfWork.States.GetASync(game.StateId);
            var tempSystem = await _unitOfWork.Systems.GetASync(game.SystemId);
            var tempPhotos =
                await _unitOfWork.Photos.FindASync(p => p.AdvertisementId.HasValue && p.AdvertisementId == ad.Id);

            result.Genre = _mapper.Map<Genre, GenreDto>(tempGenre);
            result.Region = _mapper.Map<Region, RegionDto>(tempRegion);
            result.State = _mapper.Map<State, StateDto>(tempState);
            result.System = _mapper.Map<Models.System, SystemDto>(tempSystem);

            result.Photos = new List<PhotoDto>();
            foreach (var photo in tempPhotos)
            {
                result.Photos.Add(_mapper.Map<Photo, PhotoDto>(photo));
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
                Description = ad.Description,
                ExchangeActive = ad.ExchangeActive.GetValueOrDefault(),
                Price = ad.Price,
                Title = ad.Title,

                DateReleased = console.DateReleased
            };
            var tempRegion = await _unitOfWork.Regions.GetASync(console.ConsoleRegionId);
            var tempState = await _unitOfWork.States.GetASync(console.StateId);
            var tempSystem = await _unitOfWork.Systems.GetASync(console.SystemId);
            var tempPhotos =
                await _unitOfWork.Photos.FindASync(p => p.AdvertisementId.HasValue && p.AdvertisementId == ad.Id);


            result.Region = _mapper.Map<Region, RegionDto>(tempRegion);
            result.State = _mapper.Map<State, StateDto>(tempState);
            result.System = _mapper.Map<Models.System, SystemDto>(tempSystem);

            result.Photos = new List<PhotoDto>();
            foreach (var photo in tempPhotos)
            {
                result.Photos.Add(_mapper.Map<Photo, PhotoDto>(photo));
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
                Description = ad.Description,
                ExchangeActive = ad.ExchangeActive.GetValueOrDefault(),
                Price = ad.Price,
                Title = ad.Title,

                AccessoryManufacturer = accessory.AccessoryManufacturer,
                AccessoryModel = accessory.AccessoryModel,
                DateReleased = accessory.DateReleased
            };
            var tempState = await _unitOfWork.States.GetASync(accessory.StateId);
            var tempSystem = await _unitOfWork.Systems.GetASync(accessory.SystemId);
            var tempPhotos =
                await _unitOfWork.Photos.FindASync(p => p.AdvertisementId.HasValue && p.AdvertisementId == ad.Id);

            result.State = _mapper.Map<State, StateDto>(tempState);
            result.System = _mapper.Map<Models.System, SystemDto>(tempSystem);

            result.Photos = new List<PhotoDto>();
            foreach (var photo in tempPhotos)
            {
                result.Photos.Add(_mapper.Map<Photo, PhotoDto>(photo));
            }

            return result;
        }

    }
}
