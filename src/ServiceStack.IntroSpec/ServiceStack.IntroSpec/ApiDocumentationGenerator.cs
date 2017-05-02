// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enrichers.Interfaces;
    using Host;
    using Logging;
    using Models;

    public class ApiDocumentationGenerator : IApiDocumentationGenerator
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ApiDocumentationGenerator));
        private readonly Func<IEnumerable<IApiResourceEnricher>> getEnrichers;

        public ApiDocumentationGenerator(Func<IEnumerable<IApiResourceEnricher>> getEnrichers)
        {
            this.getEnrichers = getEnrichers;
        }

        public ApiDocumentation GenerateDocumentation(IEnumerable<Operation> operations, IAppHost appHost, IApiSpecSettings settings)
        {
            var apiDoc = GetApiDocumentation(appHost, settings);

            var resourceDocs = GenerateResourceDocumentation(operations);
            apiDoc.Resources = resourceDocs.ToArray();

            return apiDoc;
        }

        private static ApiDocumentation GetApiDocumentation(IAppHost appHost, IApiSpecSettings settings)
        {
            var apiDoc = new ApiDocumentation
            {
                Title = (appHost as ServiceStackHost)?.ServiceName,
                ApiVersion = appHost.Config?.ApiVersion,
                Contact =
                    new ApiContact
                    {
                        Email = settings.ContactEmail,
                        Name = settings.ContactName,
                        Url = settings.ContactUrl.OriginalString
                    },
                Description = settings.Description,
                Plugins =
                    appHost.Plugins.Select(
                        x =>
                        new ApiPlugin
                        {
                            Name = x.GetType().FullName,
                            Version = x.GetType().Assembly.GetName().Version.ToString()
                        }).ToArray()
            };

            if (settings.LicenseUrl != null)
                apiDoc.LicenceUrl = settings.LicenseUrl.ToString();
            return apiDoc;
        }

        private IEnumerable<ApiResourceDocumentation> GenerateResourceDocumentation(IEnumerable<Operation> operations)
        {
            var resourceDocs = new List<ApiResourceDocumentation>();
            var enrichers = getEnrichers().ToList();

            foreach (var operation in operations ?? Enumerable.Empty<Operation>())
            {
                log.Debug($"Found type: {operation.RequestType}");

                var resourceDoc = new ApiResourceDocumentation { TypeName = operation.RequestType.Name };

                foreach (var apiSpecPopulater in enrichers)
                {
                    log.Debug($"Populating {operation.RequestType} with {apiSpecPopulater.GetType().Name}");
                    apiSpecPopulater.Enrich(resourceDoc, operation);
                }

                resourceDocs.Add(resourceDoc);
            }

            return resourceDocs;
        }
    }
}