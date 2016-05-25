// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Services
{
    using System;
    using System.Linq;
    using DataAnnotations;
    using Models;
    using Models.Postman;
    using Text;

    public class ApiSpecPostmanService : IService
    {
        // TODO Have this use the same filtering logic as ApiSpecService to get a subset of data
        public object Get(PostmanRequest request)
        {
            // Get the documentation object
            var apiSpecFeature = HostContext.GetPlugin<ApiSpecFeature>();
            var documentation = apiSpecFeature.Documentation;

            // Massage it into the postmanRequest object.
            // TODO Look at the cookies that are in the current postman plugin

            // TODO Use SS AutoMapping for this?
            var collection = new PostmanSpecCollection();

            var collectionId = Guid.NewGuid().ToString();
            collection.Id = collectionId;
            collection.Name = documentation.Title;
            collection.Description = documentation.Description;
            collection.Timestamp = DateTime.UtcNow.ToUnixTimeMs();

            collection.Requests =
                documentation.Resources.Select(r => GetRequests(r, documentation, collectionId)).ToArray();

            return new PostmanResponse { Collection = collection };
        }

        private PostmanSpecRequest[] GetRequests(ApiDocumentation documentation, string collectionId)
        {
            throw new NotImplementedException();
        }

        private PostmanSpecRequest GetRequests(ApiResourceDocumentation resource, ApiDocumentation documentation, string collectionId)
        {
            // Pick Header

            var request = new PostmanSpecRequest();
            request.Id = Guid.NewGuid().ToString();
            //request.Headers = 
            request.Url = documentation.ApiBaseUrl.CombineWith(request.Url);
            //request.PathVariables =
            //request.Method =
            //request.Data =
            //request.DataMode =

            request.Time = DateTime.UtcNow.ToUnixTimeMs();
            request.Name = resource.Title;
            request.Description = resource.Description;
            request.CollectionId = collectionId;

            return request;
        }
    }

    [Route(Constants.PostmanSpecUri)]
    [Exclude(Feature.Metadata | Feature.ServiceDiscovery)]
    public class PostmanRequest : IReturn<PostmanResponse> { }

    public class PostmanResponse
    {
        public PostmanSpecCollection Collection { get; set; }
    }
}
