namespace ServiceStack.Documentation.Tests.Enrichers.Infrastructure
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Documentation.Enrichers.Infrastructure;
    using Documentation.Enrichers.Interfaces;
    using Documentation.Models;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class PropertyEnricherManagerTests
    {
        private readonly PropertyEnricherManager _nullPropertyManager;
        private readonly PropertyEnricherManager manager;
        private readonly IPropertyEnricher propertyEnricher;

        public static TheoryData<ApiPropertyDocumention[]> TestData = new TheoryData<ApiPropertyDocumention[]>
        {
            new ApiPropertyDocumention[0],
            new[] { new ApiPropertyDocumention() }
        };

        public static TheoryData<ApiPropertyDocumention[]> TestDataEmpty = new TheoryData<ApiPropertyDocumention[]>
        {
            null,
            new ApiPropertyDocumention[0]
        };

        public PropertyEnricherManagerTests()
        {
            _nullPropertyManager = new PropertyEnricherManager(null, ResourceEnricher);

            propertyEnricher = A.Fake<IPropertyEnricher>();
            manager = new PropertyEnricherManager(propertyEnricher, ResourceEnricher);
        }

        [Fact]
        public void Ctor_AllowsNullResourceEnricher()
        {
            Action action = () => new PropertyEnricherManager(null, ResourceEnricher);
            action.ShouldNotThrow<ArgumentNullException>();
        }

        [Fact]
        public void EnrichParameters_ReturnsNull_IfParameterEnricherNull_AndPassedParametersNull()
        {
            var actual = _nullPropertyManager.EnrichParameters(null, typeof(string));
            actual.Should().BeNull();
        }

        [Theory]
        [MemberData("TestData")]
        public void EnrichParameters_ReturnsPassedParameters_IfParameterEnricherNull(ApiPropertyDocumention[] properties)
        {
            var actual = _nullPropertyManager.EnrichParameters(properties, typeof (string));
            actual.Should().BeEquivalentTo(properties);
        }

        [Fact]
        public void EnrichParameters_ReturnsEmpty_IfNoPropsInType_AndPassedNull()
        {
            var actual = manager.EnrichParameters(null, typeof (NoProps));
            actual.Should().BeEmpty();
        }

        [Fact]
        public void EnrichParameters_ReturnsEmpty_IfNoPropsInType_AndEmpty()
        {
            var actual = manager.EnrichParameters(new ApiPropertyDocumention[0], typeof(NoProps));
            actual.Should().BeEmpty();
        }

        [Fact]
        public void EnrichParameters_ReturnsPassedParameters_IfNoPropsInType()
        {
            var parameters = new[] { new ApiPropertyDocumention { Id = "yes" } };
            var actual = manager.EnrichParameters(parameters, typeof(NoProps));
            actual.Should().BeEquivalentTo(parameters);
        }

        [Fact]
        public void EnrichParameters_ReturnsEmpty_IfOnlyIgnoredPropsInType_AndPassedNull()
        {
            var actual = manager.EnrichParameters(null, typeof(IgnoredProps));
            actual.Should().BeEmpty();
        }

        [Fact]
        public void EnrichParameters_ReturnsEmpty_IfOnlyIgnoredPropsInType_AndEmpty()
        {
            var actual = manager.EnrichParameters(new ApiPropertyDocumention[0], typeof(IgnoredProps));
            actual.Should().BeEmpty();
        }

        [Fact]
        public void EnrichParameters_ReturnsPassedParameters_IfOnlyIgnoredPropsInType()
        {
            var parameters = new[] { new ApiPropertyDocumention { Id = "yes" } };
            var actual = manager.EnrichParameters(parameters, typeof(IgnoredProps));
            actual.Should().BeEquivalentTo(parameters);
        }
        
        [Theory]
        [MemberData("TestDataEmpty")]
        public void EnrichParameters_ReturnsParamPerProperty(ApiPropertyDocumention[] properties)
        {
            var count = properties?.Length ?? 0;
            var actual = manager.EnrichParameters(properties, typeof(SingleProp));
            actual.Length.Should().Be(count + 1);
        }

        [Fact]
        public void EnrichParameters_DoesNotAddToReturn_IfPopulated()
        {
            var actual = manager.EnrichParameters(new[] { new ApiPropertyDocumention { Id = "X" } }, typeof(SingleProp));
            actual.Length.Should().Be(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichParameters_CallsGetTitle_IfTitleNullOrEmpty(string title)
        {
            var param = GetApiParameterDocumention();
            param.Title = title;
            manager.EnrichParameters(new[] { param }, typeof (SingleProp));
            A.CallTo(() => propertyEnricher.GetTitle(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetTitle_IfHasTitle()
        {
            var param = GetApiParameterDocumention();
            param.Title = "put in the work";
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetTitle(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichParameters_CallsGetDescription_IfDescriptionNullOrEmpty(string desc)
        {
            var param = GetApiParameterDocumention();
            param.Description = desc;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetDescription(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetDescription_IfHasDescription()
        {
            var param = GetApiParameterDocumention();
            param.Description = "put in the work";
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetDescription(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichParameters_CallsGetNotes_IfNotesNullOrEmpty(string notes)
        {
            var param = GetApiParameterDocumention();
            param.Notes = notes;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetNotes(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetNotes_IfHasNotes()
        {
            var param = GetApiParameterDocumention();
            param.Notes = "put in the work";
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetNotes(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichParameters_CallsGetParamType_IfParamTypeNullOrEmpty(string paramType)
        {
            var param = GetApiParameterDocumention();
            param.ParamType = paramType;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetParamType(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetParamType_IfHasParamType()
        {
            var param = GetApiParameterDocumention();
            param.ParamType = "put in the work";
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetParamType(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichParameters_CallsGetContraints_IfContraintsNullOrEmpty(string contraints)
        {
            var param = GetApiParameterDocumention();
            param.Contraints = contraints;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetContraints(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetContraints_IfHasContraints()
        {
            var param = GetApiParameterDocumention();
            param.Contraints = "put in the work";
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetContraints(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }
        
        [Fact]
        public void EnrichParameters_CallsGetIsRequired_IfIsRequiredNull()
        {
            var param = GetApiParameterDocumention();
            param.IsRequired = null;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetIsRequired(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetIsRequired_IfHasIsRequired()
        {
            var param = GetApiParameterDocumention();
            param.IsRequired = false;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetIsRequired(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichParameters_CallsGetAllowMultiple_IfAllowMultipleNull()
        {
            var param = GetApiParameterDocumention();
            param.AllowMultiple = null;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetAllowMultiple(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetAllowMultiple_IfHasAllowMultiple()
        {
            var param = GetApiParameterDocumention();
            param.AllowMultiple = false;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetAllowMultiple(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichParameters_CallsGetExternalLinks_IfExternalLinksNull()
        {
            var param = GetApiParameterDocumention();
            param.ExternalLinks = null;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetExternalLinks(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetExternalLinks_IfHasExternalLinks()
        {
            var param = GetApiParameterDocumention();
            param.ExternalLinks = new[] { "http://example.com" };
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetExternalLinks(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        private static ApiPropertyDocumention GetApiParameterDocumention() => new ApiPropertyDocumention { Id = "X" };
        private void ResourceEnricher(IApiResourceType resource, Type type) { }
    }

    public class NoProps { }

    public class IgnoredProps
    {
        [IgnoreDataMember]
        public string S { get; set; }
    }

    public class SingleProp
    {
        [IgnoreDataMember]
        public string S { get; set; }

        public int X { get; set; }
    }
}
