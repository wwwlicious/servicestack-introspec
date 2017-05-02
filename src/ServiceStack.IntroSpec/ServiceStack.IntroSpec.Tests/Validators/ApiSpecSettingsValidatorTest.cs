// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Validators
{
    using System;
    using FluentValidation.TestHelper;
    using IntroSpec.Validators;
    using Xunit;

    public class ApiSpecSettingsValidatorTest
    {
        private readonly ApiSpecSettingsValidator validator = new ApiSpecSettingsValidator();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ContactName_IsMandatory(string name)
        {
            validator.ShouldHaveValidationErrorFor(v => v.ContactName, new TestSettings { ContactName = name });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ContactEmail_IsMandatory(string email)
        {
            validator.ShouldHaveValidationErrorFor(v => v.ContactEmail, new TestSettings { ContactEmail = email });
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test@example")]
        [InlineData("test.example.com")]
        public void ContactEmail_MustBeValidEmail(string email)
        {
            validator.ShouldHaveValidationErrorFor(v => v.ContactEmail, new TestSettings { ContactEmail = email });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Description_IsMandatory(string description)
        {
            validator.ShouldHaveValidationErrorFor(v => v.Description, new TestSettings { Description = description });
        }
    }

    public class TestSettings : IApiSpecSettings
    {
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public Uri ContactUrl { get; set; }
        public string Description { get; set; }
        public Uri LicenseUrl { get; set; }
    }
}
