using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Games4Trade;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

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

    }
}
