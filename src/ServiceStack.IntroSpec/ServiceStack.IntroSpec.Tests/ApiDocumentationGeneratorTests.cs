// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Host;
    using IntroSpec;
    using IntroSpec.Enrichers.Interfaces;
    using IntroSpec.Models;
    using Testing;
    using Xunit;

    [Collection("AppHost")]
    public class ApiDocumentationGeneratorTests
    {
        private const string Desc = "I'm jim morrison. I'm dead.";
        private const string LicenseUri = "http://mozilla.org/MPL/2.0/";
        private const string ContactEmail = "email@address.com";
        private const string ContactName = "Joe Bloggs";
        private const string ContactUri = "http://example.com";

        private readonly ApiDocumentationGenerator generator;
        private readonly IApiResourceEnricher enricher1 = A.Fake<IApiResourceEnricher>();
        private readonly IApiResourceEnricher enricher2 = A.Fake<IApiResourceEnricher>();
        private readonly IAppHost appHost = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
        private readonly IApiSpecSettings settings;

        public ApiDocumentationGeneratorTests()
        {
            generator = new ApiDocumentationGenerator(() => new[] { enricher1, enricher2 });

            settings = new ApiSpecSettings
            {
                ContactEmail = ContactEmail,
                ContactName = ContactName,
                ContactUrl = new Uri(ContactUri),
                Description = Desc,
                LicenseUrl = new Uri(LicenseUri)
            };
        }

        [Fact]
        public void GenerateDocumentation_SetsTitle_AsServiceName()
        {
            const string serviceName = "das service";
            var host = new BasicAppHost { ServiceName = serviceName, Config = new HostConfig { WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, settings);
            doc.Title.Should().Be(serviceName);
        }

        [Fact]
        public void GenerateDocumentation_SetsApiVersion_FromConfig()
        {
            const string apiVersion = "16.2.3.1";
            var host = new BasicAppHost { Config = new HostConfig { ApiVersion = apiVersion, WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, settings);
            doc.ApiVersion.Should().Be(apiVersion);
        }

        [Fact]
        public void GenerateDocumentation_DoesNotSetApiBaseUrl()
        {
            var host = new BasicAppHost();
            var doc = generator.GenerateDocumentation(null, host, settings);
            doc.ApiBaseUrl.Should().BeNull();
        }

        [Fact]
        public void GenerateDocumentation_SetsContact()
        {
            var host = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, settings);
            doc.Contact.Name.Should().Be(ContactName);
            doc.Contact.Email.Should().Be(ContactEmail);
            doc.Contact.Url.Should().Be(ContactUri);
        }

        [Fact]
        public void GenerateDocumentation_SetsDescriptiont()
        {
            var host = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, settings);
            doc.Description.Should().Be(Desc);
        }

        [Fact]
        public void GenerateDocumentation_SetsLicense()
        {
            var host = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
            var doc = generator.GenerateDocumentation(null, host, settings);
            doc.LicenceUrl.Should().Be(LicenseUri);
        }

        [Fact]
        public void GenerateDocumentation_HandlesNullLicense()
        {
            var host = new BasicAppHost { Config = new HostConfig { WebHostUrl = "a" } };
            settings.LicenseUrl = null;
            var doc = generator.GenerateDocumentation(null, host, settings);
            doc.LicenceUrl.Should().BeNull();
        }

        [Fact]
        public void GenerateDocumentation_CallsEnrichersInOrder()
        {
            var operation = new Operation { RequestType = typeof(string) };
            generator.GenerateDocumentation(new[] { operation }, appHost, settings);

            A.CallTo(() => enricher1.Enrich(A<ApiResourceDocumentation>.Ignored, operation)).MustHaveHappened()
                .Then(
                    A.CallTo(() => enricher2.Enrich(A<ApiResourceDocumentation>.Ignored, operation)).MustHaveHappened());
        }

        [Fact]
        public void GenerateDocumentation_CallsEnrichersInOrder_WithAllOperations()
        {
            var operation = new Operation { RequestType = typeof(string) };
            var operation2 = new Operation { RequestType = typeof(string) };

            generator.GenerateDocumentation(new[] { operation, operation2 }, appHost, settings);

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

    public class ApiSpecSettings : IApiSpecSettings
    {
        public string ContactEmail { get; set; }

        public string ContactName { get; set; }

        public Uri ContactUrl { get; set; }

        public string Description { get; set; }

        public Uri LicenseUrl { get; set; }
    }
}
