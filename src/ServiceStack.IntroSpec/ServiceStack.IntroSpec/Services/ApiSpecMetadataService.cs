// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Services
{
    using DTO;
    using Utilities;

#if !DEBUG
    [CacheResponse(MaxAge = 300, Duration = 600)]
#endif
    public class ApiSpecMetadataService : IService
    {
        private readonly IApiDocumentationProvider documentationProvider;

        public ApiSpecMetadataService(IApiDocumentationProvider documentationProvider)
        {
            documentationProvider.ThrowIfNull(nameof(documentationProvider));
            this.documentationProvider = documentationProvider;
        }

        public object Get(SpecMetadataRequest request)
        {
            var documentation = documentationProvider.GetApiDocumentation();

            return ApiSpecMetadataUtilities.GenerateResponse(documentation);
        }
    }
}
