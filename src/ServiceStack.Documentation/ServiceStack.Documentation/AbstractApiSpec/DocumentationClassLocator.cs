// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.AbstractApiSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Logging;
    using Settings;

    public static class DocumentationClassLocator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentationClassLocator));

        public static Dictionary<Type, IApiResource> GetLookup()
        {
            try
            {
                var typesToScan = DocumenterSettings.Assemblies.SelectMany(a => a.GetTypes());
                var lookup = FindAllTypeSpecs(typesToScan)
                    .ToDictionary(k => k.BaseType.GenericTypeArguments[0],
                        v => (IApiResource)v.CreateInstance());

                return lookup;
            }
            catch (Exception ex)
            {
                log.Error("Error getting documentation classes", ex);
                return new Dictionary<Type, IApiResource>();
            }
        }

        private static IEnumerable<Type> FindAllTypeSpecs(IEnumerable<Type> types)
        {
            var target = typeof (TypeSpec<>);

            var foundTypes = from t in types
                let h = t.GetInheritanceHierarchy().Where(b => b.IsGenericType)
                where
                    t.IsClass && !t.IsAbstract && t.IsVisible &&
                    h.Any(b => target.IsAssignableFrom(b.GetGenericTypeDefinition()))
                select t;

            return foundTypes;
        }
    }
}