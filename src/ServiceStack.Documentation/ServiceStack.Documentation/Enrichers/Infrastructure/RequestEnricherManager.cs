// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

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
    public class RequestEnricherManager
    {
        private readonly IRequestEnricher requestEnricher;
        private readonly Action<IApiResourceType, Operation> enrichResource;

        public RequestEnricherManager(IRequestEnricher requestEnricher, Action<IApiResourceType, Operation> enrichResource)
        {
            this.requestEnricher = requestEnricher;
            this.enrichResource = enrichResource;
        }

        /// <summary>
        /// Enrich supplied IApiResponseStatus object with details in operation
        /// </summary>
        /// <param name="response">The object to be enriched</param>
        /// <param name="operation">Details of operation to use for enrichment</param>
        public void EnrichRequest(IApiResponseStatus response, Operation operation)
        {
            bool unionCollections = DocumenterSettings.CollectionStrategy == EnrichmentStrategy.Union;

            if (requestEnricher != null)
            {
                // The object that has ResponseStatus is built up from request object
                response.Verbs = unionCollections
                    ? response.Verbs.SafeUnion(() => requestEnricher.GetVerbs(operation))
                    : response.Verbs.GetIfNullOrEmpty(() => requestEnricher.GetVerbs(operation));

                response.StatusCodes = unionCollections
                    ? response.StatusCodes.SafeUnion(() => requestEnricher.GetStatusCodes(operation))
                    : response.StatusCodes.GetIfNullOrEmpty(() => requestEnricher.GetStatusCodes(operation));

                response.RelativePath =
                    response.RelativePath.GetIfNullOrEmpty(() => requestEnricher.GetRelativePath(operation));

                response.Category = response.Category.GetIfNullOrEmpty(() => requestEnricher.GetCategory(operation));

                response.Tags = unionCollections
                    ? response.Tags.SafeUnion(() => requestEnricher.GetTags(operation))
                    : response.Tags.GetIfNullOrEmpty(() => requestEnricher.GetTags(operation));
            }

            response.ReturnType = response.ReturnType.GetIfNull(() => new ApiResourceType());

            enrichResource(response.ReturnType, operation);
        }
    }
}