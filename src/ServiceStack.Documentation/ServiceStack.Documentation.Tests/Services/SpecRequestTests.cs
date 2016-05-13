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
            => typeof (SpecRequest).FirstAttribute<ExcludeAttribute>().Feature.Should().Be(Feature.Metadata);

        [Fact]
        public void HasRouteAttribute()
            => typeof (SpecRequest).FirstAttribute<RouteAttribute>().Path.Should().Be("/spec");
    }
}
