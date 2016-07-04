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

    public class PropertyEnricherManagerTests
    {
        private readonly PropertyEnricherManager nullPropertyManager;
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
            var actual = nullPropertyManager.EnrichParameters(null, typeof(string));
            actual.Should().BeNull();
        }

        [Theory]
        [MemberData("TestData")]
        public void EnrichParameters_ReturnsPassedParameters_IfParameterEnricherNull(ApiPropertyDocumention[] properties)
        {
            var actual = nullPropertyManager.EnrichParameters(properties, typeof (string));
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

        [Fact]
        public void EnrichParameters_CallsGetContraints_IfContraintsNull()
        {
            var param = GetApiParameterDocumention();
            param.Contraints = null;
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetConstraints(A<PropertyInfo>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallGetContraints_IfHasContraints()
        {
            var param = GetApiParameterDocumention();
            param.Contraints = new PropertyConstraint();
            manager.EnrichParameters(new[] { param }, typeof(SingleProp));
            A.CallTo(() => propertyEnricher.GetConstraints(A<PropertyInfo>.Ignored)).MustNotHaveHappened();
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

        [Fact]
        public void EnrichParameters_CallsEnrichResource_IfTypeIsNotSystemType()
        {
            var param = GetApiParameterDocumention();

            bool called = false;
            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, type) => { called = true; });

            enricherManager.EnrichParameters(new[] { param }, typeof(ComplexProp));
            called.Should().BeTrue();
        }

        [Fact]
        public void EnrichParameters_SetsTypeName_WhenCallingEnrichResource()
        {
            var param = GetApiParameterDocumention();

            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, type) =>
            {
                resource.TypeName.Should().Be("IgnoredProps");
            });

            enricherManager.EnrichParameters(new[] { param }, typeof(ComplexProp));
        }

        [Fact]
        public void EnrichParameters_DoesNotCallEnrichResource_IfIsSystemType()
        {
            var param = GetApiParameterDocumention();
            
            bool called = false;
            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, type) => { called = true; });

            enricherManager.EnrichParameters(new[] { param }, typeof(SingleProp));
            called.Should().BeFalse();
        }

        [Fact]
        public void EnrichParameters_DoesNotCallEnrichResource_IfIsEnumType()
        {
            var param = GetApiParameterDocumention();

            bool called = false;
            var enricherManager = new PropertyEnricherManager(propertyEnricher, (resource, type) => { called = true; });

            enricherManager.EnrichParameters(new[] { param }, typeof(MyEnum));
            called.Should().BeFalse();
        }

        [Fact]
        public void EnrichParameters_ArrayType_ReturnsPropertiesOfType()
        {
            var actual = manager.EnrichParameters(null, typeof(ComplexProp[]));
            actual[0].ClrType.Should().Be<IgnoredProps>();
        }

        [Fact]
        public void EnrichParameters_AllowMultipleTrue_IfIsCollectionType()
        {
            var actual = manager.EnrichParameters(null, typeof(ContainsArray[]));
            actual[0].ClrType.Should().Be<ComplexProp[]>();
            actual[0].AllowMultiple.Should().BeTrue();
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
}
