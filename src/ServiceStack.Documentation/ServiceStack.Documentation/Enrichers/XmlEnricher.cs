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

    public class XmlEnricher : IResourceEnricher, IPropertyEnricher
    {
        private readonly IXmlDocumentationLookup lookup;

        public XmlEnricher(IXmlDocumentationLookup lookup)
        {
            this.lookup = lookup;
        }

        public string GetTitle(Type type) => GetXmlMember(type)?.Name;
        public string GetDescription(Type type) => GetDescriptionInternal(type);
        public string GetNotes(Type type) => GetNotesInternal(type);

        public string GetDescription(PropertyInfo pi) => GetDescriptionInternal(pi);
        public string GetNotes(PropertyInfo pi) => GetNotesInternal(pi);

        public string GetTitle(PropertyInfo pi) => GetXmlMember(pi)?.Name;
        public bool? GetAllowMultiple(PropertyInfo pi) => null;
        public string[] GetExternalLinks(PropertyInfo pi) => null;
        public PropertyConstraint GetConstraints(PropertyInfo pi) => null;
        public bool? GetIsRequired(PropertyInfo pi) => null;
        public string GetParamType(PropertyInfo pi) => null;

        private string GetNotesInternal(MemberInfo mi) => GetXmlMember(mi)?.Remarks?.Text;
        private string GetDescriptionInternal(MemberInfo mi) => GetXmlMember(mi)?.Summary?.Text;

        private XmlMember GetXmlMember(MemberInfo mi)
        {
            return lookup?.GetXmlMember(mi);
        }
    }
}