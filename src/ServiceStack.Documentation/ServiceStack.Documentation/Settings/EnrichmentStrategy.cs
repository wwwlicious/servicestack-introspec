// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Settings
{
    public enum EnrichmentStrategy
    {
        /// <summary>
        /// Each enricher will be called and result will be union of all
        /// </summary>
        Union = 0,

        /// <summary>
        /// Lower priority enrichers will only be called if property null/empty
        /// </summary>
        SetIfEmpty = 1
    }
}