// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers
{
    using System;
    using System.Linq;
    using Host;
    using Interfaces;
    using Models;
    using Settings;

    /// <summary>
    /// Enricher that will use global settings properties to enrich object
    /// </summary>
    public class FallbackEnricher : IResourceEnricher, IRequestEnricher, IActionEnricher
    {
        public string GetTitle(Type type) => null;

        public string GetDescription(Type type) => null;

        public string GetNotes(Type type) => DocumenterSettings.FallbackNotes;

        public string[] GetContentTypes(Operation operation, string verb) => DocumenterSettings.DefaultContentTypes?.ToArray();

        public StatusCode[] GetStatusCodes(Operation operation, string verb) => DocumenterSettings.DefaultStatusCodes?.ToArray();

        public string GetNotes(Operation operation, string verb) => DocumenterSettings.FallbackRouteNotes;

        public string[] GetRelativePaths(Operation operation, string verb) => null;

        public string GetCategory(Operation operation) => DocumenterSettings.FallbackCategory;

        public string[] GetTags(Operation operation) => DocumenterSettings.DefaultTags?.ToArray();
    }
}
