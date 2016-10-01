// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Postman.Services
{
    using DTO;
    using Extensions;
    using IntroSpec.Services;

#if !DEBUG
    [CacheResponse(MaxAge = 300, Duration = 600)]
#endif
    public class ApiSpecPostmanService : Service
    {
        private readonly IApiDocumentationProvider documentationProvider;

        public ApiSpecPostmanService(IApiDocumentationProvider documentationProvider)
        {
            documentationProvider.ThrowIfNull(nameof(documentationProvider));
            this.documentationProvider = documentationProvider;
        }

        [AddHeader(ContentType = MimeTypes.Json)]
        public object Get(PostmanRequest request)
        {
            // Get the filtered documentation object
            var documentation = documentationProvider.GetApiDocumentation(Request.GetApplicationUrl()).Filter(request);
            
            var postmanGenerator = new PostmanCollectionGenerator();
            var collection = postmanGenerator.Generate(documentation);
            return collection;
        }
    }
}
 