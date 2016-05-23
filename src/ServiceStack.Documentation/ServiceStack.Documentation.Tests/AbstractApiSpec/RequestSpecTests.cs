// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.AbstractApiSpec
{
    using Documentation.AbstractApiSpec;
    using Documentation.Models;
    using FluentAssertions;
    using Xunit;

    public class RequestSpecTests
    {
        private RequestDocumenter documenter = new RequestDocumenter();

        [Fact]
        public void Ctor_InitialisesLists()
        {
            var d = new RequestDocumenter();
            d.Verbs.Should().NotBeNull();
            d.StatusCodes.Should().NotBeNull();
            d.Tags.Should().NotBeNull();
            d.ContentTypes.Should().NotBeNull();
        }

        [Fact]
        public void AddVerbs_PopulatesVerbsCollection()
        {
            documenter.SetVerbs("GET", "POST");
            documenter.Verbs.Count.Should().Be(2);
            documenter.Verbs.Should().Contain("GET").And.Contain("POST");
        }

        [Fact]
        public void AddTags_PopulatesTagsCollection()
        {
            documenter.SetTags("Tag1", "Tag2");
            documenter.Tags.Count.Should().Be(2);
            documenter.Tags.Should().Contain("Tag1").And.Contain("Tag2");
        }

        [Fact]
        public void AddStatusCodes_PopulatesStatusCodesCollection()
        {
            var code = new StatusCode { Code = 201 };
            var code2 = new StatusCode { Code = 204 };
            documenter.SetStatusCodes(code, code2);
            documenter.StatusCodes.Count.Should().Be(2);
            documenter.StatusCodes.Should().Contain(code).And.Contain(code2);
        }

        [Fact]
        public void AddContentTypes_PopulatesContentTypesCollection()
        {
            documenter.SetContentTypes("text/json", "text/xml");
            documenter.ContentTypes.Count.Should().Be(2);
            documenter.ContentTypes.Should().Contain("text/json").And.Contain("text/xml");
        }
    }

    internal class RequestDocumenter : RequestSpec<ToDocument>
    {
        internal void SetVerbs(params string[] verbs) => AddVerbs(verbs);
        internal void SetTags(params string[] tags) => AddTags(tags);
        internal void SetStatusCodes(params StatusCode[] codes) => AddStatusCodes(codes);
        internal void SetContentTypes(params string[] contentTypes) => AddContentTypes(contentTypes);
    }
}
