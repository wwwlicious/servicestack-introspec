namespace ServiceStack.Documentation.Enrichers.Infrastructure
{
    using System;
    using Extensions;
    using Host;
    using Interfaces;
    using Models;
    using Settings;

    /// <summary>
    /// Manages default logic for enriching response objects
    /// </summary>
    public class ResponseEnricherManager
    {
        private readonly IResponseEnricher responseEnricher;
        private readonly Action<IApiResourceType, Operation> enrichResource;

        public ResponseEnricherManager(IResponseEnricher responseEnricher, Action<IApiResourceType, Operation> enrichResource)
        {
            this.responseEnricher = responseEnricher;
            this.enrichResource = enrichResource;
        }

        /// <summary>
        /// Enrich supplied IApiResponseStatus object with details in operation
        /// </summary>
        /// <param name="response">The object to be enriched</param>
        /// <param name="operation">Details of operation to use for enrichment</param>
        public void EnrichResponse(IApiResponseStatus response, Operation operation)
        {
            bool unionCollections = DocumenterSettings.CollectionStrategy == EnrichmentStrategy.Union;

            if (responseEnricher != null)
            {
                // The object that has ResponseStatus is built up from request object
                response.Verbs = unionCollections
                    ? response.Verbs.SafeUnion(() => responseEnricher.GetVerbs(operation))
                    : response.Verbs.GetIfNullOrEmpty(() => responseEnricher.GetVerbs(operation));

                response.StatusCodes = unionCollections
                    ? response.StatusCodes.SafeUnion(() => responseEnricher.GetStatusCodes(operation))
                    : response.StatusCodes.GetIfNullOrEmpty(() => responseEnricher.GetStatusCodes(operation));
            }

            response.ReturnType = response.ReturnType.GetIfNull(() => new ApiResourceType());

            enrichResource(response.ReturnType, operation);
        }
    }
}