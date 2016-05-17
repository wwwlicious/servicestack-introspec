// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

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