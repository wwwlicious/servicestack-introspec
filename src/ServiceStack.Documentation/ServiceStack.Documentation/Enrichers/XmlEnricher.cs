// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers
{
    using System;
    using System.Reflection;
    using Interfaces;
    using Models;
    using XmlDocumentation;

    /// <summary>
    /// Enricher that will use XML Documentation comments to enrich object
    /// </summary>
    public class XmlEnricher : IResourceEnricher, IPropertyEnricher
    {
        private readonly IXmlDocumentationLookup lookup;

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

        private string GetNotesInternal(MemberInfo mi) => GetXmlMember(mi)?.Remarks?.Text;
        private string GetDescriptionInternal(MemberInfo mi) => GetXmlMember(mi)?.Summary?.Text;

        private XmlMember GetXmlMember(MemberInfo mi)
        {
            return lookup?.GetXmlMember(mi);
        }
    }
}