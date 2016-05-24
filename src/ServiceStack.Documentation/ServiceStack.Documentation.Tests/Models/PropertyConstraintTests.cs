// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Models
{
    using System;
    using Documentation.Models;
    using FluentAssertions;
    using Xunit;

    public class PropertyConstraintTests
    {
        private const string name = "TheName";

        [Fact]
        public void CreateRangeConstraint_Throws_IfMinAndMaxNull()
        {
            Action action = () => PropertyConstraint.RangeConstraint(name, null, null);
            action.ShouldThrow<InvalidOperationException>().WithMessage("You must supply either a Min or Max value");
        }

        [Fact]
        public void CreateRangeConstraint_Throws_IfMinGreaterThanMax()
        {
            Action action = () => PropertyConstraint.RangeConstraint(name, 10, 9);
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(null, 40)]
        [InlineData(12, null)]
        [InlineData(2, 20)]
        [InlineData(10, 10)]
        public void CreateRangeConstraint_CreatesConstraint_WithCorrectValues(int? min, int? max)
        {
            var constraint = PropertyConstraint.RangeConstraint(name, min, max);
            constraint.Name.Should().Be(name);
            constraint.Min.Should().Be(min);
            constraint.Max.Should().Be(max);
            constraint.Type.Should().Be(ConstraintType.Range);
            constraint.Values.Should().BeNull();
        }

        [Theory]
        [MemberData("InvalidValuesArray")]
        public void CreateListConstraint_Throws_IfValuesNullOrEmpty(string[] values)
        {
            Action action = () => PropertyConstraint.ListConstraint(name, values);
            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void CreateListConstraint_CreatesConstraint_WithCorrectValues()
        {
            var values = new[] { "one", "six" };
            var constraint = PropertyConstraint.ListConstraint(name, values);
            constraint.Name.Should().Be(name);
            constraint.Min.Should().NotHaveValue();
            constraint.Max.Should().NotHaveValue();
            constraint.Type.Should().Be(ConstraintType.List);
            constraint.Values.Should().BeEquivalentTo(values);
        }

        public static TheoryData<string[]> InvalidValuesArray = new TheoryData<string[]> { null, new string[0] };
    }
}
