using System;
using System.Collections.Generic;

namespace MdDocGenerator
{
    static class ValueBuilder
    {
        private static Dictionary<Type, object> _valueProvider = new Dictionary<Type, object>()
        {
            {typeof(string),default(string)},
            {typeof(DateTime),$"\"{DateTime.Today.ToString("yyyy-MM-dd")}\""},
            {typeof(decimal),default(decimal)},
            {typeof(Guid),$"\"{default(Guid)}\""},
            {typeof(bool),$"\"false\"" },
            {typeof(char),default(char) },
            {typeof(byte[]),default(byte[]) },
            {typeof(Enum),default(Enum) },
            {typeof(object),default(object) },
            {typeof(double),default(double) },
            {typeof(int),default(int)}
        };

        public static string TryGetValue(Type type)
        {
            if (type == null)
            {
                return "null";
            }
            if (_valueProvider.ContainsKey(type))
            {
                return _valueProvider[type]?.ToString();
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return "null";
            }
            if (type.IsGenericType)
            {
                var t = type.GetGenericArguments()[0];
                if (t == typeof(Guid))
                {
                    return $"[\"{Guid.NewGuid()}\"]";
                }
                if (_valueProvider.ContainsKey(t))
                {
                    return $"[\"{_valueProvider[t]}\"]";
                }
                else
                {
                    return $"[\"\"]";
                }
            }
            return string.Empty;
        }
    }

}