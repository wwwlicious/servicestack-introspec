// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    // Based on JsConfig and JsConfigScope from ServiceStack
    public static class DocumenterSettings
    {
        public static IEnumerable<string> AnyVerbs
        {
            get { return DocumenterSettingsScope.Current.AnyVerbs; }
            set { DocumenterSettingsScope.Current.AnyVerbs = value; }
        }

        public static IEnumerable<Assembly> Assemblies
        {
            get { return DocumenterSettingsScope.Current.Assemblies; }
            set { DocumenterSettingsScope.Current.Assemblies = value; }
        }

        public static EnrichmentStrategy CollectionStrategy
        {
            get { return DocumenterSettingsScope.Current.CollectionStrategy; }
            set { DocumenterSettingsScope.Current.CollectionStrategy = value; }
        }

        public static DocumenterSettingsScope BeginScope() => new DocumenterSettingsScope();

        public static DocumenterSettingsScope With(
            IEnumerable<string> verbs = null,
            IEnumerable<Assembly> assemblies = null,
            EnrichmentStrategy collectionStrategy = EnrichmentStrategy.Union)
        {
            return new DocumenterSettingsScope
            {
                AnyVerbs = verbs,
                CollectionStrategy = collectionStrategy,
                Assemblies = assemblies
            };
        }
    }

    public class DocumenterSettingsScope : IDisposable
    {
        private IEnumerable<string> anyVerbs;
        public IEnumerable<string> AnyVerbs
        {
            get { return anyVerbs ?? new[] { "GET", "POST" }; }
            internal set { anyVerbs = value; }
        }

        private IEnumerable<Assembly> assemblies;
        public IEnumerable<Assembly> Assemblies
        {
            get { return assemblies ?? new[] { Assembly.GetEntryAssembly() }; }
            internal set { assemblies = value; }
        }

        private EnrichmentStrategy? collectionStrategy;
        public EnrichmentStrategy CollectionStrategy
        {
            get { return collectionStrategy ?? EnrichmentStrategy.Union; }
            internal set { collectionStrategy = value; }
        }

        private bool disposed;
        private DocumenterSettingsScope parent;

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