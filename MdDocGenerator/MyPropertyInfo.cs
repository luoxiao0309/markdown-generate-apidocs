using System;
using System.Collections.Generic;
using System.Reflection;

namespace MdDocGenerator
{
    class MyPropertyInfo
    {
        private PropertyInfo _propertyInfo;

        public Type ThisType { get; set; }

        public object Owner { get; set; }

        public PropertyInfo PropertyInfo {
            get => _propertyInfo;
            set {
                _propertyInfo = value;
                ThisType = _propertyInfo.PropertyType;
                SetValue(_propertyInfo.PropertyType);
            }
        }

        /// <summary>
        /// 真正的值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object DefaultValue { get; set; }

        private void SetValue(Type type)
        {
            var v = CreateDefaultValue(this);
            DefaultValue = v.Item1;
            Value = v.Item2;
        }


        private static Tuple<object, object> CreateDefaultValue(MyPropertyInfo info)
        {
            if (ValueProvider.TryGetValue(info.ThisType, out Func<object> gettter))
            {
                var val = gettter();
                return new Tuple<object, object>(val, val);
            }
            else if (info.ThisType.IsGenericType)
            {

                var obj = Activator.CreateInstance(info.ThisType);
                return new Tuple<object, object>(obj, obj);
            }
            //else if (typeof(Entity).IsAssignableFrom(info.ThisType))
            //{
            //    var n = EntityFactory.New(info.ThisType);
            //    return new Tuple<object, object>(n, n);
            //}
            else if (info.ThisType.IsClass)
            {
                try
                {
                    var defVal = Activator.CreateInstance(info.ThisType);
                    return new Tuple<object, object>(defVal, defVal);
                }
                catch
                {
                    return new Tuple<object, object>(null, null);
                }
            }
            else
            {
                return new Tuple<object, object>(null, null);
            }

        }


        private static readonly Dictionary<Type, Func<object>> ValueProvider = new Dictionary<Type, Func<object>>()
        {
            {typeof(string),()=>"abc"},
            {typeof(DateTime),()=>"2019-09-20"},
            {typeof(DateTime?),()=>"2019-09-20"},
            {typeof(decimal),()=>123.12M},
            {typeof(decimal?),()=>123.12M},
            {typeof(Guid),()=>Guid.NewGuid()},
            {typeof(Guid?),()=>Guid.NewGuid() },
            {typeof(bool),()=>true },
            {typeof(bool?),()=>true },
            {typeof(char),()=>"1" },
            {typeof(byte[]),()=>null },
            {typeof(Enum),()=>1 },
            {typeof(object),()=>null },
            {typeof(double),()=>123.00D },
            {typeof(double?),()=>123.00D },
            {typeof(int),()=> 1},
            {typeof(int?),()=> 1}
        };
    }
}