using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;
using Games4Trade.Services;
using Moq;
using Xunit;

namespace Games4TradeTests
{
    public class AdvertisementsServiceFixture : IDisposable
    {
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
        public async void AddAdPositive()
        {
            // Arrange
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

            // Act
            await service.AddAdvertisement(userId: 1, ad: newAdd);

            // Assert
            Assert.NotNull(addedAdd);
            Assert.NotNull(addedGame);
            unitMock.Verify();

        }

        [Fact]
        public async void GetSearchedAdPositive()
        {
            // Arrange
            var unitMock = new Mock<IUnitOfWork>();
            unitMock.Setup(u => u.Advertisements.GetQueriedAds(It.IsAny<AdQueryOptions>()))
                .ReturnsAsync(new List<Advertisement>() {new Advertisement()
                {
                    Id = 1,
                    Item = new Game()
                }}).Verifiable();

            var service = new AdvertisementService(unitMock.Object, _fixture.Mapper);

            // Act
            var result = await service.GetAdvetisements(new AdQueryOptions()
            {
                Sort = "price",
                Desc = true,
                Systems = new int[0],
                Genres = new int[0]
            });
            var ads = result.Payload as List<AdvertisementWithoutItemDto>;

            // Assert
            Assert.NotNull(ads);
            Assert.Single(ads);
            unitMock.Verify();
        }
    }
}
