﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Configuration;
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

    public class IntroSpecFeature : IPlugin, IApiSpecSettings
    {
        private IAppSettings appSettings;
        private readonly ILog log = LogManager.GetLogger(typeof (IntroSpecFeature));
        private static readonly ApiSpecSettingsValidator ConfigValidator = new ApiSpecSettingsValidator();

        public ApiDocumentation Documentation { get; set; }
        public IApiDocumentationGenerator DocumentationGenerator { get; set; }
        public Func<IEnumerable<IApiResourceEnricher>> Enrichers { get; set; }

        public string ContactName
        {
            get { return appSettings.GetString(ConfigKeys.ContactName); }
            set { appSettings.Set(ConfigKeys.ContactName, value); }
        }

        public string ContactEmail
        {
            get { return appSettings.GetString(ConfigKeys.ContactEmail); }
            set { appSettings.Set(ConfigKeys.ContactEmail, value); }
        }

        public Uri ContactUrl
        {
            get { return appSettings.Get<Uri>(ConfigKeys.ContactUrl); }
            set { appSettings.Set(ConfigKeys.ContactUrl, value); }
        }

        public string Description
        {
            get { return appSettings.GetString(ConfigKeys.Description); }
            set { appSettings.Set(ConfigKeys.Description, value); }
        }

        public Uri LicenseUrl
        {
            get { return appSettings.Get<Uri>(ConfigKeys.LicenseUrl); }
            set { appSettings.Set(ConfigKeys.LicenseUrl, value); }
        }

        public delegate ApiSpecConfig ApiConfigDelegate(ApiSpecConfig config);

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
            "ServiceStack.IntroSpec",
            "ServiceStack.IntroSpec.DTO"
        };

        public Func<KeyValuePair<Type, Operation>, bool> OperationsMapFilter { get; private set; } =
            kvp =>
            !IgnoreTypesInNamespaces.Contains(kvp.Key.Namespace) &&
            !kvp.Key.ExcludesFeature(Feature.Metadata) &&
            !kvp.Key.ExcludesFeature(Feature.ServiceDiscovery);

        public IntroSpecFeature()
        {
            appSettings = AppHostBase.Instance.AppSettings;

            if (log.IsDebugEnabled)
                log.Debug($"Using {appSettings.GetType().Name} appSettings for appSettings provider");
        }

        public void Register(IAppHost appHost)
        {
            appSettings = appHost.AppSettings ?? new AppSettings();

            ConfigValidator.ValidateAndThrow(this);

            if (!appHost.Plugins.Any(p => p is MetadataFeature))
                throw new ArgumentException("The Metadata Feature must be enabled to use the ApiSpec Feature");

            EnsureDependencies(appHost.GetContainer());

            RegisterServices(appHost);

            // Get a filtered list of all operations
            var operations = appHost.Metadata.OperationsMap.FilterValues(OperationsMapFilter);

            // Generate documentation from these 
            Stopwatch s = null;
            if (log.IsDebugEnabled)
            {
                log.Debug("Generating documentation...");
                s = new Stopwatch();
                s.Start();
            }

            Documentation = DocumentationGenerator.GenerateDocumentation(operations, appHost, this);

            if (log.IsDebugEnabled)
            {
                s.Stop();
                log.Debug($"Generated documentation. Took {s.ElapsedMilliseconds}ms");
            }
        }

        /// <summary>
        /// Implementation of IApiDocumentationGenerator used for creating documentation POCOs
        /// Defaults to ApiDocumentationGenerator
        /// </summary>
        /// <param name="generator">Predicate to use to filter operations map</param>
        /// <returns>ApiSpecFeature object</returns>
        public IntroSpecFeature WithGenerator(IApiDocumentationGenerator generator)
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
        public IntroSpecFeature WithOperationsFilter(Func<KeyValuePair<Type, Operation>, bool> filter)
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
        public IntroSpecFeature WithEnrichers(Func<IEnumerable<IApiResourceEnricher>> enrichers)
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

            metadataFeature.AddPluginLink(Constants.SpecSummaryUri, "IntroSpec API Summary");
            metadataFeature.AddPluginLink(Constants.SpecUri, "IntroSpec API Spec");
            metadataFeature.AddPluginLink(Constants.PostmanSpecUri, "IntroSpec Postman");
        }

        private void RegisterDependencies(Container container)
        {
            container.RegisterAutoWiredAs<ApiDocumentationProvider, IApiDocumentationProvider>();
            container.RegisterAutoWiredAs<XmlDocumentationReader, IXmlDocumentationReader>();
            container.RegisterAutoWiredAs<XmlDocumentationLookup, IXmlDocumentationLookup>();

            container.Register<IEnrich>(Constants.ReflectionEnricherKey, c => new ReflectionEnricher());
            container.Register<IEnrich>(Constants.ClassEnricherKey, c => new AbstractClassEnricher());
            container.Register<IEnrich>(Constants.XmlEnricherKey, c => new XmlEnricher(c.Resolve<IXmlDocumentationLookup>()));
            container.Register<IEnrich>(Constants.FallbackEnricherKey, c => new FallbackEnricher());
        }
    }
}