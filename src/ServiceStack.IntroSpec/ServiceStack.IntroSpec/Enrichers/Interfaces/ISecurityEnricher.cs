// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers.Interfaces
{
    using Host;
    using Models;

    /// <summary>
    /// Methods for populating security information for an action
    /// </summary>
    public interface ISecurityEnricher : IEnrich
    {
        ApiSecurity GetSecurity(Operation operation, string verb);
    }
}