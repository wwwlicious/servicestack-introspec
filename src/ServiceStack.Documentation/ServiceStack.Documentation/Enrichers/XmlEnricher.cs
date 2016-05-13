namespace ServiceStack.Documentation.Enrichers
{
    using System;
    using System.Reflection;
    using Interfaces;
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
        public string GetContraints(PropertyInfo pi) => null;
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