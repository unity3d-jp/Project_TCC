using UTJ.StringQueueExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UTJ
{
    public class SpringSetupComponentDefiners
    {
        public static IEnumerable<ComponentDefiner> GetComponentDefiners()
        {
            return new ComponentDefiner[] {
                new ComponentDefiner(typeof(HighLeg))
            };
        }

        public class ComponentDefiner
        {
            public ComponentDefiner(System.Type newType)
            {
                componentType = newType;
            }

            public bool TryToAppendDefinition(StringBuilder builder, GameObject rootObject)
            {
                var component = rootObject.GetComponent(componentType);
                var componentExists = component != null;
                if (componentExists)
                {
                    AppendDefinition(builder, component);
                }
                return componentExists;
            }

            public void AppendDefinition(StringBuilder builder, Component component)
            {
                AppendRecordItem(builder, GetTypeToken());
                AppendProperties(builder, component);
            }

            public Component BuildFromDefinition(GameObject owner, Queue<string> definitionItems)
            {
                if (definitionItems.Peek() != GetTypeToken())
                {
                    return null;
                }

                definitionItems.Dequeue();
                Component newComponent = null;
                try
                {
                    InternalBuildFromDefinition(owner, definitionItems);
                }
                catch (System.Exception exception)
                {
                    Debug.LogError(GetTypeToken() + " 読み込みエラー\n\n" + exception.ToString());
                    newComponent = null;
                }
                return newComponent;
            }

            // protected

            protected System.Type componentType;

            protected virtual string GetTypeToken()
            {
                return componentType.ToString();
            }

            protected virtual void AppendProperties(StringBuilder builder, Component component)
            {
                var builderStrings = UnityComponentStringListBuilder.BuildBuilderStringList(component);
                builder.Append(string.Join(",", builderStrings.ToArray()));
                builder.Append(",");
            }

            protected static void AppendRecordItem<T>(StringBuilder builder, T item, char separator = ',')
            {
                builder.Append(item);
                builder.Append(separator);
            }

            protected static string GetComponentName(Component component)
            {
                return (component != null) ? component.name : "";
            }

            protected virtual Component InternalBuildFromDefinition
            (
                GameObject owner,
                Queue<string> definitionItems
            )
            {
                // Default implementation
                // First remove all old components of the same type
                // Todo: What if we want two or more of the same type of component on the same GameObject?
                var oldComponents = owner.GetComponents(componentType);
                var newComponent = owner.AddComponent(componentType);
                var rootObject = owner.transform.root.gameObject;
                if (definitionItems.DequeueComponent(newComponent, rootObject))
                {
                    // Succeeded; destroy the old components
                    foreach (var oldComponent in oldComponents)
                    {
                        Object.DestroyImmediate(oldComponent);
                    }
                }
                else
                {
                    // Failed; destroy the component we added
                    Object.DestroyImmediate(newComponent);
                    newComponent = null;
                }
                return newComponent;
            }
        }
    }
}