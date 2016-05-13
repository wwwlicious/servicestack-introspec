
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

    public class ResponseEnricherManagerTests
    {
        private readonly Operation operation;
        private readonly ResponseEnricherManager nullParameterManager;
        private readonly ResponseEnricherManager manager;
        private readonly IResponseEnricher responseEnricher;

        private ResponseEnricherManager GetEnricherManager(Action<IApiResourceType, Operation> action) => new ResponseEnricherManager(responseEnricher, action);

        public ResponseEnricherManagerTests()
        {
            nullParameterManager = new ResponseEnricherManager(null, ResourceEnricher);
            operation = new Operation { RequestType = typeof(int), ResponseType = typeof(string) };

            responseEnricher = A.Fake<IResponseEnricher>();
            manager = new ResponseEnricherManager(responseEnricher, ResourceEnricher);
        }

        [Fact]
        public void Ctor_AllowsNullResourceEnricher()
        {
            Action action = () => new ResponseEnricherManager(null, ResourceEnricher);
            action.ShouldNotThrow<ArgumentNullException>();
        }

        [Fact]
        public void EnrichResponse_HandlesNullResourceEnricher()
        {
            nullParameterManager.EnrichResponse(new ApiResourceDocumentation(), new Operation());
            // no assert but no error
        }

        [Fact]
        public void EnrichResponse_CallsGetVerbs_IfResourceHasNullVerbs()
        {
            manager.EnrichResponse(new ApiResourceDocumentation(), operation);
            A.CallTo(() => responseEnricher.GetVerbs(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetVerbs_IfResourceHasEmptyVerbs()
        {
            manager.EnrichResponse(new ApiResourceDocumentation { Verbs = new string[0]}, operation);
            A.CallTo(() => responseEnricher.GetVerbs(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetVerbs_IfResourceHasVerbs_AndUnionAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Verbs = new[] { "GET" } };
                manager.EnrichResponse(apiResourceDocumentation, operation);
                A.CallTo(() => responseEnricher.GetVerbs(operation)).MustHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetVerbs_IfResourceHasVerbs_AndSetIfEmptyAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Verbs = new[] { "GET" } };
                manager.EnrichResponse(apiResourceDocumentation, operation);
                A.CallTo(() => responseEnricher.GetVerbs(operation)).MustNotHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_CallsGetStatusCodes_IfResourceHasNullStatusCodes()
        {
            manager.EnrichResponse(new ApiResourceDocumentation(), operation);
            A.CallTo(() => responseEnricher.GetStatusCodes(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetStatusCodes_IfResourceHasEmptyStatusCodes()
        {
            manager.EnrichResponse(new ApiResourceDocumentation { StatusCodes = new StatusCode[0]}, operation);
            A.CallTo(() => responseEnricher.GetStatusCodes(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetStatusCodes_IfResourceHasStatusCodes_AndUnionAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { StatusCodes = new[] { new StatusCode() } };
                manager.EnrichResponse(apiResourceDocumentation, operation);
                A.CallTo(() => responseEnricher.GetStatusCodes(operation)).MustHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetStatusCodes_IfResourceHasStatusCodes_AndSetIfEmptyAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { StatusCodes = new[] { new StatusCode() } };
                manager.EnrichResponse(apiResourceDocumentation, operation);
                A.CallTo(() => responseEnricher.GetStatusCodes(operation)).MustNotHaveHappened();
            }
        }

        [Fact]
        public void EnrichResource_CallsEnrichResourceAction_WithPassedOperation()
        {
            var enricherManager = GetEnricherManager((type, op) => { op.Should().Be(operation); });
            enricherManager.EnrichResponse(new ApiResourceDocumentation(), operation);
        }

        [Fact]
        public void EnrichResource_CallsEnrichResourceAction_WithEmptyResourceType_IfReturnTypeNull()
        {
            var enricherManager = GetEnricherManager((returnType, op) => { returnType.Should().NotBeNull(); });
            enricherManager.EnrichResponse(new ApiResourceDocumentation(), operation);
        }

        [Fact]
        public void EnrichResource_CallsEnrichResourceAction_WithApiResourceType_IfReturnTypeNotNull()
        {
            var apiResourceType = new ApiResourceType { Title = "meow the jewels" };
            var enricherManager = GetEnricherManager((returnType, op) => { returnType.Should().Be(apiResourceType); });

            enricherManager.EnrichResponse(new ApiResourceDocumentation { ReturnType = apiResourceType }, operation);
        }

        private void ResourceEnricher(IApiResourceType type, Operation operation) { }
    }
}
