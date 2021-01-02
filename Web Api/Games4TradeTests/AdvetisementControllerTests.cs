using System;
using System.Security.Claims;
using Games4TradeAPI.Controllers;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Games4TradeAPI.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Games4TradeAPI.Common;

namespace Games4TradeAPITests
{
    public class AdvertisementsControllerFixture : IDisposable
    {
        public ClaimsPrincipal User;
        public AdvertisementSaveDto NewAdd;

        public AdvertisementsControllerFixture()
        {
             User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            NewAdd = new AdvertisementSaveDto
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
        }

        public void Dispose()
        {
        }
    }

    public class AdvetisementControllerTests : IClassFixture<AdvertisementsControllerFixture>
    {
        private readonly AdvertisementsControllerFixture _fixture;

        public AdvetisementControllerTests(AdvertisementsControllerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void AddAdPositive()
        {
            // Arrange
            var adServiceMock = new Mock<IAdvertisementService>();
            adServiceMock
                .Setup(a => a.AddAdvertisement(It.IsAny<int>(), It.IsAny<AdvertisementSaveDto>()))
                .ReturnsAsync(new OperationResult()
                {
                    IsSuccessful = true
                })
                .Verifiable();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(u => u.GetUserIdByLogin(It.IsAny<string>())).ReturnsAsync(1);

            var controller = new AdvertisementsController(adServiceMock.Object, userServiceMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _fixture.User }
            };

            // Act
            var result = await controller.AddAd(_fixture.NewAdd);

            // Assert 
            adServiceMock.Verify();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void AddAdNegative()
        {
            // Arrange
            var adServiceMock = new Mock<IAdvertisementService>();
            adServiceMock
                .Setup(a =>
                    a.AddAdvertisement(It.IsAny<int>(), It.IsAny<AdvertisementSaveDto>()))
                .ReturnsAsync(new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true
                })
                .Verifiable();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(u => u.GetUserIdByLogin(It.IsAny<string>())).ReturnsAsync(1);

            var controller = new AdvertisementsController(adServiceMock.Object, userServiceMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                    HttpContext = new DefaultHttpContext() {User = _fixture.User}
            };
            
            // Act
            var result = await controller.AddAd(_fixture.NewAdd);

            // Assert 
            adServiceMock.Verify();
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}