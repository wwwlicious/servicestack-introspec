// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.TypeSpec
{
    using FluentAssertions;
    using Html;
    using IntroSpec.Models;
    using IntroSpec.TypeSpec;
    using Xunit;

    public class RequestSpecTests
    {
        private const string GlobalKey = "_all";
        private RequestDocumenter documenter = new RequestDocumenter();

        [Fact]
        public void Ctor_InitialisesLists()
        {
            var d = new RequestDocumenter();
            d.StatusCodes.Should().NotBeNull();
            d.Tags.Should().NotBeNull();
            d.ContentTypes.Should().NotBeNull();
        }
        
        [Fact]
        public void AddTags_PopulatesTagsCollection()
        {
            documenter.SetTags("Tag1", "Tag2");
            documenter.Tags.Count.Should().Be(2);
            documenter.Tags.Should().Contain("Tag1").And.Contain("Tag2");
        }

        [Fact]
        public void AddStatusCodes_NoVerb_PopulatesStatusCodesCollection()
        {
            var code = new StatusCode { Code = 201 };
            var code2 = new StatusCode { Code = 204 };
            documenter.SetStatusCodes(code, code2);
            documenter.StatusCodes[GlobalKey].Count.Should().Be(2);
            documenter.StatusCodes[GlobalKey].Should().Contain(code).And.Contain(code2);
        }

        [Fact]
        public void AddStatusCodes_Verb_PopulatesStatusCodesCollection()
        {
            var verb = HttpVerbs.Get;
            var verbString = verb.ToString();

            var code = new StatusCode { Code = 201 };
            var code2 = new StatusCode { Code = 204 };
            documenter.SetStatusCodes(verb, code, code2);
            documenter.StatusCodes[verbString].Count.Should().Be(2);
            documenter.StatusCodes[verbString].Should().Contain(code).And.Contain(code2);
        }

        [Fact]
        public void AddContentTypes_NoVerb_PopulatesContentTypesCollection()
        {
            documenter.SetContentTypes("text/json", "text/xml");
            documenter.ContentTypes[GlobalKey].Count.Should().Be(2);
            documenter.ContentTypes[GlobalKey].Should().Contain("text/json").And.Contain("text/xml");
        }

        [Fact]
        public void AddContentTypes_Verb_PopulatesContentTypesCollection()
        {
            var verb = HttpVerbs.Get;
            var verbString = verb.ToString();
            documenter.SetContentTypes(verb, "text/json", "text/xml");

            documenter.ContentTypes[verbString].Count.Should().Be(2);
            documenter.ContentTypes[verbString].Should().Contain("text/json").And.Contain("text/xml");
        }

        [Fact]
        public void AddRouteNotes_NoVerb_PopulatesRouteNotesCollection()
        {
            const string notes = "foo bar baz";
            documenter.SetNotes(notes);
            documenter.RouteNotes[GlobalKey].Should().Be(notes);
        }

        [Fact]
        public void AddRouteNotes_Verb_PopulatesRouteNotesCollection()
        {
            var verb = HttpVerbs.Get;
            var verbString = verb.ToString();
            const string notes = "foo bar baz";
            documenter.SetNotes(verb, notes);
            documenter.RouteNotes[verbString].Should().Be(notes);
        }
    }

    internal class RequestDocumenter : RequestSpec<ToDocument>
    {
        internal void SetTags(params string[] tags) => AddTags(tags);

        internal void SetStatusCodes(params StatusCode[] codes) => AddStatusCodes(codes);
        internal void SetStatusCodes(HttpVerbs verb, params StatusCode[] codes) => AddStatusCodes(verb, codes);

        internal void SetContentTypes(params string[] contentTypes) => AddContentTypes(contentTypes);
        internal void SetContentTypes(HttpVerbs verb, params string[] contentTypes) => AddContentTypes(verb, contentTypes);

        internal void SetNotes(string notes) => AddRouteNotes(notes);
        internal void SetNotes(HttpVerbs verb, string notes) => AddRouteNotes(verb, notes);
    }
}
