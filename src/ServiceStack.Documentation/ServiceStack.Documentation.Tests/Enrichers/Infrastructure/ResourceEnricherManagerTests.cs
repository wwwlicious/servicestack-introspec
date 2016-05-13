namespace ServiceStack.Documentation.Tests.Enrichers.Infrastructure
{
    using System;
    using Documentation.Enrichers.Infrastructure;
    using Documentation.Enrichers.Interfaces;
    using Documentation.Models;
    using FakeItEasy;
    using FluentAssertions;
    using Host;
    using Xunit;

    public class ResourceEnricherManagerTests
    {
        private readonly Operation operation;
        private readonly ResourceEnricherManager nullParameterManager;
        private readonly ResourceEnricherManager manager;
        private readonly IResourceEnricher resourceEnricher;
        private readonly IPropertyEnricher propertyEnricher;

        public ResourceEnricherManagerTests()
        {
            nullParameterManager = new ResourceEnricherManager(null, null);
            operation = new Operation { RequestType = typeof (int), ResponseType = typeof (string) };

            resourceEnricher = A.Fake<IResourceEnricher>();
            manager = new ResourceEnricherManager(resourceEnricher, null);
        }

        [Fact]
        public void Ctor_AllowsNullResourceEnricher()
        {
            Action action = () => new ResourceEnricherManager(null, propertyEnricher);
            action.ShouldNotThrow<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_AllowsNullParameterEnricher()
        {
            Action action = () => new ResourceEnricherManager(resourceEnricher, null);
            action.ShouldNotThrow<ArgumentNullException>();
        }

        [Fact]
        public void EnrichResource_HandlesNullResourceEnricher()
        {
            nullParameterManager.EnrichResource(new ApiResourceDocumentation(), new Operation());
            // no assert but no error
        }

        [Fact]
        public void EnrichResource_CallsGetTitle_IfResourceHasNoTitle()
        {
            manager.EnrichResource(new ApiResourceType(), new Operation());
            A.CallTo(() => resourceEnricher.GetTitle(A<Type>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResource_DoesNotCallGetTitle_IfResourceHasTitle()
        {
            manager.EnrichResource(new ApiResourceType { Title = "mo" }, new Operation());
            A.CallTo(() => resourceEnricher.GetTitle(A<Type>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichResource_CallsGetDescription_IfResourceHasNoDescription()
        {
            manager.EnrichResource(new ApiResourceType(), new Operation());
            A.CallTo(() => resourceEnricher.GetDescription(A<Type>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResource_DoesNotCallGetDescription_IfResourceHasDescription()
        {
            manager.EnrichResource(new ApiResourceType { Description = "desk" }, new Operation());
            A.CallTo(() => resourceEnricher.GetDescription(A<Type>.Ignored)).MustNotHaveHappened();
        }
        
        [Fact]
        public void EnrichResource_CallsGetNotes_IfResourceHasNoNotes()
        {
            manager.EnrichResource(new ApiResourceType(), new Operation());
            A.CallTo(() => resourceEnricher.GetNotes(A<Type>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichResource_DoesNotCallGetNotes_IfResourceHasNotes()
        {
            manager.EnrichResource(new ApiResourceType { Notes = "mo" }, new Operation());
            A.CallTo(() => resourceEnricher.GetNotes(A<Type>.Ignored)).MustNotHaveHappened();
        }
    }
}
