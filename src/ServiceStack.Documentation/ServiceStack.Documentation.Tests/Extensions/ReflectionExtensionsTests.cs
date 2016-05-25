// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Extensions
{
    using System;
    using System.Linq;
    using Documentation.Extensions;
    using FluentAssertions;
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
