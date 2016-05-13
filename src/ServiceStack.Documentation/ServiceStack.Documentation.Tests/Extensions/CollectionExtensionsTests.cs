namespace ServiceStack.Documentation.Tests.Extensions
{
    using Documentation.Extensions;
    using FluentAssertions;
    using Xunit;

    public class CollectionExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_True_IfNull() => CollectionExtensions.IsNullOrEmpty<string>(null).Should().BeTrue();

        [Fact]
        public void IsNullOrEmpty_True_IfEmpty() => new string[0].IsNullOrEmpty().Should().BeTrue();

        [Fact]
        public void IsNullOrEmpty_False_IfPopulated() => new[] { "water" }.IsNullOrEmpty().Should().BeFalse();

        [Fact]
        public void SafeUnion_ReturnsNull_IfBothNull()
            => CollectionExtensions.SafeUnion<int>(null, () => null).Should().BeNull();

        [Fact]
        public void SafeUnion_ReturnsResultOfFunc_IfArrayIsNull()
        {
            var result = new[] { "Say", "To" };
            CollectionExtensions.SafeUnion(null, () => result).Should().BeEquivalentTo(result);
        }

        [Fact]
        public void SafeUnion_FirstArray_IfFuncReturnsNull()
        {
            var array = new[] { "Say", "To" };
            array.SafeUnion(() => null).Should().BeEquivalentTo(array);
        }

        [Fact]
        public void SafeUnion_ReturnsUnion()
        {
            var array = new[] { "Say", "To" };
            var result = new[] { "To", "Frog" };
            var expected = new[] { "Say", "To", "Frog" };

            array.SafeUnion(() => result).Should().BeEquivalentTo(expected);
        }
    }
}
