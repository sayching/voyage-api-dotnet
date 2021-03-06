﻿using FluentValidation.TestHelper;
using Voyage.Models.UnitTests.Common;
using Voyage.Models.Validators;
using Xunit;

namespace Voyage.Models.UnitTests.Validators
{
    [Trait("Category", "Model.Validation")]
    public class RoleModelValidatorTests : BaseUnitTest
    {
        private readonly RoleModelValidator _validator;

        public RoleModelValidatorTests()
        {
            _validator = new RoleModelValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Null()
        {
            _validator.ShouldHaveValidationErrorFor(role => role.Name, null as string);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            _validator.ShouldHaveValidationErrorFor(role => role.Name, string.Empty);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Populated()
        {
            _validator.ShouldNotHaveValidationErrorFor(role => role.Name, "Role Name");
        }
    }
}
