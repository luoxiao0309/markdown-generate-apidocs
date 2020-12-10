using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace MdDocGenerator.Util
{

    /// <summary>
    /// 动态对象
    /// </summary>
    [Serializable]
    public sealed class DataDynamicObject : DynamicObject, ISerializable
    {
        private static HashSet<string> _IgnoreProperties = new HashSet<string>
        {
            "CreatedGUID","CreatedName","CreatedTime","ModifiedGUID","ModifiedName","ModifiedTime","Attributes","EntityState","EntityName"
        };
        private readonly Dictionary<string, object> _extendValue = new Dictionary<string, object>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataDynamicObject()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            if (_extendValue.TryGetValue(binder.Name, out result)) return true;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataDynamicObject SetMember(string name, object value)
        {
            _extendValue[name] = value;
            return this;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var val in _extendValue)
            {
                var jobj = new JObject(val.Value);
                foreach (var ignoreProperty in _IgnoreProperties)
                {
                    jobj.Remove(ignoreProperty);
                }

                info.AddValue(val.Key, jobj.ToString());
            }
        }
    }
}
