// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers.Interfaces
{
    using Host;
    using Models;

    /// <summary>
    /// Basic operations for implementing a new documentation enricher
    /// </summary>
    public interface IApiResourceEnricher
    {
        /// <summary>
        /// Enriches ApiResourceDocumentation with information from specified Operation
        /// </summary>
        /// <param name="resourceSpecification"></param>
        /// <param name="operation"></param>
        void Enrich(ApiResourceDocumentation resourceSpecification, Operation operation);
    }
}