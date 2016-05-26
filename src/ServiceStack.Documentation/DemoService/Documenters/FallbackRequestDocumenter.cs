
namespace DemoService.Documenters
{
    using ServiceStack.Documentation.AbstractApiSpec;
    using ServiceStack.Documentation.Models;

    public class FallbackRequestDocumenter : RequestSpec<FallbackRequest>
    {
        public FallbackRequestDocumenter()
        {
            Title = "Fallback request title from abstract class";
            Description = "Fallback request desc from abstract";
            Notes = "Fallback request notes from abstract";

            Category = "Category1";

            AddVerbs("GET", "PUT", "POST");
            AddTags("Tag1", "Tag2", "Tag3");

            AddStatusCodes(
                new StatusCode
                {
                    Code = 503,
                    Name = "Service Unavailable",
                    Description = "Service is unavailable, try again later"
                },
                new StatusCode
                {
                    Code = 204,
                    Name = "No Content",
                    Description = "A more verbose explanation that won't be shown"
                });

            AddContentTypes("application/hal+json");

            For(t => t.MyField)
                .With(p => p.Title, "A field, not a property");

            For(t => t.Name)
                .With(p => p.Title, "Name parameter abstract class definition")
                .With(p => p.IsRequired, true)
                .With(p => p.Description, "Description from abstract class");

            For(t => t.Age)
                .With(p => p.Title, "Age is optional")
                .With(p => p.IsRequired, false)
                .With(p => p.Constraint, PropertyConstraint.RangeConstraint("Age Range", 0, 120));

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

    public class NameDocumenter : TypeSpec<Name>
    {
        public NameDocumenter()
        {
            Title = "Title of Name";
            Description = "This comes from TypeSpec implementation and will be used everywhere that Name is.";

            For(t => t.Forename)
                .With(p => p.IsRequired, true);
        }
    }
}