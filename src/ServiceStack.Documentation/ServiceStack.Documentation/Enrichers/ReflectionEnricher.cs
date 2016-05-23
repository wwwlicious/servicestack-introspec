// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Extensions;
    using Host;
    using Interfaces;
    using Logging;
    using Models;
    using Settings;

    public class ReflectionEnricher : IResourceEnricher, IRequestEnricher, IPropertyEnricher
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ReflectionEnricher));

        // ConcurrentDictionary?
        private readonly Dictionary<string, ApiMemberAttribute> apiMemberLookup;

        public ReflectionEnricher()
        {
            apiMemberLookup = new Dictionary<string, ApiMemberAttribute>();
        }

        public string GetTitle(Type type) => type.Name;

        // [Api] then [ComponentModel.Description] then [DataAnnotations.Description]
        public string GetDescription(Type type) => type.GetDescription();

        public string GetNotes(Type type) => type.FirstAttribute<RouteAttribute>()?.Notes;

        public string[] GetVerbs(Operation operation)
        {
            return operation.Actions.Contains("ANY")
                ? DocumenterSettings.ReplacementVerbs as string[] ?? DocumenterSettings.ReplacementVerbs.ToArray()
                : operation.Actions.ToArray();
        }

        public string[] GetContentTypes(Operation operation)
        {
            var type = operation.RequestType;

            var contentTypes = new List<string>(9);

            if (type.HasXmlSupport()) contentTypes.Add(MimeTypes.Xml);
            if (type.HasJsonSupport()) contentTypes.Add(MimeTypes.Json);
            if (type.HasJsvSupport()) contentTypes.Add(MimeTypes.Jsv);
            if (type.HasSoap11Support()) contentTypes.Add(MimeTypes.Soap11);
            if (type.HasSoap12Support()) contentTypes.Add(MimeTypes.Soap12);
            if (type.HasCsvSupport()) contentTypes.Add(MimeTypes.Csv);
            if (type.HasHtmlSupport()) contentTypes.Add(MimeTypes.Html);
            if (type.HasProtoBufSupport()) contentTypes.Add(MimeTypes.ProtoBuf);
            if (type.HasMsgPackSupport()) contentTypes.Add(MimeTypes.MsgPack);

            return contentTypes.ToArray();
        }

        public StatusCode[] GetStatusCodes(Operation operation)
        {
            // From [ApiResponse] attributes
            var apiResponseAttributes = operation.RequestType.GetCustomAttributes<ApiResponseAttribute>();
            var responseAttributes = apiResponseAttributes as ApiResponseAttribute[] ?? apiResponseAttributes.ToArray();

            // TODO Only instantiate if needed?
            var list = new List<StatusCode>(responseAttributes.Length + 1);
            if (HasOneWayMethod(operation))
                // add a 204
                list.Add((StatusCode) HttpStatusCode.NoContent);

            if (responseAttributes.Length > 0)
            {
                foreach (var apiResponseAttribute in responseAttributes)
                {
                    var statusCode = (StatusCode) apiResponseAttribute.StatusCode;
                    statusCode.Description = apiResponseAttribute.Description;
                    list.Add(statusCode);
                }
            }

            return list.ToArray();
        }

        public string GetRelativePath(Operation operation)
        {
            // TODO Handle multiple routes for same DTO. Anything accessing [Route] will be affected
            var requestType = operation.RequestType;

            // NOTE Routes aren't populated until service call. Get directly from [Route]
            var routeFromAttribute = requestType.FirstAttribute<RouteAttribute>()?.Path;
            if (!string.IsNullOrWhiteSpace(routeFromAttribute))
                return routeFromAttribute;

            // If no route then make one up
            return operation.IsOneWay ? ToOneWayUrlOnly(requestType) : ToReplyUrlOnly(requestType);
        }

        public string GetCategory(Operation operation) => null;
        public string[] GetTags(Operation operation) => null;

        public string GetTitle(PropertyInfo pi) => GetApiMemberAttribute(pi)?.Name;
        public string GetDescription(PropertyInfo pi) => GetApiMemberAttribute(pi)?.Description;
        public string GetParamType(PropertyInfo pi) => GetApiMemberAttribute(pi)?.ParameterType;
        public bool? GetAllowMultiple(PropertyInfo pi) => GetApiMemberAttribute(pi)?.AllowMultiple;
        public bool? GetIsRequired(PropertyInfo pi) => GetApiMemberAttribute(pi)?.IsRequired;

        public string GetNotes(PropertyInfo pi) => null;
        public string[] GetExternalLinks(PropertyInfo pi) => null;
        public string GetContraints(PropertyInfo pi) => null;

        private static bool HasOneWayMethod(Operation operation)
        {
            if (operation.IsOneWay)
                return true;

            return operation.ServiceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Any(m =>
                    m.ReturnType == typeof (void)
                    && m.GetParameters().Any(p => p.ParameterType == operation.RequestType));
        }

        private ApiMemberAttribute GetApiMemberAttribute(PropertyInfo pi)
        {
            ApiMemberAttribute attr;
            var piName = GetPiName(pi);

            if (!apiMemberLookup.TryGetValue(piName, out attr))
            {
                attr = pi.FirstAttribute<ApiMemberAttribute>();
                if (attr == null)
                    return null;

                apiMemberLookup.Add(piName, attr);
            }

            return attr;
        }

        // Move this
        private string GetPiName(PropertyInfo pi)
        {
            return $"{pi.DeclaringType.FullName}.{pi.Name}";
        }

        // TODO Find a better way of getting routes. Fallback routes. Changed predefined etc
        // Based on method of same name from ServiceStack UrlExtensions
        private string ToOneWayUrlOnly(Type dtoType, string format = "json")
            => $"/{format}/oneway/{UrlExtensions.GetOperationName(dtoType)}";

        // Based on method of same name from ServiceStack UrlExtensions
        private string ToReplyUrlOnly(Type dtoType, string format = "json")
            => $"/{format}/reply/{UrlExtensions.GetOperationName(dtoType)}";
    }
}