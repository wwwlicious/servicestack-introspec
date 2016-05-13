namespace ServiceStack.Documentation.Enrichers.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Extensions;
    using Interfaces;
    using Models;

    /// <summary>
    /// Manages default logic for enriching Properties
    /// </summary>
    public class PropertyEnricherManager
    {
        private readonly IPropertyEnricher propertyEnricher;
        private readonly Action<IApiResourceType, Type> enrichResource;

        public PropertyEnricherManager(IPropertyEnricher propertyEnricher, Action<IApiResourceType, Type> enrichResource)
        {
            this.propertyEnricher = propertyEnricher;
            this.enrichResource = enrichResource;
        }

        public ApiPropertyDocumention[] EnrichParameters(ApiPropertyDocumention[] properties, Type dtoType)
        {
            if (propertyEnricher == null)
                return properties;

            // There might be a collection of Properties already - if so build up an easy lookup
            Dictionary<string, ApiPropertyDocumention> indexedParams;
            List<ApiPropertyDocumention> parameterDocuments = null;
            bool newList = false;

            PropertyInfo[] allProperties = dtoType.GetAllProperties();

            if ((properties == null) || (properties.Length == 0))
            {
                indexedParams = new Dictionary<string, ApiPropertyDocumention>();
                newList = true;
                parameterDocuments = new List<ApiPropertyDocumention>(allProperties.Length);
            }
            else
                indexedParams = properties.ToDictionary(k => k.Id, v => v);

            foreach (var pi in allProperties)
            {
                if (ShouldIgnoreParameter(pi))
                    continue;

                // 1. Check if the property already exists. 
                // If so get it, If not create it 
                ApiPropertyDocumention property;

                // As we look for object by pi.Id, set it if instantiated
                if (!indexedParams.TryGetValue(pi.Name, out property))
                    property = new ApiPropertyDocumention { Id = pi.Name, ClrType = pi.PropertyType };

                // Pass it to method to be populated. Would this be a recursive call?
                EnrichParameter(property, pi);

                // Smelly?
                if (newList)
                    parameterDocuments.Add(property);
            }

            // Do I need to return here?
            return newList ? parameterDocuments.ToArray() : properties;
        }

        private static bool ShouldIgnoreParameter(PropertyInfo pi) => pi.HasAttribute<IgnoreDataMemberAttribute>();

        private void EnrichParameter(ApiPropertyDocumention property, PropertyInfo pi)
        {
            property.Title = property.Title.GetIfNullOrEmpty(() => propertyEnricher.GetTitle(pi));
            property.Description = property.Description.GetIfNullOrEmpty(() => propertyEnricher.GetDescription(pi));
            property.Notes = property.Notes.GetIfNullOrEmpty(() => propertyEnricher.GetNotes(pi));
            property.ParamType = property.ParamType.GetIfNullOrEmpty(() => propertyEnricher.GetParamType(pi));
            property.Contraints = property.Contraints.GetIfNullOrEmpty(() => propertyEnricher.GetContraints(pi));

            property.IsRequired = property.IsRequired.GetIfNoValue(() => propertyEnricher.GetIsRequired(pi));
            property.AllowMultiple = property.AllowMultiple.GetIfNoValue(() => propertyEnricher.GetAllowMultiple(pi));

            property.ExternalLinks = property.ExternalLinks.GetIfNullOrEmpty(() => propertyEnricher.GetExternalLinks(pi));

            // TODO Is this the best way to identify these?
            if (!pi.PropertyType.IsSystemType())
                EnrichEmbeddedResource(property, pi);
        }

        private void EnrichEmbeddedResource(ApiPropertyDocumention property, PropertyInfo pi)
        {
            // Call enrichResource on this type
            if (property.EmbeddedResource == null)
                property.EmbeddedResource = new ApiResourceType();
            enrichResource(property.EmbeddedResource, pi.PropertyType);
        }
    }
}
