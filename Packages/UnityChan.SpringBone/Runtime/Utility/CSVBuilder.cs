using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UTJ
{
    public class CSVBuilder
    {
        public CSVBuilder(char newSeparatorCharacter = ',')
        {
            builder = new System.Text.StringBuilder();
            separatorCharacter = newSeparatorCharacter;
            currentSeparator = "";
        }

        public void Append(string item)
        {
            builder.Append(currentSeparator);
            if (item == null)
            {
                item = "";
            }
            builder.Append(CSVUtilities.BuildCSVItem(item));
            currentSeparator = separatorCharacter.ToString();
        }

        public void Append(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            Append(gameObject.name);
            var components = gameObject.GetComponents<Component>();
            Append(components.Length.ToString());
            foreach (var component in components)
            {
                Append(component.GetType().ToString());
                Append(component);
            }
        }

        public void Append(System.Object item)
        {
            if (item == null)
            {
                return;
            }

            var itemType = item.GetType();
            if (itemType == typeof(Transform))
            {
                Append(item as Transform);
            }
            else if (!itemType.IsPrimitive
                && !itemType.IsEnum)
            {
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                var members = itemType.GetFields(bindingFlags);
                foreach (var member in members)
                {
                    InternalAppend(member.FieldType, member.GetValue(item));
                }
            }
            else
            {
                InternalAppend(itemType, item);
            }
        }

        public void Append(Vector3 vector)
        {
            Append(vector.x);
            Append(vector.y);
            Append(vector.z);
        }

        public void Append(Transform transform)
        {
            if (transform == null)
            {
                return;
            }

            Append((transform.parent != null) ? transform.parent.name : "");
            Append(transform.localPosition);
            Append(transform.localRotation.eulerAngles);
            Append(transform.localScale);
        }

        public void Append(IEnumerable<string> list)
        {
            foreach (var item in list)
            {
                Append(item);
            }
        }

        public void AppendLine()
        {
            builder.AppendLine();
            currentSeparator = "";
        }

        public void AppendLine(string item)
        {
            Append(item);
            AppendLine();
        }

        public void AppendLine(IEnumerable<string> list)
        {
            Append(list);
            AppendLine();
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        // private

        private System.Text.StringBuilder builder;
        private char separatorCharacter;
        private string currentSeparator;

        private void InternalAppend(System.Type itemType, System.Object item)
        {
            if (itemType.IsPrimitive
                || itemType.IsEnum
                || itemType == typeof(string))
            {
                Append(item.ToString());
                return;
            }

            if (itemType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                var unityObject = item as UnityEngine.Object;
                Append((unityObject != null) ? unityObject.name : "");
                return;
            }

            if (itemType.IsArray)
            {
                var array = item as System.Array;
                if (array == null || itemType.GetArrayRank() > 1)
                {
                    Append("0");
                }
                else
                {
                    var objectList = new List<System.Object>();
                    foreach (var subItem in array)
                    {
                        if (subItem != null)
                        {
                            objectList.Add(subItem);
                        }
                    }
                    Append(objectList.Count.ToString());
                    foreach (var subItem in objectList)
                    {
                        InternalAppend(itemType.GetElementType(), subItem);
                    }
                }
                return;
            }

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            var members = itemType.GetFields(bindingFlags);
            foreach (var member in members)
            {
                InternalAppend(member.FieldType, member.GetValue(item));
            }
        }
    }
}