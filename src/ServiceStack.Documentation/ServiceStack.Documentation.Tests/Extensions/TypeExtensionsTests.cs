// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Extensions
{
    using DataAnnotations;
    using Documentation.Extensions;
    using FluentAssertions;
    using Xunit;

    [Collection("ApiSpecFeatureTests")]
    public class TypeExtensionsTests
    {
        [Fact]
        public void HasXmlSupport_True_IfXml() => typeof(All).HasXmlSupport().Should().BeTrue();

        [Fact]
        public void HasXmlSupport_False_IfExcludeXml() => typeof(XmlExclude).HasXmlSupport().Should().BeFalse();

        [Fact]
        public void HasXmlSupport_True_IfXmlRestrict() => typeof(XmlRestrict).HasXmlSupport().Should().BeTrue();

        [Fact]
        public void HasXmlSupport_True_IfExclude_NotXml() => typeof(JsvRestrict).HasXmlSupport().Should().BeFalse();

        [Fact]
        public void HasJsvSupport_True_IfJsv() => typeof(All).HasJsvSupport().Should().BeTrue();

        [Fact]
        public void HasJsvSupport_False_IfExcludeJsv() => typeof(JsvExclude).HasJsvSupport().Should().BeFalse();

        [Fact]
        public void HasJsvSupport_True_IfJsvRestrict() => typeof(JsvRestrict).HasJsvSupport().Should().BeTrue();

        [Fact]
        public void HasJsvSupport_True_IfExclude_NotJsv() => typeof(JsonRestrict).HasJsvSupport().Should().BeFalse();

        [Fact]
        public void HasJsonSupport_True_IfJson() => typeof(All).HasJsonSupport().Should().BeTrue();

        [Fact]
        public void HasJsonSupport_False_IfExcludeJson() => typeof(JsonExclude).HasJsonSupport().Should().BeFalse();

        [Fact]
        public void HasJsonSupport_True_IfJsonRestrict() => typeof(JsonRestrict).HasJsonSupport().Should().BeTrue();

        [Fact]
        public void HasJsonSupport_True_IfExclude_NotJson() => typeof(Soap11Restrict).HasJsonSupport().Should().BeFalse();

        [Fact]
        public void HasSoap11Support_True_IfSoap() => typeof(All).HasSoap11Support().Should().BeTrue();

        [Fact]
        public void HasSoap11Support_False_IfExcludeSoap() => typeof(SoapExclude).HasSoap11Support().Should().BeFalse();

        [Fact]
        public void HasSoap11Support_True_IfSoapRestrict() => typeof(Soap11Restrict).HasSoap11Support().Should().BeTrue();

        [Fact]
        public void HasSoap11Support_True_IfExclude_NotSoap() => typeof(Soap12Restrict).HasSoap11Support().Should().BeFalse();

        [Fact]
        public void HasSoap12Support_True_IfSoap() => typeof(All).HasSoap12Support().Should().BeTrue();

        [Fact]
        public void HasSoap12Support_False_IfExcludeSoap() => typeof(SoapExclude).HasSoap12Support().Should().BeFalse();

        [Fact]
        public void HasSoap12Support_True_IfSoapRestrict() => typeof(Soap12Restrict).HasSoap12Support().Should().BeTrue();

        [Fact]
        public void HasSoap12Support_True_IfExclude_NotSoap() => typeof(Soap11Restrict).HasSoap12Support().Should().BeFalse();

        [Fact]
        public void HasCsvSupport_True_IfCsv() => typeof(All).HasCsvSupport().Should().BeTrue();

        [Fact]
        public void HasCsvSupport_False_IfExcludeCsv() => typeof(CsvExclude).HasCsvSupport().Should().BeFalse();

        [Fact]
        public void HasCsvSupport_True_IfCsvRestrict() => typeof(CsvRestrict).HasCsvSupport().Should().BeTrue();

        [Fact]
        public void HasCsvSupport_True_IfExclude_NotCsv() => typeof(Soap11Restrict).HasCsvSupport().Should().BeFalse();

        [Fact]
        public void HasHtmlSupport_True_IfHtml() => typeof(All).HasHtmlSupport().Should().BeTrue();

        [Fact]
        public void HasHtmlSupport_False_IfExcludeHtml() => typeof(HtmlExclude).HasHtmlSupport().Should().BeFalse();

        [Fact]
        public void HasHtmlSupport_True_IfHtmlRestrict() => typeof(HtmlRestrict).HasHtmlSupport().Should().BeTrue();

        [Fact]
        public void HasHtmlSupport_True_IfExclude_NotHtml() => typeof(Soap11Restrict).HasHtmlSupport().Should().BeFalse();
        
        [Fact]
        public void HasMsgPackSupport_False_IfNoMsgPackFeature() => typeof(All).HasMsgPackSupport().Should().BeFalse();

        [Fact(Skip = "Need appHost with MsgPack")]
        public void HasMsgPackSupport_True_IfMsgPack() => typeof(All).HasMsgPackSupport().Should().BeTrue();

        [Fact(Skip = "Need appHost with MsgPack")]
        public void HasMsgPackSupport_False_IfExcludeMsgPack() => typeof(MsgPackExclude).HasMsgPackSupport().Should().BeFalse();

        [Fact(Skip = "Need appHost with MsgPack")]
        public void HasMsgPackSupport_True_IfMsgPackRestrict() => typeof(MsgPackRestrict).HasMsgPackSupport().Should().BeTrue();

        [Fact(Skip = "Need appHost with MsgPack")]
        public void HasMsgPackSupport_True_IfExclude_NotMsgPack() => typeof(Soap11Restrict).HasMsgPackSupport().Should().BeFalse();

        [Fact]
        public void HasProtoBufSupport_False_IfNoProtoBufFeature() => typeof(All).HasProtoBufSupport().Should().BeFalse();

        [Fact(Skip = "Need appHost with ProtoBuf")]
        public void HasProtoBufSupport_True_IfProtoBuf() => typeof(All).HasProtoBufSupport().Should().BeTrue();

        [Fact(Skip = "Need appHost with ProtoBuf")]
        public void HasProtoBufSupport_False_IfExcludeProtoBuf() => typeof(ProtoBufExclude).HasProtoBufSupport().Should().BeFalse();

        [Fact(Skip = "Need appHost with ProtoBuf")]
        public void HasProtoBufSupport_True_IfProtoBufRestrict() => typeof(ProtoBufRestrict).HasProtoBufSupport().Should().BeTrue();

        [Fact(Skip = "Need appHost with ProtoBuf")]
        public void HasProtoBufSupport_True_IfExclude_NotProtoBuf() => typeof(Soap11Restrict).HasProtoBufSupport().Should().BeFalse();
    }

    public class All{ }

    [Exclude(Feature.Xml)] public class XmlExclude { }
    [Restrict(RequestAttributes.Xml)] public class XmlRestrict { }

    [Exclude(Feature.Jsv)] public class JsvExclude { }
    [Restrict(RequestAttributes.Jsv)] public class JsvRestrict { }

    [Exclude(Feature.Json)] public class JsonExclude { }
    [Restrict(RequestAttributes.Json)] public class JsonRestrict { }

    [Exclude(Feature.Soap)] public class SoapExclude { }
    [Restrict(RequestAttributes.Soap11)] public class Soap11Restrict { }
    [Restrict(RequestAttributes.Soap12)] public class Soap12Restrict { }

    [Exclude(Feature.Csv)] public class CsvExclude { }
    [Restrict(RequestAttributes.Csv)] public class CsvRestrict { }

    [Exclude(Feature.Html)] public class HtmlExclude { }
    [Restrict(RequestAttributes.Html)] public class HtmlRestrict { }

    [Exclude(Feature.MsgPack)] public class MsgPackExclude { }
    [Restrict(RequestAttributes.MsgPack)] public class MsgPackRestrict { }

    [Exclude(Feature.ProtoBuf)] public class ProtoBufExclude { }
    [Restrict(RequestAttributes.ProtoBuf)] public class ProtoBufRestrict { }
}
