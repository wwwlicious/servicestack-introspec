namespace DemoService
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using ServiceStack;
    using ServiceStack.DataAnnotations;

    public enum Names
    {
        DaveDee,
        Dozy,
        Beaky,
        Mick,
        Tich
    }

    [Restrict(RequestAttributes.Xml | RequestAttributes.Json)]
    public class DemoService : Service
    {
        public object Post(DemoRequest demoRequest) => new DemoResponse { Message = "Response from Demo Service POST" };

        public object Get(DemoRequest demoRequest) => new DemoResponse { Message = "Response from Demo Service GET" };
    }

    [Api("Demo Request Description")]
    [ApiResponse(HttpStatusCode.OK, "Everything is hunky dory")]
    [ApiResponse(HttpStatusCode.InternalServerError, "Something went wrong")]
    [Route("/request/{Name}/", "GET,POST", Summary = "Route summary", Notes = "Notes from route attr")]
    public class DemoRequest : IReturn<DemoResponse>
    {
        [ApiMember(Name = "Name", Description = "This is a description of name", ParameterType = "body", DataType = "string", IsRequired = true)]
        [ApiAllowableValues("Name", typeof(Names))]
        public string Name { get; set; }

        [IgnoreDataMember]
        public string Ignored { get; set; }

        /// <summary>
        /// This is an optional property
        /// </summary>
        /// <remarks>More info on the optional property</remarks>
        [ApiMember(ExcludeInSchema = true)]
        public int AnotherProperty { get; set; }
    }

    public class DemoResponse
    {
        [ApiMember(Name = "Response Message", Description = "The returned message")]
        public string Message { get; set; }
    }

    public class GlassesService : Service
    {
        public object Any(GlassesRequest request) => new DemoResponse { Message = request.SunShining ? "Wear sun glasses" : "No glasses required" };
    }

    /// <remarks>I don't really have much to add here</remarks>
    [Api("Glasses Request Description")]
    [ApiResponse(HttpStatusCode.OK, "Ok")]
    [Authenticate]
    [Restrict(RequestAttributes.Xml | RequestAttributes.Json | RequestAttributes.Jsv)]
    public class GlassesRequest : IReturn<DemoResponse>, IGet, IPost
    {
        /// <summary>
        /// XML Comment on a property
        /// </summary>
        public bool SunShining { get; set; }

        public Name GlassesName { get; set; }
    }

    /// <summary>
    /// This is a description of FallbackRequest from XML
    /// </summary>
    /// <remarks>Notes from FallbackRequest</remarks>
    [FallbackRoute("/fallback")]
    public class FallbackRequest : IReturn<ComplexResponse>
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Name MyName { get; set; }

        /// <summary>
        /// Description of myfield from XML
        /// </summary>
        public string MyField = "Hi";
    }

    public class PlainService : Service
    {
        public object Any(FallbackRequest request) => new ComplexResponse { Message = "Plain request received" };

        public void Get(FallbackRequest request)
        {
            // no-op }
        }
    }

    /// <summary>
    /// This is a response type with embedded properties
    /// </summary>
    [Api("Response type with embedded type")]
    public class ComplexResponse
    {
        [ApiMember(Description = "The returned message")]
        public string Message { get; set; }

        [ApiMember(Name = "Embedded Type", Description = "Lets go")]
        public SubComplexResponse EmbeddedType { get; set; }
    }

    /// <summary>
    /// This is a response type with more complex thingamies
    /// </summary>
    [Api("Response type with embedded type")]
    public class SubComplexResponse
    {
        [ApiMember(Name = "Age", Description = "The age")]
        public int Age { get; set; }

        [ApiMember(Name = "DOB", Description = "The date of the birth")]
        public DateTime DateOfBirth { get; set; }

        [ApiMember(Name = "Details of my name", Description = "The age")]
        public Name MyName { get; set; }
        public Name YourName { get; set; }

        [IgnoreDataMember]
        public Name TheirName { get; set; }
    }

    /// <remarks>The people the people</remarks>
    public class Name
    {
        /// <summary>
        /// This is an xml desc of summary
        /// </summary>
        public string Forename { get; set; }

        /// <summary>
        /// Xml surname
        /// </summary>
        [ApiMember(Name = "Also known as last name")]
        public string Surname { get; set; }
    }

    public class MetaDataExcludeService : Service
    {
        public object Any(MetaExcludeRequest request)
            => new DemoResponse { Message = "You shouldn't see me, I'm excluded by metadata" };
    }

    [Api("Metadata ignored")]
    [Exclude(Feature.Metadata)]
    public class MetaExcludeRequest : IReturn<DemoResponse>{}

    public class ServiceDiscoveryExcludeService : Service
    {
        public object Any(DiscoveryExcludeRequest request)
            => new DemoResponse { Message = "You shouldn't see me, I'm excluded by service discovery" };
    }

    [Api("Service Discovery ignored")]
    [Exclude(Feature.ServiceDiscovery)]
    public class DiscoveryExcludeRequest : IReturn<DemoResponse> {}

    public class EmptyDtoService : Service
    {
        public object Any(EmptyDtoRequest request) => new DemoResponse { Message = "I have no params" };
    }

    public class EmptyDtoRequest : IReturn<DemoResponse>{}

    public class PersonRequest{}

    public class PersonRequestResponse{}

    public class PersonService : IService
    {
        public object Any(PersonRequest request) => new PersonRequestResponse();
    }

    public class OneWayService : IService
    {
        [RequiresAnyRole("Butcher", "Baker", "Candlestick Maker")]
        public void Get(OneWayRequest requiest) { }
    }

    [Api("One Way Request. Should have a 204 response")]
    public class OneWayRequest { }

    public class SecureResponse { }

    [Authenticate(ApplyTo.Get | ApplyTo.Put | ApplyTo.Post | ApplyTo.Delete)]
    [RequiredRole("Admin")]
    [RequiredPermission("CanAccess")]
    [RequiredPermission(ApplyTo.Put | ApplyTo.Post, "CanAdd")]
    [RequiresAnyPermission("CanDelete", "AdminRights", ApplyTo = ApplyTo.Delete)]
    public class SecureRequest : IReturn<SecureResponse> { }

    public class SecureService : IService
    {
        public object Any(SecureRequest request)
        {
            return new SecureResponse();
        }
    }
}   