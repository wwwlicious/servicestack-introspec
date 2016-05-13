namespace ServiceStack.Documentation.XmlDocumentation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Logging;

    public class XmlDocumentationLookup : IXmlDocumentationLookup
    {
        private readonly ILog log = LogManager.GetLogger(typeof(XmlDocumentationLookup));
        private readonly Dictionary<string, XmlMember> lookup;

        public XmlDocumentationLookup(IXmlDocumentationReader documentationReader)
        {
            documentationReader.ThrowIfNull();

            lookup = PopulateLookup(documentationReader.GetXmlDocumentation());
        }

        public XmlMember GetXmlMember(MemberInfo member)
        {
            if (lookup == null)
                return XmlMember.Default;

            // Get the specific lookup name
            var name = member.GetMemberElementName();

            // Lookup the value
            XmlMember xmlMember;
            return lookup.TryGetValue(name, out xmlMember) ? xmlMember : XmlMember.Default;
        }

        private Dictionary<string, XmlMember> PopulateLookup(XmlDocumentation xmlDocumentation)
        {
            if (xmlDocumentation?.Members == null)
            {
                log.Info("No xml documentation available.");
                return null;
            }

            return xmlDocumentation.Members.ToDictionary(k => k.Name, v => v);
        }
    }
}