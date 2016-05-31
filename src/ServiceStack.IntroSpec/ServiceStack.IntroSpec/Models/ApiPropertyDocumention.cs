// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
    using System;

    /// <summary>
    /// Each request property within a DTO
    /// </summary>
    public class ApiPropertyDocumention : IApiSpec
    {
        // Set when instantiated
        public string Id { get; set; }
        public Type ClrType { get; set; }

        // From IApiSpec
        private string title;
        public string Title
        {
            get { return title ?? Id; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    title = value; 
            }
        }
        public string Description { get; set; }
        public string Notes { get; set; }
    
        public string ParamType { get; set; } // used to denote if the param is request querystring or body restricted
        public bool? IsRequired { get; set; }
        public string[] ExternalLinks { get; set; }
        public bool? AllowMultiple { get; set; }

        public IApiResourceType EmbeddedResource { get; set; }

        public PropertyConstraint Contraints { get; set; }
    }
}