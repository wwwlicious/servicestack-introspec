
namespace DemoService.TypeSpecs
{
    using ServiceStack.Html;
    using ServiceStack.IntroSpec.Models;
    using ServiceStack.IntroSpec.TypeSpec;

    public class FallbackRequestSpec : RequestSpec<FallbackRequest>
    {
        public FallbackRequestSpec()
        {
            Title = "Fallback request title from abstract class";
            Description = "Fallback request desc from abstract";
            Notes = "Fallback request notes from abstract";

            Category = "Category1";

            AddTags("Tag1", "Tag2", "Tag3");

            AddStatusCodes(HttpVerbs.Get,
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

            AddStatusCodes(HttpVerbs.Post, (StatusCode) 400);

            AddContentTypes("application/hal+json");

            AddRouteNotes(HttpVerbs.Get, "This is a note about GET route");

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

    public class SecureDocumenter : RequestSpec<SecureRequest>
    {
        public SecureDocumenter()
        {
            AddContentTypes(HttpVerbs.Delete, "application/x-custom");

            AddStatusCodes(HttpVerbs.Delete, ((StatusCode)400).WithDescription("Only for delete"));
        }
    }
}