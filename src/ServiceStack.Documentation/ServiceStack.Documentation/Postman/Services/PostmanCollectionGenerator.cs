// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Postman.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Documentation.Models;
    using Extensions;
    using Models;
    using Text;

    public class PostmanModelGenerator
    {
        public Dictionary<string, string> FriendlyTypeNames = new Dictionary<string, string>
        {
            {"Int32", "int"},
            {"Int64", "long"},
            {"Boolean", "bool"},
            {"String", "string"},
            {"Double", "double"},
            {"Single", "float"},
        };

        public PostmanSpecCollection GetPostmanCollection(ApiDocumentation documentation)
        {
            // TODO Use SS AutoMapping for this?
            // Convert apiDocumentation to postman spec
            var collection = new PostmanSpecCollection();

            var collectionId = Guid.NewGuid().ToString();
            collection.Id = collectionId;
            collection.Name = documentation.Title;
            collection.Description = documentation.Description;
            collection.Timestamp = DateTime.UtcNow.ToUnixTimeMs();

            PopulateRequests(documentation, collection);

            return collection;
        }

        private void PopulateRequests(ApiDocumentation documentation, PostmanSpecCollection collection)
        {
            var requests = new List<PostmanSpecRequest>();

            foreach (var resource in documentation.Resources)
            {
                var folder = CreateFolder(resource);

                var contentType = GetContentTypes(resource);

                var data = GetPostmanSpecData(resource);

                // Get any pathVariables that are present (variable place holders in route)
                var pathVariableNames = resource.RelativePath.HasPathParams();
                string relativePath = pathVariableNames.Aggregate(resource.RelativePath,
                    (current, match) => current.Replace($"{{{match}}}", $":{match}"));

                // Add path vars regardless of verb
                Dictionary<string, string> pathVars = GetPathVariabless(data, pathVariableNames);

                // Generate a collection request per verb/resource combo
                foreach (var verb in resource.Verbs)
                {
                    var hasRequestBody = verb.HasRequestBody();
                    string verbPath = relativePath;
                    if (!hasRequestBody)
                        verbPath = ProcessQueryStringParams(data, pathVariableNames, relativePath);

                    var requestId = Guid.NewGuid().ToString();
                    var request = new PostmanSpecRequest
                    {
                        Id = requestId,
                        Url = documentation.ApiBaseUrl.CombineWith(verbPath),
                        Method = verb,
                        Time = DateTime.UtcNow.ToUnixTimeMs(),
                        Name = resource.Title,
                        Description = resource.Description,
                        CollectionId = collection.Id,
                        Headers = $"Accept: {contentType}",
                        FolderId = folder.Id,
                        PathVariables = pathVars
                    };

                    request.Data = hasRequestBody
                                       ? data.Where(t =>
                                                    !pathVariableNames.Contains(t.Key, StringComparer.OrdinalIgnoreCase)).ToList()
                                       : null;

                    folder.RequestIds.Add(requestId);
                    requests.Add(request);
                }

                collection.Folders.Add(folder);
            }

            collection.Requests = requests.ToArray();
        }

        private static PostmanFolder CreateFolder(ApiResourceDocumentation resource)
        {
            var folderId = Guid.NewGuid().ToString();
            var folder = new PostmanFolder
            {
                Name = resource.Title,
                Description = $"DTO Folder: {resource.Title}",
                Id = folderId,
                RequestIds = new List<string>(resource.Verbs.Length)
            };
            return folder;
        }

        private static Dictionary<string, string> GetPathVariabless(List<PostmanSpecData> data, List<string> pathVariables)
        {
            var pathVars = data.Where(t => pathVariables.Contains(t.Key, StringComparer.OrdinalIgnoreCase))
                               .ToDictionary(k => k.Key, v => v.Value);
            return pathVars;
        }

        private static string GetContentTypes(ApiResourceDocumentation resource)
        {
            // TODO Tighten up the logic used here
            var contentType = resource.ContentTypes.Contains(MimeTypes.Json)
                                  ? MimeTypes.Json
                                  : resource.ContentTypes.First();
            return contentType;
        }

        private static string ProcessQueryStringParams(List<PostmanSpecData> data, List<string> pathVariables, string relativePath)
        {
            var queryParams = data.Where(d => !pathVariables.Contains(d.Key, StringComparer.OrdinalIgnoreCase));
            return queryParams.Aggregate(relativePath,
                (current, queryParam) => current.AddQueryParam(queryParam.Key, queryParam.Value));
        }

        private List<PostmanSpecData> GetPostmanSpecData(ApiResourceDocumentation resource)
        {
            var data = resource.Properties.Select(r =>
            {
                var type = FriendlyTypeNames.SafeGet(r.ClrType.Name, r.ClrType.Name);
                return new PostmanSpecData
                {
                    Enabled = true,
                    Key = r.Title,
                    Type = type,
                    Value = $"val-{type}"
                };
            }).ToList();
            return data;
        }
    }
}