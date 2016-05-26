// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Extensions
{
    using Documentation.Extensions;
    using FluentAssertions;
    using Xunit;

    public class StringExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ToSpaced_EmptyString_IfNullOrWhitespace(string text) => text.ToSpaced().Should().BeEmpty();

        [Theory]
        [InlineData("test", "test")]
        [InlineData("testString", "test String")]
        [InlineData("TestString", "Test String")]
        [InlineData("MultiplePascalCaseWords", "Multiple Pascal Case Words")]
        [InlineData("OK", "OK")]
        [InlineData("TestOKString", "Test OK String")]
        [InlineData("TestOK", "Test OK")]
        public void ToSpaced_CorrectResult(string text, string expected) => text.ToSpaced().Should().Be(expected);

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TrimStart_ReturnsString_IfNullOrEmpty(string text) => text.TrimStart("aa").Should().Be(text);

        [Fact]
        public void TrimStart_ReturnsString_IfNotStartWith()
        {
            const string toRemove = "ab";
            const string str = "banana";
            str.TrimStart(toRemove).Should().Be(str);
        }

        [Theory]
        [InlineData("banana", "b", "anana")]
        [InlineData("banana", "ban", "ana")]
        [InlineData("banana", "banana", "")]
        public void TrimStart_TrimsIfStringStartsWith(string text, string toTrim, string expected)
            => text.TrimStart(toTrim).Should().Be(expected);
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnsureEndsWith_ReturnsString_IfNullOrEmpty(string text) => text.EnsureEndsWith("aa").Should().Be(text);

        [Fact]
        public void EnsureEndsWith_ReturnsString_IfNotEndsWith()
        {
            const string toEndWith = "na";
            const string str = "banana";
            str.EnsureEndsWith(toEndWith).Should().Be(str);
        }

        [Fact]
        public void EnsureEndsWith_AppendsString_IfDoesntEndWith()
        {
            const string toEndWith = "app";
            const string str = "banana";
            str.EnsureEndsWith(toEndWith).Should().Be("bananaapp");
        }
    }
}
