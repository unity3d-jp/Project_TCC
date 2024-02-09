using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UTJ
{
    public static class UnityComponentStringListBuilder
    {
        // Intended for constructing a row of strings for a CSV
        public static void BuildBuilderStringList
        (
            System.Object sourceObject,
            List<string> outputStrings,
            IEnumerable<TypedStringToValueMap> valueMaps = null
        )
        {
            var type = sourceObject.GetType();
            if (type.IsSubclassOf(typeof(UnityEngine.Component)))
            {
                ConvertFieldsToStrings(sourceObject, type, valueMaps, outputStrings);
            }
            else
            {
                ConvertObjectToStrings(sourceObject, type, valueMaps, outputStrings);
            }
        }

        public static IEnumerable<string> BuildBuilderStringList
        (
            System.Object sourceObject,
            IEnumerable<TypedStringToValueMap> valueMaps = null
        )
        {
            var outputStrings = new List<string>();
            BuildBuilderStringList(sourceObject, outputStrings, valueMaps);
            return outputStrings;
        }

        public static IEnumerable<string> BuildBuilderStringList
        (
            System.Object sourceObject,
            TypedStringToValueMap valueMap
        )
        {
            return BuildBuilderStringList(
                sourceObject,
                new TypedStringToValueMap[] { valueMap });
        }

        // private

        private static void ConvertFieldsToStrings
        (
            System.Object sourceObject,
            System.Type type,
            IEnumerable<TypedStringToValueMap> valueMaps,
            List<string> outputStrings
        )
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            var members = type.GetFields(bindingFlags);
            foreach (var member in members)
            {
                var memberItem = member.GetValue(sourceObject);
                ConvertObjectToStrings(memberItem, member.FieldType, valueMaps, outputStrings);
            }
        }

        private static void ConvertObjectToStrings
        (
            System.Object sourceObject,
            System.Type type,
            IEnumerable<TypedStringToValueMap> valueMaps,
            List<string> outputStrings
        )
        {
            if (valueMaps != null)
            {
                var matchingMap = valueMaps
                    .Where(map => map.Type == type)
                    .FirstOrDefault();
                if (matchingMap != null)
                {
                    outputStrings.Add(matchingMap.GetKey(sourceObject));
                    return;
                }
            }

            if (type.IsArray)
            {
                if (sourceObject == null)
                {
                    outputStrings.Add(0.ToString());
                }
                else
                {
                    // Arrays are messy...
                    var publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    var countMethod = publicMethods
                        .Where(method => method.Name == "GetLength")
                        .First();
                    var itemCount = (int)countMethod.Invoke(sourceObject, new object[] { 0 });
                    var getValueMethod = publicMethods
                        .Where(method => method.Name == "GetValue"
                            && method.GetParameters().Count() == 1
                            && method.GetParameters()[0].ParameterType == typeof(int))
                        .First();
                    var items = new List<System.Object>(itemCount);
                    for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
                    {
                        var item = getValueMethod.Invoke(sourceObject, new object[] { itemIndex });
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }

                    outputStrings.Add(items.Count.ToString());
                    var elementType = type.GetElementType();
                    foreach (var item in items)
                    {
                        ConvertObjectToStrings(item, elementType, valueMaps, outputStrings);
                    }
                }
            }
            else if (sourceObject == null)
            {
                outputStrings.Add("");
            }
            else if (type.IsPrimitive)
            {
                outputStrings.Add(sourceObject.ToString());
            }
            else if (type.IsEnum)
            {
                outputStrings.Add(sourceObject.ToString());
            }
            else if (type == typeof(System.String))
            {
                outputStrings.Add((string)sourceObject);
            }
            else if (type == typeof(UnityEngine.GameObject))
            {
                // Shallow reference
                var gameObject = sourceObject as GameObject;
                outputStrings.Add((gameObject != null) ? gameObject.name : "");
            }
            else if (type.IsSubclassOf(typeof(UnityEngine.Component)))
            {
                var component = sourceObject as UnityEngine.Component;
                outputStrings.Add((component != null) ? component.name : "");
            }
            else
            {
                ConvertFieldsToStrings(sourceObject, type, valueMaps, outputStrings);
            }
        }
    }
}