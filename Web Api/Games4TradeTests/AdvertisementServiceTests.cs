using Games4TradeAPI.Core.Advertisements;
using Games4TradeAPI.Core.Images;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Models;
using Games4TradeAPI.Services;
using Moq;
using Xunit;

namespace Games4TradeAPITests;

public class AdvertisementServiceTests
{
    [Theory]
    [InlineData("game", typeof(Game))]
    [InlineData("Console", typeof(Games4TradeAPI.Models.Console))]
    [InlineData("Accessory", typeof(Accessory))]
    public void StrategyResolver_ResolvesDiscriminatorAndRuntimeType(
        string discriminator,
        Type expectedItemType)
    {
        var stateRepository = Mock.Of<IRepository<State>>();
        var systemRepository = Mock.Of<ISystemRepository>();
        var genreRepository = Mock.Of<IGenreRepository>();
        var regionRepository = Mock.Of<IRepository<Region>>();
        IAdvertisementItemStrategy[] strategies =
        {
            new GameAdvertisementItemStrategy(
                stateRepository,
                systemRepository,
                genreRepository,
                regionRepository),
            new ConsoleAdvertisementItemStrategy(
                stateRepository,
                systemRepository,
                regionRepository),
            new AccessoryAdvertisementItemStrategy(stateRepository, systemRepository)
        };
        var resolver = new AdvertisementItemStrategyResolver(strategies);
        var item = Assert.IsAssignableFrom<AdvertisementItem>(Activator.CreateInstance(expectedItemType));

        Assert.True(resolver.TryResolve(discriminator, out var resolvedByDiscriminator));
        Assert.Equal(expectedItemType, resolvedByDiscriminator.ItemType);
        Assert.Same(resolvedByDiscriminator, resolver.Resolve(item));
    }

    [Fact]
    public async Task AddAdvertisement_UsesGameStrategyAndManualMapping()
    {
        var regionRepository = new Mock<IRepository<Region>>();
        regionRepository.Setup(repository => repository.GetAsync(2))
            .ReturnsAsync(new Region { Id = 2 });

        var systemRepository = new Mock<ISystemRepository>();
        systemRepository.Setup(repository => repository.GetAsync(3))
            .ReturnsAsync(new Games4TradeAPI.Models.System { Id = 3 });

        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(repository => repository.GetAsync(1))
            .ReturnsAsync(new Genre { Id = 1 });

        var stateRepository = new Mock<IRepository<State>>();
        stateRepository.Setup(repository => repository.GetAsync(4))
            .ReturnsAsync(new State { Id = 4 });

        var advertisementRepository = new Mock<IAdvertisementReposiotry>();
        var advertisementItemRepository = new Mock<IRepository<AdvertisementItem>>();
        Advertisement? savedAdvertisement = null;

        advertisementRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Advertisement>()))
            .Callback<Advertisement>(advertisement => savedAdvertisement = advertisement)
            .Returns(Task.CompletedTask);
        advertisementRepository.Setup(repository => repository.SaveChangesAsync()).ReturnsAsync(1);
        advertisementItemRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Game>()))
            .Returns(Task.CompletedTask);

        var gameStrategy = new GameAdvertisementItemStrategy(
            stateRepository.Object,
            systemRepository.Object,
            genreRepository.Object,
            regionRepository.Object);
        var resolver = new AdvertisementItemStrategyResolver(new[] { gameStrategy });
        var service = CreateService(
            advertisementRepository.Object,
            advertisementItemRepository.Object,
            resolver);
        var releaseDate = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Unspecified);
        var newAdvertisement = new AdvertisementSaveDto
        {
            GenreId = 1,
            RegionId = 2,
            SystemId = 3,
            StateId = 4,
            Discriminator = "Game",
            Title = "Test of Add",
            Description = "Test description",
            Developer = "Test developer",
            DateReleased = releaseDate,
            ExchangeActive = true,
            Price = 10
        };

        var result = await service.AddAdvertisement(userId: 1, ad: newAdvertisement);

        Assert.True(result.IsSuccessful);
        Assert.NotNull(savedAdvertisement);
        Assert.Equal(1, savedAdvertisement.UserId);
        Assert.Equal("Test of Add", savedAdvertisement.Title);
        var game = Assert.IsType<Game>(savedAdvertisement.Item);
        Assert.Equal("Test developer", game.Developer);
        Assert.Equal(1, game.GenreId);
        Assert.Equal(2, game.GameRegionId);
        Assert.Equal(DateTimeKind.Utc, game.DateReleased?.Kind);
        advertisementRepository.Verify(repository => repository.AddAsync(savedAdvertisement), Times.Once);
        advertisementItemRepository.Verify(repository => repository.AddAsync(game), Times.Once);
    }

    [Fact]
    public async Task AddAdvertisement_ReturnsClientErrorForUnknownDiscriminator()
    {
        var advertisementRepository = new Mock<IAdvertisementReposiotry>();
        var advertisementItemRepository = new Mock<IRepository<AdvertisementItem>>();
        var resolver = new AdvertisementItemStrategyResolver(Array.Empty<IAdvertisementItemStrategy>());
        var service = CreateService(
            advertisementRepository.Object,
            advertisementItemRepository.Object,
            resolver);

        var result = await service.AddAdvertisement(1, new AdvertisementSaveDto
        {
            Discriminator = "Unknown"
        });

        Assert.False(result.IsSuccessful);
        Assert.True(result.IsClientError);
        Assert.Equal("Invalid discriminator!", result.Message);
        advertisementRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Advertisement>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAdvertisements_UsesManualSummaryMapping()
    {
        var advertisementRepository = new Mock<IAdvertisementReposiotry>();
        advertisementRepository.Setup(repository => repository.GetQueriedAds(It.IsAny<AdQueryOptions>()))
            .ReturnsAsync(new List<Advertisement>
            {
                new()
                {
                    Id = 1,
                    UserId = 7,
                    Title = "Mapped advertisement",
                    ExchangeActive = true,
                    Photos = new List<Photo> { new() { Id = 42 } },
                    Item = new Game()
                }
            });

        var resolver = new AdvertisementItemStrategyResolver(Array.Empty<IAdvertisementItemStrategy>());
        var service = CreateService(
            advertisementRepository.Object,
            Mock.Of<IRepository<AdvertisementItem>>(),
            resolver);

        var result = await service.GetAdvetisements(new AdQueryOptions
        {
            Sort = "price",
            Desc = true,
            Systems = Array.Empty<int>(),
            Genres = Array.Empty<int>()
        });

        var advertisements = Assert.IsType<List<AdvertisementWithoutItemDto>>(result.Payload);
        var advertisement = Assert.Single(advertisements);
        Assert.Equal(1, advertisement.Id);
        Assert.Equal(7, advertisement.UserId);
        Assert.Equal(42, advertisement.MainPhotoId);
        Assert.Equal("Mapped advertisement", advertisement.Title);
        advertisementRepository.Verify();
    }

    private static AdvertisementService CreateService(
        IAdvertisementReposiotry advertisementRepository,
        IRepository<AdvertisementItem> advertisementItemRepository,
        IAdvertisementItemStrategyResolver resolver)
    {
        return new AdvertisementService(
            advertisementRepository,
            Mock.Of<IUserRepository>(),
            Mock.Of<IRepository<Photo>>(),
            advertisementItemRepository,
            resolver,
            Mock.Of<IThumbnailGenerator>());
    }
}
