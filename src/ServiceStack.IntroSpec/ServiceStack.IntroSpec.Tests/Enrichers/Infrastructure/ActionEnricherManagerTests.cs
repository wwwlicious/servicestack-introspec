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
        private readonly ISecurityEnricher securityEnricher;
        private readonly IActionEnricher actionEnricher;
        private readonly ActionEnricherManager enricherManager;
        private readonly Operation operation;

        public ActionEnricherManagerTests()
        {
            actionEnricher = A.Fake<IActionEnricher>();
            securityEnricher = A.Fake<ISecurityEnricher>();

            enricherManager = new ActionEnricherManager(actionEnricher, securityEnricher);
            operation = new Operation { Actions = new List<string> { "GET" } };
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
            using (DocumenterSettings.With(replacementVerbs: new[] { "GET", "PUT", "DELETE" }))
            {
                var result = enricherManager.EnrichActions(null, operation);
                result.Length.Should().Be(3);
                result[0].Verb.Should().Be("GET");
                result[1].Verb.Should().Be("PUT");
                result[2].Verb.Should().Be("DELETE");
            }
        }

        [Fact]
        public void EnrichActions_ReturnsObjectPerVerb_IfAnyNotPresent()
        {
            var operation = new Operation { Actions = new List<string> { "GET", "PUT", "DELETE" } };

            var result = enricherManager.EnrichActions(null, operation);
            result.Length.Should().Be(3);
            result[0].Verb.Should().Be("GET");
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
            A.CallTo(() => actionEnricher.GetStatusCodes(operation, "GET")).MustHaveHappened();
        }

        [Fact]
        public void EnrichActions_DoesNotCallGetStatusCodes_IfStatusCodesNotEmpty_SetIfEmptyStrategy()
        {
            var actions = new[] { new ApiAction { Verb = "GET", StatusCodes = new[] { (StatusCode) 404 } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetStatusCodes(operation, "GET")).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichActions_CallsGetStatusCodes_IfStatusCodesNotEmpty_UnionStrategy()
        {
            var actions = new[] { new ApiAction { Verb = "GET", StatusCodes = new[] { (StatusCode)404 } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetStatusCodes(operation, "GET")).MustHaveHappened();
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
            A.CallTo(() => actionEnricher.GetContentTypes(operation, "GET")).MustHaveHappened();
        }

        [Fact]
        public void EnrichActions_DoesNotCallGetContentTypes_IfContentTypesNotEmpty_SetIfEmptyStrategy()
        {
            var actions = new[] { new ApiAction { Verb = "GET", ContentTypes = new[] { "text/xml" } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetContentTypes(operation, "GET")).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichActions_CallsGetContentTypes_IfContentTypesNotEmpty_UnionStrategy()
        {
            var actions = new[] { new ApiAction { Verb = "GET", ContentTypes = new[] { "text/xml" } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetContentTypes(operation, "GET")).MustHaveHappened();
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
            A.CallTo(() => actionEnricher.GetRelativePaths(operation, "GET")).MustHaveHappened();
        }

        [Fact]
        public void EnrichActions_DoesNotCallGetRelativePaths_IfRelativePathsNotEmpty_SetIfEmptyStrategy()
        {
            var actions = new[] { new ApiAction { Verb = "GET", RelativePaths = new[] { "/path" } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.SetIfEmpty))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetRelativePaths(operation, "GET")).MustNotHaveHappened();
        }

        [Fact]
        public void EnrichActions_CallsGetContentTypes_IfRelativePathsNotEmpty_UnionStrategy()
        {
            var actions = new[] { new ApiAction { Verb = "GET", RelativePaths = new[] { "/path" } } };
            using (DocumenterSettings.With(collectionStrategy: EnrichmentStrategy.Union))
                enricherManager.EnrichActions(actions, operation);

            A.CallTo(() => actionEnricher.GetRelativePaths(operation, "GET")).MustHaveHappened();
        }

        [Fact]
        public void EnrichActions_CallGetSecurity()
        {
            enricherManager.EnrichActions(null, operation);

            A.CallTo(() => securityEnricher.GetSecurity(operation, "GET")).MustHaveHappened();
        }
    }
}
