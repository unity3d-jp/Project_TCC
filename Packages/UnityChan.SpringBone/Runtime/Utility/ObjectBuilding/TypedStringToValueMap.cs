using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    public class TypedStringToValueMap
    {
        public TypedStringToValueMap
        (
            System.Type inputType,
            Dictionary<string, System.Object> inputMap, 
            System.Object inputDefaultValue
        )
        {
            Type = inputType;
            map = inputMap;
            DefaultValue = inputDefaultValue;
        }

        public static TypedStringToValueMap Create<T>
        (
            Dictionary<string, T> inputMap,
            T inputDefaultValue
        )
        {
            var translatedMap = inputMap
                .ToDictionary(item => item.Key, item => (System.Object)item.Value);
            return new TypedStringToValueMap(
                typeof(T),
                translatedMap,
                (System.Object)inputDefaultValue);
        }

        public static TypedStringToValueMap Create<T>(Dictionary<string, T> inputMap)
        {
            var translatedMap = inputMap
                .ToDictionary(item => item.Key, item => (System.Object)item.Value);
            return new TypedStringToValueMap(
                typeof(T),
                translatedMap,
                (System.Object)default(T));
        }

        public System.Type Type { get; private set; }
        public System.Object DefaultValue { get; private set; }

        public System.Object this[string key]
        {
            get
            {
                var value = DefaultValue;
                var valueFound = map.TryGetValue(key, out value);
                if (!valueFound)
                {
                    value = DefaultValue;
                    Debug.LogError("Value not found: " + key);
                }
                return value;
            }
        }

        public string GetKey(System.Object value)
        {
            var key = map
                .Where(item => item.Value == value)
                .Select(item => item.Key)
                .FirstOrDefault();
            return (key != null) ? key : "";
        }

        // private

        private Dictionary<string, System.Object> map;
    }
}