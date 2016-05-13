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
    }
}
