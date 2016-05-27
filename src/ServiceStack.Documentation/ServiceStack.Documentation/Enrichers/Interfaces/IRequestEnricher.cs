// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers.Interfaces
{
    using Host;

    /// <summary>
    /// Methods for populating a request DTO class
    /// </summary>
    public interface IRequestEnricher : IEnrich
    {
        string GetCategory(Operation operation);
        string[] GetTags(Operation operation);
    }
}