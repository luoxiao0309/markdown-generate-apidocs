using System;
using System.Linq;
using System.Reflection;

namespace MdDocGenerator.Util
{
    public static class TypeHelper
    {
        public static bool IsIgnoreProperty(string propertyName)
        {
            var ignoreProperty = ConfigHelper.IgnoreProperty.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(x => x, x => x);
            return ignoreProperty.ContainsKey(propertyName);
        }
    }
}