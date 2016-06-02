namespace DemoService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using Funq;
    using ServiceStack;
    using ServiceStack.Api.Swagger;
    using ServiceStack.IntroSpec;
    using ServiceStack.IntroSpec.Models;
    using ServiceStack.IntroSpec.Settings;
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

            //DocumenterSettings.ReplacementVerbs = new[] { "GET", "PUT", "POST", "DELETE" };
            //DocumenterSettings.CollectionStrategy = EnrichmentStrategy.SetIfEmpty;

            JsConfig.IncludePublicFields = true; // Serialize Fields 
            DocumenterSettings.DefaultStatusCodes = new List<StatusCode>
            {
                new StatusCode { Code = 429, Description = "This is rate limited", Name = "Too Many Requests" },
                ((StatusCode)HttpStatusCode.Forbidden).WithDescription("Set at a global level"),
                ((StatusCode)200).WithDescription("Ok. Set at a global level")
            };
            DocumenterSettings.FallbackCategory = "Fallback Category";
            DocumenterSettings.DefaultTags = new[] { "DefaultTag" };
            DocumenterSettings.FallbackNotes = "Default notes, set at a global level";

            Plugins.Add(new ApiSpecFeature(config =>
                    config.WithDescription("This is a demo app host setup for testing documentation.")
                          .WithLicenseUrl(new Uri("http://mozilla.org/MPL/2.0/"))
                          .WithContactName("Joe Bloggs")
                          .WithContactEmail("email@address.com")));
        }
    }
}
