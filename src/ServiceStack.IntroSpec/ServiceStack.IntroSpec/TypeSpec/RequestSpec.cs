// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.TypeSpec
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using Html;
    using Models;

    /// <summary>
    /// Documentation class for a request DTO, including Verbs, status codes etc
    /// </summary>
    /// <typeparam name="T">DTO Type that is being decorated</typeparam>
    public abstract class RequestSpec<T> : TypeSpec<T>, IApiRequestSpec
        where T : class, new()
    {
        public Dictionary<string, List<StatusCode>> StatusCodes { get; }
        public List<string> Tags { get; }
        public Dictionary<string, string> RouteNotes { get; }
        public Dictionary<string, List<string>> ContentTypes { get; }

        public string Category { get; protected set; }

        /// <summary>
        /// Set tags for this DTO
        /// </summary>
        /// <param name="tags">List of tags to set for this DTO</param>
        protected void AddTags(params string[] tags) => Tags.AddRange(tags);

        /// <summary>
        /// Set status codes for this DTO that are available across all verbs
        /// </summary>
        /// <param name="statusCodes">List of status codes to set for this DTO</param>
        protected void AddStatusCodes(params StatusCode[] statusCodes)
            => StatusCodes.UpdateList(Constants.GlobalSettingsKey, statusCodes);

        /// <summary>
        /// Set content-types for this DTO that are available across all verbs
        /// </summary>
        /// <param name="contentTypes">List of content types to set for this DTO</param>
        protected void AddContentTypes(params string[] contentTypes)
            => ContentTypes.UpdateList(Constants.GlobalSettingsKey, contentTypes);

        /// <summary>
        /// Set route notes for this DTO for all verbs
        /// </summary>
        /// <param name="notes">Notes to set for this DTO</param>
        protected void AddRouteNotes(string notes)
            => RouteNotes[Constants.GlobalSettingsKey] = notes;

        /// <summary>
        /// Set status codes for this DTO that may be returned for specified verb
        /// </summary>
        /// <param name="verb">The verb to set status codes for</param>
        /// <param name="statusCodes">List of status codes  to set for this DTO</param>
        protected void AddStatusCodes(HttpVerbs verb, params StatusCode[] statusCodes)
            => StatusCodes.UpdateList(verb.ToString(), statusCodes);

        /// <summary>
        /// Set content-types for this DTO that are available for specified verb
        /// </summary>
        /// <param name="verb">The verb to set content types for</param>
        /// <param name="contentTypes">List of content types to set for this DTO</param>
        protected void AddContentTypes(HttpVerbs verb, params string[] contentTypes)
            => ContentTypes.UpdateList(verb.ToString(), contentTypes);

        /// <summary>
        /// Set route notes for this DTO for specified verb
        /// </summary>
        /// <param name="verb">The verb to set notes for</param>
        /// <param name="notes">Notes to set for this DTO</param>
        protected void AddRouteNotes(HttpVerbs verb, string notes)
            => RouteNotes[verb.ToString()] = notes;

        protected RequestSpec()
        {
            StatusCodes = new Dictionary<string, List<StatusCode>>(StringComparer.OrdinalIgnoreCase);
            RouteNotes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Tags = new List<string>();
            ContentTypes = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        }
    }
}