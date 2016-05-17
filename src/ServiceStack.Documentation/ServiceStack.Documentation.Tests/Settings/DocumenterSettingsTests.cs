// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Settings
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Documentation.Enrichers.Interfaces;
    using Documentation.Models;
    using Documentation.Settings;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class DocumenterSettingsTests
    {
        [Fact]
        public void Test()
        {
            /*var zz = DocumenterSettings.GetAnyVerbs();
            using (var x = DocumenterSettings.BeginScope())
            {
                var p = x.GetAnyVerbs();
                x.WithAnyVerbs(new[] { "PATCH" });
                var xx = x.GetAnyVerbs();
                var pp = DocumenterSettings.GetAnyVerbs();
            }
            var zsdz = DocumenterSettings.GetAnyVerbs();*/

        }
        /*private readonly ApiSpecConfig apiSpecConfig;
        private readonly DocumenterSettingsScope documenter;

        public DocumenterSettingsTests()
        {
            apiSpecConfig = new ApiSpecConfig
            {
                Contact = new ApiContact { Email = "ronald.macdonald@macdonalds.hq", Name = "ronnie mcd" },
                Description = "great api"
            };

            documenter = new DocumenterSettingsScope(apiSpecConfig);
        }

        [Fact]
        public void Ctor_ThrowsIfConfigNull()
        {
            Action action = () => new DocumenterSettingsScope(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_ThrowsIfConfigInvalid()
        {
            Action action = () => new DocumenterSettingsScope(new ApiSpecConfig());
            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetSpecConfig_ReturnsConfig_FromConstructur()
        {
            var actual = documenter.GetSpecConfig();
            actual.ShouldBeEquivalentTo(apiSpecConfig);
        }

        [Fact]
        public void GetAssemblies_ReturnsEntryAssembly_IfNotSet()
        {
            var assemblies = documenter.GetAssemblies();
            assemblies.Count().Should().Be(1);
        }

        [Fact]
        public void GetAssemblies_ReturnsAssemblies_SetViaWithDocumenterAssemblies()
        {
            var assemblies = new[] { Assembly.GetAssembly(GetType()) };

            var actual = documenter.WithDocumenterAssemblies(assemblies).GetAssemblies();
            actual.ShouldBeEquivalentTo(assemblies);
        }

        [Fact]
        public void GetAssemblies_ReturnsGetAndPost_IfNotSet()
        {
            var expected = new[] { "GET", "POST" };
            var actual = documenter.GetAnyVerbs();
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void GetAssemblies_ReturnsVerbs_SetViaWithAnyVerbs()
        {
            var expected = new[] { "GET", "POST", "PUT", "PATCH" };

            var actual = documenter.WithAnyVerbs(expected).GetAnyVerbs();
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void GetCollectionStrategy_ReturnsUnion_IfNotSet()
        {
            var actual = documenter.GetCollectionStrategy();
            actual.Should().Be(EnrichmentStrategy.Union);
        }

        [Fact]
        public void GetCollectionStrategy_ReturnsStrategy_SetViaWithAnyVerbs()
        {
            var actual = documenter.WithCollectionStrategy(EnrichmentStrategy.SetIfEmpty).GetCollectionStrategy();
            actual.Should().Be(EnrichmentStrategy.SetIfEmpty);
        }

        [Fact]
        public void GetEnrichers_ReturnsEnrichers_SetViaWithEnrichers()
        {
            var expected = new[] { A.Fake<IApiResourceEnricher>(), A.Fake<IApiResourceEnricher>() };
        
            var actual = documenter.WithEnrichers(expected).GetEnrichers();
            actual.ShouldBeEquivalentTo(expected);
        }*/
    }
}
