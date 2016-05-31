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
    /// Manages default logic for enriching request DTO objects
    /// </summary>
    public class RequestEnricherManager
    {
        private readonly IRequestEnricher requestEnricher;
        private readonly IActionEnricherManager actionEnricherManager;
        private readonly Action<IApiResourceType, Operation> enrichResource;

        public RequestEnricherManager(IRequestEnricher requestEnricher, IActionEnricherManager actionEnricherManager,
            Action<IApiResourceType, Operation> enrichResource)
        {
            this.requestEnricher = requestEnricher;
            this.enrichResource = enrichResource;

            this.actionEnricherManager = actionEnricherManager;
        }

        /// <summary>
        /// Enrich supplied IApiRequest object with details in operation
        /// </summary>
        /// <param name="request">The object to be enriched</param>
        /// <param name="operation">Details of operation to use for enrichment</param>
        public void EnrichRequest(IApiRequest request, Operation operation)
        {
            if (requestEnricher != null)
            {
                request.Category = request.Category.GetIfNullOrEmpty(() => requestEnricher.GetCategory(operation));
                request.Tags = request.Tags.GetBasedOnStrategy(() => requestEnricher.GetTags(operation));
            }

            request.ReturnType = request.ReturnType.GetIfNull(() => new ApiResourceType());

            enrichResource(request.ReturnType, operation);
            request.Actions = actionEnricherManager.EnrichActions(request.Actions, operation);
        }
    }
}