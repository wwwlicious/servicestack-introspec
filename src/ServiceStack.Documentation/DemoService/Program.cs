namespace DemoService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Runtime.Serialization;
    using Funq;
    using ServiceStack;
    using ServiceStack.Api.Swagger;
    using ServiceStack.DataAnnotations;
    using ServiceStack.Documentation;
    using ServiceStack.Documentation.Models;
    using ServiceStack.Documentation.Settings;
    using ServiceStack.Logging;
    using ServiceStack.MsgPack;
    using ServiceStack.Text;

    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            var serviceUrl = "http://127.0.0.1:8090/";
            var x = new AppHost(serviceUrl).Init().Start("http://*:8090/");
            $"ServiceStack SelfHost listening at {serviceUrl} ".Print();
            Process.Start(serviceUrl);

            if (x.StartUpErrors.Count > 0)
            {
                foreach (var responseStatus in x.StartUpErrors)
                    log.Warn($"Error in Startup. {responseStatus.Message} - {responseStatus.StackTrace}");
            }

            Console.ReadLine();
        }
    }

    public class AppHost : AppSelfHostBase
    {
        private readonly string serviceUrl;

        public AppHost(string serviceUrl) : base("DemoDocumentationService", typeof(DemoService).Assembly)
        {
            this.serviceUrl = serviceUrl;
        }

        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                WebHostUrl = serviceUrl,
                ApiVersion = "2.0"
            });

            LogManager.LogFactory = new ConsoleLogFactory();

            SetupPlugins();
        }

        private void SetupPlugins()
        {
            Plugins.Add(new MsgPackFormat());
            Plugins.Add(new PostmanFeature());
            Plugins.Add(new SwaggerFeature());

            var apiSpecConfig = new ApiSpecConfig
            {
                Contact = new ApiContact { Email = "email@address.com", Name = "Donald Gray" },
                Description = "I'm jim morrison. I'm dead.",
                LicenseUrl = new Uri("http://mozilla.org/MPL/2.0/")
            };

            //DocumenterSettings.With(verbs: new List<string> { "HEAD", "PATCH" });
            //DocumenterSettings.AnyVerbs = new List<string> { "HEAD", "PATCH" };
            //DocumenterSettings.CollectionStrategy = EnrichmentStrategy.SetIfEmpty;

            JsConfig.IncludePublicFields = true;
            DocumenterSettings.DefaultStatusCodes = new List<StatusCode>
            {
                new StatusCode { Code = 429, Description = "This is rate limited", Name = "Too Many Requests" },
                ((StatusCode)HttpStatusCode.Forbidden).WithDescription("Set at a global level"),
                ((StatusCode)200).WithDescription("Set at a global level")
            };
            DocumenterSettings.FallbackCategory = "Default Category";
            DocumenterSettings.DefaultTags = new[] { "Default1" };

            DocumenterSettings.FallbackNotes = "Default notes, set at a global level";
            Plugins.Add(new ApiSpecFeature(apiSpecConfig));
        }
    }

    [Restrict(RequestAttributes.Xml | RequestAttributes.Json)]
    public class DemoService : Service
    {
        //public object Any(DemoRequest demoRequest) => new DemoResponse { Message = "Response from Demo Service" };

        public object Post(DemoRequest demoRequest) => new DemoResponse { Message = "Response from Demo Service" };

        public object Get(DemoRequest demoRequest) => new DemoResponse { Message = "Response from Demo Service" };
    }
    
    [Api("Demo Request Description")]
    [ApiResponse(HttpStatusCode.OK, "Everything is hunky dory")]
    [ApiResponse(HttpStatusCode.InternalServerError, "Something went wrong")]
    // thrown exceptions. documentation.
    [Route("/request/{Name}/", "GET,POST", Summary = "Route summary", Notes = "Notes from route attr")]
    public class DemoRequest : IReturn<DemoResponse>
    {
        [ApiMember(Name = "Name", Description = "This is a description of name", ParameterType = "body", DataType = "string", IsRequired = true)]
        [ApiAllowableValues("Name", typeof(HttpStatusCode))]
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

    public class GlassesService : Service
    {
        public object Any(GlassesRequest request) => new DemoResponse { Message = request.SunShining ? "Wear sun glasses" : "No glasses required" };
    }

    /// <remarks>I don't really have much to add here</remarks>
    [Api("Glasses Request Description")]
    [ApiResponse(HttpStatusCode.OK, "Ok")]
    [Authenticate]
    [Restrict(RequestAttributes.Xml | RequestAttributes.Json | RequestAttributes.Jsv)]
    public class GlassesRequest : IReturn<DemoResponse>, IGet, IPost //default if none
    {
        /// <summary>
        /// XML Comment on a property
        /// </summary>
        public bool SunShining { get; set; }

        public Name GlassesName { get; set; }
    }

    public class DemoResponse
    {
        [ApiMember(Name="Response Message", Description = "The returned message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// This is a response type with more complex thingamies
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

    /// <summary>
    /// This is a description of PlainText from XML
    /// </summary>
    /// <remarks>Notes from PlainRequest</remarks>
    [FallbackRoute("/fallback")]
    public class PlainRequest : IReturn<ComplexResponse>
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Name MyName { get; set; }

        [DataMember]
        public string MyField = "Hi";

        [DataMember]
        public const string NotMyField = "Hi";
    }

    public class PlainService : Service
    {
        public object Any(PlainRequest request) => new ComplexResponse { Message = "Plain request received" };

        public void Get(PlainRequest request)
        {
            // no-op
        }
    }

    public class MetaDataExcludeService : Service
    {
        public object Any(MetaExcludeRequest request) => new DemoResponse { Message = "You shouldn't see me, I'm excluded by metadata" };
    }

    [Api("Metadata ignored")]
    [Exclude(Feature.Metadata)]
    public class MetaExcludeRequest : IReturn<DemoResponse> //default if none
    {
    }

    public class ServiceDiscoveryExcludeService : Service
    {
        public object Any(DiscoveryExcludeRequest request) => new DemoResponse { Message = "You shouldn't see me, I'm excluded by service discovery" };
    }

    [Api("Service Discovery ignored")]
    [Exclude(Feature.ServiceDiscovery)]
    public class DiscoveryExcludeRequest : IReturn<DemoResponse> //default if none
    {
    }
}
