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
            var urlBase = "http://*:8090/";
            var x = new AppHost().Init().Start(urlBase);
            $"ServiceStack SelfHost listening at {urlBase} ".Print();
            Process.Start("http://localhost:8090/");

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
        public AppHost() : base("DemoDocumentationService", typeof(DemoService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
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
