// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Utilities
{
    using System.Collections.Generic;
    using System.Linq;
    using DTO;
    using Extensions;
    using Models;

    public static class ApiSpecMetadataUtilities
    {
        public static SpecMetadataResponse GenerateResponse(ApiDocumentation doc)
        {
            if (doc == null || doc.Resources.IsNullOrEmpty())
                return null;

            var categories = GetCategoryLookup(doc);
            var tags = GetTagsLookup(doc);

            var response = new SpecMetadataResponse
            {
                Categories =
                    categories.Select(
                        grouping => new DtoGrouping { Key = grouping.Key, DtoNames = grouping.Select(v => v) }),
                Tags =
                    tags.Select(
                        grouping => new DtoGrouping { Key = grouping.Key, DtoNames = grouping.SelectMany(v => v) }),
                DtoNames = doc.Resources.Select(d => d.TypeName)
            };

            return response;
        }

        private static ILookup<string, IEnumerable<string>> GetTagsLookup(ApiDocumentation doc)
        {
            var withTags = doc.Resources.Where(r => r.Tags != null).ToList();
            var tags = withTags.SelectMany(r => r.Tags)
                               .Distinct()
                               .ToLookup(k => k, v => withTags.Where(r => r.Tags.Contains(v))
                                                              .Select(r => r.TypeName));
            return tags;
        }

        private static ILookup<string, string> GetCategoryLookup(ApiDocumentation doc)
        {
            var cats = doc.Resources.Where(r => !string.IsNullOrWhiteSpace(r.Category))
                          .ToLookup(k => k.Category, v => v.TypeName);
            return cats;
        }
    }
}