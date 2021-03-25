using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MdDocGenerator.CodeTemplate;
using MdDocGenerator.Model;
using MdDocGenerator.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace MdDocGenerator
{
    public class ApiDocProfile
    {
        private static readonly ApiDocProfile _instance = new ApiDocProfile();
        public static ApiDocProfile Instance()
        {
            return _instance;
        }

        private static Dictionary<string, MemberNode> _memberNodes;

        /// <summary>
        /// 生成文档
        /// </summary>
        /// <param name="dlls"></param>
        /// <param name="destPath"></param>
        public void Output(List<string> dlls, string destPath)
        {
            //1、加载注释信息
            _memberNodes = XmlDocCommentHelper.LoadMemberNodeToDictionary(dlls);

            //2、获取需要生成文档的方法
            var methods = AssemblyHelper.GetApiMethods(dlls);

            //3、循环输出文档
            methods = string.IsNullOrEmpty(ConfigHelper.OutputMethod) ? methods : methods.Where(x => ConfigHelper.OutputMethod.IndexOf(x.MethodName) >= 0).ToList();
            foreach (var apiMethod in methods)
            {
                var summary = GetSummary(apiMethod.FullName);
                var fileName = GetOutputFileName(apiMethod, destPath);
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    var content = new MarkdownBuilder();
                    content.Header(2, $"接口定义");
                    content.AppendLine(GenerateSummary(apiMethod.MethodName, summary));
                    content.AppendLine(GenerateRequestInfo(apiMethod));
                    content.AppendLine(GenerateFunctionRemark(apiMethod.FullName));

                    content.Header(2, $"入参说明");
                    content.AppendLine(GenerateParamTable(apiMethod));
                    content.AppendLine(GenerateParamSample(apiMethod.ApiParams));

                    content.Header(2, $"响应信息");
                    content.AppendLine(GenerateReturnInfo(apiMethod, apiMethod.FullName));
                    var buffer = Encoding.UTF8.GetBytes(content.ToString());
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// 生成接口功能
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        static string GenerateSummary(string methodName, string summary, bool isObsolete = false)
        {
            var builder = new MarkdownBuilder();
            builder.AppendLine($"接口名称：{methodName}_{summary}".Trim());
            return builder.ToString();
        }

        /// <summary>
        /// 生成请求信息
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        static string GenerateRequestInfo(ApiMethod methodInfo)
        {
            var builder = new MarkdownBuilder();
            builder.AppendLine("请求方式：POST");
            //builder.AppendLine($"请求Path：/api/P73330101/SaleChg/{methodInfo.MethodName}");
            builder.AppendLine($"请求Path：/api/{methodInfo.Namespace}.{methodInfo.ServiceName}/{methodInfo.MethodName}");
            return builder.ToString();
        }

        /// <summary>
        /// 生成功能逻辑
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        static string GenerateFunctionRemark(string methodName)
        {
            var builder = new MarkdownBuilder();
            builder.Header(3, "功能逻辑");
            builder.Append("<pre>");
            string remark = GetRemark(methodName);
            string newRemark = string.IsNullOrEmpty(remark) ? "" : GetNewRemark(remark, methodName);
            newRemark.Split(new string[] { "\r\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .ForEach(o =>
                {
                    builder.Append(o.Replace("            ", ""));
                });
            builder.AppendLine("</pre>");
            return builder.ToString();
        }

        /// <summary>
        /// 生成参数信息
        /// </summary>
        /// <param name="apiMethod"></param>
        /// <returns></returns>
        string GenerateParamTable(ApiMethod apiMethod)
        {
            var builder = new MarkdownBuilder();
            var data = new List<string[]>();
            foreach (var parameterInfo in apiMethod.ApiParams)
            {
                var item = new string[] { $"{parameterInfo.Name}", $"{parameterInfo.TypeName.Replace("<", "\\<")}", $"{GetParamDesc(apiMethod.FullName, parameterInfo.Name)}" };
                data.Add(item);
            }
            if (data.Any())
            {
                builder.Table(new[] { "参数", "类型", "说明" }, data);
            }
            var hs = new HashSet<string>();
            foreach (var apiParam in apiMethod.ApiParams)
            {

                var type = AssemblyHelper.GetType(apiParam.SourceType);
                if (type != null)
                {
                    data = new List<string[]>();
                    var properties = AssemblyHelper.GetProperties(type);
                    foreach (var property in properties)
                    {
                        var item = new string[] { $"{property.Name}", $"{property.PropertyType.Name.Replace("<", "\\<")}", $"{GetPropertyDesc($"{type.FullName}.{property.Name}")}" };
                        data.Add(item);
                    }
                    if (data.Any())
                    {
                        if (hs.Contains(type.Name) == false)
                        {
                            hs.Add(type.Name);
                            builder.AppendLine("");
                            builder.AppendLine($"{type.Name}说明");
                            builder.Table(new[] { "参数", "类型", "说明" }, data);
                        }
                    }
                    foreach (var property in properties)
                    {
                        var tempProperty = AssemblyHelper.GetType(property.PropertyType);
                        if (tempProperty != null && hs.Contains(tempProperty.Name) == false)
                        {
                            CreateParamTable(tempProperty, builder, hs);
                        }
                    }

                }
            }
            return builder.ToString();
        }

        void CreateParamTable(Type type, MarkdownBuilder builder, HashSet<string> hs)
        {
            var data = new List<string[]>();
            var properties = AssemblyHelper.GetProperties(type);
            foreach (var property in properties)
            {
                if (property.Name == "DiscountDetailList")
                {

                }
                var item = new string[] { $"{property.Name}", $"{property.PropertyType.Name.Replace("<", "\\<")}", $"{GetPropertyDesc($"{type.FullName}.{property.Name}")}" };
                data.Add(item);
            }
            if (data.Any())
            {
                if (hs.Contains(type.Name) == false)
                {
                    hs.Add(type.Name);
                    builder.AppendLine("");
                    builder.AppendLine($"{type.Name}说明");
                    builder.Table(new[] { "参数", "类型", "说明" }, data);
                }
            }
            builder.AppendLine("");
            foreach (var property in properties)
            {
                var tempProperty = AssemblyHelper.GetType(property.PropertyType);
                if (tempProperty != null && hs.Contains(tempProperty.Name) == false)
                {
                    CreateParamTable(tempProperty, builder, hs);
                }
            }
        }


        /// <summary>
        /// 获取请求示例
        /// </summary>
        /// <param name="paramList"></param>
        /// <returns></returns>
        static string GenerateParamSample(List<ApiParam> paramList)
        {
            if (paramList.Any() == false)
            {
                return "无";
            }
            var builder = new MarkdownBuilder();
            //url参数
            builder.Header(3, $"url参数（拼接至url中的参数）");
            if (paramList.All(x => x.IsBasicType))
            {
                var urlParams = new List<string[]>();
                foreach (var param in paramList)
                {
                    var item = new string[] { param.Name, param.TypeName };
                    urlParams.Add(item);
                }
                builder.Table(new[] { "名称", "数据类型" }, urlParams);
                return builder.ToString();
            }
            else
            {
                builder.AppendLine("无");
            }
            //body参数
            builder.Header(3, $"body参数");
            var sbParam = new StringBuilder();
            sbParam.Append("{");
            var paramCount = paramList.Count;
            var index = 1;
            foreach (var p in paramList)
            {
                var existsChild = p.Children.Any();
                var childParam = new StringBuilder();
                if (existsChild)
                {
                    childParam.Append("{");
                    if (p.Children.ContainsKey(p.Name))
                    {
                        var temIndex = 1;
                        foreach (var item in p.Children[p.Name])
                        {
                            childParam.Append($"\"{item.Name}\":{(item.Children.Any() ? GetChildProperty(item) : item.GetDefaultValue())}");
                            if (p.Children[p.Name].Count > temIndex)
                            {
                                childParam.Append(",");
                            }
                        }
                    }
                    childParam.Append("}");
                }

                if (p.IsList)
                {
                    sbParam.Append($"\"{p.Name}\":[{(existsChild ? childParam.ToString() : p.GetDefaultValue())}]");
                }
                else
                {
                    sbParam.Append($"\"{p.Name}\":{(existsChild ? childParam.ToString() : p.GetDefaultValue())}");
                }

                if (paramCount > index)
                {
                    sbParam.Append(",");
                }

                index++;
            }
            sbParam.Append("}");
            builder.Header(3, "请求json");
            var jobj = JObject.Parse(sbParam.ToString());
            var ja = jobj.Descendants()
                .Where(t => t.Type == JTokenType.Property && TypeHelper.IsIgnoreProperty(((JProperty)t).Name))
                .ToList();
            if (ja.Any())
            {
                ja.ForEach(o => o.Remove());
            }
            builder.Code("json", FormatJsonString(jobj.ToString()));
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        static string GetChildProperty(ApiParam param)
        {
            var existsChild = param.Children.Any();
            var childParam = new StringBuilder();
            if (existsChild)
            {
                if (param.IsList)
                {
                    childParam.Append("[");
                }
                childParam.Append("{");
                if (param.Children.ContainsKey(param.Name))
                {
                    var temIndex = 1;
                    foreach (var item in param.Children[param.Name])
                    {
                        childParam.Append($"\"{item.Name}\":{(item.Children.Any() ? GetChildProperty(item) : item.GetDefaultValue())}");
                        if (param.Children[param.Name].Count > temIndex)
                        {
                            childParam.Append(",");
                        }
                    }
                }
                childParam.Append("}");
                if (param.IsList)
                {
                    childParam.Append("]");
                }
            }
            return childParam.ToString();
        }

        /// <summary>
        /// 生成返回信息
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        static string GenerateReturnInfo(ApiMethod methodInfo, string methodName)
        {
            var content = new MarkdownBuilder();
            var retType = methodInfo.ReturnType;
            if (retType == typeof(void))
            {
                content.Append("返回值：无");
                return content.ToString();
            }
            content.List($"类型 {retType.Name}");
            content.List($"说明 {GetReturns(methodName)}");
            content.Header(4, "返回结果示例");
            content.AppendLine();
            content.Append("无");
            return content.ToString();
        }

        /// <summary>
        /// 获取输出文件名称
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        static string GetOutputFileName(ApiMethod methodInfo, string destPath)
        {
            string serviceName = FormatFileName(methodInfo.ServiceName);
            var fileName = Path.Combine(($"{destPath}/{serviceName}/{methodInfo.MethodName}.md"));
            var dir = Directory.GetParent(fileName).FullName;
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            Console.WriteLine($"start output -->{fileName}");
            return fileName;
        }

        /// <summary>
        /// 格式化文件名，替换文件名中不合法的字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string FormatFileName(string str) => string.IsNullOrEmpty(str) ? Guid.NewGuid().ToString() : Path.GetInvalidFileNameChars().Aggregate(str, (current, fileNameChar) => current.Replace(fileNameChar, '_'));


        /// <summary>
        /// 递归提取see的注释
        /// </summary>
        /// <param name="oldRemark"></param>
        /// <returns></returns>
        static string GetNewRemark(string oldRemark, string methodName)
        {
            Regex reg = new Regex(@"<see cref=""M:(?<see>.*?)"" />", RegexOptions.Multiline);
            var matchList = reg.Matches(oldRemark);

            foreach (Match match in matchList)
            {
                if (match.Success == false) continue;

                var matchValue = match.Value;
                var functionName = match.Groups["see"].Value;
                if (string.IsNullOrEmpty(functionName)) continue;
                //找到see的方法的备注
                string seeRemark = GetRemark(functionName);
                if (functionName.StartsWith($"{methodName}"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"see了自己，请检查{functionName}方法注释");
                    Console.ForegroundColor = ConsoleColor.White;
                    return oldRemark;
                }
                oldRemark = oldRemark.Replace(matchValue, seeRemark);
            }

            matchList = reg.Matches(oldRemark);
            if (matchList.Count > 0)
            {
                oldRemark = GetNewRemark(oldRemark, methodName);
            }
            return oldRemark;
        }

        /// <summary>
        /// 获取方法返回值说明
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static string GetReturns(string key)
        {
            var pKey = _memberNodes.Keys.FirstOrDefault(x => x.Contains(key)) ?? "";
            return (_memberNodes.ContainsKey(pKey) ? _memberNodes[pKey].Returns : string.Empty)?.Replace("\r\n", "");
        }

        string GetPropertyDesc(string key)
        {
            var pKey = _memberNodes.Keys.FirstOrDefault(x => x.Contains(key)) ?? "";
            if (_memberNodes.ContainsKey(pKey))
            {
                return (_memberNodes[pKey].Summary?.Value ?? string.Empty).Replace("\r\n", "").Replace("\t", "").Trim();
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取参数描述
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        static string GetParamDesc(string key, string paramName)
        {
            var pKey = _memberNodes.Keys.FirstOrDefault(x => x.Contains(key)) ?? "";
            if (_memberNodes.ContainsKey(pKey))
            {
                foreach (var paramNode in _memberNodes[pKey].ParamList)
                {
                    if (paramNode.Name == paramName)
                    {
                        return paramNode.Value;
                    }
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// 获取接口Summary注释
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetSummary(string key)
        {
            key = $"{key}(";
            var pKey = _memberNodes.Keys.FirstOrDefault(x => x.Contains(key)) ?? "";
            return (_memberNodes.ContainsKey(pKey) ? _memberNodes[pKey].Summary.Value : string.Empty)?.Replace("\r\n", "").Replace(" ", "")
                .Replace("<br/>", "")
                .Replace("<br>", "");
        }

        /// <summary>
        /// 获取方法备注
        /// </summary>
        /// <returns></returns>
        static string GetRemark(string key)
        {
            var pKey = _memberNodes.Keys.FirstOrDefault(x => x.Contains(key + "("));
            pKey = pKey ?? _memberNodes.Keys.FirstOrDefault(x => x.Contains(key));
            var remark = (_memberNodes.ContainsKey(pKey ?? "") ? _memberNodes[pKey ?? ""].Remarks : string.Empty);
            return remark;
        }

        /// <summary>
        /// 格式化Json字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string FormatJsonString(string str)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            var obj = serializer.Deserialize(jtr);
            if (obj == null) return str;
            StringWriter textWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            serializer.Serialize(jsonWriter, obj);
            return textWriter.ToString();
        }
    }
}