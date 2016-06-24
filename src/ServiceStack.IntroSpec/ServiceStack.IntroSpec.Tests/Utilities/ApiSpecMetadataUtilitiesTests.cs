// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Utilities
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using IntroSpec.Models;
    using IntroSpec.Utilities;
    using Xunit;

    public class ApiSpecMetadataUtilitiesTests
    {
        [Fact]
        public void GenerateResponse_ReturnsNull_IfDocumentationNull()
            => ApiSpecMetadataUtilities.GenerateResponse(null).Should().BeNull();

        [Fact]
        public void GenerateResponse_ReturnsNull_IfResourcesNull()
        {
            var docs = new ApiDocumentation();
            ApiSpecMetadataUtilities.GenerateResponse(docs).Should().BeNull();
        }

        [Fact]
        public void GenerateResponse_ReturnsNull_IfResourcesEmpty()
        {
            var docs = new ApiDocumentation { Resources = new ApiResourceDocumentation[0] };
            ApiSpecMetadataUtilities.GenerateResponse(docs).Should().BeNull();
        }

        [Fact]
        public void GenerateResponse_ReturnsCorrect_SingleType_NoCategoriesOrTags()
        {
            var docs = new ApiDocumentation
            {
                Resources = new List<ApiResourceDocumentation>
                {
                    new ApiResourceDocumentation { TypeName = "Type1" }
                }.ToArray()
            };

            var result = ApiSpecMetadataUtilities.GenerateResponse(docs);
            result.DtoNames.Should().Contain("Type1");
            result.DtoNames.Count().Should().Be(1);
            result.Categories.Should().BeNullOrEmpty();
            result.Tags.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GenerateResponse_ReturnsCorrect_MultipleType_NoCategoriesOrTags()
        {
            var docs = new ApiDocumentation
            {
                Resources = new List<ApiResourceDocumentation>
                {
                    new ApiResourceDocumentation { TypeName = "Type1" },
                    new ApiResourceDocumentation { TypeName = "Type2" }
                }.ToArray()
            };

            var result = ApiSpecMetadataUtilities.GenerateResponse(docs);
            result.DtoNames.Should().Contain("Type1").And.Contain("Type2");
            result.DtoNames.Count().Should().Be(2);
            result.Categories.Should().BeNullOrEmpty();
            result.Tags.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GenerateResponse_ReturnsCorrect_MultipleSameCategory()
        {
            var docs = new ApiDocumentation
            {
                Resources = new List<ApiResourceDocumentation>
                {
                    new ApiResourceDocumentation { TypeName = "Type1", Category = "Cat1" },
                    new ApiResourceDocumentation { TypeName = "Type2", Category = "Cat1" }
                }.ToArray()
            };

            var result = ApiSpecMetadataUtilities.GenerateResponse(docs);

            var category = result.Categories.Single();
            category.Key.Should().Be("Cat1");
            category.DtoNames.Should().Contain("Type1").And.Contain("Type2");

            result.Tags.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GenerateResponse_ReturnsCorrect_MultipleCategory()
        {
            var docs = new ApiDocumentation
            {
                Resources = new List<ApiResourceDocumentation>
                {
                    new ApiResourceDocumentation { TypeName = "Type1", Category = "Cat1" },
                    new ApiResourceDocumentation { TypeName = "Type2", Category = "Cat2" }
                }.ToArray()
            };

            var result = ApiSpecMetadataUtilities.GenerateResponse(docs);

            var category = result.Categories.First();
            category.Key.Should().Be("Cat1");
            category.DtoNames.Should().Contain("Type1");
            category.DtoNames.Count().Should().Be(1);

            var category2 = result.Categories.Last();
            category2.Key.Should().Be("Cat2");
            category2.DtoNames.Should().Contain("Type2");
            category2.DtoNames.Count().Should().Be(1);
        }



        [Fact]
        public void GenerateResponse_ReturnsCorrect_MultipleSameTags()
        {
            var docs = new ApiDocumentation
            {
                Resources = new List<ApiResourceDocumentation>
                {
                    new ApiResourceDocumentation { TypeName = "Type1", Tags = new[] { "Tag1" } },
                    new ApiResourceDocumentation { TypeName = "Type2", Tags = new[] { "Tag1" } }
                }.ToArray()
            };

            var result = ApiSpecMetadataUtilities.GenerateResponse(docs);

            var tag = result.Tags.Single();
            tag.Key.Should().Be("Tag1");
            tag.DtoNames.Should().Contain("Type1").And.Contain("Type2");
        }

        [Fact]
        public void GenerateResponse_ReturnsCorrect_MultipleTags()
        {
            var docs = new ApiDocumentation
            {
                Resources = new List<ApiResourceDocumentation>
                {
                    new ApiResourceDocumentation { TypeName = "Type1", Tags = new[] { "Tag1", "Tag2" } },
                    new ApiResourceDocumentation { TypeName = "Type2", Tags = new[] { "Tag2" } }
                }.ToArray()
            };

            var result = ApiSpecMetadataUtilities.GenerateResponse(docs);

            var tag = result.Tags.First();
            tag.Key.Should().Be("Tag1");
            tag.DtoNames.Should().Contain("Type1");
            tag.DtoNames.Count().Should().Be(1);

            var tag2 = result.Tags.Last();
            tag2.Key.Should().Be("Tag2");
            tag2.DtoNames.Should().Contain("Type1").And.Contain("Type2");
            tag2.DtoNames.Count().Should().Be(2);
        }
    }
}
