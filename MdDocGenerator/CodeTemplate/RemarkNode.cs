using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MdDocGenerator.CodeTemplate
{
    [Serializable]
    [XmlType("remarks")]
    public class RemarkNode
    {
        [XmlText]
        public string Value { get; set; }

    }
}