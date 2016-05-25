// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Services
{
    using DataAnnotations;

    public class ApiSpecPostmanService : IService
    {
        // TODO Have this use the same filtering logic as ApiSpecService to get a subset of data
        public object Get(PostmanRequest request)
        {
            return new PostmanResponse();
        }
    }

    [Route(Constants.PostmanSpecUri)]
    [Exclude(Feature.Metadata | Feature.ServiceDiscovery)]
    public class PostmanRequest : IReturn<PostmanResponse> { }

    public class PostmanResponse
    {
        public PostmanCollection Collection { get; set; }
    }
}
