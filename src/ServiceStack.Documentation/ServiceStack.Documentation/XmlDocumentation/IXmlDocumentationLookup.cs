namespace ServiceStack.Documentation.XmlDocumentation
{
    using System.Reflection;

    public interface IXmlDocumentationLookup
    {
        XmlMember GetXmlMember(MemberInfo member);
    }
}