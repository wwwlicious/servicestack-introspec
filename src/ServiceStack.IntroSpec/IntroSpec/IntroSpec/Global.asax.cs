using System;
using System.Collections.Generic;
using System.Net;
using Funq;
using ServiceStack;
using ServiceStack.Api.Swagger;
using ServiceStack.IntroSpec;
using ServiceStack.IntroSpec.Models;
using ServiceStack.IntroSpec.Settings;
using ServiceStack.Logging;
using ServiceStack.Text;

namespace IntroSpec
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("DemoDocumentationService", typeof(DemoService.DemoService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                ApiVersion = "2.0",
                HandlerFactoryPath = "something"
            });

            LogManager.LogFactory = new ConsoleLogFactory();

            SetupPlugins();
        }

        private void SetupPlugins()
        {
            Plugins.Add(new MetadataFeature());
            Plugins.Add(new PostmanFeature());
            Plugins.Add(new SwaggerFeature());

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

            Plugins.Add(new IntroSpecFeature
            {
                Description = "This is a demo app host setup for testing documentation.",
                ContactName = "Joe Bloggs",
                ContactEmail = "email@address.com",
                LicenseUrl = new Uri("http://mozilla.org/MPL/2.0/")
            });
        }
    }

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new AppHost().Init();
        }
    }
}