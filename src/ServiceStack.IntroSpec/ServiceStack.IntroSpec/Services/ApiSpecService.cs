// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Services
{
    using DTO;
    using Extensions;

#if !DEBUG
    [CacheResponse(MaxAge = 300, Duration = 600)]
#endif
    public class ApiSpecService : Service
    {
        private readonly IApiDocumentationProvider documentationProvider;

        public ApiSpecService(IApiDocumentationProvider documentationProvider)
        {
            documentationProvider.ThrowIfNull(nameof(documentationProvider));
            this.documentationProvider = documentationProvider;
        }

        public object Get(SpecRequest request)
        {
            // Get the filtered documentation to return
            var documentation = documentationProvider.GetApiDocumentation().Filter(request);

            // TODO Filter out by auth permissions
            return new SpecResponse { ApiDocumentation = documentation };
        }
    }
}
