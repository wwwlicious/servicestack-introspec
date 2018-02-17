// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
    using System;

    using ServiceStack.IntroSpec.Extensions;

    public class ApiResourceType : IApiResourceType
    {
        private string title;
        public string Title
        {
            get { return title ?? TypeName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    title = value;
            }
        }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string TypeName { get; set; }
        public ApiPropertyDocumentation[] Properties { get; set; }
        public bool? AllowMultiple { get; set; }

        public bool? IsCollection { get; set; }

        public static ApiResourceType Create(Type resourceType)
            =>
            new ApiResourceType
                {
                    TypeName = resourceType.GetDocumentationTypeName(),
                    IsCollection = resourceType.IsCollection()
                };
    }
}