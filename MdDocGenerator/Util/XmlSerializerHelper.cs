using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MdDocGenerator.Util
{
    /// <summary>
    /// Xml序列化功能
    /// </summary>
    public static class XmlSerializerHelper
    {
        /// <summary>
        /// 按路径加载xml文件
        /// </summary>
        /// <typeparam name="TXmlType"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TXmlType DeserializeFromFile<TXmlType>(string filePath)
        {
            using (var fr = new FileStream(filePath, FileMode.Open))
            {
                var sr = new StreamReader(fr);
                return Deserialize<TXmlType>(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// 反序列化Xml对象
        /// </summary>
        /// <typeparam name="TXmlType"></typeparam>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        public static TXmlType Deserialize<TXmlType>(string xmlContent)
        {
            using (var xmlContentSr = new StringReader(xmlContent))
            {
                using (var sr = new XmlTextReader(xmlContentSr))
                {
                    var xmlSr = new XmlSerializer(typeof(TXmlType));
                    if (!xmlSr.CanDeserialize(sr))
                    {
                        throw new System.Exception("配置文件序列化失败！");
                    }

                    var xmlObj = (TXmlType)xmlSr.Deserialize(sr);
                    if (xmlObj is IDeserializeExtend<TXmlType> extendDeserialize)
                    {
                        if (extendDeserialize.CheckDeserialize(xmlObj))
                        {
                            return extendDeserialize.AfterDeserialize(xmlObj, xmlContent);
                        }

                        return default(TXmlType);
                    }

                    return xmlObj;
                }
            }
        }

        /// <summary>
        /// 序列化Xml对象
        /// </summary>
        /// <typeparam name="TXmlObj"></typeparam>
        /// <param name="xmlContent"></param>
        /// <param name="fileName"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Serializer<TXmlObj>(TXmlObj xmlContent, string fileName, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = "    ";

            // 不生成声明头
            settings.OmitXmlDeclaration = true;

            // 强制指定命名空间，覆盖默认的命名空间。
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var xmlContentSr = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(xmlContentSr, encoding??Encoding.UTF8))
                {
                    var xmlSr = new XmlSerializer(typeof(TXmlObj));
                    xmlSr.Serialize(writer, xmlContent, namespaces);
                    writer.Flush();
                }

                using (var r = new StreamReader(xmlContentSr))
                {
                    return r.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 反序列化扩展
        /// </summary>
        public interface IDeserializeExtend<TXmlObj>
        {
            /// <summary>
            /// 反序列化后
            /// </summary>
            /// <typeparam name="TXmlObj"></typeparam>
            /// <param name="obj"></param>
            bool CheckDeserialize(TXmlObj obj);

            /// <summary>
            /// 反序列化后
            /// </summary>
            /// <typeparam name="TXmlObj"></typeparam>
            /// <param name="obj"></param>
            TXmlObj AfterDeserialize(TXmlObj obj,string xmlContent);
        }
    }
}