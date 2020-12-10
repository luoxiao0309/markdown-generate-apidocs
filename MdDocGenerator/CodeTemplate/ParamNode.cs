using System;
using System.Xml.Serialization;

namespace MdDocGenerator.CodeTemplate
{
    [Serializable]
    [XmlType("param")]
    public class ParamNode
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}