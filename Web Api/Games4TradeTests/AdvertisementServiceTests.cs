using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade;
using Games4Trade.Data;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;
using Games4Trade.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System = Games4Trade.Models.System;

namespace Games4TradeTests
{
    public class AdvertisementsServiceFixture : IDisposable
    {
        private readonly List<Advertisement> ads = new List<Advertisement>()
        {
            new Advertisement
            {
                Id = 1,
                UserId = 8,
                Title = "test1",
                DateCreated = DateTime.Now.AddMonths(-1),
                ExchangeActive = true,
                IsActive = true,
                Price = 100,
                Item = new Game()
                {
                    AdvertisementId = 1,
                    Description = "Hey",
                    GameRegionId = 1,
                    GenreId = 1,
                    SystemId = 1,
                    Id = 1
                }
            },
            new Advertisement
            {
                Id = 2,
                UserId = 8,
                DateCreated = DateTime.Now.AddMonths(-1),
                ExchangeActive = true,
                Title = "test1",
                IsActive = false,
                Price = 200,
                Item = new Game()
                {
                    AdvertisementId = 2,
                    GameRegionId = 1,
                    Description = "Not active",
                    GenreId = 2,
                    SystemId = 2,
                    Id = 2
                }
            },
            new Advertisement
            {
                Id = 3,
                UserId = 8,
                DateCreated = DateTime.Now.AddMonths(-2),
                ExchangeActive = true,
                IsActive = true,
                Title = "search",
                Price = 300,
                Item = new Games4Trade.Models.Console()
                {
                    AdvertisementId = 3,
                    ConsoleRegionId = 1,
                    Description = "Konsola",
                    SystemId = 3,
                    Id = 3
                }
            },
            new Advertisement
            {
                Id = 4,
                UserId = 8,
                DateCreated = DateTime.Now.AddMonths(1),
                ExchangeActive = true,
                IsActive = true,
                Title = "test1",
                Price = 400,
                Item = new Games4Trade.Models.Console()
                {
                    AdvertisementId = 4,
                    ConsoleRegionId = 1,
                    Description = "Konsola",
                    SystemId = 2,
                    Id = 4
                }
            },
            new Advertisement
            {
                Id = 5,
                UserId = 10,
                DateCreated = DateTime.Now,
                ExchangeActive = true,
                IsActive = true,
                Title = "test1",
                Price = 100,
                Item = new Games4Trade.Models.Console()
                {
                    AdvertisementId = 5,
                    ConsoleRegionId = 1,
                    Description = "Konsola",
                    SystemId = 5,
                    Id = 5
                }
            }
        };

        public readonly IMapper Mapper;

        public AdvertisementsServiceFixture()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            Mapper = config.CreateMapper();
        }

        public void Dispose()
        {
        }
    }

    public class AdvertisementServiceTests : IClassFixture<AdvertisementsServiceFixture>
    {
        private readonly AdvertisementsServiceFixture _fixture;

        public AdvertisementServiceTests(AdvertisementsServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void TestAddAd()
        {
            var unitMock = new Mock<IUnitOfWork>();
            unitMock.Setup(u => u.Regions.GetASync(It.IsAny<int>()))
                .ReturnsAsync(new Region()).Verifiable();
            unitMock.Setup(u => u.Systems.GetASync(It.IsAny<int>()))
                .ReturnsAsync(new Games4Trade.Models.System()).Verifiable(); 
            unitMock.Setup(u => u.Genres.GetASync(It.IsAny<int>()))
                .ReturnsAsync(new Genre()).Verifiable(); 
            unitMock.Setup(u => u.States.GetASync(It.IsAny<int>()))
                .ReturnsAsync(new State()).Verifiable(); 

            Advertisement addedAdd = null;
            unitMock
                .Setup(u => u.Advertisements.AddASync(It.IsAny<Advertisement>()))
                .Callback<Advertisement>(a =>
                    {
                        addedAdd = a;
                    })
                .Returns(Task.CompletedTask);

            Game addedGame = null;
            unitMock
                .Setup(u => u.Games.AddASync(It.IsAny<Game>()))
                .Callback<Game>(g =>
                {
                    addedGame = g;
                })
                .Returns(Task.CompletedTask);
            var service = new AdvertisementService(unitMock.Object, _fixture.Mapper);
            var newAdd = new AdvertisementSaveDto
            {
                GenreId = 1,
                RegionId = 2,
                SystemId = 3,
                StateId = 4,
                Discriminator = "Game",
                Title = "Test of Add",
                Description = "Test of Add",
                ExchangeActive = true,
                Price = 10
            };

            await service.AddAdvertisement(userId: 1, ad: newAdd);

            Assert.NotNull(addedAdd);
            Assert.NotNull(addedGame);
            unitMock.Verify();

        }
    }
}
