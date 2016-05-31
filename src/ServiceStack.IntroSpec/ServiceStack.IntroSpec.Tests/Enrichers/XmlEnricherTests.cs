// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Enrichers
{
    using System;
    using System.Reflection;
    using FakeItEasy;
    using Fixtures;
    using FluentAssertions;
    using Host;
    using IntroSpec.Enrichers;
    using IntroSpec.XmlDocumentation;
    using Xunit;

    [Collection("AppHost")]
    public class XmlEnricherTests
    {
        private readonly XmlEnricher nullEnricher = new XmlEnricher(null);
        private readonly IXmlDocumentationLookup lookup = A.Fake<IXmlDocumentationLookup>();
        private static readonly Type Type = typeof(TestClass);

        private XmlEnricher enricher => new XmlEnricher(lookup);

        private static PropertyInfo GetProperty() => Type.GetProperty("TheProp");
        private readonly AppHostFixture fixture;

        public XmlEnricherTests(AppHostFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void GetTitle_ReturnsNull_IfLookupNull() 
            => nullEnricher.GetTitle(Type).Should().BeNull();

        [Fact]
        public void GetTitle_ReturnsNull_IfMemberNotFound() 
            => enricher.GetTitle(Type).Should().BeNull();

        [Fact]
        public void GetTitle_ReturnsNull()
        {
            const string name = "the sun smells too loud";
            A.CallTo(() => lookup.GetXmlMember(Type)).Returns(new XmlMember { Name = name });
            enricher.GetTitle(Type).Should().BeNull();
        }

        [Fact]
        public void GetDescription_ReturnsNull_IfLookupNull()
            => nullEnricher.GetDescription(Type).Should().BeNull();

        [Fact]
        public void GetDescription_ReturnsNull_IfMemberNotFound()
            => enricher.GetDescription(Type).Should().BeNull();

        [Fact]
        public void GetDescription_ReturnsNull_IfMemberFoundButSummaryNull()
        {
            A.CallTo(() => lookup.GetXmlMember(Type)).Returns(new XmlMember());
            enricher.GetDescription(Type).Should().BeNull();
        }

        [Fact]
        public void GetDescription_ReturnsDescription_IfMemberFound()
        {
            const string description = "mogwai fear satan";
            var summary = new XmlBase { Text = description };
            A.CallTo(() => lookup.GetXmlMember(Type)).Returns(new XmlMember { Summary = summary });
            enricher.GetDescription(Type).Should().Be(description);
        }

        [Fact]
        public void GetNotes_ReturnsNull_IfLookupNull() 
            => nullEnricher.GetNotes(Type).Should().BeNull();

        [Fact]
        public void GetNotes_ReturnsNull_IfMemberNotFound()
            => enricher.GetNotes(Type).Should().BeNull();

        [Fact]
        public void GetNotes_ReturnsNull_IfMemberFoundButRemarksNull()
        {
            A.CallTo(() => lookup.GetXmlMember(Type)).Returns(new XmlMember());
            enricher.GetNotes(Type).Should().BeNull();
        }

        [Fact]
        public void GetNotes_ReturnsDescription_IfMemberFound()
        {
            const string notes = "hunted by a freak";
            var remarks = new XmlBase { Text = notes };
            A.CallTo(() => lookup.GetXmlMember(Type)).Returns(new XmlMember { Remarks = remarks });
            enricher.GetNotes(Type).Should().Be(notes);
        }

        [Fact]
        public void GetDescription_PI_ReturnsNull_IfLookupNull()
            => nullEnricher.GetDescription(GetProperty()).Should().BeNull();

        [Fact]
        public void GetDescription_PI_ReturnsNull_IfMemberNotFound()
            => enricher.GetDescription(GetProperty()).Should().BeNull();

        [Fact]
        public void GetDescription_PI_ReturnsNull_IfMemberFoundButSummaryNull()
        {
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember());
            enricher.GetDescription(GetProperty()).Should().BeNull();
        }

        [Fact]
        public void GetDescription_PI_ReturnsDescription_IfSummaryFound()
        {
            const string description = "travel is dangerious";
            var summary = new XmlBase { Text = description };
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember { Summary = summary });
            enricher.GetDescription(GetProperty()).Should().Be(description);
        }

        [Fact]
        public void GetDescription_PI_ReturnsDescription_IfValueFound()
        {
            const string description = "travel is dangerious";
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember { Value = description });
            enricher.GetDescription(GetProperty()).Should().Be(description);
        }

        [Fact]
        public void GetDescription_PI_ReturnsDescription_IfSummaryAndValueFound()
        {
            const string value = "travel is dangerious";
            const string summary = "a brief history of seven killings";
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember
            {
                Value = value,
                Summary = new XmlBase { Text = summary }
            });
            enricher.GetDescription(GetProperty()).Should().Be($"{summary} {value}");
        }

        [Fact]
        public void GetNotes_PI_ReturnsNull_IfLookupNull() 
            => nullEnricher.GetNotes(GetProperty()).Should().BeNull();

        [Fact]
        public void GetNotes_PI_ReturnsNull_IfMemberNotFound()
            => enricher.GetNotes(GetProperty()).Should().BeNull();

        [Fact]
        public void GetNotes_PI_ReturnsNull_IfMemberFoundButRemarksNull()
        {
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember());
            enricher.GetNotes(GetProperty()).Should().BeNull();
        }

        [Fact]
        public void GetNotes_PI_ReturnsDescription_IfMemberFound()
        {
            const string notes = "how to be a werewolf";
            var remarks = new XmlBase { Text = notes };
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember { Remarks = remarks });
            enricher.GetNotes(GetProperty()).Should().Be(notes);
        }

        [Fact]
        public void GetTitle_PI_ReturnsNull_IfLookupNull()
            => nullEnricher.GetTitle(GetProperty()).Should().BeNull();

        [Fact]
        public void GetTitle_PI_ReturnsNull_IfMemberNotFound()
            => enricher.GetTitle(GetProperty()).Should().BeNull();

        [Fact]
        public void GetTitle_PI_ReturnsNull_IfMemberFoundButNameNull()
        {
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember());
            enricher.GetTitle(GetProperty()).Should().BeNull();
        }

        [Fact]
        public void GetTitle_PI_ReturnsNull()
        {
            const string name = "mr November";
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember { Name = name });
            enricher.GetTitle(GetProperty()).Should().BeNull();
        }

        [Fact]
        public void GetStatusCodes_Null_IfLookupNull()
            => enricher.GetStatusCodes(new Operation { RequestType = Type }, "GET").Should().BeNull();

        [Theory]
        [InlineData("T:System.ArgumentException", "Argument Wrong", 400)]
        [InlineData("T:System.NotImplementedException", "Can't do that", 405)]
        public void GetStatusCodes_ReturnsException_DependantOnType(string exceptionName, string exceptionText, int expected)
        {
            var operation = new Operation { RequestType = Type };

            A.CallTo(() => lookup.GetXmlMember(Type)).Returns(new XmlMember
            {
                Exceptions = new[] { new XmlHasCref { Reference = exceptionName, Text = exceptionText } }
            });

            var result = enricher.GetStatusCodes(operation, "GET");
            result.Length.Should().Be(1);
            result[0].Code.Should().Be(expected);
            result[0].Description.Should().Be(exceptionText);
        }
    }

    public class TestClass
    {
        public string TheProp { get; set; }
    }
}
