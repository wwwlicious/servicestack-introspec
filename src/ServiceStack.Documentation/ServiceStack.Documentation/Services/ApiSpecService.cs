namespace ServiceStack.Documentation.Services
{
    using DataAnnotations;
    using Models;

    public class ApiSpecService : Service
    {
        public object Get(SpecRequest request)
        {
            // Get the documentation from the plugin
            var documentation = HostContext.GetPlugin<ApiSpecFeature>().Documentation;

            // TODO Filter out by auth permissions
            return new SpecResponse { ApiDocumentation = documentation };
        }
    }

    [Route(Constants.SpecUri)]
    [Exclude(Feature.Metadata)]
    public class SpecRequest : IReturn<SpecResponse> { }

    public class SpecResponse
    {
        public ApiDocumentation ApiDocumentation { get; set; }
    }
}
