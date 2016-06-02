// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using DTO;
    using Linq;
    using Models;

    public static class ApiDocumentationExtensions
    {
        public static ApiDocumentation Filter(this ApiDocumentation documentation, IFilterableSpecRequest request)
        {
            request.ThrowIfNull(nameof(request));
            documentation.ThrowIfNull(nameof(documentation));

            var filter = GetFilter(request);
            if (filter != null)
                documentation = documentation.CreateCopy(filter.Compile());
            return documentation;
        }

        private static Expression<Func<ApiResourceDocumentation, bool>> GetFilter(IFilterableSpecRequest request)
        {
            bool hasFilter = false;

            // The true predicate is just a starter to use as base
            var predicate = PredicateBuilder.True<ApiResourceDocumentation>();
            if (!request.DtoNames.IsNullOrEmpty())
            {
                hasFilter = true;
                predicate = predicate.And(doc => request.DtoNames.Contains(doc.TypeName, StringComparer.OrdinalIgnoreCase));
            }

            if (!request.Tags.IsNullOrEmpty())
            {
                hasFilter = true;
                predicate = predicate.And(doc => request.Tags.Any(t => doc.Tags.Contains(t, StringComparer.OrdinalIgnoreCase)));
            }

            if (!request.Categories.IsNullOrEmpty())
            {
                hasFilter = true;
                predicate = predicate.And(doc => request.Categories.Contains(doc.Category, StringComparer.OrdinalIgnoreCase));
            }

            return hasFilter ? predicate : null;
        }
    }
}