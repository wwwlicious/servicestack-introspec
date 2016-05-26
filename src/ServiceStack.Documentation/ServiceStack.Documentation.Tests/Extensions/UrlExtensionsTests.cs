// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Extensions
{
    using Documentation.Extensions;
    using FluentAssertions;
    using Xunit;

    public class UrlExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("/url")]
        [InlineData("/url/multi/level")]
        public void HasPathParams_ReturnsEmptyList_IfNoPathParams(string url) => url.GetPathParams().Should().BeEmpty();

        [Theory]
        [InlineData("/{foo}")]
        [InlineData("/{foo}/")]
        [InlineData("/url/{foo}/level")]
        [InlineData("/url/multi/{foo}/")]
        [InlineData("/url/multi/{foo}")]
        public void HasPathParams_ReturnsSinglePathParams(string url)
        {
            var result = url.GetPathParams();
            result.Count.Should().Be(1);
            result[0].Should().Be("foo");
        }

        [Theory]
        [InlineData("/{foo}/{bar}")]
        [InlineData("/{foo}/{bar}/")]
        [InlineData("/url/{foo}/{bar}/level")]
        [InlineData("/{foo}/multi/{bar}/")]
        [InlineData("/{foo}/multi/{bar}")]
        public void HasPathParams_ReturnsMultiPathParams(string url)
        {
            var result = url.GetPathParams();
            result.Count.Should().Be(2);
            result[0].Should().Be("foo");
            result[1].Should().Be("bar");
        }
    }
}
