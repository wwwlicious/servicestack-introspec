// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Services
{
    using System;
    using FluentAssertions;
    using IntroSpec.DTO;
    using IntroSpec.Extensions;
    using IntroSpec.Models;
    using Xunit;

    public class ApiDocumentationFilterTests
    {
        [Fact]
        public void GetApiDocumentation_Throws_IfRequestNull()
        {
            Action action = () => new ApiDocumentation().Filter(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void GetApiDocumentation_Throws_IfDocumentationNull()
        {
            ApiDocumentation documentation = null;
            Action action = () => documentation.Filter(new Filterable());
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void GetApiDocumentation_ReturnsWholeObject_IfNoFilterCriteria()
        {
            var documentation = new ApiDocumentation { Title = "Test Documentation" };
            var result = documentation.Filter(new Filterable());

            result.Should().Be(documentation);
        }

        [Fact]
        public void GetApiDocumentation_FiltersDtoName()
        {
            var resources = new ApiResourceDocumentation[]
            {
                new ApiResourceDocumentation { TypeName = "DTO1" },
                new ApiResourceDocumentation { TypeName = "DTO2" }
            };

            var documentation = new ApiDocumentation { Title = "Test Documentation", Resources = resources };
            var filter = new Filterable { DtoNames = new[] { "DTO1" } };

            var result = documentation.Filter(filter);

            result.Resources.Length.Should().Be(1);
            result.Resources[0].TypeName.Should().Be("DTO1");
        }

        [Fact]
        public void GetApiDocumentation_FiltersMultipleDtoName()
        {
            var resources = new ApiResourceDocumentation[]
            {
                new ApiResourceDocumentation { TypeName  = "DTO1" },
                new ApiResourceDocumentation { TypeName  = "DTO2" },
                new ApiResourceDocumentation { TypeName  = "DTO3" }
            };

            var documentation = new ApiDocumentation { Title = "Test Documentation", Resources = resources };
            var filter = new Filterable { DtoNames = new[] { "DTO1", "DTO3" } };

            var result = documentation.Filter(filter);

            result.Resources.Length.Should().Be(2);
            result.Resources[0].TypeName.Should().Be("DTO1");
            result.Resources[1].TypeName.Should().Be("DTO3");
        }

        [Fact]
        public void GetApiDocumentation_HandlesUnknownDtoName()
        {
            var resources = new ApiResourceDocumentation[]
            {
                new ApiResourceDocumentation { TypeName  = "DTO1" },
                new ApiResourceDocumentation { TypeName  = "DTO2" }
            };

            var documentation = new ApiDocumentation { Title = "Test Documentation", Resources = resources };
            var filter = new Filterable { DtoNames = new[] { "sunkilmoon" } };

            var result = documentation.Filter(filter);

            result.Resources.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetApiDocumentation_FiltersTags()
        {
            var resources = new ApiResourceDocumentation[]
            {
                new ApiResourceDocumentation { Tags = new[] { "Tag1" } },
                new ApiResourceDocumentation { Tags = new[] { "Tag2" } }
            };

            var documentation = new ApiDocumentation { Title = "Test Documentation", Resources = resources };
            var filter = new Filterable { Tags = new[] { "Tag1" } };

            var result = documentation.Filter(filter);

            result.Resources.Length.Should().Be(1);
            result.Resources[0].Tags[0].Should().Be("Tag1");
        }

        [Fact]
        public void GetApiDocumentation_FiltersMultipleTags()
        {
            var resources = new ApiResourceDocumentation[]
            {
                new ApiResourceDocumentation { Tags = new[] { "Tag1" } },
                new ApiResourceDocumentation { Tags = new[] { "Tag2" } },
                new ApiResourceDocumentation { Tags = new[] { "Tag1", "Tag3" } }
            };

            var documentation = new ApiDocumentation { Title = "Test Documentation", Resources = resources };
            var filter = new Filterable { Tags = new[] { "Tag1", "Tag2" } };

            var result = documentation.Filter(filter);

            result.Resources.Length.Should().Be(3);
        }

        [Fact]
        public void GetApiDocumentation_HandlesUnknownTags()
        {
            var resources = new ApiResourceDocumentation[]
            {
                new ApiResourceDocumentation { Tags = new[] { "Tag1" } },
                new ApiResourceDocumentation { Tags = new[] { "Tag2" } }
            };

            var documentation = new ApiDocumentation { Title = "Test Documentation", Resources = resources };
            var filter = new Filterable { Tags = new[] { "sunkilmoon" } };

            var result = documentation.Filter(filter);

            result.Resources.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetApiDocumentation_FiltersCategory()
        {
            var resources = new ApiResourceDocumentation[]
            {
                new ApiResourceDocumentation { Category = "Category1" },
                new ApiResourceDocumentation { Category = "Category2" }
            };

            var documentation = new ApiDocumentation { Title = "Test Documentation", Resources = resources };
            var filter = new Filterable { Categories = new[] { "Category1" } };

            var result = documentation.Filter(filter);

            result.Resources.Length.Should().Be(1);
            result.Resources[0].Category.Should().Be("Category1");
        }

        [Fact]
        public void GetApiDocumentation_HandlesUnknownCategory()
        {
            var resources = new ApiResourceDocumentation[]
            {
                new ApiResourceDocumentation { Category = "Category1" },
                new ApiResourceDocumentation { Category = "Category2" }
            };

            var documentation = new ApiDocumentation { Title = "Test Documentation", Resources = resources };
            var filter = new Filterable { Categories = new[] { "unknown" } };

            var result = documentation.Filter(filter);

            result.Resources.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetApiDocumentation_MultipleFieldFilter()
        {
            var resources = new ApiResourceDocumentation[]
            {
                new ApiResourceDocumentation { Category = "Category1", Tags = new[] { "Tag1" }, TypeName  = "DTO1" },
                new ApiResourceDocumentation { Category = "Category1", Tags = new[] { "Tag2" }, TypeName  = "DTO2" },
                new ApiResourceDocumentation { Category = "Category2", Tags = new[] { "Tag2", "Tag3" }, TypeName  = "DTO3" }
            };

            var documentation = new ApiDocumentation { Title = "Test Documentation", Resources = resources };
            var filter = new Filterable
            {
                Categories = new[] { "Category2" },
                Tags = new[] { "Tag2" },
                DtoNames = new[] { "DTO3", "DTO2" }
            };

            var result = documentation.Filter(filter);
            result.Resources.Length.Should().Be(1);
            result.Resources[0].TypeName.Should().Be("DTO3");
        }
    }

    public class Filterable : IFilterableSpecRequest
    {
        public string[] DtoNames { get; set; }
        public string[] Categories { get; set; }
        public string[] Tags { get; set; }
    }
}
