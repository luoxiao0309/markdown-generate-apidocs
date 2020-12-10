using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MdDocGenerator.Util
{
    public class JsonConverter
    {
        /// <summary>  
        /// To Serialize a object  
        /// </summary>  
        /// <param name="obj">object for serialization</param>  
        /// <returns>json string of object</returns>  
        public static string Serialize(object obj)
        {
            ///// To parse base class object  
            var json = ParsePreDefinedClassObject(obj);

            ///// Null means it is not a base class object  
            if (!string.IsNullOrEmpty(json))
            {
                return json;
            }

            //// For parsing user defined class object  
            //// To get all properties of object  
            //// and then store object properties and their value in dictionary container  
            var objectDataContainer = obj.GetType().GetProperties().ToDictionary(i => i.Name, i => i.GetValue(obj));

            StringBuilder jsonfile = new StringBuilder();
            jsonfile.Append("{");
            foreach (var data in objectDataContainer)
            {
                jsonfile.Append($"\"{data.Key}\":{Serialize(data.Value)},");
            }

            //// To remove last comma  
            jsonfile.Remove(jsonfile.Length - 1, 1);
            jsonfile.Append("}");
            return jsonfile.ToString();
        }

        /// <summary>  
        /// To Serialize C# Pre defined classes  
        /// </summary>  
        /// <param name="obj">object for serialization</param>  
        /// <returns>json string of object</returns>  
        private static string ParsePreDefinedClassObject(object obj)
        {
            if (obj is null)
            {
                return "null";
            }
            if (IsJsonValueType(obj))
            {
                return obj.ToString().ToLower();
            }
            else if (IsJsonStringType(obj))
            {
                return $"\"{obj.ToString()}\"";
            }
            else if (obj is IDictionary)
            {
                return SearlizeDictionaryObject((IDictionary)obj);
            }
            else if (obj is IList || obj is Array)
            {
                return SearlizeListObject((IEnumerable)obj);
            }

            return null;
        }

        /// <summary>  
        /// To Serialize Dictionary type object  
        /// </summary>  
        /// <param name="obj">object for serialization</param>  
        /// <returns>json string of object</returns>  
        private static string SearlizeDictionaryObject(IDictionary dict)
        {
            StringBuilder jsonfile = new StringBuilder();
            jsonfile.Append("{");
            var keysAsJson = new List<string>();
            var valuesAsJson = new List<string>();
            foreach (var item in (IEnumerable)dict.Keys)
            {
                keysAsJson.Add(Serialize(item));
            }
            foreach (var item in (IEnumerable)dict.Values)
            {
                valuesAsJson.Add(Serialize(item));
            }
            for (int i = 0; i < dict.Count; i++)
            {
                ////To check whether data is under double quotes or not  
                keysAsJson[i] = keysAsJson[i].Contains("\"") ? keysAsJson[i] : $"\"{keysAsJson[i]}\"";

                jsonfile.Append($"{keysAsJson[i]}:{valuesAsJson[i]},");
            }
            jsonfile.Remove(jsonfile.Length - 1, 1);
            jsonfile.Append("}");
            return jsonfile.ToString();
        }

        /// <summary>  
        /// To Serialize Enumerable (IList,Array..etc) type object  
        /// </summary>  
        /// <param name="obj">object for serialization</param>  
        /// <returns>json string of object</returns>  
        private static string SearlizeListObject(IEnumerable obj)
        {
            StringBuilder jsonfile = new StringBuilder();
            jsonfile.Append("[");
            foreach (var item in obj)
            {
                jsonfile.Append($"{Serialize(item)},");
            }
            jsonfile.Remove(jsonfile.Length - 1, 1);
            jsonfile.Append("]");
            return jsonfile.ToString();
        }

        private static bool IsJsonStringType(object obj)
        {
            return obj is string || obj is DateTime;
        }

        private static bool IsJsonValueType(object obj)
        {
            return obj.GetType().IsPrimitive;
        }
    }
}