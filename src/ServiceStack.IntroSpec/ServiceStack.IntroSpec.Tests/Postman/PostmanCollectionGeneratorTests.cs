// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Postman
{
    using System.Linq;
    using FluentAssertions;
    using IntroSpec.Models;
    using IntroSpec.Postman.Services;
    using ServiceStack.IntroSpec.Extensions;
    using Xunit;

    public class PostmanCollectionGeneratorTests
    {
        private readonly PostmanCollectionGenerator generator;

        public PostmanCollectionGeneratorTests()
        {
            generator = new PostmanCollectionGenerator();
        }

        [Fact]
        public void Generate_PopulatesBasicCollectionValues()
        {
            const string title = "Documentation Title";
            const string description = "Documentation Description";
            var documentation = new ApiDocumentation
            {
                Title = title,
                Description = description
            };

            var collection = generator.Generate(documentation);
            collection.Id.Should().NotBeNullOrEmpty();
            collection.Name.Should().Be(title);
            collection.Description.Should().Be(description);
            collection.Order.Should().NotBeNull();
        }

        [Fact]
        public void Generate_PopulatesBasicRequestValues()
        {
            const string description = "Documentation Description";

            var resources = new[]
            {
                new ApiResourceDocumentation
                {
                    Title = "dto",
                    Actions = new[]
                    {
                        new ApiAction
                        {
                            Verb = "GET",
                            RelativePaths = new RelativePath[] { "/dto" },
                            ContentTypes = new[] { "application/json" }
                        }
                    },
                    Description = description
                }
            };

            var documentation = new ApiDocumentation
            {
                Title = "title",
                Description = "description",
                Resources = resources,
                ApiBaseUrl = "http://acme.com/api"
            };

            var collection = generator.Generate(documentation);
            collection.Requests.Length.Should().Be(1);
            var request = collection.Requests.First();

            request.CollectionId.Should().Be(collection.Id);
            request.Url.Should().Be($"{documentation.ApiBaseUrl}/dto");
            request.Description.Should().Be(description);
            request.Name.Should().Be("dto");
            request.Method.Should().Be("GET");
        }

        [Fact]
        public void Generate_ReturnsRequestPerResourceVerbCombo()
        {
            var resources = new[]
            {
                new ApiResourceDocumentation
                {
                    Title = "dto",
                    Actions = new[]
                    {
                        new ApiAction
                        {
                            Verb = "GET",
                            RelativePaths = new RelativePath[] { "/dto" },
                            ContentTypes = new[] { "application/json" }
                        },
                        new ApiAction
                        {
                            Verb = "POST",
                            RelativePaths = new RelativePath[] { "/dto" },
                            ContentTypes = new[] { "application/json" }
                        }
                    }
                },
                new ApiResourceDocumentation
                {
                    Title = "anotherdto",
                    Actions = new[]
                    {
                        new ApiAction
                        {
                            Verb = "GET",
                            RelativePaths = new RelativePath[] { "/anotherdto" },
                            ContentTypes = new[] { "application/json" }
                        }
                    }
                }
            };

            var documentation = new ApiDocumentation
            {
                Title = "title",
                Description = "description",
                Resources = resources
            };

            var collection = generator.Generate(documentation);
            collection.Requests.Length.Should().Be(3);
        }

        [Fact]
        public void Generate_HandlesPathVariables_AllVerbs()
        {
            var resources = new[]
            {
                new ApiResourceDocumentation
                {
                    Title = "dto",
                    Actions = new[]
                    {
                        new ApiAction
                        {
                            Verb = "GET",
                            RelativePaths = new RelativePath[] { "/dto/{Name}/" },
                            ContentTypes = new[] { "application/json" }
                        },
                        new ApiAction
                        {
                            Verb = "POST",
                            RelativePaths = new RelativePath[] { "/dto/{Name}/" },
                            ContentTypes = new[] { "application/json" }
                        }
                    },
                    Properties = new[]
                    { new ApiPropertyDocumentation { Id = "Name", Title = "Name", ClrType = typeof(string).ToApiClrType() } }
                }
            };

            var documentation = new ApiDocumentation
            {
                Title = "title",
                Description = "description",
                Resources = resources,
                ApiBaseUrl = "http://acme.com/api"
            };

            const string expectedUrl = "http://acme.com/api/dto/:Name/";
            var collection = generator.Generate(documentation);
            collection.Requests.Length.Should().Be(2);

            foreach (var request in collection.Requests)
            {
                request.Url.Should().Be(expectedUrl);
                request.PathVariables.Should().ContainKey("Name");
            }
        }

        [Fact]
        public void Generate_HandlesPathVariables_NoTrailingSlash()
        {
            var resources = new[]
            {
                new ApiResourceDocumentation
                {
                    Title = "dto",
                    Actions = new[]
                    {
                        new ApiAction
                        {
                            Verb = "GET",
                            RelativePaths = new RelativePath[] { "/dto/{Name}/" },
                            ContentTypes = new[] { "application/json" }
                        }
                    },
                    Properties = new[]
                    { new ApiPropertyDocumentation { Id = "Name", Title = "Name", ClrType = typeof(string).ToApiClrType() } }
                }
            };

            var documentation = new ApiDocumentation
            {
                Title = "title",
                Description = "description",
                Resources = resources,
                ApiBaseUrl = "http://acme.com/api"
            };

            const string expectedUrl = "http://acme.com/api/dto/:Name/";
            var collection = generator.Generate(documentation);
            collection.Requests.Length.Should().Be(1);
            var request = collection.Requests[0];

            request.Url.Should().Be(expectedUrl);
            request.PathVariables.Should().ContainKey("Name");
        }

        [Fact]
        public void Generate_AddsNonPathVariableParams_ToQueryString_ForGet()
        {
            var resources = new[]
            {
                new ApiResourceDocumentation
                {
                    Title = "dto",
                    Actions = new[]
                    {
                        new ApiAction
                        {
                            Verb = "GET",
                            RelativePaths = new RelativePath[] { "/dto/{Name}/" },
                            ContentTypes = new[] { "application/json" }
                        }
                    },
                    Properties = new[]
                    {
                        new ApiPropertyDocumentation { Id = "Name", Title = "Name", ClrType = typeof(string).ToApiClrType() },
                        new ApiPropertyDocumentation { Id = "Age", Title = "Age", ClrType = typeof(int).ToApiClrType() }
                    }
                }
            };

            var documentation = new ApiDocumentation
            {
                Title = "title",
                Description = "description",
                Resources = resources,
                ApiBaseUrl = "http://acme.com/api"
            };

            const string expectedUrl = "http://acme.com/api/dto/:Name/?Age=val-int";
            var collection = generator.Generate(documentation);
            collection.Requests.Length.Should().Be(1);

            var request = collection.Requests[0];

            request.Url.Should().Be(expectedUrl);
            request.PathVariables.Should().ContainKey("Name");
        }

        [Fact]
        public void Generate_AddsNonPathVariableParams_ToQueryString_ForPost()
        {
            var resources = new[]
            {
                new ApiResourceDocumentation
                {
                    Title = "dto",
                    Actions = new[]
                    {
                        new ApiAction
                        {
                            Verb = "POST",
                            RelativePaths = new RelativePath[] { "/dto/{Name}/" },
                            ContentTypes = new[] { "application/json" }
                        }
                    },
                    Properties = new[]
                    {
                        new ApiPropertyDocumentation { Id = "Name", Title = "Name", ClrType = typeof(string).ToApiClrType() },
                        new ApiPropertyDocumentation { Id = "Age", Title = "Age", ClrType = typeof(int).ToApiClrType() }
                    }
                }
            };

            var documentation = new ApiDocumentation
            {
                Title = "title",
                Description = "description",
                Resources = resources,
                ApiBaseUrl = "http://acme.com/api"
            };

            const string expectedUrl = "http://acme.com/api/dto/:Name/";
            var collection = generator.Generate(documentation);
            collection.Requests.Length.Should().Be(1);

            var request = collection.Requests[0];

            request.Url.Should().Be(expectedUrl);
            request.PathVariables.Should().ContainKey("Name");
            request.Data.Should().Contain(x => x.Key == "Age");
        }
    }
}