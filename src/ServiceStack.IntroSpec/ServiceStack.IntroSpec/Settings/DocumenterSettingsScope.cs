// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Models;

    // Based on JsConfig and JsConfigScope from ServiceStack
    public class DocumenterSettingsScope : IDisposable
    {
        /// <summary>
        /// The verbs used in the event of 'Any' being found for DTO. Default: GET, POST
        /// </summary>
        private IEnumerable<string> standardVerbs;
        public IEnumerable<string> ReplacementVerbs
        {
            get { return standardVerbs ?? new[] { "GET", "POST" }; }
            internal set { standardVerbs = value; }
        }

        /// <summary>
        /// The assemblies to containing implementations of RequestSpec and TypeSpec. Default: EntryAssembly
        /// </summary>
        private IEnumerable<Assembly> assemblies;
        public IEnumerable<Assembly> Assemblies
        {
            get { return assemblies ?? new[] { Assembly.GetEntryAssembly() }; }
            internal set { assemblies = value; }
        }

        /// <summary>
        /// The EnrichmentStrategy to use when populating collections. Default: Union
        /// </summary>
        private EnrichmentStrategy? collectionStrategy;
        public EnrichmentStrategy CollectionStrategy
        {
            get { return collectionStrategy ?? EnrichmentStrategy.Union; }
            internal set { collectionStrategy = value; }
        }

        /// <summary>
        /// The default notes to be set for a request/response object.
        /// </summary>
        public string FallbackNotes { get; internal set; }

        /// <summary>
        /// The default status codes to be set for a request/response object.
        /// </summary>
        public IEnumerable<StatusCode> DefaultStatusCodes { get; internal set; }

        /// <summary>
        /// The default category to be set for a request/response object.
        /// </summary>
        public string FallbackCategory { get; internal set; }

        /// <summary>
        /// The default tags to be set for a request/response object.
        /// </summary>
        public IEnumerable<string> DefaultTags { get; internal set; }

        /// <summary>
        /// The default contentTypes to be set for a request/response object.
        /// </summary>
        public IEnumerable<string> DefaultContentTypes { get; internal set; }

        /// <summary>
        /// The default notes to be set for a route
        /// </summary>
        public string FallbackRouteNotes { get; internal set; }

        private bool disposed;
        private readonly DocumenterSettingsScope parent;

        [ThreadStatic] private static DocumenterSettingsScope head;

        internal static DocumenterSettingsScope Current => head ?? new DocumenterSettingsScope();

        internal DocumenterSettingsScope()
        {
            parent = head;
            head = this;
        }

        public static void DisposeCurrent() => head?.Dispose();

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                head = parent;
            }
        }
    }
}