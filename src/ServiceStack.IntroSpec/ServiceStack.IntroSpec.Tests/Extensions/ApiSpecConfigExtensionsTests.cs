// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace ServiceStack.IntroSpec.Tests.Extensions
{
    using FluentAssertions;
    using ServiceStack.Configuration;
    using ServiceStack.IntroSpec.Extensions;
    using ServiceStack.IntroSpec.Settings;
    using Xunit;

    public class ApiSpecConfigExtensionsTests
    {
        private readonly ApiSpecConfig config = new ApiSpecConfig();
        private readonly AppSettings appSettings = new AppSettings();

        [Fact]
        public void Can_Get_ContactName()
        {
            appSettings.Set(ApiSpecConfigExtensions.Config_Key_ContactName, "bleh");

            config.FromAppSettings(appSettings);

            config.Contact.Name.Should().Be("bleh");
        }

        [Fact]
        public void Can_Get_ContactEmail()
        {
            appSettings.Set(ApiSpecConfigExtensions.Config_Key_ContactEmail, "bleh");

            config.FromAppSettings(appSettings);

            config.Contact.Email.Should().Be("bleh");
        }

        [Fact]
        public void Can_Get_ContactUrl()
        {
            appSettings.Set(ApiSpecConfigExtensions.Config_Key_ContactUrl, "http://bleh");

            config.FromAppSettings(appSettings);

            config.Contact.Url.Should().Be("http://bleh");
        }

        [Fact]
        public void Can_Get_Description()
        {
            appSettings.Set(ApiSpecConfigExtensions.Config_Key_Description, "bleh");

            config.FromAppSettings(appSettings);

            config.Description.Should().Be("bleh");
        }

        [Fact]
        public void Can_Get_LicenseUrl()
        {
            appSettings.Set(ApiSpecConfigExtensions.Config_Key_LicenseUrl, "http://bleh");

            config.FromAppSettings(appSettings);

            config.LicenseUrl.Should().Be("http://bleh");
        }
    }
}