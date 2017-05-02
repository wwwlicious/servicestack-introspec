// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Extensions;
    using FluentValidation;
    using Host;
    using Interfaces;
    using Logging;
    using Models;
    using Utilities;

    /// <summary>
    /// Enricher that will use Reflection to enrich object. 
    /// </summary>
    public class ReflectionEnricher : IResourceEnricher, IRequestEnricher, IPropertyEnricher, ISecurityEnricher, IActionEnricher
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ReflectionEnricher));

        private readonly Dictionary<string, ApiMemberAttribute> apiMemberLookup;

        public ReflectionEnricher()
        {
            apiMemberLookup = new Dictionary<string, ApiMemberAttribute>();
        }

        public string GetTitle(Type type) => type.IsCollection() ? type.GetEnumerableType()?.Name : type.Name;
        public string GetDescription(Type type) => type.GetDescription();
        public string GetTitle(MemberInfo mi) => GetApiMemberAttribute(mi)?.Name;
        public string GetDescription(MemberInfo mi) => GetApiMemberAttribute(mi)?.Description;
        public string GetParamType(MemberInfo mi) => GetApiMemberAttribute(mi)?.ParameterType;
        public bool? GetAllowMultiple(MemberInfo mi) => GetApiMemberAttribute(mi)?.AllowMultiple;

        public bool? GetIsRequired(MemberInfo mi)
        {
            var apiMemberAttribute = GetApiMemberAttribute(mi);

            if (apiMemberAttribute != null)
                return apiMemberAttribute.IsRequired;

            return mi.GetFieldPropertyType().IsNullableType() ? false : (bool?) null;
        }

        public bool? GetAllowMultiple(Type type) => type.IsCollection() ? true : (bool?)null;

        public bool? GetHasValidator(Type type)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(type);
            return HostContext.Container.TryResolve(validatorType) != null;
        }

        public string GetNotes(Type type) => null;
        public string GetCategory(Operation operation) => null;
        public string[] GetTags(Operation operation) => null;
        public string GetNotes(MemberInfo mi) => null;
        public string[] GetExternalLinks(MemberInfo mi) => null;

        public PropertyConstraint GetConstraints(MemberInfo mi)
        {
            var propertyType = mi.GetFieldPropertyType();
            if (propertyType.IsEnum)
            {
                log.Debug($"{mi.Name} is of enum type {propertyType.Name}. Generating List constraint.");
                return PropertyConstraint.ListConstraint(propertyType.Name, propertyType.GetEnumNames());
            }

            var allowableValues = mi.FirstAttribute<ApiAllowableValuesAttribute>();
            if (allowableValues == null)
                return null;

            var constraint = allowableValues.Type == "LIST"
                                 ? PropertyConstraint.ListConstraint(allowableValues.Name, allowableValues.Values)
                                 : PropertyConstraint.RangeConstraint(allowableValues.Name, allowableValues.Min,
                                     allowableValues.Max);

            log.Debug($"Created {allowableValues.Type} constraint for property {mi.Name}");
            return constraint;
        }

        public ApiSecurity GetSecurity(Operation operation, string verb)
        {
            if (!operation.RequiresAuthentication)
                return null;

            // Check if [Authenticate] is for current verb
            var hasAuth = operation.AuthenticationAppliesForVerb(verb);

            // check Role + Permission specific for this verb
            var roles = operation.GetRoles(verb);
            var permissions = operation.GetPermissions(verb);

            // If no auth and there are no perms/roles for verb then null
            if (VerbHasSecurityRestrictions(hasAuth, roles, permissions))
                return null;

            var apiSecurity = new ApiSecurity
            {
                IsProtected = true,
                Roles = roles,
                Permissions = permissions
            };

            return apiSecurity;
        }

        public string[] GetContentTypes(Operation operation, string verb)
        {
            // Get a list of all available formats
            var availableFormats = HostContext.MetadataPagesConfig.AvailableFormatConfigs.Select(a => a.Format);

            // NOTE Restriction can come from either DTO or Service
            var restrictedTo = operation.RestrictTo;
            var requestType = operation.RequestType;

            var mimeTypes = new List<string>();
            foreach (var format in availableFormats.Select(s => s.TrimStart("x-")))
            {
                // Verify RestrictAttribute not preventing access
                var requestAttrResult = EnumUtilities.SafeParse<RequestAttributes>(format);
                if (!restrictedTo.CanAccess(requestAttrResult)) continue;

                // Verify ExcludeAttribute not preventing access
                var featureResult = EnumUtilities.SafeParse<Feature>(format);
                if (requestType.HasAccessToFeature(featureResult))
                    mimeTypes.Add(MimeTypeUtilities.GetMimeType(format));
            }

            ProcessAddHeaderAttribute(requestType, mimeTypes);

            return mimeTypes.Distinct().ToArray();
        }

        public RelativePath[] GetRelativePaths(Operation operation, string verb)
        {
            var requestType = operation.RequestType;
            var routeAttributes = GetRouteAttributesForVerb(requestType, verb)
                .Where(r => !string.IsNullOrEmpty(r.Path)).ToList();

            if (!routeAttributes.IsNullOrEmpty())
                return routeAttributes.Select(
                    r =>
                    new RelativePath
                    {
                        Path = r.Path,
                        Source = GetRelativePathSource(r)
                    }).ToArray();

            // If no route then make one up
            var emptyType = requestType.CreateInstance();
            var route = operation.IsOneWay ? emptyType.ToOneWayUrlOnly() : emptyType.ToReplyUrlOnly();
            return new[] { new RelativePath { Path = route, Source = Constants.RouteSources.AutoRoute } };
        }

        private static string GetRelativePathSource(RouteAttribute r)
        {
            return r is FallbackRouteAttribute
                       ? Constants.RouteSources.FallbackRoute
                       : Constants.RouteSources.Attribute;
        }

        public StatusCode[] GetStatusCodes(Operation operation, string verb)
        {
            // From [ApiResponse] attributes
            var apiResponseAttributes = operation.RequestType.GetCustomAttributes<ApiResponseAttribute>();
            var responseAttributes = apiResponseAttributes as ApiResponseAttribute[] ?? apiResponseAttributes.ToArray();

            // NOTE +3 as could have 204, 401 and a 403 in addition to those set
            var list = new List<StatusCode>(responseAttributes.Length + 3);
            if (HasOneWayMethod(operation, verb))
            {
                log.Debug($"Operation for request {operation.RequestType.Name} has void return. Adding 204 response.");
                list.Add((StatusCode)HttpStatusCode.NoContent);
            }

            if (GetSecurity(operation, verb) != null)
            {
                log.Debug($"Operation for request {operation.RequestType.Name} requires auth. Adding 401 + 403 response.");
                list.Add((StatusCode)401);
                list.Add((StatusCode)403); // Should 403 only be returned if there's a Permission or Role restriction?
            }

            if (responseAttributes.Length == 0) return list.ToArray();

            foreach (var apiResponseAttribute in responseAttributes)
            {
                var statusCode = (StatusCode)apiResponseAttribute.StatusCode;
                statusCode.Description = apiResponseAttribute.Description;
                list.Add(statusCode);
            }

            return list.ToArray();
        }

        public string GetNotes(Operation operation, string verb)
        {
            var requestType = operation.RequestType;
            var routeAttributes = GetRouteAttributesForVerb(requestType, verb)
                .Where(r => !string.IsNullOrEmpty(r.Notes)).ToList();

            return routeAttributes.IsNullOrEmpty() ? null : string.Join(". ", routeAttributes.Select(r => r.Notes));
        }

        private static bool HasOneWayMethod(Operation operation, string verb)
        {
            if (operation.IsOneWay)
                return true;

            var voidOperations = GetVoidReturningServiceOperations(operation);

            return !voidOperations.IsNullOrEmpty() && voidOperations.Contains(verb, StringComparer.OrdinalIgnoreCase);
        }

        private ApiMemberAttribute GetApiMemberAttribute(MemberInfo pi)
        {
            ApiMemberAttribute attr;
            var piName = GetPropertyInfoName(pi);

            if (!apiMemberLookup.TryGetValue(piName, out attr))
            {
                attr = pi.FirstAttribute<ApiMemberAttribute>();
                if (attr == null)
                    return null;

                apiMemberLookup.Add(piName, attr);
            }

            return attr;
        }

        private string GetPropertyInfoName(MemberInfo pi) => $"{pi.DeclaringType?.FullName}.{pi.Name}";

        private static void ProcessAddHeaderAttribute(Type requestType, List<string> mimeTypes)
        {
            var addHeader = requestType.FirstAttribute<AddHeaderAttribute>();
            if (addHeader == null)
                return;

            // If [AddHeader] present then add that content-type
            var contentType = addHeader.ContentType ?? addHeader.DefaultContentType;
            if (!string.IsNullOrEmpty(contentType))
                mimeTypes.Add(contentType);
        }

        private static bool VerbHasSecurityRestrictions(bool hasAuth, Permissions roles, Permissions permissions)
            => !hasAuth && (roles == null) && (permissions == null);

        private static List<string> GetVoidReturningServiceOperations(Operation operation)
        {
            return operation.ServiceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .Where(m =>
                                   m.ReturnType == typeof(void)
                                   && m.GetParameters().Any(p => p.ParameterType == operation.RequestType))
                            .Select(o => o.Name).ToList();
        }

        private static IEnumerable<RouteAttribute> GetRouteAttributesForVerb(Type requestType, string verb)
        {
            var routeAttributes = requestType.GetCustomAttributes<RouteAttribute>().Where(
                r => r.IsForVerb(verb));
            return routeAttributes;
        }
    }
}