// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Extensions
{
    using Documentation.Extensions;
    using FluentAssertions;
    using Xunit;

    public class AttributeExtensionsTests
    {
        [Fact]
        public void IsForVerb_False_AttributeNull()
        {
            RouteAttribute attr = null;
            attr.IsForVerb("GET").Should().BeFalse();
        }

        [Fact]
        public void IsForVerb_True_AttributeHasNoVerbs()
        {
            var attr = new RouteAttribute("/here");
            attr.IsForVerb("GET").Should().BeTrue();
        }

        [Theory]
        [InlineData("get")]
        [InlineData("GET")]
        [InlineData("POST GET")]
        [InlineData("GET POST")]
        [InlineData("POST GET put")]
        public void IsForVerb_True_IfForVerb(string verbs)
        {
            var attr = new RouteAttribute("/here", verbs);
            attr.IsForVerb("GET").Should().BeTrue();
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("PUT POST")]
        public void IsForVerb_False_IfNotForVerb(string verbs)
        {
            var attr = new RouteAttribute("/here", verbs);
            attr.IsForVerb("GET").Should().BeFalse();
        }
    }
}