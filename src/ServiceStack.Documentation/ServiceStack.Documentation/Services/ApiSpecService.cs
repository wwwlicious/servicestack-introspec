// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Services
{
    using DataAnnotations;
    using Models;

    public class ApiSpecService : Service
    {
        public object Get(SpecRequest request)
        {
            // Get the documentation from the plugin
            var apiSpecFeature = HostContext.GetPlugin<ApiSpecFeature>();
            var documentation = apiSpecFeature.Documentation;

            // TODO Filter out by auth permissions
            return new SpecResponse { ApiDocumentation = documentation };
        }
    }

    [Route(Constants.SpecUri)]
    [Exclude(Feature.Metadata | Feature.ServiceDiscovery)]
    public class SpecRequest : IReturn<SpecResponse> { }

    public class SpecResponse
    {
        public ApiDocumentation ApiDocumentation { get; set; }
    }
}
