// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Enrichers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using DataAnnotations;
    using Fixtures;
    using FluentAssertions;
    using Host;
    using IntroSpec.Enrichers;
    using IntroSpec.Models;
    using Xunit;

    [Collection("AppHost")]
    public class ReflectionEnricherTests
    {
        private ReflectionEnricher enricher = new ReflectionEnricher();
        private readonly AppHostFixture fixture;
        private const string Verb = "GET";

        private static PropertyInfo noPropertyInfo => typeof(SomeAttributes).GetProperty("NoAttr");
        private static PropertyInfo propertyInfo => typeof (SomeAttributes).GetProperty("Thing");

        public ReflectionEnricherTests(AppHostFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void GetTitle_ReturnsTypeName() => enricher.GetTitle(typeof(AllAttributes)).Should().Be("AllAttributes");

        [Theory]
        [InlineData(typeof (AllAttributes), "ApiDescription")]
        [InlineData(typeof (SomeAttributes), "ComponentModelDescription")]
        [InlineData(typeof (OneAttribute), "ServiceStackDescription")]
        public void GetDescription_ReturnsCorrectPrecedence(Type type, string expected)
            => enricher.GetDescription(type).Should().Be(expected);

        [Fact]
        public void GetNotes_ReturnsNull()
            => enricher.GetNotes(typeof (SomeAttributes)).Should().BeNull();

        [Fact]
        public void GetNotes_ForVerb_ReturnsNull_IfNoRouteAttribute()
        {
            var operation = new Operation { RequestType = typeof(SomeAttributes) };
            enricher.GetNotes(operation, "GET").Should().BeNull();
        }

        [Fact]
        public void GetNotes_ForVerb_ReturnsNotes_IfRouteAttributeForVerb()
        {
            var operation = new Operation { RequestType = typeof(AllAttributes) };
            enricher.GetNotes(operation, "GET").Should().Be("These are some notes");
        }

        [Fact]
        public void GetNotes_ForVerb_ReturnsNull_IfNoRouteAttributeForVerb()
        {
            var operation = new Operation { RequestType = typeof(AllAttributes) };
            enricher.GetNotes(operation, "DELETE").Should().BeNull();
        }

        [Fact]
        public void GetNotes_ForVerb_ReturnsNull_IfRouteAttributeForVerbWithoutNotes()
        {
            var operation = new Operation { RequestType = typeof(AllAttributes) };
            enricher.GetNotes(operation, "POST").Should().BeNull();
        }

        [Fact]
        public void GetStatusCodes_ReturnsEmptyArray_IfNoApiResponseAttribute()
        {
            var operation = new Operation
            {
                ServiceType = typeof (AllAttributes),
                RequestType = typeof (SomeAttributes),
                ResponseType = typeof (AllAttributes) //This stops it being marked as one way
            };
            var result = enricher.GetStatusCodes(operation, Verb);
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        [InlineData("PUT")]
        public void GetStatusCodes_204_IfNoOneWayRequest(string verb)
        {
            var operation = new Operation
            {
                ServiceType = typeof(OneWay),
                RequestType = typeof(SomeAttributes)
            };
            var result = enricher.GetStatusCodes(operation, verb);
            result.Length.Should().Be(1);
            result[0].Code.Should().Be(204);
            result[0].Name.Should().Be("No Content");
        }

        [Fact]
        public void GetStatusCodes_204_IfVoidReturnForVerb()
        {
            var operation = new Operation
            {
                ServiceType = typeof(OneAttribute),
                RequestType = typeof(SomeAttributes),
                ResponseType = typeof(AllAttributes) //This stops it being marked as one way
            };
            var result = enricher.GetStatusCodes(operation, Verb);
            result.Length.Should().Be(1);
            result[0].Code.Should().Be(204);
            result[0].Name.Should().Be("No Content");
        }

        [Fact]
        public void GetStatusCodes_No204_IfNoReturnForVerb()
        {
            var operation = new Operation
            {
                ServiceType = typeof(OneAttribute),
                RequestType = typeof(SomeAttributes),
                ResponseType = typeof(AllAttributes) //This stops it being marked as one way
            };
            var result = enricher.GetStatusCodes(operation, "POST");
            result.Length.Should().Be(0);
        }

        [Fact]
        public void GetStatusCodes_401And403_IfRequiresAuthentication()
        {
            var operation = new Operation
            {
                ServiceType = typeof(AllAttributes),
                RequestType = typeof(SomeAttributes),
                ResponseType = typeof(AllAttributes), //This stops it being marked as one way
                RequiresAuthentication = true
            };

            var result = enricher.GetStatusCodes(operation, Verb);
            result.Length.Should().Be(2);
            result[0].Code.Should().Be(401);
            result[0].Name.Should().Be("Unauthorized");
            result[1].Code.Should().Be(403);
            result[1].Name.Should().Be("Forbidden");
        }

        [Fact]
        public void GetStatusCodes_ReturnsCodes_IfApiResponseAttribute()
        {
            var operation = new Operation
            {
                ServiceType = typeof(SomeAttributes),
                RequestType = typeof(AllAttributes),
                ResponseType = typeof(AllAttributes) //This stops it being marked as one way
            };

            var result = enricher.GetStatusCodes(operation, Verb);
            result.Length.Should().Be(2);
            result[0].Code.Should().Be(201);
            result[0].Description.Should().Be("Thing created");
            result[0].Name.Should().Be("Created");

            result[1].Code.Should().Be(503);
            result[1].Description.Should().Be("Not available");
            result[1].Name.Should().Be("Service Unavailable");
        }

        [Fact]
        public void GetStatusCodes_ReturnsCodesAnd204_IfApiResponseAttribute_AndVoidReturn()
        {
            var operation = new Operation
            {
                ServiceType = typeof(OneAttribute),
                RequestType = typeof(AllAttributes),
            };

            var result = enricher.GetStatusCodes(operation, Verb);
            result.Length.Should().Be(3);

            result[0].Code.Should().Be(204);
            result[0].Name.Should().Be("No Content");

            result[1].Code.Should().Be(201);
            result[1].Description.Should().Be("Thing created");
            result[1].Name.Should().Be("Created");

            result[2].Code.Should().Be(503);
            result[2].Description.Should().Be("Not available");
            result[2].Name.Should().Be("Service Unavailable");
        }

        [Fact]
        public void GetTitle_PI_ReturnsNull_NoAttribute() => enricher.GetTitle(noPropertyInfo).Should().BeNull();

        [Fact]
        public void GetTitle_PI_Returns_ApiAttributeName() => enricher.GetTitle(propertyInfo).Should().Be("batcat");

        [Fact]
        public void GetDescription_PI_ReturnsNull_NoAttribute()
            => enricher.GetDescription(noPropertyInfo).Should().BeNull();

        [Fact]
        public void GetDescription_PI_Returns_ApiAttributeName()
            => enricher.GetDescription(propertyInfo).Should().Be("we're no here");

        [Fact]
        public void GetParamType_PI_ReturnsNull_NoAttribute() => enricher.GetParamType(noPropertyInfo).Should().BeNull();

        [Fact]
        public void GetParamType_PI_Returns_ApiAttributeName() => enricher.GetParamType(propertyInfo).Should().Be("body");

        [Fact]
        public void GetAllowMultiple_PI_ReturnsNull_NoAttribute()
            => enricher.GetAllowMultiple(noPropertyInfo).Should().NotHaveValue();

        [Fact]
        public void GetAllowMultiple_PI_Returns_ApiAttributeName()
            => enricher.GetAllowMultiple(propertyInfo).Should().BeTrue();

        [Fact]
        public void GetIsRequired_ReturnsNull_NoAttribute()
            => enricher.GetIsRequired(noPropertyInfo).Should().NotHaveValue();

        [Fact]
        public void GetIsRequired_Returns_ApiAttribute()
            => enricher.GetIsRequired(propertyInfo).Should().BeTrue();

        [Fact]
        public void GetIsRequired_ReturnsTrue_NullableType_NoAttribute()
        {
            var nullableProp = typeof(SomeAttributes).GetProperty("OptionalVal");
            enricher.GetIsRequired(nullableProp).Should().BeFalse();
        }

        [Fact]
        public void GetRelativePaths_ReturnsPath_IfRouteAttribute()
        {
            var operation = new Operation { RequestType = typeof(AllAttributes) };
            var relativePaths = enricher.GetRelativePaths(operation, Verb);
            relativePaths.Length.Should().Be(1);
            relativePaths.Should().OnlyContain(x => x.Path == "/here" && x.Source == "Attribute");
        }

        [Fact]
        public void GetRelativePaths_HandlesMultipleRouteAttributes()
        {
            var operation = new Operation { RequestType = typeof(MultiRoute) };
            var relativePaths = enricher.GetRelativePaths(operation, Verb);
            relativePaths.Length.Should().Be(2);
            relativePaths.Should()
                         .Contain(x => x.Path == "/here" && x.Source == "Attribute")
                         .And.Contain(x => x.Path == "/there" && x.Source == "Attribute");
        }

        [Fact]
        public void GetRelativePaths_HandlesFallbackRouteAttribute()
        {
            var operation = new Operation { RequestType = typeof(FallbackRoute) };
            var relativePaths = enricher.GetRelativePaths(operation, Verb);
            relativePaths.Length.Should().Be(1);
            relativePaths.Should().Contain(x => x.Path == "/fallback" && x.Source == "Fallback");
        }

        [Fact]
        public void GetRelativePaths_ReturnsOneWayPath_IfNoRouteAttribute_AndOneWay()
        {
            var operation = new Operation { RequestType = typeof(SomeAttributes) };
            var relativePaths = enricher.GetRelativePaths(operation, Verb);
            relativePaths.Length.Should().Be(1);
            relativePaths.Should().OnlyContain(x => x.Path == "/json/oneway/SomeAttributes" && x.Source == "AutoRoute");
        }

        [Fact]
        public void GetRelativePaths_ReturnsReplyPath_IfNoRouteAttribute_AndNotOneWay()
        {
            var operation = new Operation { RequestType = typeof(SomeAttributes), ResponseType = typeof(OneAttribute) };
            var relativePaths = enricher.GetRelativePaths(operation, Verb);
            relativePaths.Length.Should().Be(1);
            relativePaths.Should().OnlyContain(x => x.Path == "/json/reply/SomeAttributes" && x.Source == "AutoRoute");
        }

        [Fact]
        public void GetRelativePaths_ReturnsOneWayPath_IfRouteAttributeWithEmptyPath_AndOneWay()
        {
            var operation = new Operation { RequestType = typeof(EmptyRouteAttribute) };
            var relativePaths = enricher.GetRelativePaths(operation, Verb);
            relativePaths.Length.Should().Be(1);
            relativePaths.Should().OnlyContain(x => x.Path == "/json/oneway/EmptyRouteAttribute" && x.Source == "AutoRoute");
        }

        [Theory]
        [InlineData("GET", "/foo-bar", "Attribute")]
        [InlineData("POST", "/foo-bar", "Attribute")]
        [InlineData("PUT", "/json/oneway/RootForVerbs", "AutoRoute")]
        public void GetRelativePaths_ReturnsPath_IfRouteAttributeWithPathForVerb(string verb, string path, string source)
        {
            var operation = new Operation { RequestType = typeof(RootForVerbs) };
            var relativePaths = enricher.GetRelativePaths(operation, verb);
            relativePaths.Length.Should().Be(1);
            relativePaths.Should().OnlyContain(x => x.Path == path && x.Source == source);
        }

        [Fact]
        public void GetCategory_ReturnsNull() => enricher.GetCategory(new Operation()).Should().BeNull();

        [Fact]
        public void GetTags_ReturnsNull() => enricher.GetTags(new Operation()).Should().BeNull();

        [Fact]
        public void GetContentTypes_ReturnsAllDefault()
        {
            var operation = new Operation { RequestType = typeof(SomeAttributes) };
            var contentTypes = enricher.GetContentTypes(operation, Verb);

            contentTypes.Length.Should().Be(6);
            contentTypes.Should().Contain(MimeTypes.Xml)
                        .And.Contain(MimeTypes.Json)
                        .And.Contain(MimeTypes.Jsv)
                        .And.Contain(MimeTypes.Soap11)
                        .And.Contain(MimeTypes.Soap12)
                        .And.Contain(MimeTypes.Csv);
        }

        [Fact]
        public void GetContentTypes_ObeysRestrictAttribute()
        {
            var requestType = typeof(OneAttribute);
            var operation = new Operation
            {
                RequestType = requestType,
                RestrictTo = requestType.FirstAttribute<RestrictAttribute>()
            };
            var contentTypes = enricher.GetContentTypes(operation, Verb);

            contentTypes.Length.Should().Be(2);
            contentTypes.Should().Contain(MimeTypes.Json)
                        .And.Contain(MimeTypes.Jsv);
        }

        [Fact]
        public void GetContentTypes_ObeysExcludeAttribute()
        {
            var operation = new Operation { RequestType = typeof(AllAttributes) };
            var contentTypes = enricher.GetContentTypes(operation, Verb);

            contentTypes.Length.Should().Be(4);
            contentTypes.Should().Contain(MimeTypes.Xml)
                        .And.Contain(MimeTypes.Json)
                        .And.Contain(MimeTypes.Jsv)
                        .And.Contain(MimeTypes.Csv);
        }

        [Fact]
        public void GetContentTypes_ObeysAddHeader()
        {
            var operation = new Operation { RequestType = typeof(EmptyRouteAttribute) };
            var contentTypes = enricher.GetContentTypes(operation, Verb);

            contentTypes.Should().Contain(MimeTypes.Bson);
        }

        [Fact]
        public void GetConstraints_Null_IfNoApiAllowableValuesAttribute()
            => enricher.GetConstraints(noPropertyInfo).Should().BeNull();

        [Fact]
        public void GetConstraints_ReturnsListConstraint_IfEnumType()
        {
            var pi = typeof(RootForVerbs).GetProperty("EnumType");
            var constraint = enricher.GetConstraints(pi);

            constraint.Name.Should().Be("MyEnum");
            constraint.Type.Should().Be(ConstraintType.List);
            constraint.Values.Should().Contain("One").And.Contain("Two").And.Contain("Three");
            constraint.Values.Length.Should().Be(3);
            constraint.Min.Should().NotHaveValue();
            constraint.Max.Should().NotHaveValue();
        }

        [Fact]
        public void GetConstraints_ReturnsListConstraint_IfAttributeFound()
        {
            var pi = typeof(SomeAttributes).GetProperty("List");
            var constraint = enricher.GetConstraints(pi);

            constraint.Name.Should().Be("List");
            constraint.Type.Should().Be(ConstraintType.List);
            constraint.Values.Should().Contain("Range").And.Contain("List");
            constraint.Values.Length.Should().Be(2);
            constraint.Min.Should().NotHaveValue();
            constraint.Max.Should().NotHaveValue();
        }

        [Fact]
        public void GetConstraints_ReturnsRangeConstraint()
        {
            var pi = typeof(SomeAttributes).GetProperty("Range");
            var constraint = enricher.GetConstraints(pi);

            constraint.Name.Should().Be("Range");
            constraint.Type.Should().Be(ConstraintType.Range);
            constraint.Values.Should().BeNull();
            constraint.Min.Should().Be(1);
            constraint.Max.Should().Be(10);
        }

        [Fact]
        public void GetNotes_MI_ReturnsNull() => enricher.GetNotes(noPropertyInfo).Should().BeNull();

        [Fact]
        public void GetExternalLinks_ReturnsNull() => enricher.GetExternalLinks(noPropertyInfo).Should().BeNull();

        [Fact]
        public void GetSecurity_Null_IfNoAuthentication()
            => enricher.GetSecurity(new Operation { RequiresAuthentication = false }, "GET").Should().BeNull();

        [Theory]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(IList))]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(ICollection<int>))]
        public void GetAllowMultiple_True_ForCollectionTypes(Type collectionType)
            => enricher.GetAllowMultiple(collectionType).Should().BeTrue();

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(AllAttributes))]
        public void GetAllowMultiple_Null_ForNonCollectionTypes(Type collectionType)
            => enricher.GetAllowMultiple(collectionType).Should().NotHaveValue();
    }

    [Api("ApiDescription")]
    [System.ComponentModel.Description("ComponentModelDescription")]
    [DataAnnotations.Description("ServiceStackDescription")]
    [Route("/here", Notes = "These are some notes", Verbs = "GET")]
    [Route("/there", Verbs = "POST")]
    [ApiResponse(201, "Thing created")]
    [ApiResponse(503, "Not available")]
    [Exclude(Feature.Soap)]
    public class AllAttributes
    {
        
    }

    [Route("/here", Verbs = "GET")]
    [Route("/there")]
    public class MultiRoute {}

    [FallbackRoute("/fallback")]
    public class FallbackRoute { }

    [System.ComponentModel.Description("ComponentModelDescription")]
    [Description("ServiceStackDescription")]
    public class SomeAttributes
    {
        public int NoAttr { get; set; }

        [ApiAllowableValues("Range", 1, 10)]
        public int Range { get; set; }

        [ApiAllowableValues("List", typeof(ConstraintType))]
        public int List { get; set; }

        [ApiMember(AllowMultiple = true, ParameterType = "body", Description = "we're no here", IsRequired = true, Name = "batcat")]
        public string Thing { get; set; }

        public int? OptionalVal { get; set; }
    }

    public class OneWay : IService
    {
        public void Any(SomeAttributes requestDto) { }
    }

    [Description("ServiceStackDescription")]
    [Restrict(RequestAttributes.Json | RequestAttributes.Jsv)]
    public class OneAttribute : IService
    {
        public void Get(SomeAttributes requestDto) { }

        public object Post(SomeAttributes requestDto) => Response();

        public string Response() => "travel is dangerous";
    }

    [Route("")]
    [AddHeader(ContentType = MimeTypes.Bson)]
    public class EmptyRouteAttribute { }

    [Route("/foo-bar", "GET,POST")]
    public class RootForVerbs
    {
        [ApiAllowableValues("List", "foo", "bar")]
        public MyEnum EnumType { get; set; }
    }

    public enum MyEnum
    {
        One,
        Two,
        Three
    }
}
