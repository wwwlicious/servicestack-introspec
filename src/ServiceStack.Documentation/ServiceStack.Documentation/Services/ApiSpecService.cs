// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Services
{
    using DTO;

    public class ApiSpecService : Service
    {
        public object Get(SpecRequest request)
        {
            // Get the filtered documentation to return
            var documentation = ApiDocumentationFilter.GetApiDocumentation(request);

            // TODO Filter out by auth permissions
            return new SpecResponse { ApiDocumentation = documentation };
        }
    }
}
