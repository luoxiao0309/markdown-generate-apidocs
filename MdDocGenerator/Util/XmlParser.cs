using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MdDocGenerator.CodeTemplate;

namespace MdDocGenerator.Util
{
    /// <summary>
    /// Xml解析器
    /// </summary>
    public class XmlParser
    {
        private static readonly Dictionary<string, string> XmlFiles = new Dictionary<string, string>();

        public List<RootNode> LoadXml(List<string> dllNames)
        {
            var xmlMembers = new List<RootNode>();
            dllNames.ForEach(o=>GetXmlFiles(o));
            if (!XmlFiles.Any()) return xmlMembers;
            foreach (var key in XmlFiles.Keys)
            {
                xmlMembers.Add(XmlSerializerHelper.Deserialize<RootNode>(XmlFiles[key]));
            }

            return xmlMembers;
        }

        private void GetXmlFiles(string dll)
        {
            try
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
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}