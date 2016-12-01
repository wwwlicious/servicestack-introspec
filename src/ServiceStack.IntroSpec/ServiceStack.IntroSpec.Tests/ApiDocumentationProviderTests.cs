// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests
{
    using System.Collections.Generic;
    using FakeItEasy;
    using Fixtures;
    using FluentAssertions;
    using Host;
    using IntroSpec;
    using IntroSpec.Models;
    using IntroSpec.Services;
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
            var generator = A.Fake<IApiDocumentationGenerator>();
            var apiDocumentation = new ApiDocumentation();

            var apiSpecFeature = new IntroSpecFeature().WithGenerator(generator);
            A.CallTo(() =>
                     generator.GenerateDocumentation(A<IEnumerable<Operation>>.Ignored, fixture.AppHost, apiSpecFeature))
             .Returns(apiDocumentation);

            fixture.AppHost.LoadPlugin(apiSpecFeature);

            var provider = new ApiDocumentationProvider();
            provider.GetApiDocumentation("http://anything/").Should().Be(apiDocumentation);
        }
    }
}
