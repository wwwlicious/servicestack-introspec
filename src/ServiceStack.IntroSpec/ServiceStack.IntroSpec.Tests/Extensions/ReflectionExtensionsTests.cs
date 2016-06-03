// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using IntroSpec.Extensions;
    using Xunit;

    public class ReflectionExtensionsTests
    {
        [Fact]
        public void GetInheritanceHierarchy_Empty_IfTypeNull()
        {
            Type t = null;
            t.GetInheritanceHierarchy().Should().BeEmpty();
        }

        [Fact]
        public void GetInheritanceHierarchy_SingleLevel()
        {
            var result = typeof (FirstClass).GetInheritanceHierarchy().ToList();
            result.Count.Should().Be(2);
            result[0].Should().Be<FirstClass>();
            result[1].Should().Be<object>(); // will always get this
        }

        [Fact]
        public void GetInheritanceHierarchy_MultiLevel()
        {
            var result = typeof (ThirdClass).GetInheritanceHierarchy().ToList();
            result.Count.Should().Be(4);
            result[0].Should().Be<ThirdClass>(); 
            result[1].Should().Be<SecondClass>();
            result[2].Should().Be<FirstClass>();
            result[3].Should().Be<object>();
        }

        [Fact]
        public void GetFieldPropertyType_Throws_IfNotPropertyInfoOrFieldInfo()
        {
            Action action = () => typeof(FirstClass).GetMethod("MyMethod").GetFieldPropertyType();
            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetFieldPropertyType_ReturnsPropertyInfoType()
        {
            var propertyInfo = typeof(FirstClass).GetProperty("Property");
            propertyInfo.GetFieldPropertyType().Should().Be<string>();
        }

        [Fact]
        public void GetFieldPropertyType_ReturnsFieldInfoType()
        {
            var fieldInfo = typeof(FirstClass).GetField("Field");
            fieldInfo.GetFieldPropertyType().Should().Be<DateTime>();
        }

        [Theory]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(IList))]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(ICollection<int>))]
        public void IsCollection_True_ForCollectionTypes(Type collectionType)
            => collectionType.IsCollection().Should().BeTrue();

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(SecondClass))]
        public void IsCollection_False_ForNotCollectionTypes(Type collectionType)
            => collectionType.IsCollection().Should().BeFalse();

        public class FirstClass
        {
            public string Property { get; set; }
            public DateTime Field;
            public void MyMethod() { }
        }

        public class SecondClass : FirstClass { }

        public class ThirdClass : SecondClass { }
    }
}
