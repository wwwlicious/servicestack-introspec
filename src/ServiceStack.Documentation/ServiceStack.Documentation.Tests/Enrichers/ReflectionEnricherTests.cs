namespace ServiceStack.Documentation.Tests.Enrichers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using Documentation.Enrichers;
    using Documentation.Settings;
    using FluentAssertions;
    using Host;
    using Xunit;

    public class ReflectionEnricherTests
    {
        private ReflectionEnricher enricher = new ReflectionEnricher();

        private static PropertyInfo noPropertyInfo => typeof(SomeAttributes).GetProperty("NoAttr");
        private static PropertyInfo propertyInfo => typeof (SomeAttributes).GetProperty("Thing");

        [Fact]
        public void GetTitle_ReturnsTypeName() => enricher.GetTitle(typeof(AllAttributes)).Should().Be("AllAttributes");

        [Theory]
        [InlineData(typeof (AllAttributes), "ApiDescription")]
        [InlineData(typeof (SomeAttributes), "ComponentModelDescription")]
        [InlineData(typeof (OneAttribute), "ServiceStackDescription")]
        public void GetDescription_ReturnsCorrectPrecedence(Type type, string expected)
            => enricher.GetDescription(type).Should().Be(expected);

        [Fact]
        public void GetNotes_ReturnsNull_IfNoRouteAttribute()
            => enricher.GetNotes(typeof (SomeAttributes)).Should().BeNull();

        [Fact]
        public void GetNotes_ReturnsNotes_IfRouteAttribute()
            => enricher.GetNotes(typeof(AllAttributes)).Should().Be("These are some notes");

        [Fact]
        public void GetVerbs_ReturnsAllVerbs_IfOperationActionsIsAny()
        {
            var operation = new Operation { Actions = new List<string> { "ANY" } };
            var result = enricher.GetVerbs(operation);
            result.Should().BeEquivalentTo("GET", "POST");
        }

        [Fact]
        public void GetVerbs_ReturnsVerbsFromSettings_IfOperationActionsContainsAny()
        {
            var defaultVerbs = new[] { "GET", "PUT", "HEAD", "LESSERKNOWN" };
            using (DocumenterSettings.With(verbs: defaultVerbs))
            {
                var operation = new Operation { Actions = new List<string> { "GET", "PUT", "ANY" } };
                var result = enricher.GetVerbs(operation);
                result.Should().BeEquivalentTo(defaultVerbs);
            }
        }

        [Fact]
        public void GetVerbs_ReturnsVerbs()
        {
            var operation = new Operation { Actions = new List<string> { "GET", "POST", "PUT" } };
            var result = enricher.GetVerbs(operation);
            result.Should().BeEquivalentTo("GET", "POST", "PUT" );
        }

        [Fact]
        public void GetSatusCodes_ReturnsEmptyArray_IfNoApiResponseAttribute()
        {
            var operation = new Operation
            {
                ServiceType = typeof (AllAttributes),
                RequestType = typeof (SomeAttributes),
                ResponseType = typeof (AllAttributes) //This stops it being marked as one way
            };
            var result = enricher.GetStatusCodes(operation);
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetSatusCodes_204_IfNoResponseType()
        {
            var operation = new Operation
            {
                ServiceType = typeof(OneAttribute),
                RequestType = typeof(SomeAttributes)
            };
            var result = enricher.GetStatusCodes(operation);
            result.Length.Should().Be(1);
            result[0].Code.Should().Be(204);
            result[0].Name.Should().Be("No Content");
        }

        [Fact]
        public void GetSatusCodes_204_IfContainsVoidReturn()
        {
            var operation = new Operation
            {
                ServiceType = typeof(OneAttribute),
                RequestType = typeof(SomeAttributes),
                ResponseType = typeof(AllAttributes) //This stops it being marked as one way
            };
            var result = enricher.GetStatusCodes(operation);
            result.Length.Should().Be(1);
            result[0].Code.Should().Be(204);
            result[0].Name.Should().Be("No Content");
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

            var result = enricher.GetStatusCodes(operation);
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

            var result = enricher.GetStatusCodes(operation);
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
        public void GetIsRequired_PI_ReturnsNull_NoAttribute()
            => enricher.GetIsRequired(noPropertyInfo).Should().NotHaveValue();

        [Fact]
        public void GetIsRequired_PI_Returns_ApiAttributeName()
            => enricher.GetIsRequired(propertyInfo).Should().BeTrue();

        [Fact]
        public void GetRelativePath_ReturnsPath_IfRouteAttribute()
        {
            var operation = new Operation { RequestType = typeof(AllAttributes) };
            enricher.GetRelativePath(operation).Should().Be("/here");
        }

        [Fact]
        public void GetRelativePath_ReturnsOneWayPath_IfNoRouteAttribute_AndOneWay()
        {
            var operation = new Operation { RequestType = typeof(SomeAttributes) };
            enricher.GetRelativePath(operation).Should().Be("/json/oneway/SomeAttributes");
        }

        [Fact]
        public void GetRelativePath_ReturnsReplyPath_IfNoRouteAttribute_AndNotOneWay()
        {
            var operation = new Operation { RequestType = typeof(SomeAttributes), ResponseType = typeof(OneAttribute) };
            enricher.GetRelativePath(operation).Should().Be("/json/reply/SomeAttributes");
        }

        [Fact]
        public void GetRelativePath_ReturnsOneWayPath_IfRouteAttributeWithEmptyPath_AndOneWay()
        {
            var operation = new Operation { RequestType = typeof(EmptyRouteAttribute) };
            enricher.GetRelativePath(operation).Should().Be("/json/oneway/EmptyRouteAttribute");
        }
    }

    [Api("ApiDescription")]
    [Description("ComponentModelDescription")]
    [DataAnnotations.Description("ServiceStackDescription")]
    [Route("/here", Notes = "These are some notes")]
    [ApiResponse(201, "Thing created")]
    [ApiResponse(503, "Not available")]
    public class AllAttributes
    {
        
    }

    [Description("ComponentModelDescription")]
    [DataAnnotations.Description("ServiceStackDescription")]
    public class SomeAttributes
    {
        public int NoAttr { get; set; }

        [ApiMember(AllowMultiple = true, ParameterType = "body", Description = "we're no here", IsRequired = true, Name = "batcat")]
        public string Thing { get; set; }
    }

    [DataAnnotations.Description("ServiceStackDescription")]
    public class OneAttribute
    {
        public void Holla(SomeAttributes requestDto) { }

        public string Response() => "travel is dangerous";
    }

    [Route("")]
    public class EmptyRouteAttribute { }
}
