using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MdDocGenerator.CodeTemplate;
using MdDocGenerator.Extensions;

namespace MdDocGenerator.Util
{
    public static class XmlDocCommentHelper
    {
        private static readonly Dictionary<string, string> XmlFiles = new Dictionary<string, string>();

        public static List<RootNode> LoadXmlNode(List<string> dlls)
        {
            var xmlMembers = new List<RootNode>();
            dlls.ForEach(GetXmlFiles);
            if (!XmlFiles.Any()) return xmlMembers;
            foreach (var key in XmlFiles.Keys)
            {
                xmlMembers.Add(XmlSerializerHelper.Deserialize<RootNode>(XmlFiles[key]));
            }

            return xmlMembers;
        }

        public static Dictionary<string, MemberNode> LoadMemberNodeToDictionary(List<string> dlls)
        {
            Dictionary<string, MemberNode> memberNodes = new Dictionary<string, MemberNode>();
            var xmlData = LoadXmlNode(dlls);
            foreach (var rootNode in xmlData)
            {
                if (!rootNode.MemberList.Any()) continue;
                foreach (var memberNode in rootNode.MemberList)
                {
                    memberNodes.TryAdd(memberNode.Name, memberNode);
                }
            }

            return memberNodes;
        }

        private static void GetXmlFiles(string dll)
        {
            var xmlPath = Path.Combine(Directory.GetParent(dll).FullName, Path.GetFileNameWithoutExtension(dll) + ".xml");
            if (File.Exists(xmlPath) == false)
            {
                return;
            }
            using (var stream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(stream))
            {
                XmlFiles.Add(xmlPath, sr.ReadToEnd());
            }
        }
    }
}