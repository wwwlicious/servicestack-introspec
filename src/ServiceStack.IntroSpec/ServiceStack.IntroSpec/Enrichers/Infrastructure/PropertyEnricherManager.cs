// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers.Infrastructure
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
        private readonly Action<IApiResourceType, ResourceModel> enrichResource;
        private static readonly Dictionary<Type, MemberInfo[]> PropertyDictionary = new Dictionary<Type, MemberInfo[]>();

        public PropertyEnricherManager(IPropertyEnricher propertyEnricher, Action<IApiResourceType, ResourceModel> enrichResource)
        {
            this.propertyEnricher = propertyEnricher;
            this.enrichResource = enrichResource;
        }

        public ApiPropertyDocumentation[] EnrichParameters(ApiPropertyDocumentation[] properties, ResourceModel resource)
        {
            if (propertyEnricher == null)
                return properties;

            // There might be a collection of Properties already - if so build up an easy lookup
            Dictionary<string, ApiPropertyDocumentation> indexedParams;
            List<ApiPropertyDocumentation> parameterDocuments = null;
            var newList = false;

            // If the type is collection use the element rather than collection type (e.g. if string[] use System.String, not System.Array)
            var resourceType = resource.ResourceType;
            if (resourceType.OriginalType.IsCollection())
                resourceType = resourceType.OriginalType.GetEnumerableType().ToApiClrType();

            var allMembers = GetMemberInfo(resourceType.OriginalType);

            if (properties.IsNullOrEmpty())
            {
                //? Make this static to avoid needing to populate multiple times
                indexedParams = new Dictionary<string, ApiPropertyDocumentation>();
                newList = true;
                parameterDocuments = new List<ApiPropertyDocumentation>(allMembers.Length);
            }
            else
                indexedParams = properties.ToDictionary(k => k.Id, v => v);

            foreach (var mi in allMembers)
            {
                // Check if the property already exists. if so get it, If not create it 
                var property = indexedParams.SafeGet(mi.Name, () =>
                    new ApiPropertyDocumentation
                    {
                        Id = mi.Name,
                        ClrType = mi.GetFieldPropertyType().ToApiClrType()
                    });

                // Pass it to method to be populated.
                EnrichParameter(property, mi, resource);

                if (newList)
                    parameterDocuments.Add(property);
            }

            // Do I need to return here?
            return newList ? parameterDocuments.ToArray() : properties;
        }

        private MemberInfo[] GetMemberInfo(Type dtoType)
        {
            // The same properties will be required multiple times (per enricher type) so build lookup
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

        private void EnrichParameter(ApiPropertyDocumentation property, MemberInfo mi, ResourceModel resource)
        {
            if (property.Title == property.Id || string.IsNullOrEmpty(property.Title))
                property.Title = propertyEnricher.GetTitle(mi);

            property.Description = property.Description.GetIfNullOrEmpty(() => propertyEnricher.GetDescription(mi));
            property.Notes = property.Notes.GetIfNullOrEmpty(() => propertyEnricher.GetNotes(mi));
            property.ParamType = property.ParamType.GetIfNullOrEmpty(() => propertyEnricher.GetParamType(mi));
            property.Constraints = property.Constraints.GetIfNull(() => propertyEnricher.GetConstraints(mi));

            property.IsRequired = property.IsRequired.GetIfNoValue(() => propertyEnricher.GetIsRequired(mi));

            if (resource.IsRequest)
            {
                if (mi.GetFieldPropertyType().IsCollection())
                    property.AllowMultiple = true;
                else
                    property.AllowMultiple =
                        property.AllowMultiple.GetIfNoValue(
                            () => propertyEnricher.GetAllowMultiple(mi));
            }
            else
            {
                property.IsCollection = mi.GetFieldPropertyType().IsCollection();
            }
            property.ExternalLinks = property.ExternalLinks.GetIfNullOrEmpty(() => propertyEnricher.GetExternalLinks(mi));

            EnrichEmbeddedResource(property, mi, resource);
        }

        private void EnrichEmbeddedResource(ApiPropertyDocumentation property, MemberInfo mi, ResourceModel resource)
        {
            var fieldPropertyType = mi.GetFieldPropertyType();
            if (!ShouldPopulateEmbeddedResource(fieldPropertyType, resource.ResourceType.OriginalType))
                return;

            if (property.EmbeddedResource == null)
                property.EmbeddedResource = ApiResourceType.Create(fieldPropertyType);

            enrichResource(property.EmbeddedResource, new ResourceModel(fieldPropertyType, resource.IsRequest));
        }

        // Avoid populating if SystemType, enum or embedded property type == parent resource type
        private bool ShouldPopulateEmbeddedResource(Type embeddedType, Type resourceType)
            => !((embeddedType.IsGenericType ? embeddedType.IsSystemType() && embeddedType.GenericTypeArguments.All(t => t.IsSystemType()) : embeddedType.IsSystemType()) || embeddedType.IsEnum || embeddedType == resourceType);

    }
}
