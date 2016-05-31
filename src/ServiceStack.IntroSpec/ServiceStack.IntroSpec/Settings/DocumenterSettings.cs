// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Settings
{
    using System.Collections.Generic;
    using System.Reflection;
    using Models;

    // Based on JsConfig and JsConfigScope from ServiceStack
    public static class DocumenterSettings
    {
        /// <summary>
        /// The verbs used in the event of 'Any' being found for DTO. Default: GET, POST
        /// </summary>
        public static IEnumerable<string> ReplacementVerbs
        {
            get { return DocumenterSettingsScope.Current.ReplacementVerbs; }
            set { DocumenterSettingsScope.Current.ReplacementVerbs = value; }
        }

        /// <summary>
        /// The assemblies to containing implementations of RequestSpec and TypeSpec. Default: EntryAssembly
        /// </summary>
        public static IEnumerable<Assembly> Assemblies
        {
            get { return DocumenterSettingsScope.Current.Assemblies; }
            set { DocumenterSettingsScope.Current.Assemblies = value; }
        }

        /// <summary>
        /// The EnrichmentStrategy to use when populating collections. Default: Union
        /// </summary>
        public static EnrichmentStrategy CollectionStrategy
        {
            get { return DocumenterSettingsScope.Current.CollectionStrategy; }
            set { DocumenterSettingsScope.Current.CollectionStrategy = value; }
        }

        /// <summary>
        /// The default notes to be set for a request/response object.
        /// </summary>
        public static string FallbackNotes
        {
            get { return DocumenterSettingsScope.Current.FallbackNotes; }
            set { DocumenterSettingsScope.Current.FallbackNotes = value; }
        }

        /// <summary>
        /// The default status codes to be set for a request/response object.
        /// </summary>
        public static IEnumerable<StatusCode> DefaultStatusCodes
        {
            get { return DocumenterSettingsScope.Current.DefaultStatusCodes; }
            set { DocumenterSettingsScope.Current.DefaultStatusCodes = value; }
        }

        /// <summary>
        /// The default category to be set for a request/response object.
        /// </summary>
        public static string FallbackCategory
        {
            get { return DocumenterSettingsScope.Current.FallbackCategory; }
            set { DocumenterSettingsScope.Current.FallbackCategory = value; }
        }

        /// <summary>
        /// The default tags to be set for a request/response object.
        /// </summary>
        public static IEnumerable<string> DefaultTags
        {
            get { return DocumenterSettingsScope.Current.DefaultTags; }
            set { DocumenterSettingsScope.Current.DefaultTags = value; }
        }

        /// <summary>
        /// The default content types to be set for a request/response object.
        /// </summary>
        public static IEnumerable<string> DefaultContentTypes
        {
            get { return DocumenterSettingsScope.Current.DefaultContentTypes; }
            set { DocumenterSettingsScope.Current.DefaultContentTypes = value; }
        }

        /// <summary>
        /// The default notes to be set for a route.
        /// </summary>
        public static string FallbackRouteNotes
        {
            get { return DocumenterSettingsScope.Current.FallbackRouteNotes; }
            set { DocumenterSettingsScope.Current.FallbackRouteNotes = value; }
        }

        public static DocumenterSettingsScope BeginScope() => new DocumenterSettingsScope();

        public static DocumenterSettingsScope With(
            IEnumerable<string> replacementVerbs = null,
            IEnumerable<Assembly> assemblies = null,
            EnrichmentStrategy collectionStrategy = EnrichmentStrategy.Union,
            string fallbackNotes = null,
            IEnumerable<StatusCode> defaultStatusCodes = null,
            string fallbackCategory = null,
            IEnumerable<string> defaultTags = null,
            IEnumerable<string> defaultContentTypes = null,
            string fallbackRouteNotes = null)
        {
            return new DocumenterSettingsScope
            {
                ReplacementVerbs = replacementVerbs,
                CollectionStrategy = collectionStrategy,
                Assemblies = assemblies,
                FallbackNotes = fallbackNotes,
                DefaultStatusCodes = defaultStatusCodes,
                FallbackCategory = fallbackCategory,
                DefaultTags = defaultTags,
                DefaultContentTypes = defaultContentTypes,
                FallbackRouteNotes = fallbackRouteNotes
            };
        }
    }
}