// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers.Interfaces
{
    using Host;
    using Models;

    public interface IActionEnricher : IEnrich
    {
        string[] GetContentTypes(Operation operation, string verb);
        string[] GetRelativePaths(Operation operation, string verb);
        StatusCode[] GetStatusCodes(Operation operation, string verb);
    }
}