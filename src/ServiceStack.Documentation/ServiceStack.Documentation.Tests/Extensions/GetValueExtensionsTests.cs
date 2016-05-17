// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Extensions
{
    using System;
    using Documentation.Extensions;
    using FluentAssertions;
    using Xunit;

    public class GetValueExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetIfNullOrEmpty_ReturnsValueFromFunc_IfNullOrEmpty(string str)
        {
            var newVal = Guid.NewGuid().ToString();
            var actual = str.GetIfNullOrEmpty(() => newVal);

            actual.Should().Be(actual);
        }

        [Fact]
        public void GetIfNullOrEmpty_ReturnsValue_IfNotNullOrEmpty()
        {
            var str = Guid.NewGuid().ToString();
            str.GetIfNullOrEmpty(() => "hi").Should().Be(str);
        }

        [Fact]
        public void GetIfNull_ReturnsValueFromFunc_IfNull()
        {
            string nullObject = null;
            var newString = "irnbru";

            var actual = nullObject.GetIfNull(() => newString);
            actual.Should().Be(newString);
        }

        [Fact]
        public void GetIfNull_ReturnsValue_IfNotNull()
        {
            var str = Guid.NewGuid().ToString();
            str.GetIfNull(() => "hi").Should().Be(str);
        }

        [Fact]
        public void GetIfNullOrEmpty_Array_ReturnsValueFromFunc_IfNull()
        {
            int[] array = null;
            var newArray = new[] { 5, 46 };

            var actual = array.GetIfNullOrEmpty(() => newArray);
            actual.Should().BeEquivalentTo(newArray);
        }

        [Fact]
        public void GetIfNullOrEmpty_Array_ReturnsValueFromFunc_IfEmpty()
        {
            int[] array = new int[0];
            var newArray = new[] { 5, 46 };

            var actual = array.GetIfNullOrEmpty(() => newArray);
            actual.Should().BeEquivalentTo(newArray);
        }

        [Fact]
        public void GetIfNullOrEmpty_Array_ReturnsValue_IfNotNull()
        {
            var array = new[] { 5, 46 };

            var actual = array.GetIfNullOrEmpty(() => new[] { 4, 3 });
            actual.Should().BeEquivalentTo(array);
        }

        [Fact]
        public void GetIfNoValue_ReturnsValueFromFunc_IfNull()
        {
            int? value = null;
            int? newValue = 123;

            var actual = value.GetIfNoValue(() => newValue);
            actual.Should().Be(newValue);
        }

        [Fact]
        public void GetIfNoValue_ReturnsValue_IfHasValue()
        {
            int? value = 123;

            var actual = value.GetIfNoValue(() => (int?) 999);
            actual.Should().Be(value);
        }
    }
}
