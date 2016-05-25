// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAnnotations;
    using Extensions;
    using Models;
    using Models.Postman;
    using Text;

    public class ApiSpecPostmanService : IService
    {
        // Set as property in feature?
        public Dictionary<string, string> FriendlyTypeNames = new Dictionary<string, string>
        {
            {"Int32", "int"},
            {"Int64", "long"},
            {"Boolean", "bool"},
            {"String", "string"},
            {"Double", "double"},
            {"Single", "float"},
        };

        // Need to be able to set which Headers to add???
        // TODO Auth

        // TODO Have this use the same filtering logic as ApiSpecService to get a subset of data
        // TODO Take filter of Verb??
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

            collection.Requests = GetRequests(documentation, collectionId).ToArray();

            return collection;
        }

        private IEnumerable<PostmanSpecRequest> GetRequests(ApiDocumentation documentation, string collectionId)
        {
            // Iterate over all resources
            foreach (var resource in documentation.Resources)
            {
                // TODO Tighten up the logic used here
                var contentType = resource.ContentTypes.Contains(MimeTypes.Json)
                                      ? MimeTypes.Json
                                      : resource.ContentTypes.First();

                var data = resource.Properties.Select(r =>
                        new PostmanSpecData
                        {
                            Enabled = true,
                            Key = r.Title,
                            Type = FriendlyTypeNames.SafeGet(r.ClrType.Name, r.ClrType.Name),
                            Value = $"{r.Title}_value"
                        }).ToList();

                // Iterate through every verb of every resource. Generate a collection request per verb
                foreach (var verb in resource.Verbs)
                {
                    var request = new PostmanSpecRequest
                    {
                        Id = Guid.NewGuid().ToString(),
                        Url = documentation.ApiBaseUrl.CombineWith(resource.RelativePath),
                        Method = verb,
                        Time = DateTime.UtcNow.ToUnixTimeMs(),
                        Name = resource.Title,
                        Description = resource.Description,
                        CollectionId = collectionId,
                        Headers = $"Accept: {contentType}"
                    };

                    // Build the list of PostmanSpecData here as this will be then split into PathVariables or used straight

                    if (verb.HasRequestBody())
                    {
                        request.Data = data;
                        request.PathVariables = null;
                    }
                    else
                    {
                        request.Data = null;

                        // TODO Only set PathVariables if within 2 slashes.
                        // TODO Handle query string parameters
                        request.PathVariables = data.ToDictionary(k => k.Key, v => v.Value);
                    }

                    yield return request;
                }
            }
        }
    }

    [Route(Constants.PostmanSpecUri)]
    [Exclude(Feature.Metadata | Feature.ServiceDiscovery)]
    public class PostmanRequest : IReturn<PostmanSpecCollection> { }

    public class PostmanResponse
    {
        public PostmanSpecCollection Collection { get; set; }
    }
}
