
namespace ServiceStack.Documentation.AbstractApiSpec
{
    using System.Collections.Generic;
    using System.Reflection;
    using Models;

    public interface IApiResource
    {
        string Title { get; }
        string Description { get; }
        string Notes { get; }

        IProperty GetPropertySpec(PropertyInfo pi);
    }

    public interface IApiRequest : IApiResource
    {
        List<string> Verbs { get; }
        List<StatusCode> StatusCodes { get; }

        string Category { get; }
        List<string> Tags { get; }
    }
}
