// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enrichers;
    using Enrichers.Infrastructure;
    using Enrichers.Interfaces;
    using Extensions;
    using FluentValidation;
    using Funq;
    using Host;
    using Logging;
    using Models;
    using Postman.Services;
    using Services;
    using Settings;
    using Validators;
    using XmlDocumentation;

    public class ApiSpecFeature : IPlugin
    {
        private readonly ILog log = LogManager.GetLogger(typeof (ApiSpecFeature));
        private static readonly ApiSpecConfigValidator ConfigValidator = new ApiSpecConfigValidator();

        private readonly ApiSpecConfig config;

        public ApiDocumentation Documentation { get; private set; }
        public IApiDocumentationGenerator DocumentationGenerator { get; private set; }
        public Func<IEnumerable<IApiResourceEnricher>> Enrichers { get; private set; }

        // NOTE Similar to what's in NativeTypesFeature - use that if present?
        public static List<string> IgnoreTypesInNamespaces = new List<string>
        {
            "ServiceStack",
            "ServiceStack.Auth",
            "ServiceStack.Caching",
            "ServiceStack.Configuration",
            "ServiceStack.Data",
            "ServiceStack.IO",
            "ServiceStack.Logging",
            "ServiceStack.Messaging",
            "ServiceStack.Model",
            "ServiceStack.Redis",
            "ServiceStack.Web",
            "ServiceStack.Admin",
            "ServiceStack.NativeTypes",
            "ServiceStack.Api.Swagger",
            "ServiceStack.IntroSpec"
        };

        public Func<KeyValuePair<Type, Operation>, bool> OperationsMapFilter { get; private set; } = kvp =>
            !IgnoreTypesInNamespaces.Contains(kvp.Key.Namespace) && 
            !kvp.Key.ExcludesFeature(Feature.Metadata) &&
            !kvp.Key.ExcludesFeature(Feature.ServiceDiscovery);

        // Take object that represents contact details etc
        public ApiSpecFeature(ApiSpecConfig config)
        {
            config.ThrowIfNull(nameof(config));
            ConfigValidator.ValidateAndThrow(config);
            this.config = config;
        }

        public void Register(IAppHost appHost)
        {
            if (!appHost.Plugins.Any(p => p is MetadataFeature))
                throw new ArgumentException("The Metadata Feature must be enabled to use the ApiSpec Feature");

            EnsureDependencies(appHost.GetContainer());

            RegisterServices(appHost);

            // Get a filtered list of all operations
            var operations = appHost.Metadata.OperationsMap.FilterValues(OperationsMapFilter);

            // Generate documentation from these 
            log.Debug("Generating documentation...");
            Documentation = DocumentationGenerator.GenerateDocumentation(operations, appHost, config);

            log.Debug("Generated documentation");
        }

        /// <summary>
        /// Implementation of IApiDocumentationGenerator used for creating documentation POCOs
        /// Defaults to ApiDocumentationGenerator
        /// </summary>
        /// <param name="generator">Predicate to use to filter operations map</param>
        /// <returns>ApiSpecFeature object</returns>
        public ApiSpecFeature WithGenerator(IApiDocumentationGenerator generator)
        {
            DocumentationGenerator = generator;
            return this;
        }

        /// <summary>
        /// Predicate to use to filter metadata OperationsMap to get resources to document. 
        /// Default RestrictTo == null && doesn't have [Exclude(Feature.Metadata | Feature.ServiceDiscovery)]
        /// </summary>
        /// <param name="filter">Predicate to use to filter operations map</param>
        /// <returns>ApiSpecFeature object</returns>
        public ApiSpecFeature WithOperationsFilter(Func<KeyValuePair<Type, Operation>, bool> filter)
        {
            OperationsMapFilter = filter;
            return this;
        }

        /// <summary>
        /// Function to get a list of enrichers to use for generating documentation. 
        /// Default Reflection - ApiSpec - XmlComments
        /// </summary>
        /// <param name="enrichers">Function to get an ordered list of enrichers (most - least important)</param>
        /// <returns>ApiSpecFeature object</returns>
        public ApiSpecFeature WithEnrichers(Func<IEnumerable<IApiResourceEnricher>> enrichers)
        {
            this.Enrichers = enrichers;
            return this;
        }

        private void EnsureDependencies(Container container)
        {
            RegisterDependencies(container);

            if (Enrichers == null)
            {
                Enrichers = () => new[]
                {
                    new EnricherCoordinator(container.ResolveNamed<IEnrich>(Constants.ReflectionEnricherKey)),
                    new EnricherCoordinator(container.ResolveNamed<IEnrich>(Constants.ClassEnricherKey)),
                    new EnricherCoordinator(container.ResolveNamed<IEnrich>(Constants.XmlEnricherKey)),
                    new EnricherCoordinator(container.ResolveNamed<IEnrich>(Constants.FallbackEnricherKey))

                };
            }

            if (DocumentationGenerator == null)
                DocumentationGenerator = new ApiDocumentationGenerator(Enrichers);
        }

        private void RegisterServices(IAppHost appHost)
        {
            var metadataFeature = appHost.GetPlugin<MetadataFeature>();

            appHost.RegisterService<ApiSpecService>();
            appHost.RegisterService<ApiSpecPostmanService>();
            appHost.RegisterService<ApiSpecMetadataService>();

            metadataFeature.AddDebugLink(Constants.SpecUri, "Raw API Spec");
            metadataFeature.AddPluginLink(Constants.PostmanSpecUri, "Postman API Metadata");
            metadataFeature.AddPluginLink(Constants.SpecSummaryUri, "API Spec Metadata");
        }

        private void RegisterDependencies(Container container)
        {
            container.RegisterAutoWiredAs<XmlDocumentationReader, IXmlDocumentationReader>();
            container.RegisterAutoWiredAs<XmlDocumentationLookup, IXmlDocumentationLookup>();

            container.Register<IEnrich>(Constants.ReflectionEnricherKey, c => new ReflectionEnricher());
            container.Register<IEnrich>(Constants.ClassEnricherKey, c => new AbstractClassEnricher());
            container.Register<IEnrich>(Constants.XmlEnricherKey, c => new XmlEnricher(c.Resolve<IXmlDocumentationLookup>()));
            container.Register<IEnrich>(Constants.FallbackEnricherKey, c => new FallbackEnricher());

            container.RegisterAs<ApiDocumentationProvider, IApiDocumentationProvider>();
        }
    }
}
