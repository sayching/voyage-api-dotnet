﻿using FluentAssertions;
using Microsoft.AspNet.Identity;
using Moq;
using Voyage.Models.Entities;
using Voyage.Services.IdentityManagers;
using Voyage.Services.UnitTests.Common;
using Xunit;

namespace Voyage.Services.UnitTests.IdentityManagers
{
    public class ApplicationUserManagerTests : BaseUnitTest
    {
        private readonly ApplicationUserManager _manager;
        private readonly Mock<IUserTokenProvider<ApplicationUser, string>> _mockProvider;

        public ApplicationUserManagerTests()
        {
            _mockProvider = Mock.Create<IUserTokenProvider<ApplicationUser, string>>();
            var mockStore = Mock.Create<IUserStore<ApplicationUser>>();

            _manager = new ApplicationUserManager(mockStore.Object, _mockProvider.Object);
        }

        [Fact]
        public void PasswordValidator_Should_Be_Configured_With_Known_Values()
        {
            var validator = _manager.PasswordValidator as PasswordValidator;
            validator.Should().NotBeNull();

            validator.RequiredLength.Should().Be(6);
            validator.RequireNonLetterOrDigit.Should().BeTrue();
            validator.RequireDigit.Should().BeTrue();
            validator.RequireLowercase.Should().BeTrue();
            validator.RequireUppercase.Should().BeTrue();
        }

        [Fact]
        public void UserValidator_Should_Be_Configured_With_Known_Values()
        {
            var validator = _manager.UserValidator as UserValidator<ApplicationUser>;

            validator.AllowOnlyAlphanumericUserNames.Should().BeTrue();
            validator.RequireUniqueEmail.Should().BeFalse();
        }

        [Fact]
        public void UserTokenProvider_Should_Be_Injected_Value()
        {
            _manager.UserTokenProvider.Should().Be(_mockProvider.Object);
        }
    }
}
