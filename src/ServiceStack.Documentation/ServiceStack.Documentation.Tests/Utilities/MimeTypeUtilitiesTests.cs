// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Utilities
{
    using Documentation.Utilities;
    using FluentAssertions;
    using Xunit;

    public class MimeTypeUtilitiesTests
    {
        [Theory]
        [InlineData("Soap11", "text/xml; charset=utf-8")]
        [InlineData("msgPack", "application/x-msgpack")]
        public void GetMimeType_ReturnsKnownType(string type, string expected)
            => MimeTypeUtilities.GetMimeType(type).Should().Be(expected);

        [Fact]
        public void GetMimeType_ReturnsDefault_IfNotKnownType()
            => MimeTypeUtilities.GetMimeType("hal+json").Should().Be("application/hal+json");
    }
}
