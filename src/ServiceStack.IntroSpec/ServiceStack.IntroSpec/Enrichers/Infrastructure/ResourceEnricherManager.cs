// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers.Infrastructure
{
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
            var resourceModel = GetResourceModel(resource, operation);

            if (resourceModel?.ResourceType == null) return;

            EnrichResource(resource, resourceModel);
        }

        private static ResourceModel GetResourceModel(IApiResourceType resource, Operation operation)
            => resource is IApiRequest
                   ? new ResourceModel(operation.RequestType, true)
                   : new ResourceModel(operation.ResponseType, false);

        private void EnrichResource(IApiResourceType resource, ResourceModel resourceModel)
        {
            if (resourceEnricher != null)
            {
                var type = resourceModel.ResourceType;
                if (resource.Title == resource.TypeName)
                    resource.Title = resourceEnricher.GetTitle(type);

                resource.Description = resource.Description.GetIfNullOrEmpty(() => resourceEnricher.GetDescription(type));
                resource.Notes = resource.Notes.GetIfNullOrEmpty(() => resourceEnricher.GetNotes(type));
                if (resourceModel.IsRequest)
                {
                    resource.AllowMultiple =
                        resource.AllowMultiple.GetIfNoValue(
                            () => resourceEnricher.GetAllowMultiple(type));
                }
                else
                {
                    resource.IsCollection = type.IsCollection();
                }
            }

            resource.Properties = propertyEnricherManager.EnrichParameters(resource.Properties, resourceModel);
        }
    }
}