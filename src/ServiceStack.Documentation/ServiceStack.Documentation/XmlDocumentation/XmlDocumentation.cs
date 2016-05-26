// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.XmlDocumentation
{
    // TODO Beef up the comments for all of this.
    using System.Xml.Serialization;

    [XmlRoot("doc")]
    public class XmlDocumentation
    {
        [XmlArray("members")]
        public XmlMember[] Members { get; set; }
    }

    [XmlType("member")]
    public class XmlMember : XmlBase
    {
        /// <summary>
        /// A summary of the object
        /// </summary>
        [XmlElement(ElementName = "summary")]
        public XmlBase Summary { get; set; }

        /// <summary>
        /// A description of the return value
        /// </summary>
        [XmlElement(ElementName = "returns")]
        public string Returns { get; set; }

        /// <summary>
        /// Any exceptions that can be returned from this method
        /// </summary>
        [XmlElement(ElementName = "exception")]
        public XmlHasCref[] Exceptions { get; set; }

        /// <summary>
        /// Any Properties for method
        /// </summary>
        [XmlElement(ElementName = "param")]
        public MethodParameter Parameters { get; set; }

        /// <summary>
        /// A description of the member (in addition to summary)
        /// </summary>
        [XmlElement(ElementName = "remarks")]
        public XmlBase Remarks { get; set; }

        /// <summary>
        /// A description of the code sample
        /// </summary>
        [XmlElement(ElementName = "example")]
        public CodeExample Example { get; set; }

        [XmlElement(ElementName = "value")]
        public string Value { get; set; }

        [XmlElement(ElementName = "permission")]
        public XmlHasCref[] Permissions { get; set; }

        [XmlElement(ElementName = "typeparam")]
        public XmlBase[] TypeParam { get; set; }

        [XmlElement(ElementName = "include")]
        public XmlInclude[] Includes { get; set; }

        private static XmlMember privateDefault;

        public static XmlMember Default
        {
            get
            {
                if (privateDefault != null)
                    privateDefault = new XmlMember();

                return privateDefault;
            }
        } 
    }

    public class XmlHasCref
    {
        /// <summary>
        /// A reference to an exception that is available from the current compilation environment
        /// </summary>
        [XmlAttribute("cref")]
        public string Reference { get; set; }

        /// <summary>
        /// A description of the exception
        /// </summary>
        [XmlText]
        public string Text { get; set; }
    }

    [XmlType("param")]
    public class MethodParameter
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// A description of the paramter
        /// </summary>
        [XmlText]
        public string Text { get; set; }
    }

    [XmlType("example")]
    public class CodeExample : XmlBase
    {

        [XmlElement(ElementName = "code")]
        public string Code { get; set; }
    }

    public class See
    {
        [XmlAttribute("cref")]
        public string Cref { get; set; }
    }

    public class TermDesc
    {
        [XmlElement(ElementName = "term")]
        public string Term { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
    }

    [XmlType("list")]
    public class List
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "listheader")]
        public TermDesc ListHeader { get; set; }

        [XmlElement(ElementName = "item")]
        public TermDesc[] Item { get; set; }
    }

    public class XmlInclude
    {
        [XmlAttribute("file")]
        public string File { get; set; }

        [XmlAttribute("path")]
        public string Path { get; set; }
    }

    // This contains all the stuff that can be embedded
    public class XmlBase
    {
        private string text;

        /// <summary>
        /// A description of the example
        /// </summary>
        [XmlText]
        public string Text
        {
            get { return text?.TrimEnd(' '); }
            set { text += $"{Sanitize(value)} "; }
        }

        [XmlElement(ElementName = "see")]
        public See See { get; set; }

        [XmlElement(ElementName = "seealso")]
        public See SeeAlso { get; set; }

        [XmlElement(ElementName = "typeparamref")]
        public XmlBase TypeParamRef { get; set; }

        [XmlElement(ElementName = "paramref")]
        public XmlBase ParamRef { get; set; }

        [XmlElement(ElementName = "c")]
        public string[] RenderCode { get; set; }

        [XmlElement(ElementName = "para")]
        public XmlBase Paragraph { get; set; }

        [XmlElement(ElementName = "list")]
        public List List { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        private string Sanitize(string str)
        {
            return str.TrimStart('\n', ' ').TrimEnd('\n', ' ');
        }
    }
}
