// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Enrichers.Infrastructure
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using FakeItEasy;
    using FluentAssertions;
    using IntroSpec.Enrichers.Infrastructure;
    using IntroSpec.Enrichers.Interfaces;
    using IntroSpec.Models;
    using Xunit;
    using System.Collections.Generic;

    public class PropertyEnricherManagerTests
    {
        private readonly PropertyEnricherManager nullPropertyManager;
        private readonly PropertyEnricherManager manager;
        private readonly IPropertyEnricher propertyEnricher;

        public static TheoryData<ApiPropertyDocumentation[]> TestData = new TheoryData<ApiPropertyDocumentation[]>
        {
            new ApiPropertyDocumentation[0],
            new[] { new ApiPropertyDocumentation() }
        };

        public static TheoryData<ApiPropertyDocumentation[]> TestDataEmpty = new TheoryData<ApiPropertyDocumentation[]>
        {
            null,
            new ApiPropertyDocumentation[0]
        };

        public PropertyEnricherManagerTests()
        {
            nullPropertyManager = new PropertyEnricherManager(null, ResourceEnricher);

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
            var actual = nullPropertyManager.EnrichParameters(null, new ResourceModel(typeof(string), false));
            actual.Should().BeNull();
        }

        [Theory]
        [MemberData("TestData")]
        public void EnrichParameters_ReturnsPassedParameters_IfParameterEnricherNull(ApiPropertyDocumentation[] properties)
        {
            var actual = nullPropertyManager.EnrichParameters(properties, new ResourceModel(typeof (string), false));
            actual.Should().BeEquivalentTo(properties);
        }

        [Fact]
        public void EnrichParameters_ReturnsEmpty_IfNoPropsInType_AndPassedNull()
        {
            var actual = manager.EnrichParameters(null, new ResourceModel(typeof (NoProps), false));
            actual.Should().BeEmpty();
        }

        [Fact]
        public void EnrichParameters_ReturnsEmpty_IfNoPropsInType_AndEmpty()
        {
            var actual = manager.EnrichParameters(new ApiPropertyDocumentation[0], new ResourceModel(typeof(NoProps), true));
            actual.Should().BeEmpty();
        }

        [Fact]
        public void EnrichParameters_ReturnsPassedParameters_IfNoPropsInType()
        {
            var parameters = new[] { new ApiPropertyDocumentation { Id = "yes" } };
            var actual = manager.EnrichParameters(parameters, new ResourceModel(typeof(NoProps), true));
            actual.Should().BeEquivalentTo(parameters);
        }

        [Fact]
        public void EnrichParameters_ReturnsEmpty_IfOnlyIgnoredPropsInType_AndPassedNull()
        {
            var actual = manager.EnrichParameters(null, new ResourceModel(typeof(IgnoredProps), true));
            actual.Should().BeEmpty();
        }

        [Fact]
        public void EnrichParameters_ReturnsEmpty_IfOnlyIgnoredPropsInType_AndEmpty()
        {
            var actual = manager.EnrichParameters(new ApiPropertyDocumentation[0],
                new ResourceModel(typeof(IgnoredProps), true));
            actual.Should().BeEmpty();
        }

        [Fact]
        public void EnrichParameters_ReturnsPassedParameters_IfOnlyIgnoredPropsInType()
        {
            var parameters = new[] { new ApiPropertyDocumentation { Id = "yes" } };
            var actual = manager.EnrichParameters(parameters, new ResourceModel(typeof(IgnoredProps), true));
            actual.Should().BeEquivalentTo(parameters);
        }
        
        [Theory]
        [MemberData("TestDataEmpty")]
        public void EnrichParameters_ReturnsParamPerProperty(ApiPropertyDocumentation[] properties)
        {
            var count = properties?.Length ?? 0;
            var actual = manager.EnrichParameters(properties, new ResourceModel(typeof(SingleProp), false));
            actual.Length.Should().Be(count + 1);
        }

        [Fact]
        public void EnrichParameters_DoesNotAddToReturn_IfPopulated()
        {
            var actual = manager.EnrichParameters(new[] { new ApiPropertyDocumentation { Id = "X" } },
                new ResourceModel(typeof(SingleProp), false));
            actual.Length.Should().Be(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichParameters_CallsGetTitle_IfTitleNullOrEmpty(string title)
        {
            var param = GetApiParameterDocumention();
            param.Title = title;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof (SingleProp), false));
            A.CallTo(() => propertyEnricher.GetTitle(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetTitle_IfHasTitle()
        {
            var param = GetApiParameterDocumention();
            param.Title = "put in the work";
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetTitle(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichParameters_CallsGetDescription_IfDescriptionNullOrEmpty(string desc)
        {
            var param = GetApiParameterDocumention();
            param.Description = desc;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetDescription(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetDescription_IfHasDescription()
        {
            var param = GetApiParameterDocumention();
            param.Description = "put in the work";
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetDescription(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichParameters_CallsGetNotes_IfNotesNullOrEmpty(string notes)
        {
            var param = GetApiParameterDocumention();
            param.Notes = notes;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetNotes(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetNotes_IfHasNotes()
        {
            var param = GetApiParameterDocumention();
            param.Notes = "put in the work";
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetNotes(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichParameters_CallsGetParamType_IfParamTypeNullOrEmpty(string paramType)
        {
            var param = GetApiParameterDocumention();
            param.ParamType = paramType;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetParamType(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetParamType_IfHasParamType()
        {
            var param = GetApiParameterDocumention();
            param.ParamType = "put in the work";
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetParamType(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichParameters_CallsGetContraints_IfContraintsNull()
        {
            var param = GetApiParameterDocumention();
            param.Constraints = null;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetConstraints(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetContraints_IfHasContraints()
        {
            var param = GetApiParameterDocumention();
            param.Constraints = new PropertyConstraint();
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetConstraints(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }
        
        [Fact]
        public void EnrichParameters_CallsGetIsRequired_IfIsRequiredNull()
        {
            var param = GetApiParameterDocumention();
            param.IsRequired = null;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetIsRequired(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetIsRequired_IfHasIsRequired()
        {
            var param = GetApiParameterDocumention();
            param.IsRequired = false;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetIsRequired(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichParameters_CallsGetAllowMultiple_IfAllowMultipleNullForRequest()
        {
            var param = GetApiParameterDocumention();
            param.AllowMultiple = null;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), true));
            A.CallTo(() => propertyEnricher.GetAllowMultiple(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoessNotCallGetAllowMultiple_IfAllowMultipleNullForResponse()
        {
            var param = GetApiParameterDocumention();
            param.AllowMultiple = null;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetAllowMultiple(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetAllowMultiple_IfHasAllowMultiple()
        {
            var param = GetApiParameterDocumention();
            param.AllowMultiple = false;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetAllowMultiple(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichParameters_CallsGetExternalLinks_IfExternalLinksNull()
        {
            var param = GetApiParameterDocumention();
            param.ExternalLinks = null;
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetExternalLinks(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetExternalLinks_IfHasExternalLinks()
        {
            var param = GetApiParameterDocumention();
            param.ExternalLinks = new[] { "http://example.com" };
            manager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            A.CallTo(() => propertyEnricher.GetExternalLinks(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichParameters_CallsEnrichResource_IfTypeIsNotSystemType()
        {
            var param = GetApiParameterDocumention();

            bool called = false;
            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, resourceModel) => { called = true; });

            enricherManager.EnrichParameters(new[] { param }, new ResourceModel(typeof(ComplexProp), false));
            called.Should().BeTrue();
        }

        [Fact]
        public void EnrichParameters_CallsEnrichResource_IfTypeIsSystemTypeButHasAGenericArgumentThatIsNot()
        {
            var param = GetApiParameterDocumention();

            bool called = false;
            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, resourceModel) => { called = true; });

            enricherManager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleGenericProp), false));
            called.Should().BeTrue();
        }

        [Fact]
        public void EnrichParameters_SetsTypeName_WhenCallingEnrichResource()
        {
            var param = GetApiParameterDocumention();

            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, resourceModel) =>
            {
                resource.TypeName.Should().Be("IgnoredProps");
            });

            enricherManager.EnrichParameters(new[] { param }, new ResourceModel(typeof(ComplexProp), false));
        }

        [Fact]
        public void EnrichParameters_DoesNotCallEnrichResource_IfIsSystemType()
        {
            var param = GetApiParameterDocumention();
            
            bool called = false;
            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, resourceModel) => { called = true; });

            enricherManager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SingleProp), false));
            called.Should().BeFalse();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallEnrichResource_IfIsSelfReferencingType()
        {
            var param = GetApiParameterDocumention();

            bool called = false;
            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, resourceModel) => { called = true; });

            enricherManager.EnrichParameters(new[] { param }, new ResourceModel(typeof(SelfReference), false));
            called.Should().BeFalse();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallEnrichResource_IfIsEnumType()
        {
            var param = GetApiParameterDocumention();

            bool called = false;
            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, resourceModel) => { called = true; });

            enricherManager.EnrichParameters(new[] { param }, new ResourceModel(typeof(MyEnum), false));
            called.Should().BeFalse();
        }

        [Fact]
        public void EnrichParameters_ArrayType_ReturnsPropertiesOfType()
        {
            var actual = manager.EnrichParameters(null, new ResourceModel(typeof(ComplexProp[]), false));
            actual[0].ClrType.OriginalType.Should().Be<IgnoredProps>();
        }

        [Fact]
        public void EnrichParameters_IsCollectionTrue_IfIsCollectionTypeAndNotRequest()
        {
            var actual = manager.EnrichParameters(null, new ResourceModel(typeof(ContainsArray[]), false));
            actual[0].ClrType.OriginalType.Should().Be<ComplexProp[]>();
            actual[0].IsCollection.Should().BeTrue();
            actual[0].AllowMultiple.Should().NotHaveValue();
        }

        [Fact]
        public void EnrichParameters_IsCollectionFalse_IfIsCollectionTypeAndRequest()
        {
            var actual = manager.EnrichParameters(null, new ResourceModel(typeof(ContainsArray[]), true));
            actual[0].ClrType.OriginalType.Should().Be<ComplexProp[]>();
            actual[0].IsCollection.Should().NotHaveValue();
            actual[0].AllowMultiple.Should().BeTrue();
        }

        private static ApiPropertyDocumentation GetApiParameterDocumention() => new ApiPropertyDocumentation { Id = "X" };
        private void ResourceEnricher(IApiResourceType resource, ResourceModel resourceModel) { }
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

    public class ComplexProp
    {
        public IgnoredProps Props { get; set; }
    }

    public class ContainsArray
    {
        public ComplexProp[] Props { get; set; }
    }

    public class MasterClass
    {
        public ContainsArray[] Array { get; set; }
    }

    public class SelfReference
    {
        public SelfReference Child { get; set; }
    }

    public class SingleGenericProp
    {
        [IgnoreDataMember]
        public string S { get; set; }

        public List<NoProps> X { get; set; }
    }
}
