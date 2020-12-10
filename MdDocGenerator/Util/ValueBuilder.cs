using System;
using System.Collections.Generic;

namespace MdDocGenerator.Util
{
    public class ValueBuilder
    {
        private static readonly Dictionary<Type, Func<object>> ValueProvider = new Dictionary<Type, Func<object>>()
        {
            {typeof(string),()=>"abc"},
            {typeof(DateTime),()=>DateTime.Now.ToString("yyyy-MM-dd")},
            {typeof(decimal),()=>123.12M},
            {typeof(Guid),()=>$"{Guid.NewGuid().ToString().ToUpper()}"},
            {typeof(bool),()=>true },
            {typeof(char),()=>"1" },
            {typeof(Enum),()=>1 },
            {typeof(double),()=>123.00D },
            {typeof(int),()=>1}
        };
    }
}