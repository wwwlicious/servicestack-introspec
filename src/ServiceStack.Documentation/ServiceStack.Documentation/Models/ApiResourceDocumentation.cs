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

        // From IApiRequest
        public ApiAction[] Actions { get; set; }
        public ApiResourceType ReturnType { get; set; }

        // From IApiMetadata by way of IApiRequest
        public string Category { get; set; }
        public string[] Tags { get; set; }

        /*public string[] Verbs { get; set; }
        public StatusCode[] StatusCodes { get; set; }
        public string[] ContentTypes { get; set; }
        public string RelativePath { get; set; }*/

        // From ISecured by way of IApiRequest
        // public ApiSecurity Security { get; set; }
    }

    // Figure out a better name
    public class ApiAction : IApiAction
    {
        public ApiSecurity Security { get; set; }

        public string Verb { get; set; }

        public StatusCode[] StatusCodes { get; set; }

        public string[] ContentTypes { get; set; }

        public string RelativePath { get; set; }

        //public ApiResourceType ReturnType { get; set; }
    }

    public interface IApiAction : ISecured
    {
        string[] ContentTypes { get; set; }
        string RelativePath { get; set; }
        //ApiResourceType ReturnType { get; set; }
        StatusCode[] StatusCodes { get; set; }
        string Verb { get; set; }

        // Restrictions
    }
}