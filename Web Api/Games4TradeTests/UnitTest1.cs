using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Data;
using Games4Trade.Models;
using Xunit;
using Moq;
using Games4Trade.Repositories;
using Games4Trade.Services;
using Microsoft.EntityFrameworkCore;

namespace Games4TradeTests
{
    public class UnitTest1
    {

        [Fact]
        public async Task Test1()
        {
            var unitMock = new Mock<IUnitOfWork>();
            unitMock.Setup(u => u.Users.GetUserByLogin("asad")).ReturnsAsync(new User(){Id=1});
            var mockMapper = new Mock<IMapper>();

            var loginService = new LoginService(unitMock.Object, mockMapper.Object);

            unitMock.Setup(u => u.Users.GetUserByLogin("asad")).ReturnsAsync(new User() { Id = 1 });

            /*var options = new DbContextOptionsBuilder<ApplicationContext>()
               
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;*/
            //unitMock.Setup(u => u.Users.FindASync(It.IsAny<User>())).ReturnsAsync(new List<User>() { new User() { Id = 2 } });



            var result = await loginService.CheckIfLoginIsTaken("asad");
            Assert.True(result.IsSuccessful);
        }
    }
}
