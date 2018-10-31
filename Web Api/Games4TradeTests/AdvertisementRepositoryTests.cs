using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Games4Trade.Data;
using Games4Trade.Models;
using Games4Trade.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Games4TradeTests
{
    public class AdvertisementRepositoryTests
    {
        private readonly DbContextOptions<ApplicationContext> options;

        private readonly List<Advertisement> ads = new List<Advertisement>(){
            new Advertisement
            {
                Id = 1,
                UserId = 8,
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
                    Id = 1
                }
            },
            new Advertisement
            {
                Id = 2,
                UserId = 8,
                DateCreated = DateTime.Now.AddMonths(-1),
                ExchangeActive = true,
                IsActive = false,
                Price = 200,
                Item = new Game()
                {
                    AdvertisementId = 2,
                    GameRegionId = 1,
                    Description = "Not active",
                    GenreId = 1,
                    SystemId = 1,
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
                Price = 300,
                Item = new Games4Trade.Models.Console()
                {
                    AdvertisementId = 3,
                    ConsoleRegionId = 1,
                    Description = "Konsola",
                    SystemId = 1,
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
                Price = 400,
                Item = new Games4Trade.Models.Console()
                {
                    AdvertisementId = 4,
                    ConsoleRegionId = 1,
                    Description = "Konsola",
                    SystemId = 1,
                    Id = 4
                }
            }};

        public AdvertisementRepositoryTests()
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseInMemoryDatabase();
            options = builder.Options;
        }
        
        [Fact]
        public async void SortByPrice_Desc()
        {

            var ctx = new ApplicationContext(options);

            ctx.Advertisements.AddRange(ads);
            ctx.SaveChanges();
            var advertisementRepository = new AdvertisementRepository(ctx);

            var result = await advertisementRepository.GetQueriedAds(new AdQueryOptions()
            {
                Sort = "price",
                Desc = true,
                Systems = new int[0],
                Genres = new int[0]
            });


            Assert.Equal(4, result.FirstOrDefault().Id);
        }
    }
}
