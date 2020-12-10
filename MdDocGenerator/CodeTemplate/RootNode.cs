using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using MdDocGenerator.Util;

namespace MdDocGenerator.CodeTemplate
{
    [Serializable]
    [XmlRoot("doc")]
    public class RootNode : XmlSerializerHelper.IDeserializeExtend<RootNode>
    {
        [XmlElement("assembly")]
        public AssemblyNode Assembly { get; set; }

        [XmlArrayItem("member")]
        [XmlArray("members")]
        public List<MemberNode> MemberList { get; set; }

        public bool CheckDeserialize(RootNode obj)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public RootNode AfterDeserialize(RootNode obj, string xmlContent)
        {
            var member = obj?.MemberList;

            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            var remarksNodeList = doc.SelectNodes("/doc/members/member/remarks")?.GetEnumerator();

            if (member != null)
            {
                foreach (var node in member)
                {
                    if (node.Name == "M:Mysoft.Slxt.TradeMng.AppServices.SignAgreementAppService.ChooseRoomTurnOrder(Mysoft.Slxt.TradeMng.Model.DTO.SubscribeSaveDTO,System.Guid)")
                    {

                    }
                    var paramsList = node.ParamList ?? new List<ParamNode>();

                    var summaryParams = node.Summary?.Params;
                    if (summaryParams != null)
                    {
                        paramsList.AddRange(summaryParams);
                    }

                    node.ParamList = paramsList;
                }
            }

            if (remarksNodeList == null) return obj;
            while (remarksNodeList.MoveNext())
            {
                XmlElement itemNav = (XmlElement)remarksNodeList.Current;
                var nodeName = ((XmlElement)itemNav.ParentNode).GetAttribute("name");
                var node = member?.FirstOrDefault(p => p.Name == nodeName);
                if (node != null)
                {
                    node.Remarks = itemNav.InnerXml;
                }
            }
            return obj;
        }
    }
}
