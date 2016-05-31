// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Extensions;
    using Host;
    using Interfaces;
    using Logging;
    using Models;
    using XmlDocumentation;

    /// <summary>
    /// Enricher that will use XML Documentation comments to enrich object
    /// </summary>
    public class XmlEnricher : IResourceEnricher, IPropertyEnricher, IActionEnricher
    {
        private readonly IXmlDocumentationLookup lookup;
        private readonly ILog log = LogManager.GetLogger(typeof(XmlEnricher));

        public XmlEnricher(IXmlDocumentationLookup lookup)
        {
            this.lookup = lookup;
        }

        public string GetTitle(Type type) => null;
        public string GetDescription(Type type) => GetDescriptionInternal(type);
        public string GetNotes(Type type) => GetNotesInternal(type);

        public string GetDescription(MemberInfo mi)
        {
            var desc = GetDescriptionInternal(mi);
            var value = GetXmlMember(mi)?.Value;

            return string.IsNullOrEmpty(value)
                       ? desc
                       : string.IsNullOrEmpty(desc) ? value : $"{desc} {value}";
        }

        public string GetNotes(MemberInfo mi) => GetNotesInternal(mi);

        public string GetTitle(MemberInfo mi) => null;
        public bool? GetAllowMultiple(MemberInfo mi) => null;
        public string[] GetExternalLinks(MemberInfo mi) => null;
        public PropertyConstraint GetConstraints(MemberInfo mi) => null;
        public bool? GetIsRequired(MemberInfo mi) => null;
        public string GetParamType(MemberInfo mi) => null;
        public string[] GetContentTypes(Operation operation, string verb) => null;
        public string[] GetRelativePaths(Operation operation, string verb) => null;
        public string GetNotes(Operation operation, string verb) => null;

        public StatusCode[] GetStatusCodes(Operation operation, string verb)
        {
            var type = operation.RequestType;

            var xmlMember = lookup.GetXmlMember(type);

            if (xmlMember == null)
            {
                log.Info($"Unable to find XmlMember data for {type.Name}");
                return null;
            }

            var exceptions = xmlMember.Exceptions;
            if (exceptions.IsNullOrEmpty())
                return null;

            var statusCodes = new List<StatusCode>(xmlMember.Exceptions.Length);

            foreach (var exception in exceptions)
            {
                var code = GetStatusCodeFromExceptionName(exception.Reference);
                if (code != null)
                    statusCodes.Add(code.WithDescription(exception.Text));
                else
                    statusCodes.Add((StatusCode) 500);
            }
            return statusCodes.ToArray();
        }

        private string GetNotesInternal(MemberInfo mi) => GetXmlMember(mi)?.Remarks?.Text;
        private string GetDescriptionInternal(MemberInfo mi) => GetXmlMember(mi)?.Summary?.Text;
        private XmlMember GetXmlMember(MemberInfo mi) => lookup?.GetXmlMember(mi);

        private static StatusCode GetStatusCodeFromExceptionName(string exceptionName)
        {
            // Exception name will start "T:*" as from XML Comments
            var name = exceptionName.Substring(2);
            var type = Type.GetType(name);

            var exception = type?.CreateInstance();
            if (!(exception is Exception))
                return null;

            var statusCode = (exception as Exception).ToStatusCode();
            return (StatusCode) statusCode;
        }
    }
}