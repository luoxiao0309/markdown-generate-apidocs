using System;
using System.Xml.Serialization;

namespace MdDocGenerator.CodeTemplate
{
    [Serializable]
    [XmlType("assembly")]
    public class AssemblyNode
    {
        [XmlElement("name")]
        [XmlText]
        public string Name { get; set; }
    }
}