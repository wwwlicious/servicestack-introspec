namespace ServiceStack.Documentation.Tests.Enrichers
{
    using System;
    using System.Reflection;
    using Documentation.Enrichers;
    using Documentation.XmlDocumentation;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class XmlEnricherTests
    {
        private readonly XmlEnricher nullEnricher = new XmlEnricher(null);
        private readonly IXmlDocumentationLookup lookup = A.Fake<IXmlDocumentationLookup>();
        private static readonly Type Type = typeof(TestClass);

        private XmlEnricher enricher => new XmlEnricher(lookup);

        private static PropertyInfo GetProperty() => Type.GetProperty("TheProp");

        [Fact]
        public void GetTitle_ReturnsNull_IfLookupNull() 
            => nullEnricher.GetTitle(Type).Should().BeNull();

        [Fact]
        public void GetTitle_ReturnsNull_IfMemberNotFound() 
            => enricher.GetTitle(Type).Should().BeNull();

        [Fact]
        public void GetTitle_ReturnsTitle_IfMemberFound()
        {
            const string name = "the sun smells too loud";
            A.CallTo(() => lookup.GetXmlMember(Type)).Returns(new XmlMember { Name = name });
            enricher.GetTitle(Type).Should().Be(name);
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
        public void GetDescription_PI_ReturnsDescription_IfMemberFound()
        {
            const string description = "travel is dangerious";
            var summary = new XmlBase { Text = description };
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember { Summary = summary });
            enricher.GetDescription(GetProperty()).Should().Be(description);
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
        public void GetTitle_PI_ReturnsDescription_IfMemberFound()
        {
            const string name = "mr November";
            A.CallTo(() => lookup.GetXmlMember(GetProperty())).Returns(new XmlMember { Name = name });
            enricher.GetTitle(GetProperty()).Should().Be(name);
        }
    }

    public class TestClass
    {
        public string TheProp { get; set; }
    }
}
