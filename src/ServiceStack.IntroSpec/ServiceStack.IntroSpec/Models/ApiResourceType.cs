// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
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
        public ApiPropertyDocumention[] Properties { get; set; }
        public bool? AllowMultiple { get; set; }

        public static ApiResourceType Create(string typeName) => new ApiResourceType { TypeName = typeName };
    }
}