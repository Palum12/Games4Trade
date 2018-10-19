using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;

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

            }
            var advertisement = new Advertisement()
            {
                UserId = userId,
                DateCreated = DateTime.Now,
                Description = ad.Description,
                Title = ad.Title,
                ExchangeActive = ad.ExchangeActive,
                IsActive = true,
                Price = ad.Price,
                ShowUserPhoneNumber = ad.ShowUserPhoneNumber,
                ShowUserEmail = ad.ShowUserEmail
            };


            switch (ad.Discriminator)
            {
                case "Game":
                    var game = new Game();
                    game.DateDeveloped = ad.DateDeveloped;
                    game.Developer = ad.Developer;
                    game.GameRegionId = ad.RegionId;
                    game.GenreId = ad.GenreId;
                    game.SystemId = ad.SystemId;
                    game.StateId = ad.StateId;
                    await _unitOfWork.Games.AddASync(game);
                    advertisement.Item = game;
                    break;
                case "Accessory":
                    var accessory= new Accessory();
                    accessory.AccessoryManufacturer = ad.AccessoryManufacturer;
                    accessory.AccessoryModel = ad.AccessoryModel;
                    accessory.StateId = ad.StateId;
                    accessory.SystemId = ad.SystemId;
                    await _unitOfWork.Accessories.AddASync(accessory);
                    advertisement.Item = accessory;
                    break;
                case "Console":
                    var console = new Games4Trade.Models.Console();
                    console.ConsoleRegionId = ad.RegionId;
                    console.DateManufactured = ad.DateManufactured;
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

        private async Task<(bool, string)> CheckIfRelationshipsAreCorrect(AdvertisementSaveDto ad)
        {
            IList<Object> objects;
            //todo change to switch
            if (ad.Discriminator.Equals("Game"))
            {
                var region = await _unitOfWork.Regions.GetASync(ad.RegionId);
                var state = await _unitOfWork.States.GetASync(ad.StateId);
                var genre = await _unitOfWork.Genres.GetASync(ad.GenreId);
                var system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                objects = new List<object> { region, system, state, genre };

                if (objects.Any(o => o == null))
                {
                    var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                    return (false, message);
                }
            }

            if (ad.Discriminator.Equals("Console"))
            {
                var region = await _unitOfWork.Regions.GetASync(ad.RegionId);
                var state = await _unitOfWork.States.GetASync(ad.StateId);
                var system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                objects = new List<object> { region, system, state };

                if (objects.Any(o => o == null))
                {
                    var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                    return (false, message);
                }
            }

            if (ad.Discriminator.Equals("Accessory"))
            {
                var state = await _unitOfWork.States.GetASync(ad.StateId);
                var system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                objects = new List<object> { system, state };

                if (objects.Any(o => o == null))
                {
                    var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                    return (false, message);
                }
            }
            return (true, null);
        }

    }
}
