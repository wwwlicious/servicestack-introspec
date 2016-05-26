// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers.Infrastructure
{
    using Host;
    using Interfaces;
    using Models;

    /// <summary>
    /// Class is responsible for coordinating calls between the various enrichers
    /// </summary>
    public class EnricherCoordinator : IApiResourceEnricher
    {
        private readonly ResourceEnricherManager resourceEnricherManager;
        private readonly RequestEnricherManager requestEnricherManager;

        public EnricherCoordinator(IEnrich enricher)
        {
            resourceEnricherManager = new ResourceEnricherManager(enricher as IResourceEnricher, enricher as IPropertyEnricher);
            requestEnricherManager = new RequestEnricherManager(enricher as IRequestEnricher, resourceEnricherManager.EnrichResource);
        }

        /// <summary>
        /// Enrich supplied ApiResourceDocumentation object with details from Operation object
        /// </summary>
        /// <param name="resourceSpecification">The object to be enriched</param>
        /// <param name="operation">Details of operation to use for enrichment</param>
        public void Enrich(ApiResourceDocumentation resourceSpecification, Operation operation)
        {
            resourceEnricherManager.EnrichResource(resourceSpecification, operation);
            requestEnricherManager.EnrichRequest(resourceSpecification, operation);
        }
    }
}