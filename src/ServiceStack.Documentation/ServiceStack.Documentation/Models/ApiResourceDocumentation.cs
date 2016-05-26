// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Models
{
    using System.Diagnostics;

    /// <summary>
    /// A single API resource (DTO)
    /// </summary>
    [DebuggerDisplay("{Title}")]
    public class ApiResourceDocumentation : IApiResourceType, IApiResponseStatus
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

        // From IApiResponseStatus
        public string[] Verbs { get; set; }
        public StatusCode[] StatusCodes { get; set; }
        public string[] ContentTypes { get; set; }
        public string RelativePath { get; set; }
        public ApiResourceType ReturnType { get; set; } // ReturnType w/params

        // From IApiMetadata by way of IApiResponseStatus
        public string Category { get; set; }
        public string[] Tags { get; set; }

        // From ISecured by way of IApiResponseStatus
        public ApiSecurity Security { get; set; }
    }
}