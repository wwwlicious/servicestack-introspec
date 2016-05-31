// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Services
{
    using System.Linq;
    using DTO;

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

            return SpecMetadataResponse.Create(
                documentation.Resources.Select(r => r.TypeName).Distinct().ToArray(),
                documentation.Resources.Select(r => r.Category).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToArray(),
                documentation.Resources.SelectMany(r => r.Tags ?? new string[0]).Distinct().ToArray()
                );
        }
    }
}
