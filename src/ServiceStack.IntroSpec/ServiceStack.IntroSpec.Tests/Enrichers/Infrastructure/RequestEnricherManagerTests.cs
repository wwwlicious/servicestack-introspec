// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Enrichers.Infrastructure
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Host;
    using IntroSpec.Enrichers.Infrastructure;
    using IntroSpec.Enrichers.Interfaces;
    using IntroSpec.Models;
    using IntroSpec.Settings;
    using Xunit;

    public class RequestEnricherManagerTests
    {
        private readonly Operation operation;
        private readonly RequestEnricherManager nullParameterManager;
        private readonly RequestEnricherManager manager;
        private readonly IRequestEnricher requestEnricher;
        private readonly IActionEnricherManager actionEnricherManager;

        private RequestEnricherManager GetEnricherManager(Action<IApiResourceType, Operation> action)
            => new RequestEnricherManager(requestEnricher, actionEnricherManager, action);

        private void ResourceEnricher(IApiResourceType type, Operation operation) {}

        public RequestEnricherManagerTests()
        {
            nullParameterManager = new RequestEnricherManager(null, null, ResourceEnricher);
            operation = new Operation { RequestType = typeof(int), ResponseType = typeof(string) };

            requestEnricher = A.Fake<IRequestEnricher>();
            actionEnricherManager = A.Fake<IActionEnricherManager>();
            manager = new RequestEnricherManager(requestEnricher, actionEnricherManager, ResourceEnricher);
        }

        [Fact]
        public void Ctor_AllowsNullResourceEnricher()
        {
            Action action = () => new RequestEnricherManager(null, null, ResourceEnricher);
            action.ShouldNotThrow<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_AllowsNullActionEnricherManager()
        {
            Action action = () => new RequestEnricherManager(requestEnricher, null, ResourceEnricher);
            action.ShouldNotThrow<ArgumentNullException>();
        }

        [Fact]
        public void EnrichResponse_HandlesNullResourceAndActionEnricher()
        {
            Action action = () => nullParameterManager.EnrichRequest(new ApiResourceDocumentation(), new Operation());
            action.ShouldNotThrow<ArgumentNullException>();
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

        
    }
}
