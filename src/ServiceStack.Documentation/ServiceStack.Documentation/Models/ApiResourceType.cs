// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Models
{
    public class ApiResourceType : IApiResourceType
    {
        // These will need key'd by something (Type?) for finding + enriching
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
        public ApiPropertyDocumention[] Properties { get; set; }
    }
}