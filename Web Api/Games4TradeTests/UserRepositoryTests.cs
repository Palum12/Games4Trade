using System;
using System.Collections.Generic;
using Games4Trade.Data;
using Games4Trade.Models;
using Games4Trade.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Games4TradeTests
{
    public class UsersFixture : IDisposable
    {
        private readonly DbContextOptions<ApplicationContext> options;

        private readonly List<User> users = new List<User>()
        {
            new User()
            {
                Id = 1,
                Email = "test1@test.pl",
                Login = "test1",
                Password = "test1"

            },
            new User()
            {
                Id = 2,
                Email = "test2@test.pl",
                Login = "test2",
                Password = "test2"
            },
            new User()
            {
                Id = 3,
                Email = "test3@test.pl",
                Login = "test3",
                Password = "test3"
            },
            new User()
            {
                Id = 4,
                Email = "test4@test.pl",
                Login = "test4",
                Password = "test4"
            }

        };
        private readonly List<ObservedUsersRelationship> obs = new List<ObservedUsersRelationship>()
        {
            new ObservedUsersRelationship()
            {
                ObservedUserId = 2,
                ObservingUserId = 1
            },
            new ObservedUsersRelationship()
            {
                ObservedUserId = 3,
                ObservingUserId = 1
            },
            new ObservedUsersRelationship()
            {
                ObservedUserId = 1,
                ObservingUserId = 2
            }
        };

        public readonly ApplicationContext ctx;

        public UsersFixture()
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseInMemoryDatabase();
            options = builder.Options;
            ctx = new ApplicationContext(options);

            ctx.Users.AddRange(users);
            ctx.ObservedUsersRelationship.AddRange(obs);
            ctx.SaveChanges();
        }

        public void Dispose()
        {
            ctx.Dispose();
        }
    }

    public class UserRepositoryTests : IClassFixture<UsersFixture>
    {
        private readonly UsersFixture _fixture;

        public UserRepositoryTests(UsersFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void GetUserByLoginTest()
        {
            var ctx = _fixture.ctx;
            var repository = new UserRepository(ctx);

            var result = await repository.GetUserByLogin("test1");
            var resultEmpty = await repository.GetUserByLogin("te");

            Assert.NotNull(result);
            Assert.Null(resultEmpty);
        }

        [Fact]
        public async void GetObservedUsersForUser()
        {
            var ctx = _fixture.ctx;
            var repository = new UserRepository(ctx);

            var listOfUsersForUser1 = await repository.GetObservedUsersForUser(1);
            var listOfUsersForUser3 = await repository.GetObservedUsersForUser(3);

            Assert.Equal(2, listOfUsersForUser1.Count);
            Assert.Empty(listOfUsersForUser3);
        }

        [Fact]
        public async void AddObservedUsersForUser()
        {
            var ctx = _fixture.ctx;
            var repository = new UserRepository(ctx);

            await repository.AddObsersvedUser(observedUserId: 3, observingUserId: 4);
            await ctx.SaveChangesAsync();

            var listOfUsersForUser4 = await repository.GetObservedUsersForUser(4);

            Assert.Single(listOfUsersForUser4);
        }

    }
}
