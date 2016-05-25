// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using DataAnnotations;
    using Extensions;
    using Linq;
    using Models;

    public class ApiSpecService : Service
    {
        public object Get(SpecRequest request)
        {
            // Get the filtered documentation to return
            var documentation = GetApiDocumentation(request);

            // TODO Filter out by auth permissions
            return new SpecResponse { ApiDocumentation = documentation };
        }

        private static ApiDocumentation GetApiDocumentation(SpecRequest request)
        {
            var apiSpecFeature = HostContext.GetPlugin<ApiSpecFeature>();
            var documentation = apiSpecFeature.Documentation;

            var filter = GetFilter(request);
            if (filter != null)
                documentation = documentation.CreateCopy(filter.Compile());
            return documentation;
        }

        private static Expression<Func<ApiResourceDocumentation, bool>> GetFilter(SpecRequest request)
        {
            bool hasFilter = false;

            // The true predicate is just a starter to use as base
            var predicate = PredicateBuilder.True<ApiResourceDocumentation>();
            if (!request.DtoName.IsNullOrEmpty())
            {
                hasFilter = true;
                predicate = predicate.And(doc => request.DtoName.Contains(doc.Title, StringComparer.OrdinalIgnoreCase));
            }

            if (!request.Tags.IsNullOrEmpty())
            {
                hasFilter = true;
                predicate = predicate.And(doc => request.Tags.Any(t => doc.Tags.Contains(t, StringComparer.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(request.Category))
            {
                hasFilter = true;
                predicate = predicate.And(doc => string.Equals(request.Category, doc.Category, StringComparison.OrdinalIgnoreCase));
            }

            return hasFilter ? predicate : null;
        }
    }

    [Route(Constants.SpecUri)]
    [Exclude(Feature.Metadata | Feature.ServiceDiscovery)]
    public class SpecRequest : IReturn<SpecResponse>
    {
        public string[] DtoName { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
    }

    public class SpecResponse
    {
        public ApiDocumentation ApiDocumentation { get; set; }
    }
}
