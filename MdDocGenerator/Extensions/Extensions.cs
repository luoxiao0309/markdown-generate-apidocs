using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MdDocGenerator.Extensions
{
    public static class DictionaryExtension
    {
        public static void TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                return;
            }

            dic.Add(key, value);
        }

        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue defaultVal)
        {
            return dic.TryGetValue(key, out TValue value) ? value : defaultVal;
        }
    }
    public static class TypeExtension
    {
        public static bool IsSampleType(this Type type)
        {
            return type == typeof(int)
                   || type == typeof(int?)
                   || type == typeof(decimal)
                   || type == typeof(decimal?)
                   || type == typeof(DateTime)
                   || type == typeof(DateTime?)
                   || type == typeof(string)
                   || type == typeof(char)
                   || type == typeof(char?)
                   || type == typeof(bool)
                   || type == typeof(bool?)
                   || type == typeof(object)
                   || type.IsValueType;
        }

        public static bool IsSampleGeneric(this Type type)
        {
            if (!type.IsGenericType) return false;
            var singleType = type.GetGenericArguments().FirstOrDefault();
            return singleType.IsSampleType();
        }

        public static bool IsSampleArray(this Type type)
        {
            if (!type.IsArray) return false;
            var singleType = type.GetElementType();
            return singleType.IsSampleType();
        }

        public static bool IsCustomType(this Type type)
        {
            return type.IsGenericType == false && IsSampleGeneric(type) == false && IsSampleArray(type) == false && IsSampleType(type) == false
                   && type.IsArray == false && type.IsValueType == false;
        }

        public static bool IsIgnoreProperty(this PropertyInfo property)
        {
            return property.Name == "EntityName" || property.Name == "EntityState"
                                             || property.Name == "VersionNumber" || property.Name == "CreatedTime" || property.Name == "CreatedName"
                                             || property.Name == "CreatedGUID" || property.Name == "ModifiedName" || property.Name == "ModifiedTime"
                                             || property.Name == "ModifiedGUID"
                || property.PropertyType.Name == "Object" || property.DeclaringType?.FullName == "Mysoft.Map6.Core.EntityBase.Entity";
        }
    }
}