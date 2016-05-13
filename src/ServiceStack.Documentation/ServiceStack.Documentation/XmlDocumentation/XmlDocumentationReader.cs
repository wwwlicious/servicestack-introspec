namespace ServiceStack.Documentation.XmlDocumentation
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;
    using Logging;

    public class XmlDocumentationReader : IXmlDocumentationReader
    {
        private readonly ILog log = LogManager.GetLogger(typeof(XmlDocumentationReader));

        public XmlDocumentation GetXmlDocumentation()
        {
            try
            {
                var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                log.Debug($"Attempting to read {assemblyName} XML file");
                using (var contents = File.OpenText($"{assemblyName}.xml"))
                {
                    var serializer = new XmlSerializer(typeof(XmlDocumentation));
                    return (XmlDocumentation)serializer.Deserialize(contents);
                }
            }
            catch (Exception e)
            {
                log.Error($"Error getting xml documentation.", e);
                return null;
            }
        }
    }
}