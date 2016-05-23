// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers
{
    using System;
    using System.Linq;
    using Host;
    using Interfaces;
    using Models;
    using Settings;

    public class FallbackEnricher : IResourceEnricher, IRequestEnricher
    {
        public string GetTitle(Type type) => null;

        public string GetDescription(Type type) => null;

        public string GetNotes(Type type) => DocumenterSettings.FallbackNotes;

        public string[] GetVerbs(Operation operation) => DocumenterSettings.DefaultVerbs?.ToArray();

        public StatusCode[] GetStatusCodes(Operation operation) => DocumenterSettings.DefaultStatusCodes?.ToArray();

        public string GetRelativePath(Operation operation) => null;

        public string GetCategory(Operation operation) => DocumenterSettings.FallbackCategory;

        public string[] GetTags(Operation operation) => DocumenterSettings.DefaultTags?.ToArray();
    }
}
