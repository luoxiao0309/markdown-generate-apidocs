using MdDocGenerator.CodeTemplate;
using MdDocGenerator.Extensions;
using MdDocGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MdDocGenerator.Util
{
    public static class AssemblyHelper
    {
        private static readonly List<string> _ignoreProperties = new List<string> { "EntityState", "EntityName", "Attributes", "CreatedName", "CreatedGUID", "CreatedTime", "ModifiedGUID", "ModifiedTime", "ModifiedName", "VersionNumber", "__IsDeleted", "Item" };
        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="dllNames"></param>
        /// <returns></returns>
        public static List<ApiMethod> GetApiMethods(List<string> dllNames)
        {
            var methodInfos = GetMethodInfos(dllNames);
            List<ApiMethod> result = new List<ApiMethod>();
            methodInfos.ForEach(x =>
            {
                var apiMethod = new ApiMethod();
                //方法
                apiMethod.MethodName = x.Name;
                apiMethod.Namespace = x.DeclaringType.Namespace;
                apiMethod.ModuleName = x.Module.Name.Replace(".dll", "");
                apiMethod.FullName = $"{x.DeclaringType.FullName}.{apiMethod.MethodName}";
                apiMethod.ServiceName = x.DeclaringType.Name;

                //方法参数
                foreach (var parameter in x.GetParameters())
                {
                    var apiParam = new ApiParam();
                    apiParam.TypeName = GetTypeName(parameter.ParameterType);
                    apiParam.SourceType = parameter.ParameterType;
                    apiParam.Name = parameter.Name;
                    apiParam.FullName = $"{parameter.ParameterType.FullName}.{apiParam.Name}";
                    apiParam.ApiParamType = ApiParamType.Input;
                    string key = $"{parameter.Name}|{Guid.NewGuid()}";
                    Type parameterType = GetType(parameter.ParameterType);
                    if (parameterType != null)
                    {
                        var children = new List<ApiParam>();
                        var properties = parameterType.GetProperties().ToList().Where(p => !_ignoreProperties.Contains(p.Name) && p.DeclaringType.Name != "BaseDto");
                        foreach (var item in properties)
                        {
                            var child = new ApiParam();
                            child.Name = item.Name;
                            child.TypeName = GetTypeName(item.PropertyType);
                            child.FullName = $"{item.PropertyType.FullName}.{item.Name}";
                            child.SourceType = item.PropertyType;
                            var propertyType = GetType(item.PropertyType);
                            if (propertyType != null)
                            {
                                GetProperties(propertyType, child, item.Name, apiMethod);
                            }
                            children.Add(child);
                        }
                        if (children.Any())
                        {
                            apiParam.Children.Add(apiParam.Name, children);
                        }
                    }                    
                    apiMethod.ApiParams.Add(apiParam);
                }

                //方法返回值
                apiMethod.ReturnType = x.ReturnType;
                result.Add(apiMethod);
            });

            return result;
        }

        public static List<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties().ToList().Where(p => !_ignoreProperties.Contains(p.Name) && p.DeclaringType.Name != "BaseDto").ToList();
        }

        public static string GetPropertyDesc(string key, Dictionary<string, MemberNode> xml)
        {
            var pKey = xml.Keys.FirstOrDefault(x => x.Contains(key)) ?? "";
            if (xml.ContainsKey(pKey))
            {
                return (xml[pKey].Summary?.Value ?? string.Empty).Replace("\r\n", "").Replace("\t", "").Trim();
            }

            return string.Empty;
        }

        public static Type GetType(Type type)
        {
            Type parameterType = null;
            if(type.Name== "Nullable`1")
            {
                return parameterType;
            }
            //泛型
            if (type.IsGenericType)
            {
                parameterType = type.GetGenericArguments()[0];
            }
            else if (type.IsArray)
            {
                parameterType = type.GetElementType();
            }
            else if (type.IsClass && type != typeof(string))
            {
                parameterType = type;
            }

            if(parameterType!=null && parameterType.IsValueType || parameterType == typeof(string))
            {
                parameterType = null;
            }

            return parameterType;
        }

        static void GetProperties(Type parameterType, ApiParam apiParam, string key, ApiMethod apiMethod)
        {
            var tempType = GetType(parameterType);
            if (tempType == null)
            {
                return;
            }
            apiParam.Children = new Dictionary<string, List<ApiParam>>();
            var properties = tempType.GetProperties().ToList().Where(p => !_ignoreProperties.Contains(p.Name));
            var children = new List<ApiParam>();
            foreach (var item in properties)
            {
                var child = new ApiParam();
                child.Name = item.Name;
                child.TypeName = GetTypeName(item.PropertyType);
                child.FullName = $"{item.PropertyType.FullName}.{item.Name}";
                child.SourceType = item.PropertyType;
                var type = GetType(item.PropertyType);
                if (type != null)
                {
                    GetProperties(type, child, item.Name, apiMethod);
                }
                children.Add(child);
            }
            if (children.Any())
            {
                apiParam.Children.Add(key, children);
            }
        }

        static string GetTypeName(Type parameterType)
        {
            if (parameterType.Name == "List`1")
            {
                return $"List<{parameterType.GetGenericArguments()[0].Name}>";
            }
            else if (parameterType.Name == "Nullable`1")
            {
                return $"Nullable<{parameterType.GetGenericArguments()[0].Name}>";
            }
            return parameterType.Name;
        }
        /// <summary>
        /// 获取需要生成文档的方法
        /// </summary>
        /// <param name="dllNames"></param>
        /// <returns></returns>
        static List<MethodInfo> GetMethodInfos(List<string> dllNames)
        {
            var ignoreMethods = ConfigHelper.IgnoreMethods.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var ignoreServices = ConfigHelper.IgnoreServices.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var outputModule = ConfigHelper.OutputModule.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<MethodInfo> list = new List<MethodInfo>();
            foreach (var s in dllNames)
            {
                var assembly = Assembly.LoadFrom(s);
                var typeScope = assembly.GetTypes()
                    .Where(x => outputModule.Contains(x.Module.Name) && x.Namespace != null && (x.Namespace.EndsWith(".AppServices") || x.Namespace.EndsWith(".Interfaces")))
                    .Where(x => x.IsSealed == false && x.IsPublic && ignoreServices.Contains(x.Name) == false)
                    .ToList();
                foreach (var type1 in typeScope)
                {
                    var methods = type1.GetMethods()
                        .Where(m => m.IsPublic)
                        .Where(m => m.DeclaringType != null && m.DeclaringType.Namespace == type1.Namespace)
                        .Where(m => ignoreMethods.Contains(m.Name) == false)
                        .ToList();
                    list.AddRange(methods);
                }
            }
            return list;
        }
    }
}