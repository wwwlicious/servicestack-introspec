namespace ServiceStack.Documentation.Enrichers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Host;
    using Interfaces;
    using Models;
    using Settings;

    public class ReflectionEnricher : IResourceEnricher, IResponseEnricher, IPropertyEnricher
    {
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
                ? DocumenterSettings.AnyVerbs as string[] ?? DocumenterSettings.AnyVerbs.ToArray()
                : operation.Actions.ToArray();
        }

        public StatusCode[] GetStatusCodes(Operation operation)
        {
            // From [ApiResponse] attributes
            var apiResponseAttributes = operation.RequestType.GetCustomAttributes<ApiResponseAttribute>();
            var responseAttributes = apiResponseAttributes as ApiResponseAttribute[] ?? apiResponseAttributes.ToArray();

            // TODO Only instantiate if needed?
            var list = new List<StatusCode>(responseAttributes.Length + 1);
            if (operation.IsOneWay || HasOneWayMethod(operation.RequestType, operation.ServiceType))
            {
                // add a 204
                list.Add((StatusCode) HttpStatusCode.NoContent);
            }

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

        public string GetTitle(PropertyInfo pi) => GetApiMemberAttribute(pi)?.Name;
        public string GetDescription(PropertyInfo pi) => GetApiMemberAttribute(pi)?.Description;
        public string GetParamType(PropertyInfo pi) => GetApiMemberAttribute(pi)?.ParameterType;
        public bool? GetAllowMultiple(PropertyInfo pi) => GetApiMemberAttribute(pi)?.AllowMultiple;
        public bool? GetIsRequired(PropertyInfo pi) => GetApiMemberAttribute(pi)?.IsRequired;

        public string GetNotes(PropertyInfo pi) => null;
        public string[] GetExternalLinks(PropertyInfo pi) => null;
        public string GetContraints(PropertyInfo pi) => null;

        private static bool HasOneWayMethod(Type requestType, Type serviceType)
        {
            // NOTE This will need investivate to ensure it's not nonsense
            return serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Any(m =>
                    m.ReturnType == typeof (void)
                    && m.GetParameters().Any(p => p.ParameterType == requestType));
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
    }
}