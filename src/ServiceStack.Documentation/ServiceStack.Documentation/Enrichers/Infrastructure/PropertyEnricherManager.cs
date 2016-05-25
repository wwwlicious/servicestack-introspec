// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
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
        private static readonly Dictionary<Type, MemberInfo[]> PropertyDictionary = new Dictionary<Type, MemberInfo[]>();

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

            MemberInfo[] allMembers = GetMemberInfo(dtoType);

            if ((properties == null) || (properties.Length == 0))
            {
                indexedParams = new Dictionary<string, ApiPropertyDocumention>();
                newList = true;
                parameterDocuments = new List<ApiPropertyDocumention>(allMembers.Length);
            }
            else
                indexedParams = properties.ToDictionary(k => k.Id, v => v);

            foreach (var mi in allMembers)
            {
                // 1. Check if the property already exists. 
                // If so get it, If not create it 
                ApiPropertyDocumention property;

                // As we look for object by mi.Id, set it if instantiated
                if (!indexedParams.TryGetValue(mi.Name, out property))
                    property = new ApiPropertyDocumention
                    {
                        Id = mi.Name,
                        ClrType = mi.GetFieldPropertyType()
                    };

                // Pass it to method to be populated. Would this be a recursive call?
                EnrichParameter(property, mi);

                // Smelly?
                if (newList)
                    parameterDocuments.Add(property);
            }

            // Do I need to return here?
            return newList ? parameterDocuments.ToArray() : properties;
        }

        private MemberInfo[] GetMemberInfo(Type dtoType)
        {
            return PropertyDictionary.SafeGetOrInsert(dtoType, () =>
                {
                    var allProperties = dtoType.GetSerializableProperties();
                    var allFields = dtoType.GetSerializableFields();
                    var allMembers = allProperties
                        .Select(p => p as MemberInfo)
                        .Union(allFields.Select(f => f as MemberInfo))
                        .Distinct()
                        .ToArray();
                    return allMembers;
                });
        }

        private void EnrichParameter(ApiPropertyDocumention property, MemberInfo mi)
        {
            property.Title = property.Title.GetIfNullOrEmpty(() => propertyEnricher.GetTitle(mi));
            property.Description = property.Description.GetIfNullOrEmpty(() => propertyEnricher.GetDescription(mi));
            property.Notes = property.Notes.GetIfNullOrEmpty(() => propertyEnricher.GetNotes(mi));
            property.ParamType = property.ParamType.GetIfNullOrEmpty(() => propertyEnricher.GetParamType(mi));
            property.Contraints = property.Contraints.GetIfNull(() => propertyEnricher.GetConstraints(mi));

            property.IsRequired = property.IsRequired.GetIfNoValue(() => propertyEnricher.GetIsRequired(mi));
            property.AllowMultiple = property.AllowMultiple.GetIfNoValue(() => propertyEnricher.GetAllowMultiple(mi));

            property.ExternalLinks = property.ExternalLinks.GetIfNullOrEmpty(() => propertyEnricher.GetExternalLinks(mi));

            // TODO Is this the best way to identify these?
            if (!mi.GetFieldPropertyType().IsSystemType()) // TODO Make this an extension method
                EnrichEmbeddedResource(property, mi);
        }

        private void EnrichEmbeddedResource(ApiPropertyDocumention property, MemberInfo mi)
        {
            // Call enrichResource on this type
            if (property.EmbeddedResource == null)
                property.EmbeddedResource = new ApiResourceType();
            enrichResource(property.EmbeddedResource, mi.GetFieldPropertyType()); // TODO Make this an extension method
        }
    }
}
