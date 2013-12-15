﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class MustNotFindUserByNameTests : FluentValidationTests
    {
        [Fact]
        public void Ctor_ThrowsArgumentNullException_WhenQueryProcessor_IsNull()
        {
            //var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var exception = Assert.Throws<ArgumentNullException>(() => new MustNotFindUserByName(null));
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("queries");
        }

        [Fact]
        public void IsInvalid_WhenUserName_AlreadyExists()
        {
            var userName = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeDataCommand { UserName = userName };
            var entity = new User { Name = command.UserName };
            Expression<Func<UserBy, bool>> expectedQuery = y => y.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(entity));
            var validator = new FakeDataMustNotExistValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            Func<ValidationFailure, bool> nameError = x => x.PropertyName == command.PropertyName(y => y.UserName);
            result.Errors.Count(nameError).ShouldEqual(1);
            result.Errors.Single(nameError).ErrorMessage.ShouldEqual(Resources.Validation_AlreadyExists
                .Replace("{PropertyName}", User.Constraints.NameLabel)
                .Replace("{PropertyValue}", command.UserName)
            );
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldHaveValidationErrorFor(x => x.UserName, command.UserName);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }

        [Fact]
        public void IsValid_WhenUserByName_DoesNotExist()
        {
            var userName = Guid.NewGuid().ToString();
            var queries = new Mock<IProcessQueries>(MockBehavior.Strict);
            var command = new FakeDataCommand { UserName = userName };
            Expression<Func<UserBy, bool>> expectedQuery = y => y.Name == command.UserName;
            queries.Setup(x => x.Execute(It.Is(expectedQuery))).Returns(Task.FromResult(null as User));
            var validator = new FakeDataMustNotExistValidator(queries.Object);

            var result = validator.Validate(command);

            result.IsValid.ShouldBeTrue();
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Once);
            validator.ShouldNotHaveValidationErrorFor(x => x.UserName, command.UserName);
            queries.Verify(x => x.Execute(It.Is(expectedQuery)), Times.Exactly(2));
        }
    }
}