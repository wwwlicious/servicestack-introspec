// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using AbstractApiSpec;
    using Extensions;
    using Host;
    using Interfaces;
    using Models;

    public class AbstractClassEnricher : IResourceEnricher, IRequestEnricher, IPropertyEnricher
    {
        private readonly Dictionary<Type, IApiResource> lookup;

        public AbstractClassEnricher()
        {
            lookup = DocumentationClassLocator.GetLookup();
        }

        public string GetTitle(Type type) => lookup.SafeGetFromValue(type, v => v.Title, null);
        public string GetDescription(Type type) => lookup.SafeGetFromValue(type, v => v.Description, null);
        public string GetNotes(Type type) => lookup.SafeGetFromValue(type, v => v.Notes, null);

        public string[] GetVerbs(Operation operation)
            => lookup.SafeGetFromValue(operation.RequestType, v => (v as IApiRequest)?.Verbs.ToArray(), null);

        public string[] GetContentTypes(Operation operation)
            => lookup.SafeGetFromValue(operation.RequestType, v => (v as IApiRequest)?.ContentTypes.ToArray(), null);

        public StatusCode[] GetStatusCodes(Operation operation)
            => lookup.SafeGetFromValue(operation.RequestType, v => (v as IApiRequest)?.StatusCodes.ToArray(), null);

        public string GetTitle(PropertyInfo pi) => GetPropertyValue(pi, property => property.Title);

        public string GetDescription(PropertyInfo pi)
            => GetPropertyValue(pi, property => property.Description);

        public string GetRelativePath(Operation operation) => null;

        public string GetCategory(Operation operation)
            => lookup.SafeGetFromValue(operation.RequestType, v => (v as IApiRequest)?.Category, null);

        public string[] GetTags(Operation operation)
            => lookup.SafeGetFromValue(operation.RequestType, v => (v as IApiRequest)?.Tags.ToArray(), null);

        public string GetNotes(PropertyInfo pi) => null;
        public bool? GetAllowMultiple(PropertyInfo pi) => null;
        public string[] GetExternalLinks(PropertyInfo pi) => null;

        public PropertyConstraint GetConstraints(PropertyInfo pi)
            => GetPropertyValue(pi, property => property?.Constraint);

        public bool? GetIsRequired(PropertyInfo pi)
            => GetPropertyValue(pi, property => property.IsRequired);

        public string GetParamType(PropertyInfo pi) => null;

        private T GetPropertyValue<T>(PropertyInfo pi, Func<IProperty, T> getter)
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
    }
}
