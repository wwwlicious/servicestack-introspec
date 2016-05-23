// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Enrichers.Infrastructure
{
    using System;
    using Documentation.Enrichers.Infrastructure;
    using Documentation.Enrichers.Interfaces;
    using Documentation.Models;
    using Documentation.Settings;
    using FakeItEasy;
    using FluentAssertions;
    using Host;
    using Xunit;

    public class RequestEnricherManagerTests
    {
        private readonly Operation operation;
        private readonly RequestEnricherManager nullParameterManager;
        private readonly RequestEnricherManager manager;
        private readonly IRequestEnricher requestEnricher;

        private RequestEnricherManager GetEnricherManager(Action<IApiResourceType, Operation> action)
            => new RequestEnricherManager(requestEnricher, action);

        private void ResourceEnricher(IApiResourceType type, Operation operation) {}

        public RequestEnricherManagerTests()
        {
            nullParameterManager = new RequestEnricherManager(null, ResourceEnricher);
            operation = new Operation { RequestType = typeof(int), ResponseType = typeof(string) };

            requestEnricher = A.Fake<IRequestEnricher>();
            manager = new RequestEnricherManager(requestEnricher, ResourceEnricher);
        }

        [Fact]
        public void Ctor_AllowsNullResourceEnricher()
        {
            Action action = () => new RequestEnricherManager(null, ResourceEnricher);
            action.ShouldNotThrow<ArgumentNullException>();
        }

        [Fact]
        public void EnrichResponse_HandlesNullResourceEnricher()
        {
            nullParameterManager.EnrichRequest(new ApiResourceDocumentation(), new Operation());
            // no assert but no error
        }

        [Fact]
        public void EnrichResponse_CallsGetVerbs_IfResourceHasNullVerbs()
        {
            manager.EnrichRequest(new ApiResourceDocumentation(), operation);
            A.CallTo(() => requestEnricher.GetVerbs(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetVerbs_IfResourceHasEmptyVerbs()
        {
            manager.EnrichRequest(new ApiResourceDocumentation { Verbs = new string[0]}, operation);
            A.CallTo(() => requestEnricher.GetVerbs(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_SetsVerbs_IfResourceHasEmptyVerbs()
        {
            var verbs = new[] { "GET", "DELETE" };
            A.CallTo(() => requestEnricher.GetVerbs(operation)).Returns(verbs);
            var apiResourceDocumentation = new ApiResourceDocumentation { Verbs = new string[0] };

            manager.EnrichRequest(apiResourceDocumentation, operation);

            apiResourceDocumentation.Verbs.Should().BeEquivalentTo(verbs);
        }

        [Fact]
        public void EnrichResponse_CallsGetVerbs_IfResourceHasVerbs_AndUnionAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Verbs = new[] { "GET" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => requestEnricher.GetVerbs(operation)).MustHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_ReturnsAllVerbs_IfResourceHasVerbs_AndUnionAsStrategy()
        {
            var verbs = new[] { "GET", "DELETE" };
            A.CallTo(() => requestEnricher.GetVerbs(operation)).Returns(verbs);

            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Verbs = new[] { "PUT", "GET" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                apiResourceDocumentation.Verbs.Length.Should().Be(3);
                apiResourceDocumentation.Verbs.Should().Contain("GET").And.Contain("DELETE").And.Contain("PUT");
            }
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetVerbs_IfResourceHasVerbs_AndSetIfEmptyAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Verbs = new[] { "GET" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => requestEnricher.GetVerbs(operation)).MustNotHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_CallsGetStatusCodes_IfResourceHasNullStatusCodes()
        {
            manager.EnrichRequest(new ApiResourceDocumentation(), operation);
            A.CallTo(() => requestEnricher.GetStatusCodes(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetStatusCodes_IfResourceHasEmptyStatusCodes()
        {
            manager.EnrichRequest(new ApiResourceDocumentation { StatusCodes = new StatusCode[0]}, operation);
            A.CallTo(() => requestEnricher.GetStatusCodes(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_SetsStatusCodes_IfResourceHasEmptyStatusCodes()
        {
            var statusCodes = new[] { (StatusCode) 200 };
            A.CallTo(() => requestEnricher.GetStatusCodes(operation)).Returns(statusCodes);
            var apiResourceDocumentation = new ApiResourceDocumentation { StatusCodes = new StatusCode[0] };

            manager.EnrichRequest(apiResourceDocumentation, operation);

            apiResourceDocumentation.StatusCodes.Should().Equal(statusCodes);
        }

        [Fact]
        public void EnrichResponse_CallsGetStatusCodes_IfResourceHasStatusCodes_AndUnionAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { StatusCodes = new[] { new StatusCode() } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => requestEnricher.GetStatusCodes(operation)).MustHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_ReturnsAllStatusCodes_IfResourceHasStatusCodes_AndUnionAsStrategy()
        {
            var ok = (StatusCode) 200;
            var forbidden = (StatusCode) 403;

            var statusCodes = new[] { ok };
            A.CallTo(() => requestEnricher.GetStatusCodes(operation)).Returns(statusCodes);

            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { StatusCodes = new[] { forbidden, ok } };
                manager.EnrichRequest(apiResourceDocumentation, operation);

                apiResourceDocumentation.StatusCodes.Length.Should().Be(2);
                apiResourceDocumentation.StatusCodes.Should().Contain(ok).And.Contain(forbidden);
            }
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetStatusCodes_IfResourceHasStatusCodes_AndSetIfEmptyAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { StatusCodes = new[] { new StatusCode() } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => requestEnricher.GetStatusCodes(operation)).MustNotHaveHappened();
            }
        }

        [Fact]
        public void EnrichResource_CallsEnrichResourceAction_WithPassedOperation()
        {
            var enricherManager = GetEnricherManager((type, op) => { op.Should().Be(operation); });
            enricherManager.EnrichRequest(new ApiResourceDocumentation(), operation);
        }

        [Fact]
        public void EnrichResource_CallsEnrichResourceAction_WithEmptyResourceType_IfReturnTypeNull()
        {
            var enricherManager = GetEnricherManager((returnType, op) => { returnType.Should().NotBeNull(); });
            enricherManager.EnrichRequest(new ApiResourceDocumentation(), operation);
        }

        [Fact]
        public void EnrichResource_CallsEnrichResourceAction_WithApiResourceType_IfReturnTypeNotNull()
        {
            var apiResourceType = new ApiResourceType { Title = "meow the jewels" };
            var enricherManager = GetEnricherManager((returnType, op) => { returnType.Should().Be(apiResourceType); });

            enricherManager.EnrichRequest(new ApiResourceDocumentation { ReturnType = apiResourceType }, operation);
        }

        [Fact]
        public void EnrichResponse_CallsGetTags_IfResourceHasNullTags()
        {
            manager.EnrichRequest(new ApiResourceDocumentation(), operation);
            A.CallTo(() => requestEnricher.GetTags(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetTags_IfResourceHasEmptyTags()
        {
            manager.EnrichRequest(new ApiResourceDocumentation { Tags = new string[0]}, operation);
            A.CallTo(() => requestEnricher.GetTags(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_SetsTags_IfResourceHasEmptyTags()
        {
            var tags = new[] { "Tag1" };
            A.CallTo(() => requestEnricher.GetTags(operation)).Returns(tags);
            var apiResourceDocumentation = new ApiResourceDocumentation { Tags = new string[0] };

            manager.EnrichRequest(apiResourceDocumentation, operation);

            apiResourceDocumentation.Tags.Should().BeEquivalentTo(tags);
        }

        [Fact]
        public void EnrichResponse_CallsGetTags_IfResourceHasTags_AndUnionAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Tags = new[] { "Tag1" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => requestEnricher.GetTags(operation)).MustHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_ReturnsAllTags_IfResourceHasTags_AndUnionAsStrategy()
        {
            var tags = new[] { "Tag98", "Tag1" };
            A.CallTo(() => requestEnricher.GetTags(operation)).Returns(tags);

            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Tags = new[] { "Tag1" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);

                apiResourceDocumentation.Tags.Length.Should().Be(2);
                apiResourceDocumentation.Tags.Should().Contain("Tag98").And.Contain("Tag1");
            }
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetTags_IfResourceHasTags_AndSetIfEmptyAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Tags = new[] { "Tag1" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => requestEnricher.GetTags(operation)).MustNotHaveHappened();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichResponse_CallsGetCategory_IfResourceHasNullOrEmptyCategory(string category)
        {
            manager.EnrichRequest(new ApiResourceDocumentation { Category = category }, operation);
            A.CallTo(() => requestEnricher.GetCategory(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetCategory_IfCategoryHasValue()
        {
            manager.EnrichRequest(new ApiResourceDocumentation { Category = "cat2" }, operation);
            A.CallTo(() => requestEnricher.GetCategory(operation)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichResponse_SetsCategory_IfResourceHasNullOrEmptyCategory(string category)
        {
            const string returnCat = "asdasdasd";
            A.CallTo(() => requestEnricher.GetCategory(operation)).Returns(returnCat);

            var apiResourceDocumentation = new ApiResourceDocumentation { Category = category };
            manager.EnrichRequest(apiResourceDocumentation, operation);

            apiResourceDocumentation.Category.Should().Be(returnCat);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichResponse_CallsGetRelativePath_IfResourceHasNullOrEmptyRelativePath(string relativePath)
        {
            manager.EnrichRequest(new ApiResourceDocumentation { RelativePath = relativePath }, operation);
            A.CallTo(() => requestEnricher.GetRelativePath(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetRelativePath_IfRelativePathHasValue()
        {
            manager.EnrichRequest(new ApiResourceDocumentation { RelativePath = "/here/there" }, operation);
            A.CallTo(() => requestEnricher.GetRelativePath(operation)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichResponse_SetsRelativePath_IfResourceHasNullOrEmptyRelativePath(string relativePath)
        {
            const string returnPath = "/api/value";
            A.CallTo(() => requestEnricher.GetRelativePath(operation)).Returns(returnPath);

            var apiResourceDocumentation = new ApiResourceDocumentation { RelativePath = relativePath };
            manager.EnrichRequest(apiResourceDocumentation, operation);

            apiResourceDocumentation.RelativePath.Should().Be(returnPath);
        }


        [Fact]
        public void EnrichResponse_CallsGetContentTypes_IfResourceHasNullVerbs()
        {
            manager.EnrichRequest(new ApiResourceDocumentation(), operation);
            A.CallTo(() => requestEnricher.GetContentTypes(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetContentTypes_IfResourceHasEmptyVerbs()
        {
            manager.EnrichRequest(new ApiResourceDocumentation { ContentTypes = new string[0] }, operation);
            A.CallTo(() => requestEnricher.GetContentTypes(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_SetsContentTypes_IfResourceHasEmptyVerbs()
        {
            var contentTypes = new[] { "text/jsv", "text/xml" };
            A.CallTo(() => requestEnricher.GetContentTypes(operation)).Returns(contentTypes);
            var apiResourceDocumentation = new ApiResourceDocumentation { ContentTypes = new string[0] };

            manager.EnrichRequest(apiResourceDocumentation, operation);

            apiResourceDocumentation.ContentTypes.Should().BeEquivalentTo(contentTypes);
        }

        [Fact]
        public void EnrichResponse_CallsGetContentTypes_IfResourceHasContentTypes_AndUnionAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { ContentTypes = new[] { "text/jsv" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => requestEnricher.GetContentTypes(operation)).MustHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_ReturnsAllContentTypes_IfResourceHasContentTypes_AndUnionAsStrategy()
        {
            var contentTypes = new[] { "text/jsv", "text/csv" };
            A.CallTo(() => requestEnricher.GetContentTypes(operation)).Returns(contentTypes);

            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { ContentTypes = new[] { "text/jsv", "application/json" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                apiResourceDocumentation.ContentTypes.Length.Should().Be(3);
                apiResourceDocumentation.ContentTypes.Should().Contain("text/jsv").And.Contain("text/csv").And.Contain("application/json");
            }
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetContentTypes_IfResourceHasContentTypes_AndSetIfEmptyAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { ContentTypes = new[] { "text/jsv" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => requestEnricher.GetContentTypes(operation)).MustNotHaveHappened();
            }
        }
    }
}
