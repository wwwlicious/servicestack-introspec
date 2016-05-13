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
                var lookup = FindAllTypes()
                    .ToDictionary(k => k.BaseType.GenericTypeArguments[0],
                        v => (IApiResource) Activator.CreateInstance(v));

                return lookup;
            }
            catch (Exception ex)
            {
                log.Error("Error getting documentation classes", ex);
                return new Dictionary<Type, IApiResource>();
            }
        }

        private static IEnumerable<Type> FindAllTypes()
        {
            var target = typeof (TypeSpec<>);

            var foundTypes = from t in DocumenterSettings.Assemblies.SelectMany(a => a.GetTypes())
                let h = t.GetInheritanceHierarchy().Where(b => b.IsGenericType)
                where
                    t.IsClass && !t.IsAbstract && t.IsVisible &&
                    h.Any(b => target.IsAssignableFrom(b.GetGenericTypeDefinition()))
                select t;

            return foundTypes;
        }
    }
}