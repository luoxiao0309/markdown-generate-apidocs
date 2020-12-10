using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MdDocGenerator.Util
{
    /// <summary>
    /// 资源辅助类型
    /// </summary>
    public static class ResourceHelper
    {
        public static readonly Dictionary<string, string> FileResources = new Dictionary<string, string>();

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadConfigFile(string path)
        {
            if (FileResources.TryGetValue(path, out var fileContent))
            {
                return fileContent;
            }

            throw new System.Exception($@"资源文件{path}未找到！");
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="assembly"></param>
        public static void Register(Assembly assembly)
        {
            var names = assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                if (FileResources.ContainsKey(name)) throw new System.Exception($@"{name} 资源文件已存在！");

                using (var stream = assembly.GetManifestResourceStream(name))
                {
                    if (stream == null)
                    {
                        throw new System.Exception($@"资源文件{name}未找到！");
                    }

                    var sr = new StreamReader(stream);
                    FileResources[name] = sr.ReadToEnd();
                }
            }
        }
    }
}