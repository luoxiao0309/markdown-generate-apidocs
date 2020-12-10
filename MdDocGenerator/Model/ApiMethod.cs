using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MdDocGenerator.Model
{
    public class ApiMethod
    {
        public ApiMethod()
        {
            ApiParams = new List<ApiParam>();
        }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<ApiParam> ApiParams { get; set; }

        /// <summary>
        /// 返回类型
        /// </summary>
        public Type ReturnType { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public XmlElement XmlComment { get; set; }

    }

    public class ApiProperty
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public string Desc { get; set; }
    }

    public class ApiParam
    {
        public ApiParam()
        {
            Children = new Dictionary<string, List<ApiParam>>();
        }
        public string TypeName { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public ApiParamType ApiParamType { get; set; }

        public Dictionary<string,List<ApiParam>> Children { get; set; }


        public Type SourceType { get; set; }

        public bool IsBasicType => SourceType.IsValueType || SourceType == typeof(string);

        public bool IsList => (SourceType.IsArray || SourceType.IsGenericType) && !SourceType.Name.StartsWith("Nullable`1");

        public string GetDefaultValue()
        {
            var defaultValue = ValueBuilder.TryGetValue(SourceType);
            if (defaultValue == string.Empty)
            {
                return "\"\"";
            }
            return defaultValue;
        }
    }

    public enum ApiParamType
    {
        /// <summary>
        /// 入参
        /// </summary>
        Input=0,
        /// <summary>
        /// 返回
        /// </summary>
        Return=1,
    }
}
