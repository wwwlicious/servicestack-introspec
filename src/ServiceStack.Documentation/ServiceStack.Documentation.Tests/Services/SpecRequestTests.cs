// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Services
{
    using DataAnnotations;
    using Documentation.Services;
    using FluentAssertions;
    using Xunit;

    public class SpecRequestTests
    {
        [Fact]
        public void HasExcludeAttribute()
            =>
                typeof(SpecRequest).FirstAttribute<ExcludeAttribute>().Feature.Should().Be(Feature.Metadata |
                                                                                           Feature.ServiceDiscovery);

        [Fact]
        public void HasRouteAttribute()
            => typeof (SpecRequest).FirstAttribute<RouteAttribute>().Path.Should().Be("/spec");
    }
}
