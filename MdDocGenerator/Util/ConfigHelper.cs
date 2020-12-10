using System.Configuration;

namespace MdDocGenerator.Util
{
    public class ConfigHelper
    {
        public static string TemplatePath => GetConfigValue("TemplatePath");

        /// <summary>
        /// 文档输出目录
        /// </summary>
        public static string OutputPath => GetConfigValue("OutputPath");

        /// <summary>
        /// 程序集、XML文件所在路径
        /// </summary>
        public static string SourcePath => GetConfigValue("SourcePath");

        public static string Dll => GetConfigValue("Dll");

        public static string IgnoreMethods => GetConfigValue("IgnoreMethods");

        public static string IgnoreServices => GetConfigValue("IgnoreServices");

        public static string IgnoreProperty => GetConfigValue("IgnoreProperty");

        public static string OutputModule => GetConfigValue("OutputModule");

        public static string OutputMethod => GetConfigValue("OutputMethod");

        private static string GetConfigValue(string key)
        {
           return ConfigurationManager.AppSettings[key];
        }
    }
}