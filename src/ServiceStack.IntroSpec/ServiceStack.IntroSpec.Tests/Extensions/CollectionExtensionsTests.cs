// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Extensions
{
    using System.Collections.Generic;
    using FluentAssertions;
    using IntroSpec.Extensions;
    using IntroSpec.Settings;
    using Xunit;

    public class CollectionExtensionsTests
    {
        public static IEnumerable<object[]> NullEmpty => new[]
        {
            new object[] { null, EnrichmentStrategy.Union },
            new object[] { new string[0], EnrichmentStrategy.Union },
            new object[] { null, EnrichmentStrategy.SetIfEmpty },
            new object[] { new string[0], EnrichmentStrategy.SetIfEmpty }
        };

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
        
        [Fact]
        public void SafeUnion_Enumerable_ReturnsNull_IfBothNull()
            => CollectionExtensions.SafeUnion(null, (IEnumerable<int>)null).Should().BeNull();

        [Fact]
        public void SafeUnion_Enumerable_ReturnsSecond_IfFirstIsNull()
        {
            var result = new[] { "Say", "To" };
            CollectionExtensions.SafeUnion(null, result).Should().BeEquivalentTo(result);
        }

        [Fact]
        public void SafeUnion_Enumerable_ReturnsFirst_IfSecondIsNull()
        {
            var array = new[] { "Say", "To" };
            array.SafeUnion((IEnumerable<string>)null).Should().BeEquivalentTo(array);
        }

        [Fact]
        public void SafeUnion_Enumerable_ReturnsUnion()
        {
            var array = new[] { "Say", "To" };
            var result = new[] { "To", "Frog" };
            var expected = new[] { "Say", "To", "Frog" };

            array.SafeUnion(result).Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData("NullEmpty")]
        public void GetBasedOnStrategy_ReturnsFuncResult_IfArrayNullOrEmpty_RegardlessOfStrategy(string[] array,
            EnrichmentStrategy strategy)
        {
            var result = new[] { "foo", "bar" };
            using (DocumenterSettings.With(collectionStrategy: strategy))
                array.GetBasedOnStrategy(() => result).Should().BeEquivalentTo(result);
        }

        [Fact]
        public void GetBasedOnStrategy_ReturnsArray_IfSetIfEmptyStrategy()
        {
            var result = new[] { "foo", "bar" };
            var current = new[] { "baz", "fizz" };

            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
                current.GetBasedOnStrategy(() => result).Should().BeEquivalentTo(current);
        }

        [Fact]
        public void GetBasedOnStrategy_ReturnsUnion_IfUnionStrategy()
        {
            var result = new[] { "foo", "bar" };
            var current = new[] { "baz" };

            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var actual = current.GetBasedOnStrategy(() => result);
                actual.Length.Should().Be(3);
                actual.Should().Contain("foo").And.Contain("bar").And.Contain("baz");
            }
        }
    }
}
