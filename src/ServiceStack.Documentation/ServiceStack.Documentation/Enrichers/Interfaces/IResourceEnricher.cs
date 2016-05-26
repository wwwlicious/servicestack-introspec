// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers.Interfaces
{
    using System;

    /// <summary>
    /// Methods for populating a resource object
    /// </summary>
    public interface IResourceEnricher : IEnrich
    {
        string GetTitle(Type type);
        string GetDescription(Type type);
        string GetNotes(Type type);
    }
}