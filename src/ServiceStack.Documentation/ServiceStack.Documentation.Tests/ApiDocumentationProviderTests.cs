// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests
{
    using System.Collections.Generic;
    using Documentation.Models;
    using Documentation.Services;
    using Documentation.Settings;
    using FakeItEasy;
    using Fixtures;
    using FluentAssertions;
    using Host;
    using Xunit;

    [Collection("AppHost")]
    public class ApiDocumentationProviderTests
    {
        private readonly AppHostFixture fixture;

        public ApiDocumentationProviderTests(AppHostFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void GetApiDocumentation_ReturnsDocumentationFromApiSpecFeature()
        {
            var apiSpecConfig = new ApiSpecConfig
            {
                Contact = new ApiContact { Email = "test@example.com", Name = "Joe Bloggs" },
                Description = "Test"
            };
            var generator = A.Fake<IApiDocumentationGenerator>();
            var apiDocumentation = new ApiDocumentation();
            A.CallTo(() =>
                generator.GenerateDocumentation(A<IEnumerable<Operation>>.Ignored, fixture.AppHost,
                    A<ApiSpecConfig>.Ignored)).Returns(apiDocumentation);

            fixture.AppHost.LoadPlugin(new ApiSpecFeature(apiSpecConfig).WithGenerator(generator));

            var provider = new ApiDocumentationProvider();
            provider.GetApiDocumentation().Should().Be(apiDocumentation);
        }
    }
}
