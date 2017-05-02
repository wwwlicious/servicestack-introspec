// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
    using System;
    using System.Linq;

    /// <summary>
    /// General top level model with API (Service) wide vars
    /// </summary>
    public class ApiDocumentation
    {
        public string Title { get; set; }

        public string ApiVersion { get; set; }

        public string ApiBaseUrl { get; set; }

        public string Description { get; set; }

        public string TermsOfService { get; set; }

        public string Licence { get; set; }

        public string LicenceUrl { get; set; }

        public ApiContact Contact { get; set; }

        public ApiResourceDocumentation[] Resources { get; set; }

        public ApiPlugin[] Plugins { get; set; }

        public ApiDocumentation CreateCopy(Func<ApiResourceDocumentation, bool> resourcesFilter)
        {
            var apiDocumentation = MemberwiseClone() as ApiDocumentation;
            apiDocumentation.Resources = Resources.Where(resourcesFilter).ToArray();
            return apiDocumentation;
        }
    }
}