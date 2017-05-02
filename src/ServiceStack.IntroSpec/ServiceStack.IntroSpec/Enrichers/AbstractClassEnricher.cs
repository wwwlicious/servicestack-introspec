// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Host;
    using Interfaces;
    using Models;
    using TypeSpec;

    /// <summary>
    /// Enricher that will use implementations of AbstractRequestSpec and AbstractTypeSpec to enrich object
    /// </summary>
    public class AbstractClassEnricher : IResourceEnricher, IRequestEnricher, IPropertyEnricher, IActionEnricher
    {
        private readonly Dictionary<Type, IApiResource> lookup;

        public AbstractClassEnricher()
        {
            lookup = DocumentationClassLocator.GetLookup();
        }

        public string GetTitle(Type type) 
            => lookup.SafeGetFromValue(type, v => v.Title, null);

        public string GetDescription(Type type) 
            => lookup.SafeGetFromValue(type, v => v.Description, null);

        public string GetNotes(Type type) 
            => lookup.SafeGetFromValue(type, v => v.Notes, null);

        public bool? GetAllowMultiple(Type type) => null;
        public bool? GetHasValidator(Type type) => null;

        public string GetCategory(Operation operation)
            => lookup.SafeGetFromValue(operation.RequestType, v => (v as IApiRequestSpec)?.Category, null);

        public string[] GetTags(Operation operation)
            => lookup.SafeGetFromValue(operation.RequestType, v => (v as IApiRequestSpec)?.Tags.ToArray(), null);

        public string[] GetContentTypes(Operation operation, string verb)
            => GetFromDictionary(operation, verb, request => request.ContentTypes);

        public StatusCode[] GetStatusCodes(Operation operation, string verb)
            => GetFromDictionary(operation, verb, request => request.StatusCodes);

        public string GetNotes(Operation operation, string verb)
        {
            var apiRequest = lookup.SafeGet(operation.RequestType, (IApiRequestSpec)null) as IApiRequestSpec;
            if (apiRequest == null) return null;

            var dictionary = apiRequest.RouteNotes;

            string verbNotes;
            if (dictionary.TryGetValue(verb, out verbNotes))
                return verbNotes;

            return dictionary.SafeGet(Constants.GlobalSettingsKey);
        }

        public string GetTitle(MemberInfo mi) 
            => GetPropertyValue(mi, property => property?.Title);

        public string GetDescription(MemberInfo mi)
            => GetPropertyValue(mi, property => property?.Description);

        public PropertyConstraint GetConstraints(MemberInfo mi)
            => GetPropertyValue(mi, property => property?.Constraint);

        public bool? GetIsRequired(MemberInfo mi)
            => GetPropertyValue(mi, property => property?.IsRequired);

        public RelativePath[] GetRelativePaths(Operation operation, string verb) => null;
        public string GetNotes(MemberInfo mi) => null;
        public bool? GetAllowMultiple(MemberInfo mi) => null;
        public string[] GetExternalLinks(MemberInfo mi) => null;
        public string GetParamType(MemberInfo mi) => null;

        private T GetPropertyValue<T>(MemberInfo pi, Func<IProperty, T> getter)
        {
            IApiResource spec;
            if (lookup.TryGetValue(pi.DeclaringType, out spec))
            {
                var propertyResource = spec as IApiPropertyResource;
                if (propertyResource != null)
                {
                    var param = propertyResource.GetPropertySpec(pi);
                    return getter(param);
                }
            }

            return default(T);
        }

        private T[] GetFromDictionary<T>(Operation operation, string verb, Func<IApiRequestSpec, Dictionary<string, List<T>>> dictionaryGetter)
        {
            var apiRequest = lookup.SafeGet(operation.RequestType, (IApiRequestSpec)null) as IApiRequestSpec;
            if (apiRequest == null) return null;

            var dictionary = dictionaryGetter(apiRequest);
            return dictionary.IsNullOrEmpty()
                       ? null
                       : dictionary.SafeGet(Constants.GlobalSettingsKey).SafeUnion(dictionary.SafeGet(verb))?.ToArray();
        }
    }
}
