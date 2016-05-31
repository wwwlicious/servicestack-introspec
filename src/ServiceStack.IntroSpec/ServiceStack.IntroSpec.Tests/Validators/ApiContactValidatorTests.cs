// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Validators
{
    using FluentValidation.TestHelper;
    using IntroSpec.Models;
    using IntroSpec.Validators;
    using Xunit;

    public class ApiContactValidatorTests
    {
        private readonly ApiContactValidator validator = new ApiContactValidator();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Name_IsMandatory(string name)
        {
            validator.ShouldHaveValidationErrorFor(v => v.Name, new ApiContact { Name = name });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Email_IsMandatory(string email)
        {
            validator.ShouldHaveValidationErrorFor(v => v.Email, new ApiContact { Email = email });
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test@example")]
        [InlineData("test.example.com")]
        public void Email_MustBeValidEmail(string email)
        {
            validator.ShouldHaveValidationErrorFor(v => v.Email, new ApiContact { Email = email });
        }
    }
}
