// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Settings
{
    using Documentation.Models;
    using Documentation.Settings;
    using FluentAssertions;
    using Xunit;

    public class DocumenterSettingsTests
    {
        [Fact]
        public void ReplacementVerbs_HasDefault()
        {
            var verbs = new[] { "GET", "POST" };
            using (var scope = DocumenterSettings.BeginScope())
                scope.ReplacementVerbs.Should().BeEquivalentTo(verbs);
        }

        [Fact]
        public void With_Verbs_SetsVerbs()
        {
            var verbs = new[] { "GET", "PUT" };
            var settings = DocumenterSettings.With(replacementVerbs: verbs);
            settings.ReplacementVerbs.Should().BeEquivalentTo(verbs);
        }

        [Fact]
        public void BeginScope_SetsVerbsForScope()
        {
            var verbs = new[] { "DELETE", "OPTIONS" };
            DocumenterSettings.ReplacementVerbs = verbs;

            var scopeVerbs = new[] { "POST", "PATCH" };
            using (var settings = DocumenterSettings.With(scopeVerbs))
                settings.ReplacementVerbs.Should().BeEquivalentTo(scopeVerbs);

            DocumenterSettings.ReplacementVerbs.Should().BeEquivalentTo(verbs);
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

        [Fact]
        public void FallbackNotes_DefaultNull()
        {
            using (var scope = DocumenterSettings.BeginScope())
                scope.FallbackNotes.Should().BeNull();
        }

        [Fact]
        public void With_FallbackNotes_SetsNotes()
        {
            const string notes = "watertight notes";
            var settings = DocumenterSettings.With(fallbackNotes: notes);
            settings.FallbackNotes.Should().Be(notes);
        }

        [Fact]
        public void BeginScope_SetsFallbackNotesForScope()
        {
            const string notes = "watertight notes";
            DocumenterSettings.FallbackNotes = notes;

            const string scopeNotes = "snake eyes";
            using (var settings = DocumenterSettings.With(fallbackNotes: scopeNotes))
                settings.FallbackNotes.Should().Be(scopeNotes);

            DocumenterSettings.FallbackNotes.Should().Be(notes);
        }

        [Fact]
        public void FallbackCategory_DefaultNull()
        {
            using (var scope = DocumenterSettings.BeginScope())
                scope.FallbackCategory.Should().BeNull();
        }

        [Fact]
        public void With_FallbackCategory_SetsCategory()
        {
            const string category = "products";
            var settings = DocumenterSettings.With(fallbackCategory: category);
            settings.FallbackCategory.Should().Be(category);
        }

        [Fact]
        public void BeginScope_SetsFallbackCategoryForScope()
        {
            const string category = "products";
            DocumenterSettings.FallbackCategory = category;

            const string scopeCategory = "not products";
            using (var settings = DocumenterSettings.With(fallbackCategory: scopeCategory))
                settings.FallbackCategory.Should().Be(scopeCategory);

            DocumenterSettings.FallbackCategory.Should().Be(category);
        }

        [Fact]
        public void DefaultVerbs_DefaultNull()
        {
            using (var scope = DocumenterSettings.BeginScope())
                scope.DefaultVerbs.Should().BeNull();
        }

        [Fact]
        public void With_DefaultVerbs_SetsDefaultVerbs()
        {
            var verbs = new[] { "PATCH", "POST" };
            var settings = DocumenterSettings.With(defaultVerbs: verbs);
            settings.DefaultVerbs.Should().BeEquivalentTo(verbs);
        }

        [Fact]
        public void BeginScope_SetsDefaultVerbsForScope()
        {
            var verbs = new[] { "PATCH", "POST" };
            DocumenterSettings.DefaultVerbs = verbs;

            var scopeVerbs = new[] { "PUT", "DELETE", "OPTIONS" };
            using (var settings = DocumenterSettings.With(defaultVerbs: scopeVerbs))
                settings.DefaultVerbs.Should().BeEquivalentTo(scopeVerbs);

            DocumenterSettings.DefaultVerbs.Should().BeEquivalentTo(verbs);
        }

        [Fact]
        public void DefaultStatusCodes_DefaultNull()
        {
            using (var scope = DocumenterSettings.BeginScope())
                scope.DefaultStatusCodes.Should().BeNull();
        }

        [Fact]
        public void With_DefaultStatusCodes_SetsDefaultStatusCodes()
        {
            var codes = new[] { (StatusCode) 200, (StatusCode) 409 };
            var settings = DocumenterSettings.With(defaultStatusCodes: codes);
            settings.DefaultStatusCodes.Should().BeEquivalentTo(codes);
        }

        [Fact]
        public void BeginScope_SetsDefaultStatusCodesForScope()
        {
            var codes = new[] { (StatusCode)200, (StatusCode)409 };
            DocumenterSettings.DefaultStatusCodes = codes;

            var scopeCodes = new[] { (StatusCode)404, (StatusCode)503 };
            using (var settings = DocumenterSettings.With(defaultStatusCodes: scopeCodes))
                settings.DefaultStatusCodes.Should().BeEquivalentTo(scopeCodes);

            DocumenterSettings.DefaultStatusCodes.Should().BeEquivalentTo(codes);
        }

        [Fact]
        public void DefaultTags_DefaultNull()
        {
            using (var scope = DocumenterSettings.BeginScope())
                scope.DefaultTags.Should().BeNull();
        }

        [Fact]
        public void With_DefaultTags_SetsDefaultTags()
        {
            var tags = new[] { "Tag1", "Tag2" };
            var settings = DocumenterSettings.With(defaultTags: tags);
            settings.DefaultTags.Should().BeEquivalentTo(tags);
        }

        [Fact]
        public void BeginScope_SetsDefaultTagsForScope()
        {
            var tags = new[] { "Tag1", "Tag2" };
            DocumenterSettings.DefaultTags = tags;

            var defaultTags = new[] { "Tag4", "Tag5", "Tag6" };
            using (var settings = DocumenterSettings.With(defaultTags: defaultTags))
                settings.DefaultTags.Should().BeEquivalentTo(defaultTags);

            DocumenterSettings.DefaultTags.Should().BeEquivalentTo(tags);
        }

        [Fact]
        public void DefaultContentTypes_DefaultNull()
        {
            using (var scope = DocumenterSettings.BeginScope())
                scope.DefaultContentTypes.Should().BeNull();
        }

        [Fact]
        public void With_DefaultContentTypes_SetsDefaultTags()
        {
            var contentTypes = new[] { "text/json", "image/png" };
            var settings = DocumenterSettings.With(defaultContentTypes: contentTypes);
            settings.DefaultContentTypes.Should().BeEquivalentTo(contentTypes);
        }

        [Fact]
        public void BeginScope_SetsDefaultContentTypesForScope()
        {
            var contentTypes = new[] { "text/json", "image/png" };
            DocumenterSettings.DefaultContentTypes = contentTypes;

            var scopeTypes = new[] { "text/plain", "application/yaml" };
            using (var settings = DocumenterSettings.With(defaultContentTypes: scopeTypes))
                settings.DefaultContentTypes.Should().BeEquivalentTo(scopeTypes);

            DocumenterSettings.DefaultContentTypes.Should().BeEquivalentTo(contentTypes);
        }
    }
}
