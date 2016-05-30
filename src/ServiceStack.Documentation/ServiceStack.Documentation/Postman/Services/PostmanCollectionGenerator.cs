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
    using Logging;
    using Models;
    using Text;

    public class PostmanCollectionGenerator
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PostmanCollectionGenerator));

        public Dictionary<string, string> FriendlyTypeNames = new Dictionary<string, string>
        {
            {"Int32", "int"},
            {"Int64", "long"},
            {"Boolean", "bool"},
            {"String", "string"},
            {"Double", "double"},
            {"Single", "float"},
        };

        public PostmanSpecCollection Generate(ApiDocumentation documentation)
        {
            log.Debug($"Generating PostmanCollection for service {documentation.Title}");

            // Convert apiDocumentation to postman spec
            var collection = new PostmanSpecCollection();

            var collectionId = Guid.NewGuid().ToString();
            collection.Id = collectionId;
            collection.Name = documentation.Title;
            collection.Description = documentation.Description;
            collection.Timestamp = DateTime.UtcNow.ToUnixTimeMs();

            PopulateRequests(documentation, collection);

            log.Debug($"Generated PostmanRequest for resource {documentation.Title}");
            return collection;
        }

        private void PopulateRequests(ApiDocumentation documentation, PostmanSpecCollection collection)
        {
            if (documentation.Resources.IsNullOrEmpty())
            {
                log.Debug($"ApiDocumentatino for service {documentation.Title} has no resources");
                return;
            }

            var requests = new List<PostmanSpecRequest>();

            foreach (var resource in documentation.Resources)
            {
                log.Debug($"Generating PostmanRequest for resource {resource.Title}");

                var folder = CreateFolder(resource);

                var contentType = GetContentTypes(resource);

                var data = GetPostmanSpecData(resource);

                // Generate a collection request per verb/resource combo
                foreach (var action in resource.Actions)
                {
                    // Get any pathVariables that are present (variable place holders in route)
                    var untoucherRelativePath = action.RelativePaths.First();
                    var pathVariableNames = untoucherRelativePath.GetPathParams();
                    string relativePath = pathVariableNames.Aggregate(untoucherRelativePath,
                        (current, match) => current.Replace($"{{{match}}}", $":{match}")).EnsureEndsWith("/");

                    // Add path vars regardless of verb
                    Dictionary<string, string> pathVars = GetPathVariabless(data, pathVariableNames);

                    log.Debug($"Generating PostmanRequest for resource {resource.Title}, {action}");

                    var hasRequestBody = action.Verb.HasRequestBody();
                    string verbPath = relativePath;
                    if (!hasRequestBody)
                        verbPath = ProcessQueryStringParams(data, pathVariableNames, relativePath);

                    var requestId = Guid.NewGuid().ToString();
                    var request = new PostmanSpecRequest
                    {
                        Id = requestId,
                        Url = documentation.ApiBaseUrl.CombineWith(verbPath),
                        Method = action.Verb,
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
                                                    !pathVariableNames.Contains(t.Key, StringComparer.OrdinalIgnoreCase))
                                             .ToList()
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
                RequestIds = new List<string>(resource.Actions.Length)
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
            var contentTypes = resource.Actions.SelectMany(s => s.ContentTypes).ToList();
            var contentType = contentTypes.Contains(MimeTypes.Json)? MimeTypes.Json: contentTypes.First();
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
            if (resource.Properties.IsNullOrEmpty())
                return Enumerable.Empty<PostmanSpecData>().ToList();

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