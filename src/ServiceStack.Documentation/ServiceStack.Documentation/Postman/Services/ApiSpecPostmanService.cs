﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Postman.Services
{
    using Documentation.Services;
    using DTO;
    using Extensions;

    public class ApiSpecPostmanService : IService
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
            var documentation = documentationProvider.GetApiDocumentation().Filter(request);
            
            // TODO Look at the cookies that are in the current postman plugin
            var postmanGenerator = new PostmanCollectionGenerator();
            var collection = postmanGenerator.Generate(documentation);
            return collection;
        }
    }
}
 