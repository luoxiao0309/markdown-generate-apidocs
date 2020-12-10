using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MdDocGenerator.CodeTemplate
{
    [Serializable]
    [XmlType("member")]
    public class MemberNode
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("summary")]
        public summary Summary { get; set; }
        
        public string Remarks { get; set; }

        [XmlElement("param")]
        public List<ParamNode> ParamList { get; set; }

        [XmlElement("returns")]
        [XmlText]
        public string Returns { get; set; }
    }


    public class summary
    {
        [XmlText]
        public string Value { get; set; }

        [XmlElement("param")]
        public List<ParamNode> Params { get; set; }
    }
}