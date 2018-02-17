// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Models
{
    using System.Collections.Generic;
    using FluentAssertions;
    using IntroSpec.Models;
    using Xunit;

    public class PermissionsTests
    {
        public static IEnumerable<object[]> NullEmpty => new[]
        {
            new object[] { null, null },
            new object[] { null, new List<string>() },
            new object[] { new List<string>(), null },
            new object[] { new List<string>(), new List<string>() }
        };

        public static TheoryData<List<string>> SingleNullEmpty = new TheoryData<List<string>> { null, new List<string>() };

        [Theory]
        [MemberData("NullEmpty")]
        public void Create_ReturnsNull_IfPassedBothNulls(IList<string> any, IList<string> all)
            => Permissions.Create(any, all).Should().BeNull();

        [Theory]
        [MemberData("SingleNullEmpty")]
        public void Create_ReturnsAny_IfAllNullOrEmpty(IList<string> theOther)
        {
            var list = new List<string> { "foo" };
            var permission = Permissions.Create(list, theOther);

            permission.AnyOf.Should().BeEquivalentTo(list);
            permission.AllOf.Should().BeNull();
        }

        [Theory]
        [MemberData("SingleNullEmpty")]
        public void Create_ReturnsAll_IfAnyNullOrEmpty(IList<string> theOther)
        {
            var list = new[] { "foo" };
            var permission = Permissions.Create(theOther, list);

            permission.AllOf.Should().BeEquivalentTo(list);
            permission.AnyOf.Should().BeNull();
        }
    }
}
