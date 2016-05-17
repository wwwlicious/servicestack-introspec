// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enrichers.Interfaces;
    using Host;
    using Logging;
    using Models;
    using Settings;

    public class ApiDocumentationGenerator : IApiDocumentationGenerator
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ApiDocumentationGenerator));
        private readonly Func<IEnumerable<IApiResourceEnricher>> getEnrichers;

        public ApiDocumentationGenerator(Func<IEnumerable<IApiResourceEnricher>> getEnrichers)
        {
            this.getEnrichers = getEnrichers;
        }

        // List of IApiSpecPopulaters to iterate. Could have the XML doc outside of populaters to save doing it
        public ApiDocumentation GenerateDocumentation(IEnumerable<Operation> operations, IAppHost appHost, ApiSpecConfig config)
        {
            if (appHost.Config?.WebHostUrl == null)
                throw new ArgumentException("appHost.Config.WebHostUrl must be set to use the ApiDocumentationGenerator. This is used to generate a unique identifier.");
            
            var apiDoc = new ApiDocumentation
            {
                Title = (appHost as ServiceStackHost)?.ServiceName,
                ApiVersion = appHost.Config?.ApiVersion,
                ApiBaseUrl = appHost.Config.WebHostUrl,
                Contact = config.Contact,
                Description = config.Description
            };

            if (config.LicenseUrl != null)
                apiDoc.LicenceUrl = config.LicenseUrl.ToString();

            var resourceDocs = GenerateResourceDocumentation(operations);
            apiDoc.Resources = resourceDocs.ToArray();

            return apiDoc;
        }

        private IEnumerable<ApiResourceDocumentation> GenerateResourceDocumentation(IEnumerable<Operation> operations)
        {
            var resourceDocs = new List<ApiResourceDocumentation>();

            var enrichers = getEnrichers().ToList();

            foreach (var operation in operations ?? Enumerable.Empty<Operation>())
            {
                log.Debug($"Found type: {operation.RequestType}");

                var resourceDoc = new ApiResourceDocumentation();

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