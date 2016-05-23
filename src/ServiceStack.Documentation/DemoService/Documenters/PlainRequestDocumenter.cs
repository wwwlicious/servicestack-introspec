
namespace DemoService.Documenters
{
    using ServiceStack.Documentation.AbstractApiSpec;
    using ServiceStack.Documentation.Models;

    public class PlainRequestDocumenter : RequestSpec<PlainRequest>
    {
        public PlainRequestDocumenter()
        {
            Title = "Plain request title from abstract";
            Description = "Plain request desc from abstract";
            Notes = "Plain request notes from abstract";

            Category = "Category1";

            AddVerbs("GET", "PUT", "POST");
            AddTags("Tag1", "Tag2", "Tag3");

            AddStatusCodes(
                new StatusCode
                {
                    Code = 503,
                    Name = "Service Unavailable",
                    Description = "Service is unavailable, try again in a wee bit"
                },
                new StatusCode
                {
                    Code = 204,
                    Name = "No Content",
                    Description = "A more verbose explanation that won't be shown"
                });

            AddContentTypes("application/hal+json");

            For(t => t.Name)
                .With(p => p.Title, "Name parameter abstract class definition")
                .With(p => p.IsRequired, true)
                .With(p => p.Description, "Description from abstract class");

            For(t => t.Age)
                .With(p => p.Title, "This is an optional thing. AC")
                .With(p => p.IsRequired, false);

            For(t => t.MyName)
                .With(p => p.Description, "This is a complex return type");
        }
    }

    public class GlassesRequestDocumenter : RequestSpec<GlassesRequest>
    {
        public GlassesRequestDocumenter()
        {
            Title = "Glasses title documenter";
            Description = "Glasses desc documenter";
            Notes = "Glasses notes from documenter";

            For(t => t.SunShining).With(p => p.Title, "Sun shining. Documenter");
        }
    }

    public class DemoResponseDocumenter : TypeSpec<DemoResponse>
    {
        public DemoResponseDocumenter()
        {
            Title = "Demo response title from documenter";
            Description = "Demo response description from documenter";
        }
    }
}