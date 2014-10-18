﻿using System;
using System.Data.Entity;
using System.Linq;
using Moq;
using Should;
using Xunit;
using Xunit.Extensions;

namespace Tripod.Domain.Security
{
    public class EmailAddressesByTests
    {
        #region EmailAddressesBy UserId

        [Fact]
        public void Query_IntCtor_SetsUserIdProperty()
        {
            var userId = new Random().Next(int.MinValue, int.MaxValue);
            var query = new EmailAddressesBy(userId);
            query.UserId.ShouldEqual(userId);
        }

        [Fact]
        public void Handler_ReturnsNoEmailAddresses_ByUserId_WhenNotFound()
        {
            var userId = new Random().Next(3, int.MaxValue);
            var data = new[]
            {
                new EmailAddress { UserId = userId - 2, IsPrimary = true, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 1, IsPrimary = true, },
                new EmailAddress { UserId = userId - 1, },
            }.AsQueryable();
            var query = new EmailAddressesBy(userId);
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressesByQuery(entities.Object);

            EmailAddress[] result = handler.Handle(query).Result.ToArray();

            Assert.NotNull(result);
            result.Length.ShouldEqual(0);
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handler_ReturnsNoEmailAddress_ByUserId_WhenFound_ButIsVerified_IsNotEqual(bool isVerified)
        {
            var userId = new Random().Next(1, int.MaxValue);
            var data = new[]
            {
                new EmailAddress { UserId = userId - 2, IsPrimary = true, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 1, IsPrimary = true, },
                new EmailAddress { UserId = userId - 1, },
                new EmailAddress { UserId = userId, IsVerified = !isVerified, },
            }.AsQueryable();
            var query = new EmailAddressesBy(userId)
            {
                IsVerified = isVerified
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressesByQuery(entities.Object);

            EmailAddress[] result = handler.Handle(query).Result.ToArray();

            Assert.NotNull(result);
            result.Length.ShouldEqual(0);
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(null, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Handler_ReturnsEmailAddresses_ByUserId_WhenFound_AndIsVerifiedMatches(
            bool? queryIsVerified, bool entityIsVerified)
        {
            var userId = new Random().Next(3, int.MaxValue);
            var data = new[]
            {
                new EmailAddress { UserId = userId - 2, IsPrimary = true, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 2, },
                new EmailAddress { UserId = userId - 1, IsPrimary = true, },
                new EmailAddress { UserId = userId - 1, },
                new EmailAddress { UserId = userId, IsVerified = entityIsVerified, },
                new EmailAddress { UserId = userId, IsVerified = !entityIsVerified, },
            }.AsQueryable();
            var query = new EmailAddressesBy(userId)
            {
                IsVerified = queryIsVerified,
            };
            var dbSet = new Mock<DbSet<EmailAddress>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<EmailAddress>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<EmailAddress>()).Returns(entitySet);
            var handler = new HandleEmailAddressesByQuery(entities.Object);

            EmailAddress[] result = handler.Handle(query).Result.ToArray();

            Assert.NotNull(result);
            result.Length.ShouldEqual(queryIsVerified.HasValue ? 1 : 2);
            entities.Verify(x => x.Query<EmailAddress>(), Times.Once);
            if (queryIsVerified.HasValue)
            {
                EmailAddress expectedEntity = data.Single(x => x.UserId == userId && x.IsVerified == entityIsVerified);
                result.Single().ShouldEqual(expectedEntity);
            }
        }

        #endregion
    }
}
