// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using DataAnnotations;
    using FakeItEasy;
    using Fixtures;
    using FluentAssertions;
    using Host;
    using IntroSpec;
    using IntroSpec.Models;
    using IntroSpec.Services;
    using IntroSpec.Settings;
    using NativeTypes;
    using Xunit;
    using static FakeItEasy.A;

    [Collection("AppHost")]
    public class IntroSpecFeatureTests
    {
        private readonly IAppSettings settings;
        private readonly IntroSpecFeature feature;
        private readonly ApiSpecConfig apiSpecConfig;
        private readonly IApiDocumentationGenerator generator;
        private readonly Func<KeyValuePair<Type, Operation>, bool> filter;
        private readonly AppHostFixture fixture;

        public IntroSpecFeatureTests(AppHostFixture fixture)
        {
            this.fixture = fixture;

            apiSpecConfig = new ApiSpecConfig
            {
                Contact = new ApiContact { Email = "ronald.macdonald@macdonalds.hq", Name = "ronnie mcd" },
                Description = "great api"
            };
            settings = Fake<IAppSettings>();
            CallTo(() => settings.GetString(A<string>.Ignored)).Returns("valid@value.net");
            CallTo(() => settings.Get<Uri>(A<string>.Ignored)).Returns(new Uri("http://test.com"));

            generator = Fake<IApiDocumentationGenerator>();

            filter = Fake<Func<KeyValuePair<Type, Operation>, bool>>();

            feature = new IntroSpecFeature()
                .WithGenerator(generator)
                .WithOperationsFilter(filter);
        }

        [Fact]
        public void Ctor_Throws_IfConfigNull()
        {
            Action action = () => new ApiSpecFeature(config => null);
            action.Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void Register_Throws_IfNoMetadata()
        {
            var appHost = Fake<IAppHost>();
            CallTo(() => appHost.AppSettings).Returns(settings);
            Action action = () => feature.Register(appHost);
            action.Should().Throw<ArgumentException>()
                  .WithMessage("The Metadata Feature must be enabled to use the ApiSpec Feature");
        }

        [Fact]
        public void Register_Throws_IfConfigInvalid()
        {
            Action action = () => feature.Register(Fake<IAppHost>());
            action.Should().Throw<ArgumentException>().WithMessage(
                "Validation failed: \r\n -- 'Contact Name' should not be empty.\r\n -- 'Contact Email' should not be empty.\r\n -- 'Contact Email' is not a valid email address.\r\n -- 'Description' should not be empty.");
        }

        [Fact]
        public void Register_RegistersService()
        {
            feature.Register(fixture.AppHost);
            fixture.AppHost.Container.TryResolve<ApiSpecService>().Should().NotBeNull();
        }

        [Fact]
        public void OperationsMapFilter_ExcludesTypesInIgnoreNamespaces()
        {
            var operationsMap = new Dictionary<Type, Operation>
            {
                { typeof(int), new Operation { RequestType = typeof(int) } },
                { typeof(TypesKotlin), new Operation { RequestType = typeof(TypesKotlin) } }
            };

            var filter = new ApiSpecFeature(config => apiSpecConfig).OperationsMapFilter;

            var result = operationsMap.Where(o => filter(o)).Select(o => o.Value).ToList();
            result.Count.Should().Be(1);
            result[0].RequestType.Should().Be<int>();
        }

        [Theory]
        [InlineData(typeof(ExcludeMetaData), 1)]
        [InlineData(typeof(ExcludeServiceDiscovery), 1)]
        [InlineData(typeof(ExcludeBoth), 1)]
        [InlineData(typeof(ExcludeRandom), 2)]
        public void OperationsMapFilter_ObeysExcludeAttribute(Type requestType, int expectedCount)
        {
            var operationsMap = new Dictionary<Type, Operation>
            {
                { typeof(int), new Operation { RequestType = typeof(int) } },
                { requestType, new Operation { RequestType = requestType } }
            };

            var filter = new ApiSpecFeature(config => apiSpecConfig).OperationsMapFilter;

            var result = operationsMap.Where(o => filter(o)).Select(o => o.Value).ToList();
            result.Count.Should().Be(expectedCount);
            result[0].RequestType.Should().Be<int>();
        }

        [Fact]
        public void ContactName_Get_GetsFromAppSettings()
        {
            const string name = "Kyle Reese";
            CallTo(() => settings.GetString(ConfigKeys.ContactName)).Returns(name);

            GetFeature().ContactName.Should().Be(name);
            CallTo(() => settings.GetString(ConfigKeys.ContactName)).MustHaveHappened();
        }

        [Fact]
        public void ContactName_Set_SetsInAppSettings()
        {
            const string name = "Kyle Reese";

            GetFeature().ContactName = name;
            CallTo(() => settings.Set(ConfigKeys.ContactName, name)).MustHaveHappened();
        }

        [Fact]
        public void ContactEmail_Get_GetsFromAppSettings()
        {
            const string email = "test@example.com";
            CallTo(() => settings.GetString(ConfigKeys.ContactEmail)).Returns(email);

            GetFeature().ContactEmail.Should().Be(email);
            CallTo(() => settings.GetString(ConfigKeys.ContactEmail)).MustHaveHappened();
        }

        [Fact]
        public void ContactEmail_Set_SetsInAppSettings()
        {
            const string email = "test@example.com";

            GetFeature().ContactEmail = email;
            CallTo(() => settings.Set(ConfigKeys.ContactEmail, email)).MustHaveHappened();
        }

        [Fact]
        public void ContactUrl_Get_GetsFromAppSettings()
        {
            var url = new Uri("http://here.com");
            CallTo(() => settings.Get<Uri>(ConfigKeys.ContactUrl)).Returns(url);

            GetFeature().ContactUrl.Should().Be(url);
            CallTo(() => settings.Get<Uri>(ConfigKeys.ContactUrl)).MustHaveHappened();
        }

        [Fact]
        public void ContactUrl_Set_SetsInAppSettings()
        {
            var url = new Uri("http://here.com");

            GetFeature().ContactUrl = url;
            CallTo(() => settings.Set(ConfigKeys.ContactUrl, url)).MustHaveHappened();
        }

        [Fact]
        public void Description_Get_GetsFromAppSettings()
        {
            const string description = "foo bar";
            CallTo(() => settings.GetString(ConfigKeys.Description)).Returns(description);

            GetFeature().Description.Should().Be(description);
            CallTo(() => settings.GetString(ConfigKeys.Description)).MustHaveHappened();
        }

        [Fact]
        public void Description_Set_SetsInAppSettings()
        {
            const string description = "foo bar";

            GetFeature().Description = description;
            CallTo(() => settings.Set(ConfigKeys.Description, description)).MustHaveHappened();
        }

        [Fact]
        public void LicenseUrl_Get_GetsFromAppSettings()
        {
            var url = new Uri("http://here.com");
            CallTo(() => settings.Get<Uri>(ConfigKeys.LicenseUrl)).Returns(url);

            GetFeature().LicenseUrl.Should().Be(url);
            CallTo(() => settings.Get<Uri>(ConfigKeys.LicenseUrl)).MustHaveHappened();
        }

        [Fact]
        public void LicenseUrl_Set_SetsInAppSettings()
        {
            var url = new Uri("http://here.com");

            GetFeature().LicenseUrl = url;
            CallTo(() => settings.Set(ConfigKeys.LicenseUrl, url)).MustHaveHappened();
        }

        private IntroSpecFeature GetFeature()
        {
            var apiSpecFeature = new IntroSpecFeature();
            fixture.AppHost.AppSettings = settings;
            apiSpecFeature.Register(fixture.AppHost);
            return apiSpecFeature;
        }
    }

    [Exclude(Feature.Metadata)] public class ExcludeMetaData { }

    [Exclude(Feature.ServiceDiscovery)] public class ExcludeServiceDiscovery { }

    [Exclude(Feature.ServiceDiscovery | Feature.Metadata)] public class ExcludeBoth { }

    [Exclude(Feature.Jsv)] public class ExcludeRandom { }
}
