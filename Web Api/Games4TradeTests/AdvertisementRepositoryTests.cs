using System;
using System.Collections.Generic;
using System.Linq;
using Games4TradeAPI.Data;
using Games4TradeAPI.Models;
using Games4TradeAPI.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Games4TradeAPI.Repositories;

namespace Games4TradeAPITests
{
    public class AdvertisementRepositoryFixture : IDisposable
    {
        private readonly DbContextOptions<ApplicationContext> options;

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
                Item = new Games4TradeAPI.Models.Console()
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
                Item = new Games4TradeAPI.Models.Console()
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
                Item = new Games4TradeAPI.Models.Console()
                {
                    AdvertisementId = 5,
                    ConsoleRegionId = 1,
                    Description = "Konsola",
                    SystemId = 5,
                    Id = 5
                }
            }
        };

        private readonly List<ObservedUsersRelationship> obs = new List<ObservedUsersRelationship>()
        {
            new ObservedUsersRelationship()
            {
                ObservedUserId = 10,
                ObservingUserId = 1
            }
        };
        private readonly List<UserLikedGenre> userLikedGenres = new List<UserLikedGenre>()
        {
            new UserLikedGenre()
            {
                GenreId = 1,
                UserId = 1
            }
        };
        private readonly List<UserOwnedSystem> userOwnedSystems = new List<UserOwnedSystem>()
        {
            new UserOwnedSystem()
            {
                SystemId = 2,
                UserId = 1
            }
        };

        public readonly ApplicationContext ctx;

        public AdvertisementRepositoryFixture()
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>().UseInMemoryDatabase("testBase");
            options = builder.Options;
            ctx = new ApplicationContext(options);

            ctx.Advertisements.AddRange(ads);
            ctx.ObservedUsersRelationship.AddRange(obs);
            ctx.UserGenreRelationship.AddRange(userLikedGenres);
            ctx.UserSystemRelationship.AddRange(userOwnedSystems);
            ctx.SaveChanges();
        }

        public void Dispose()
        {
            ctx.Dispose();
        }    
    }

    public class AdvertisementRepositoryTests : IClassFixture<AdvertisementRepositoryFixture>
    {
        private readonly AdvertisementRepositoryFixture _fixture;

        public AdvertisementRepositoryTests(AdvertisementRepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void AddAdvertisement()
        {
            var ctx = _fixture.ctx;
            var advertisementRepository = new AdvertisementRepository(ctx);
            var advert = new Advertisement
            {
                Id = 6,
                UserId = 8,
                Title = "test1",
                DateCreated = DateTime.Now.AddMonths(-1),
                ExchangeActive = true,
                IsActive = true,
                Price = 100,
                Item = new Game()
                {
                    Id = 6,
                    AdvertisementId = 6,
                    Description = "Hey",
                    GameRegionId = 1,
                    GenreId = 1,
                }
            };
            await advertisementRepository.AddAsync(advert);
            var ads = await advertisementRepository.GetAsync(6);
            Assert.NotNull(ads);
        }

        [Fact]
        public async void SearchConsolesPositive()
        {
            // Arrange
            var ctx = _fixture.ctx;
            var advertisementRepository = new AdvertisementRepository(ctx);

            // Act         
            var consoles = await advertisementRepository.GetQueriedAds(new AdQueryOptions()
            {
                Type = "console",
                Systems = new int[0],
                Genres = new int[0]
            }) as List<Advertisement>;

            // Assert

            Assert.Equal(3, consoles.Count);
        }


        [Fact]
        public async void SearchAccessoriesNegative()
        {

            var ctx = _fixture.ctx;
            var advertisementRepository = new AdvertisementRepository(ctx);

            var accessories = await advertisementRepository.GetQueriedAds(new AdQueryOptions()
            {
                Type = "accessory",
                Systems = new int[0],
                Genres = new int[0]
            }) as List<Advertisement>;
            
            Assert.Empty(accessories);           
        }

        [Fact]
        public async void GetAdsForUserTest()
        {

            var ctx = _fixture.ctx;
            var advertisementRepository = new AdvertisementRepository(ctx);

            var adsActiveForUser = await advertisementRepository.GetAdsForUser(userId: 8,page: 0, pageSize: 10, skipInactive: true);
            var adsInActiveForUser =
                await advertisementRepository.GetAdsForUser(userId: 8, page: 0, pageSize: 10, skipInactive: false);
            Assert.Equal(4, adsActiveForUser.Count());
            Assert.Equal(3, adsInActiveForUser.Count());
        }

        [Fact]
        public async void GetRecommendedAdsForUserPositive()
        {

            var ctx = _fixture.ctx;
            var advertisementRepository = new AdvertisementRepository(ctx);

            var adsForUser = await advertisementRepository.GetRecommendedAdvertisements(1, 0, 10);

            // should return 3
            // 1 for owned system (other one is inactive)
            // 1 for liked genre
            // 1 for observed users
            Assert.Equal(3, adsForUser.Count());
        }

    }
}
