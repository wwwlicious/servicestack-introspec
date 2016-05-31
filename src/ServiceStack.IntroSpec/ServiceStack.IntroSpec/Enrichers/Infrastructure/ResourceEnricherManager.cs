// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers.Infrastructure
{
    using System;
    using Extensions;
    using Host;
    using Interfaces;
    using Models;

    /// <summary>
    /// Manages default logic for enriching resources
    /// </summary>
    public class ResourceEnricherManager
    {
        private readonly IResourceEnricher resourceEnricher;
        private readonly PropertyEnricherManager propertyEnricherManager;

        public ResourceEnricherManager(IResourceEnricher resourceEnricher, IPropertyEnricher propertyEnricher)
        {
            this.resourceEnricher = resourceEnricher;
            propertyEnricherManager = new PropertyEnricherManager(propertyEnricher, EnrichResource);
        }

        /// <summary>
        /// Enrich supplied IApiResourceType object with details from Operation object
        /// </summary>
        /// <param name="resource">The object to be enriched</param>
        /// <param name="operation">Details of operation to use for enrichment</param>
        public void EnrichResource(IApiResourceType resource, Operation operation)
        {
            // The object that has ResponseStatus is built up from request object
            var type = resource is IApiRequest ? operation.RequestType : operation.ResponseType;

            if (type == null) return;

            EnrichResource(resource, type);
        }

        private void EnrichResource(IApiResourceType resource, Type type)
        {
            // The object that has ResponseStatus is built up from request object
            if (resourceEnricher != null)
            {
                if (resource.Title == resource.TypeName)
                    resource.Title = resourceEnricher.GetTitle(type);

                resource.Description = resource.Description.GetIfNullOrEmpty(() => resourceEnricher.GetDescription(type));
                resource.Notes = resource.Notes.GetIfNullOrEmpty(() => resourceEnricher.GetNotes(type));
            }

            resource.Properties = propertyEnricherManager.EnrichParameters(resource.Properties, type);
        }
    }
}