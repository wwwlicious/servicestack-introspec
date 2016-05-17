// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Settings
{
    using Documentation.Settings;
    using FluentAssertions;
    using Xunit;

    public class DocumenterSettingsTests
    {
        [Fact]
        public void AnyVerbs_HasDefault()
        {
            var verbs = new[] { "GET", "POST" };
            using (var scope = DocumenterSettings.BeginScope())
                scope.AnyVerbs.Should().BeEquivalentTo(verbs);
        }

        [Fact]
        public void With_Verbs_SetsVerbs()
        {
            var verbs = new[] { "GET", "PUT" };
            var settings = DocumenterSettings.With(verbs: verbs);
            settings.AnyVerbs.Should().BeEquivalentTo(verbs);
        }

        [Fact]
        public void BeginScope_SetsVerbsForScope()
        {
            var verbs = new[] { "DELETE", "OPTIONS" };
            DocumenterSettings.AnyVerbs = verbs;

            var scopeVerbs = new[] { "POST", "PATCH" };
            using (var settings = DocumenterSettings.With(scopeVerbs))
                settings.AnyVerbs.Should().BeEquivalentTo(scopeVerbs);

            DocumenterSettings.AnyVerbs.Should().BeEquivalentTo(verbs);
        }

        [Fact]
        public void With_Assemblies_SetsAssemblies()
        {
            var assemblies = new[] { typeof(DocumenterSettingsTests).Assembly };
            var settings = DocumenterSettings.With(assemblies: assemblies);
            settings.Assemblies.Should().BeEquivalentTo(assemblies);
        }

        [Fact]
        public void BeginScope_SetsAssembliesForScope()
        {
            var assemblies = new[] { typeof(DocumenterSettings).Assembly };
            DocumenterSettings.Assemblies = assemblies;

            var scopeAssemblies = new[] { typeof(DocumenterSettingsTests).Assembly };
            using (var settings = DocumenterSettings.With(assemblies: scopeAssemblies))
                settings.Assemblies.Should().BeEquivalentTo(scopeAssemblies);

            DocumenterSettings.Assemblies.Should().BeEquivalentTo(assemblies);
        }

        [Fact]
        public void CollectionStrategy_HasDefault()
        {
            using (var scope = DocumenterSettings.BeginScope())
                scope.CollectionStrategy.Should().Be(EnrichmentStrategy.Union);
        }

        [Fact]
        public void With_CollectionStrategy_SetsStrategy()
        {
            var settings = DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty);
            settings.CollectionStrategy.Should().Be(EnrichmentStrategy.SetIfEmpty);
        }

        [Fact]
        public void BeginScope_SetsCollectionStrategyForScope()
        {
            DocumenterSettings.CollectionStrategy = EnrichmentStrategy.SetIfEmpty;

            using (var settings = DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
                settings.CollectionStrategy.Should().Be(EnrichmentStrategy.Union);

            DocumenterSettings.CollectionStrategy.Should().Be(EnrichmentStrategy.SetIfEmpty);
        }
    }
}
