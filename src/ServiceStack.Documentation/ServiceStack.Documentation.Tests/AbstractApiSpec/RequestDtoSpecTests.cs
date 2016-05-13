namespace ServiceStack.Documentation.Tests.AbstractApiSpec
{
    using Documentation.AbstractApiSpec;
    using Documentation.Models;
    using FluentAssertions;
    using Xunit;

    public class RequestDtoSpecTests
    {
        private RequestDocumenter documenter = new RequestDocumenter();

        [Fact]
        public void AddVerbs_PopulatesVerbsCollection()
        {
            documenter.SetVerbs("GET", "POST");
            documenter.Verbs.Count.Should().Be(2);
            documenter.Verbs.Should().Contain("GET");
            documenter.Verbs.Should().Contain("POST");
        }

        [Fact]
        public void AddTags_PopulatesTagsCollection()
        {
            documenter.SetTags("Tag1", "Tag2");
            documenter.Tags.Count.Should().Be(2);
            documenter.Tags.Should().Contain("Tag1");
            documenter.Tags.Should().Contain("Tag2");
        }

        [Fact]
        public void AddStatusCodes_PopulatesStatusCodesCollection()
        {
            var code = new StatusCode { Code = 201 };
            var code2 = new StatusCode { Code = 204 };
            documenter.SetStatusCodes(code, code2);
            documenter.StatusCodes.Count.Should().Be(2);
            documenter.StatusCodes.Should().Contain(code);
            documenter.StatusCodes.Should().Contain(code2);
        }
    }

    internal class RequestDocumenter : RequestSpec<ToDocument>
    {
        internal void SetVerbs(params string[] verbs) => AddVerbs(verbs);
        internal void SetTags(params string[] tags) => AddTags(tags);
        internal void SetStatusCodes(params StatusCode[] codes) => AddStatusCodes(codes);
    }
}
