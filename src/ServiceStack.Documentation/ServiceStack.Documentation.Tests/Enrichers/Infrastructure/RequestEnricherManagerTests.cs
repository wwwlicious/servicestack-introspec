
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
        private readonly IRequestEnricher _requestEnricher;

        private RequestEnricherManager GetEnricherManager(Action<IApiResourceType, Operation> action) => new RequestEnricherManager(_requestEnricher, action);

        public RequestEnricherManagerTests()
        {
            nullParameterManager = new RequestEnricherManager(null, ResourceEnricher);
            operation = new Operation { RequestType = typeof(int), ResponseType = typeof(string) };

            _requestEnricher = A.Fake<IRequestEnricher>();
            manager = new RequestEnricherManager(_requestEnricher, ResourceEnricher);
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
            A.CallTo(() => _requestEnricher.GetVerbs(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetVerbs_IfResourceHasEmptyVerbs()
        {
            manager.EnrichRequest(new ApiResourceDocumentation { Verbs = new string[0]}, operation);
            A.CallTo(() => _requestEnricher.GetVerbs(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetVerbs_IfResourceHasVerbs_AndUnionAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Verbs = new[] { "GET" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => _requestEnricher.GetVerbs(operation)).MustHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetVerbs_IfResourceHasVerbs_AndSetIfEmptyAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { Verbs = new[] { "GET" } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => _requestEnricher.GetVerbs(operation)).MustNotHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_CallsGetStatusCodes_IfResourceHasNullStatusCodes()
        {
            manager.EnrichRequest(new ApiResourceDocumentation(), operation);
            A.CallTo(() => _requestEnricher.GetStatusCodes(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetStatusCodes_IfResourceHasEmptyStatusCodes()
        {
            manager.EnrichRequest(new ApiResourceDocumentation { StatusCodes = new StatusCode[0]}, operation);
            A.CallTo(() => _requestEnricher.GetStatusCodes(operation)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResponse_CallsGetStatusCodes_IfResourceHasStatusCodes_AndUnionAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { StatusCodes = new[] { new StatusCode() } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => _requestEnricher.GetStatusCodes(operation)).MustHaveHappened();
            }
        }

        [Fact]
        public void EnrichResponse_DoesNotCallGetStatusCodes_IfResourceHasStatusCodes_AndSetIfEmptyAsStrategy()
        {
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
            {
                var apiResourceDocumentation = new ApiResourceDocumentation { StatusCodes = new[] { new StatusCode() } };
                manager.EnrichRequest(apiResourceDocumentation, operation);
                A.CallTo(() => _requestEnricher.GetStatusCodes(operation)).MustNotHaveHappened();
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

        private void ResourceEnricher(IApiResourceType type, Operation operation) { }
    }
}
