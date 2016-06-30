// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Enrichers.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy;
    using FluentAssertions;
    using Host;
    using IntroSpec.Enrichers.Infrastructure;
    using IntroSpec.Enrichers.Interfaces;
    using IntroSpec.Models;
    using IntroSpec.Settings;
    using Xunit;

    public class ActionEnricherManagerTests
    {
        private const string GetVerb = "GET";
        private readonly ISecurityEnricher securityEnricher;
        private readonly IActionEnricher actionEnricher;
        private readonly ActionEnricherManager enricherManager;
        private readonly Operation operation;

        public ActionEnricherManagerTests()
        {
            actionEnricher = A.Fake<IActionEnricher>();
            securityEnricher = A.Fake<ISecurityEnricher>();

            enricherManager = new ActionEnricherManager(actionEnricher, securityEnricher);
            operation = new Operation { Actions = new List<string> { GetVerb } };
        }

        [Fact]
        public void Ctor_AllowsNullEnrichers()
        {
            Action action = () => new ActionEnricherManager(null, null);
            action.ShouldNotThrow();
        }

        [Fact]
        public void EnrichActions_ReturnsObjectPerVerb_UsingAnyReplacementVerbs_IfAnyPresent()
        {
            var operation = new Operation { Actions = new List<string> { "ANY" } };
            using (DocumenterSettings.With(replacementVerbs: new[] { GetVerb, "PUT", "DELETE" }))
            {
                var result = enricherManager.EnrichActions(null, operation);
                result.Length.Should().Be(3);
                result[0].Verb.Should().Be(GetVerb);
                result[1].Verb.Should().Be("PUT");
                result[2].Verb.Should().Be("DELETE");
            }
        }

        [Fact]
        public void EnrichActions_ReturnsObjectPerVerb_IfAnyNotPresent()
        {
            var operation = new Operation { Actions = new List<string> { GetVerb, "PUT", "DELETE" } };

            var result = enricherManager.EnrichActions(null, operation);
            result.Length.Should().Be(3);
            result[0].Verb.Should().Be(GetVerb);
            result[1].Verb.Should().Be("PUT");
            result[2].Verb.Should().Be("DELETE");
        }

        [Theory]
        [InlineData(EnrichmentStrategy.SetIfEmpty)]
        [InlineData(EnrichmentStrategy.Union)]
        public void EnrichActions_CallsGetStatusCodes_IfStatusCodesNull_BothStrategies(EnrichmentStrategy strategy)
        {
            using (DocumenterSettings.With(collectionStrategy: strategy))
            {
                enricherManager.EnrichActions(null, operation);
            }
            A.CallTo(() => actionEnricher.GetStatusCodes(operation, GetVerb)).MustHaveHappened();
        }

        [Fact]
        public void EnrichActions_DoesNotCallGetStatusCodes_IfStatusCodesNotEmpty_SetIfEmptyStrategy()
        {
            var actions = new[] { new ApiAction { Verb = GetVerb, StatusCodes = new[] { (StatusCode) 404 } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetStatusCodes(operation, GetVerb)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichActions_CallsGetStatusCodes_IfStatusCodesNotEmpty_UnionStrategy()
        {
            var actions = new[] { new ApiAction { Verb = GetVerb, StatusCodes = new[] { (StatusCode)404 } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetStatusCodes(operation, GetVerb)).MustHaveHappened();
        }

        [Theory]
        [InlineData(EnrichmentStrategy.SetIfEmpty)]
        [InlineData(EnrichmentStrategy.Union)]
        public void EnrichActions_CallsGetContentTypes_IfContentTypesNull_BothStrategies(EnrichmentStrategy strategy)
        {
            using (DocumenterSettings.With(collectionStrategy: strategy))
            {
                enricherManager.EnrichActions(null, operation);
            }
            A.CallTo(() => actionEnricher.GetContentTypes(operation, GetVerb)).MustHaveHappened();
        }

        [Fact]
        public void EnrichActions_DoesNotCallGetContentTypes_IfContentTypesNotEmpty_SetIfEmptyStrategy()
        {
            var actions = new[] { new ApiAction { Verb = GetVerb, ContentTypes = new[] { "text/xml" } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetContentTypes(operation, GetVerb)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichActions_CallsGetContentTypes_IfContentTypesNotEmpty_UnionStrategy()
        {
            var actions = new[] { new ApiAction { Verb = GetVerb, ContentTypes = new[] { "text/xml" } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetContentTypes(operation, GetVerb)).MustHaveHappened();
        }
        
        [Theory]
        [InlineData(EnrichmentStrategy.SetIfEmpty)]
        [InlineData(EnrichmentStrategy.Union)]
        public void EnrichActions_CallsGetRelativePaths_IfRelativePathsNull_BothStrategies(EnrichmentStrategy strategy)
        {
            using (DocumenterSettings.With(collectionStrategy: strategy))
            {
                enricherManager.EnrichActions(null, operation);
            }
            A.CallTo(() => actionEnricher.GetRelativePaths(operation, GetVerb)).MustHaveHappened();
        }

        [Fact]
        public void EnrichActions_DoesNotCallGetRelativePaths_IfRelativePathsNotEmpty_SetIfEmptyStrategy()
        {
            var actions = new[] { new ApiAction { Verb = GetVerb, RelativePaths = new RelativePath[] { "/path" } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetRelativePaths(operation, GetVerb)).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichActions_CallsGetContentTypes_IfRelativePathsNotEmpty_UnionStrategy()
        {
            var actions = new[] { new ApiAction { Verb = GetVerb, RelativePaths = new RelativePath[] { "/path" } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetRelativePaths(operation, GetVerb)).MustHaveHappened();
        }

        [Fact]
        public void EnrichActions_CallGetSecurity()
        {
            enricherManager.EnrichActions(null, operation);

            A.CallTo(() => securityEnricher.GetSecurity(operation, GetVerb)).MustHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EnrichActions_CallsGetNotes_IfNotesNullOrEmpty(string notes)
        {
            var actions = new[] { new ApiAction { Verb = GetVerb, Notes = notes } };
            enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetNotes(operation, GetVerb)).MustHaveHappened();
        }

        [Fact]
        public void EnrichActions_DoesNotCallGetNotes_IfNotesNotNullOrEmpty()
        {
            var actions = new[] { new ApiAction { Verb = GetVerb, Notes = "notes" } };
            enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetNotes(operation, GetVerb)).MustNotHaveHappened();
        }
    }
}
