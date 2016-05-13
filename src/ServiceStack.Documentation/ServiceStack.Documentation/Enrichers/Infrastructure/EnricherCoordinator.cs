namespace ServiceStack.Documentation.Enrichers.Infrastructure
{
    using Host;
    using Interfaces;
    using Models;

    /// <summary>
    /// Class is responsible for coordinating calls to the various enrichers in the 
    /// </summary>
    public class EnricherCoordinator : IApiResourceEnricher
    {
        private readonly ResourceEnricherManager resourceEnricherManager;
        private readonly ResponseEnricherManager responseEnricherManager;

        public EnricherCoordinator(IEnrich enricher)
        {
            resourceEnricherManager = new ResourceEnricherManager(enricher as IResourceEnricher, enricher as IPropertyEnricher);
            responseEnricherManager = new ResponseEnricherManager(enricher as IResponseEnricher, resourceEnricherManager.EnrichResource);
        }

        /// <summary>
        /// Enrich supplied resourceSpecification object with details from Operation object
        /// </summary>
        /// <param name="resourceSpecification">The object to be enriched</param>
        /// <param name="operation">Details of operation to use for enrichment</param>
        public void Enrich(ApiResourceDocumentation resourceSpecification, Operation operation)
        {
            resourceEnricherManager.EnrichResource(resourceSpecification, operation);
            responseEnricherManager.EnrichResponse(resourceSpecification, operation);
        }
    }
}