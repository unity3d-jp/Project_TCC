using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UTJ
{
    namespace StringQueueExtensions
    {
        public static class ObjectBuilder
        {
            public static float DequeueFloat(this Queue<string> queue)
            {
                return float.Parse(queue.Dequeue());
            }

            public static int DequeueInt(this Queue<string> queue)
            {
                return int.Parse(queue.Dequeue());
            }

            public static Vector3 DequeueVector3(this Queue<string> queue)
            {
                var x = float.Parse(queue.Dequeue());
                var y = float.Parse(queue.Dequeue());
                var z = float.Parse(queue.Dequeue());
                return new Vector3(x, y, z);
            }

            public static Transform DequeueTransform(this Queue<string> queue, GameObject gameObject)
            {
                var parentName = queue.Dequeue();
                Transform newParent = null;
                if (parentName.Length > 0)
                {
                    var children = gameObject.GetComponentsInChildren<Transform>(true);
                    newParent = Object.FindObjectsOfType<Transform>()
                        .Where(item => item.name == parentName
                            && !children.Contains(item))
                        .FirstOrDefault();
                    if (newParent == null)
                    {
                        Debug.LogError("Valid parent not found: " + parentName);
                    }
                }
                gameObject.transform.parent = newParent;
                var localPosition = DequeueVector3(queue);
                var localRotation = DequeueVector3(queue);
                var localScale = DequeueVector3(queue);
                gameObject.transform.localRotation = Quaternion.Euler(localRotation);
                gameObject.transform.localScale = localScale;
                gameObject.transform.localPosition = localPosition;
                return gameObject.transform;
            }

            public static bool DequeueComponent
            (
                this Queue<string> queue,
                Component component,
                GameObject rootObject = null,
                IEnumerable<TypedStringToValueMap> valueMaps = null
            )
            {
                var type = component.GetType();
                var succeeded = true;
                try
                {
                    queue.DequeueFields(type, component, rootObject, valueMaps);
                }
                catch (System.InvalidOperationException exception)
                {
                    succeeded = false;
                    Debug.LogError("Error dequeueing fields for " + type.ToString() + ":\n"
                        + "Insufficient data in source fields\n\n"
                        + exception.ToString());
                }
                catch (System.Exception exception)
                {
                    succeeded = false;
                    Debug.LogError("Error dequeueing fields for " + type.ToString() + "\n\n"
                        + exception.ToString());
                }
                return succeeded;
            }

            public static void DequeueFields
            (
                this Queue<string> queue,
                System.Type classType,
                System.Object item,
                GameObject rootObject = null,
                IEnumerable<TypedStringToValueMap> valueMaps = null
            )
            {
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                var members = classType.GetFields(bindingFlags);
                foreach (var member in members)
                {
                    var value = queue.GetValueByType(member.FieldType, rootObject, valueMaps);
                    member.SetValue(item, value);
                }
            }

            public static void DequeueFields<T>
            (
                this Queue<string> queue,
                T item,
                string firstOptionalField = null
            ) where T : class
            {
                DequeueFields(queue, typeof(T), item, firstOptionalField);
            }

            public static void DequeueFields
            (
                this Queue<string> queue,
                System.Type classType,
                System.Object item,
                string firstOptionalField = null
            )
            {
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                var members = classType.GetFields(bindingFlags);
                foreach (var member in members)
                {
                    if (queue.Count == 0
                        && !string.IsNullOrEmpty(firstOptionalField)
                        && member.Name == firstOptionalField)
                    {
                        return;
                    }

                    var value = queue.GetValueByType(member.FieldType, null, null);
                    member.SetValue(item, value);
                }
            }

            public static T DequeueObject<T>
            (
                this Queue<string> queue,
                string firstOptionalField = null
            ) where T : class, new()
            {
                var newItem = new T();
                DequeueFields(queue, typeof(T), newItem, firstOptionalField);
                return newItem;
            }

            public static System.Object DequeueObject
            (
                this Queue<string> queue,
                System.Type type,
                string firstOptionalField = null
            )
            {
                var newItem = System.Activator.CreateInstance(type);
                DequeueFields(queue, type, newItem, firstOptionalField);
                return newItem;
            }

            // private

            private static System.Object ParsePrimitiveType(System.Type type, string valueSource)
            {
                var parseMethod = type.GetMethods()
                    .Where(method => method.Name == "Parse"
                        && method.IsStatic
                        && method.GetParameters().Length == 1)
                    .FirstOrDefault();
                if (parseMethod != null)
                {
                    return parseMethod.Invoke(null, new System.Object[] { valueSource });
                }

                Debug.LogError("Parse not found: " + type.ToString());
                return null;
            }

            private static System.Object ParseEnum(System.Type type, string valueSource)
            {
                System.Object enumValue = null;

                try
                {
                    enumValue = System.Enum.Parse(type, valueSource, true);
                }
                catch (System.Exception exception)
                {
                    Debug.LogError("Enum value not found: " + type.ToString() + " : " + valueSource + "\n\n" + exception.ToString());
                    enumValue = null;
                }

                if (enumValue == null)
                {
                    var enumValues = System.Enum.GetValues(type);
                    enumValue = enumValues.GetValue(0);
                }
                return enumValue;
            }

            private static System.Object GetValueByType
            (
                this Queue<string> queue,
                System.Type type,
                GameObject rootObject,
                IEnumerable<TypedStringToValueMap> valueMaps
            )
            {
                if (valueMaps != null)
                {
                    var matchingMap = valueMaps
                        .Where(map => map.Type == type)
                        .FirstOrDefault();
                    if (matchingMap != null)
                    {
                        return matchingMap[queue.Dequeue()];
                    }
                }

                if (type.IsPrimitive)
                {
                    return ParsePrimitiveType(type, queue.Dequeue());
                }

                if (type.IsEnum)
                {
                    return ParseEnum(type, queue.Dequeue());
                }

                if (type == typeof(System.String))
                {
                    return queue.Dequeue();
                }

                if (type == typeof(UnityEngine.GameObject))
                {
                    var targetComponent = FindComponent(typeof(Transform), rootObject, queue.Dequeue());
                    if (targetComponent != null)
                    {
                        return targetComponent.gameObject;
                    }
                    return null;
                }

                if (type.IsSubclassOf(typeof(UnityEngine.Component)))
                {
                    var targetComponent = FindComponent(type, rootObject, queue.Dequeue());
                    return targetComponent;
                }

                if (type.IsArray)
                {
                    return queue.BuildArray(type.GetElementType(), rootObject, valueMaps);
                }

                var subItem = System.Activator.CreateInstance(type);
                queue.DequeueFields(type, subItem, rootObject, valueMaps);
                return subItem;
            }

            private static System.Array BuildArray
            (
                this Queue<string> queue,
                System.Type elementType,
                GameObject rootObject,
                IEnumerable<TypedStringToValueMap> valueMaps
            )
            {
                var itemCount = int.Parse(queue.Dequeue());
                var array = System.Array.CreateInstance(elementType, itemCount);
                for (var itemIndex = 0; itemIndex < itemCount; ++itemIndex)
                {
                    var itemValue = queue.GetValueByType(elementType, rootObject, valueMaps);
                    array.SetValue(itemValue, itemIndex);
                }
                return array;
            }

            private static Component FindComponent(System.Type type, GameObject root, string objectName)
            {
                IEnumerable<Component> sourceComponents = (root == null)
                    ? Object.FindObjectsOfType(type)
                        .Select(item => item as Component)
                        .Where(item => item != null)
                    : root.GetComponentsInChildren(type, true);
                var matchingComponent = sourceComponents
                    .FirstOrDefault(child => child.name == objectName);
                if (matchingComponent == null)
                {
                    Debug.LogError("Component not found: " + objectName + "  Type: " + type.ToString());
                }
                return matchingComponent;
            }
        }
    }
}