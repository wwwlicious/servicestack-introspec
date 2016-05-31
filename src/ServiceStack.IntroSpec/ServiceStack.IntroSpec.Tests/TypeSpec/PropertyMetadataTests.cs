// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.TypeSpec
{
    using FluentAssertions;
    using IntroSpec.Models;
    using IntroSpec.TypeSpec;
    using Xunit;

    public class PropertyMetadataTests
    {
        private static PropertyMetadata GetPropertyMetadata()
            => new PropertyMetadata(typeof (MetadataTestClass).GetProperty("Name"));

        [Fact]
        public void With_ReturnsSelf()
        {
            var metadata = GetPropertyMetadata();
            var result = metadata.With(x => x.Title, "whatever");

            result.Should().Be(metadata);
        }

        [Fact]
        public void With_SetsTitle()
        {
            var metadata = GetPropertyMetadata();
            const string title = "My Title";
            metadata.With(x => x.Title, title);

            metadata.Title.Should().Be(title);
        }

        [Fact]
        public void With_SetsDescription()
        {
            var metadata = GetPropertyMetadata();
            const string desc = "My Generation";
            metadata.With(x => x.Description, desc);

            metadata.Description.Should().Be(desc);
        }

        [Fact]
        public void With_SetsIsRequired()
        {
            var metadata = GetPropertyMetadata();
            metadata.With(x => x.IsRequired, true);

            metadata.IsRequired.Should().BeTrue();
        }

        [Fact]
        public void With_SetsConstraint()
        {
            var metadata = GetPropertyMetadata();
            var propertyConstraint = PropertyConstraint.RangeConstraint("Test constraint", 1, 2);
            metadata.With(x => x.Constraint, propertyConstraint);

            metadata.Constraint.Should().Be(propertyConstraint);
        }

        [Fact]
        public void With_SetsAllowMultiple()
        {
            var metadata = GetPropertyMetadata();
            metadata.With(x => x.AllowMultiple, true);

            metadata.AllowMultiple.Should().BeTrue();
        }
    }

    public class MetadataTestClass
    {
        public string Name { get; set; }
    }
}
