// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.TypeSpec
{
    using System;
    using System.Reflection;
    using FluentAssertions;
    using IntroSpec.TypeSpec;
    using Xunit;

    public class ApiDtoSpecTests
    {
        private PropertyInfo propertyInfo;
        private Type testType;

        public ApiDtoSpecTests()
        {
            testType = typeof (ToDocument);
            propertyInfo = testType.GetProperty("Name");
        }

        [Fact]
        public void GetPropertySpec_ReturnsNull_IfNotFound()
            => new Documenter().GetPropertySpec(propertyInfo).Should().BeNull();

        [Fact]
        public void For_AddsParameterToLookup() 
            => new Documenter(true).GetPropertySpec(propertyInfo).Should().NotBeNull();

        [Fact]
        public void For_AddsParameterToLookup_WithCorrectValues()
        {
            var prop = new Documenter(true).GetPropertySpec(propertyInfo);

            prop.Title.Should().Be("The Name");
            prop.IsRequired.Should().BeTrue();
            prop.Description.Should().Be("Description of name");
        }
    }

    public class Documenter : TypeSpec<ToDocument>
    {
        public Documenter() {}
        public Documenter(bool addPropertyValue)
        {
            if (addPropertyValue)
            {
                For(p => p.Name)
                    .With(t => t.Title, "The Name")
                    .With(t => t.IsRequired, true)
                    .With(t => t.Description, "Description of name");
            }
        }
    }

    public class ToDocument
    {
        public string Name { get; set; }
    }
}
