// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
    using System.Diagnostics;

    /// <summary>
    /// A single API resource (DTO)
    /// </summary>
    [DebuggerDisplay("{Title}")]
    public class ApiResourceDocumentation : IApiResourceType, IApiRequest
    {
        // Set when instantiating
        public string TypeName { get; set; }

        // From IApiResourceType
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
        public ApiPropertyDocumention[] Properties { get; set; }
        public bool? AllowMultiple { get; set; }
        public bool? HasValidator { get; set; }
        public bool? IsCollection { get; set; }

        // From IApiRequest
        public ApiAction[] Actions { get; set; }
        public ApiResourceType ReturnType { get; set; }

        // From IApiMetadata by way of IApiRequest
        public string Category { get; set; }
        public string[] Tags { get; set; }
    }
}