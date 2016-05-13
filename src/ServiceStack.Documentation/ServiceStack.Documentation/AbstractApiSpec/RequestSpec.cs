namespace ServiceStack.Documentation.AbstractApiSpec
{
    using System.Collections.Generic;
    using Models;

    // NOTE Is the new() constraint really required?
    public abstract class RequestSpec<T> : TypeSpec<T>, IApiRequest
        where T : class, new()
    {
        public List<string> Verbs { get; }
        public List<StatusCode> StatusCodes { get; }
        public List<string> Tags { get; }

        public string Category { get; protected set; }

        protected void AddVerbs(params string[] verbs) => Verbs.AddRange(verbs);
        protected void AddTags(params string[] tags) => Tags.AddRange(tags);
        protected void AddStatusCodes(params StatusCode[] statusCodes) => StatusCodes.AddRange(statusCodes);

        protected RequestSpec()
        {
            Verbs = new List<string>();
            StatusCodes = new List<StatusCode>();
            Tags = new List<string>();
        }
    }
}