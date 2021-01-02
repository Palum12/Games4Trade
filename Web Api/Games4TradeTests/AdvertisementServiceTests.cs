using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Moq;
using Xunit;
using Games4TradeAPI.Services;
using Games4TradeAPI;
using Games4TradeAPI.Interfaces.Repositories;

namespace Games4TradeAPITests
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
            var regionRepository = new Mock<IRepository<Region>>();
            regionRepository.Setup(u => u.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Region()).Verifiable();

            var systemReposiotry = new Mock<ISystemRepository>();
            systemReposiotry.Setup(u => u.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Games4TradeAPI.Models.System()).Verifiable();

            var genreRepository = new Mock<IGenreRepository>();
            genreRepository.Setup(u => u.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Genre()).Verifiable();

            var stateRepository = new Mock<IRepository<State>>();
            stateRepository.Setup(u => u.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new State()).Verifiable();

            var advertisementRepository = new Mock<IAdvertisementReposiotry>();
            var advertisementItemRepository = new Mock<IRepository<AdvertisementItem>>();

            advertisementRepository
                .Setup(u => u.AddAsync(It.IsAny<Advertisement>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            advertisementItemRepository
                .Setup(u => u.AddAsync(It.IsAny<Game>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var service = new AdvertisementService(advertisementRepository.Object, null, null, stateRepository.Object, genreRepository.Object, regionRepository.Object, systemReposiotry.Object, advertisementItemRepository.Object, null, _fixture.Mapper);
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

            // Assert todo
            //advertisementRepository.Verify();
            //advertisementItemRepository.Verify();
        }

        [Fact]
        public async void GetSearchedAdPositive()
        {
            // Arrange
            var advertisementRepository = new Mock<IAdvertisementReposiotry>();
            advertisementRepository.Setup(u => u.GetQueriedAds(It.IsAny<AdQueryOptions>()))
                .ReturnsAsync(new List<Advertisement>() {new Advertisement()
                {
                    Id = 1,
                    Item = new Game()
                }}).Verifiable();

            var service = new AdvertisementService(advertisementRepository.Object, null, null, null, null, null, null, null, null, _fixture.Mapper);

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
            advertisementRepository.Verify();
        }
    }
}
