// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests
{
    using System;
    using Documentation.Enrichers.Interfaces;
    using Documentation.Models;
    using Documentation.Settings;
    using FakeItEasy;
    using FluentAssertions;
    using Host;
    using Ploeh.AutoFixture.Xunit2;
    using Testing;
    using Xunit;

    public class ApiDocumentationGeneratorTests
    {
        private const string Desc = "I'm jim morrison. I'm dead.";
        private const string LicenseUri = "http://mozilla.org/MPL/2.0/";

        private readonly ApiDocumentationGenerator generator;
        private readonly IApiResourceEnricher enricher1 = A.Fake<IApiResourceEnricher>();
        private readonly IApiResourceEnricher enricher2 = A.Fake<IApiResourceEnricher>();
        private readonly IAppHost appHost = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
        private readonly ApiSpecConfig apiSpecConfig;
        private readonly ApiContact apiContact;

        public ApiDocumentationGeneratorTests()
        {
            generator = new ApiDocumentationGenerator(() => new[] { enricher1, enricher2 });

            apiContact = new ApiContact { Email = "email@address.com", Name = "Donald Gray" };
            apiSpecConfig = new ApiSpecConfig
            {
                // Validate all this (regex on email etc)
                Contact = apiContact,
                Description = Desc,
                LicenseUrl = new Uri(LicenseUri)
            };
        }

        [Fact]
        public void GenerateDocumentation_Throws_IfWebHostUrlNull()
        {
            var host = new BasicAppHost { Config = new HostConfig() };
            Action action = () => generator.GenerateDocumentation(null, host, apiSpecConfig);
            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineAutoData]
        public void GenerateDocumentation_SetsTitle_AsServiceName(string serviceName)
        {
            var host = new BasicAppHost { ServiceName = serviceName, Config = new HostConfig { WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, apiSpecConfig);
            doc.Title.Should().Be(serviceName);
        }

        [Fact]
        public void GenerateDocumentation_SetsApiVersion_FromConfig()
        {
            const string apiVersion = "16.2.3.1";
            var host = new BasicAppHost { Config = new HostConfig { ApiVersion = apiVersion, WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, apiSpecConfig);
            doc.ApiVersion.Should().Be(apiVersion);
        }

        [Fact]
        public void GenerateDocumentation_SetsBaseUrl_FromConfig()
        {
            const string hostUrl = "https://127.0.0.5:9090/api";
            var host = new BasicAppHost { Config = new HostConfig { WebHostUrl = hostUrl } };
            var doc = generator.GenerateDocumentation(null, host, apiSpecConfig);
            doc.ApiBaseUrl.Should().Be(hostUrl);
        }

        [Fact]
        public void GenerateDocumentation_SetsContact()
        {
            var host = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, apiSpecConfig);
            doc.Contact.Should().Be(apiContact);
        }

        [Fact]
        public void GenerateDocumentation_SetsDescriptiont()
        {
            var host = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, apiSpecConfig);
            doc.Description.Should().Be(Desc);
        }

        [Fact]
        public void GenerateDocumentation_SetsLicense()
        {
            var host = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, apiSpecConfig);
            doc.LicenceUrl.Should().Be(LicenseUri);
        }

        [Fact]
        public void GenerateDocumentation_HandlesNullLicense()
        {
            var host = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
            apiSpecConfig.LicenseUrl = null;
            var doc = generator.GenerateDocumentation(null, host, apiSpecConfig);
            doc.LicenceUrl.Should().BeNull();
        }

        [Fact]
        public void GenerateDocumentation_CallsEnrichersInOrder()
        {
            var operation = new Operation();
            generator.GenerateDocumentation(new[] { operation }, appHost, apiSpecConfig);

            A.CallTo(() => enricher1.Enrich(A<ApiResourceDocumentation>.Ignored, operation)).MustHaveHappened()
                .Then(
                    A.CallTo(() => enricher2.Enrich(A<ApiResourceDocumentation>.Ignored, operation)).MustHaveHappened());
        }

        [Fact]
        public void GenerateDocumentation_CallsEnrichersInOrder_WithAllOperations()
        {
            var operation = new Operation();
            var operation2 = new Operation();

            generator.GenerateDocumentation(new[] { operation, operation2 }, appHost, apiSpecConfig);

            // :(
            A.CallTo(() => enricher1.Enrich(A<ApiResourceDocumentation>.Ignored, operation)).MustHaveHappened()
                .Then(
                    A.CallTo(() => enricher2.Enrich(A<ApiResourceDocumentation>.Ignored, operation)).MustHaveHappened())
                .Then(
                    A.CallTo(() => enricher1.Enrich(A<ApiResourceDocumentation>.Ignored, operation2)).MustHaveHappened())
                .Then(
                    A.CallTo(() => enricher2.Enrich(A<ApiResourceDocumentation>.Ignored, operation2)).MustHaveHappened());
        }
    }
}
