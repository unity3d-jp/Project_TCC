using System.Linq;
using UnityEngine;

namespace UTJ
{
    public static partial class SpringColliderSerialization
    {
        public static string BuildCollisionSetupString(GameObject colliderRoot)
        {
            var builder = new CSVBuilder();
            AppendColliders(builder, colliderRoot);
            AppendDynamicsNulls(builder, colliderRoot);
            // Components are a special case for now
            var setupString = builder.ToString();
            setupString += BuildComponentDefinitionString(colliderRoot);
            return setupString;
        }

        // private

        private static void AppendColliders(CSVBuilder builder, GameObject colliderRoot)
        {
            var sphereColliders = colliderRoot.GetComponentsInChildren<SpringSphereCollider>(true)
                .Select(item => SphereColliderToSerializer(item));
            var capsuleColliders = colliderRoot.GetComponentsInChildren<SpringCapsuleCollider>(true)
                .Select(item => CapsuleColliderToSerializer(item));
            var panelColliders = colliderRoot.GetComponentsInChildren<SpringPanelCollider>(true)
                .Select(item => PanelColliderToSerializer(item));
            if (!sphereColliders.Any() && !capsuleColliders.Any() && !panelColliders.Any())
            {
                return;
            }

            string[][] headerRows = {
                new string[] { "// ColliderName", "ParentName", "pos x", "y", "z", "rot x", "y", "z", "scale x", "y", "z", "ColliderType", "Parameters" },
                new string[] { "//", "", "", "", "", "", "", "", "", "", "", "Sp (Sphere)", "radius", "linkedRenderer" },
                new string[] { "//", "", "", "", "", "", "", "", "", "", "", "Cp (Capsule)", "radius", "height", "linkedRenderer" },
                new string[] { "//", "", "", "", "", "", "", "", "", "", "", "Pa (Panel)", "width", "height", "linkedRenderer" }
            };

            builder.AppendLine();
            builder.AppendLine("[Colliders]");
            foreach (var headerRow in headerRows)
            {
                builder.AppendLine(headerRow);
            }

            foreach (var serializer in sphereColliders)
            {
                builder.Append(serializer);
                builder.AppendLine();
            }

            foreach (var serializer in capsuleColliders)
            {
                builder.Append(serializer);
                builder.AppendLine();
            }

            foreach (var serializer in panelColliders)
            {
                builder.Append(serializer);
                builder.AppendLine();
            }
        }

        private static string GetComponentName(Component component)
        {
            return (component != null) ? component.name : "";
        }

        private static TransformSerializer TransformToSerializer(Transform sourceTransform)
        {
            return new TransformSerializer
            {
                name = sourceTransform.name,
                parentName = GetComponentName(sourceTransform.parent),
                position = sourceTransform.localPosition,
                eulerAngles = sourceTransform.localEulerAngles,
                scale = sourceTransform.localScale
            };
        }

        private static ColliderSerializerBaseInfo TransformToColliderSerializerBaseInfo(Transform sourceTransform, string colliderType)
        {
            return new ColliderSerializerBaseInfo
            {
                transform = TransformToSerializer(sourceTransform),
                colliderType = colliderType
            };
        }

        private static SphereColliderSerializer SphereColliderToSerializer(SpringSphereCollider sourceCollider)
        {
            return new SphereColliderSerializer
            {
                baseInfo = TransformToColliderSerializerBaseInfo(sourceCollider.transform, SphereColliderToken),
                radius = sourceCollider.radius,
                linkedRenderer = GetComponentName(sourceCollider.linkedRenderer)
            };
        }

        private static CapsuleColliderSerializer CapsuleColliderToSerializer(SpringCapsuleCollider sourceCollider)
        {
            return new CapsuleColliderSerializer
            {
                baseInfo = TransformToColliderSerializerBaseInfo(sourceCollider.transform, CapsuleColliderToken),
                radius = sourceCollider.radius,
                height = sourceCollider.height,
                linkedRenderer = GetComponentName(sourceCollider.linkedRenderer)
            };
        }

        private static PanelColliderSerializer PanelColliderToSerializer(SpringPanelCollider sourceCollider)
        {
            return new PanelColliderSerializer
            {
                baseInfo = TransformToColliderSerializerBaseInfo(sourceCollider.transform, PanelColliderToken),
                width = sourceCollider.width,
                height = sourceCollider.height,
                linkedRenderer = GetComponentName(sourceCollider.linkedRenderer)
            };
        }

        private static void AppendDynamicsNulls(CSVBuilder builder, GameObject rootObject)
        {
            var dynamicsNulls = rootObject.GetComponentsInChildren<DynamicsNull>(true)
                .Select(item => TransformToSerializer(item.transform));
            if (!dynamicsNulls.Any()) { return; }

            builder.AppendLine();
            builder.AppendLine("[DynamicsNulls]");
            foreach (var item in dynamicsNulls)
            {
                builder.Append(item);
                builder.AppendLine();
            }
        }

        private static string BuildComponentDefinitionString(GameObject colliderRoot)
        {
            var builder = new System.Text.StringBuilder();

            const string ComponentHeaderToken = "[Components]";
            builder.Append("\n");
            builder.Append(ComponentHeaderToken);
            builder.Append("\n");

            var componentDefiners = SpringSetupComponentDefiners.GetComponentDefiners();
            var children = colliderRoot.GetComponentsInChildren<Transform>(true);
            var hasAnyComponents = false;
            foreach (var child in children)
            {
                var hasUsableComponents = false;
                var componentBuilder = new System.Text.StringBuilder();
                foreach (var definer in componentDefiners)
                {
                    if (definer.TryToAppendDefinition(componentBuilder, child.gameObject))
                    {
                        hasUsableComponents = true;
                    }
                }

                if (hasUsableComponents)
                {
                    AppendRecordItem(builder, child.name);
                    AppendRecordItem(builder, componentBuilder.ToString());
                    builder.Append("\n");
                    hasAnyComponents = true;
                }
            }

            return hasAnyComponents ? builder.ToString() : "";
        }

        private static void AppendRecordItem<T>(System.Text.StringBuilder builder, T item, char separator = ',')
        {
            builder.Append(item);
            builder.Append(separator);
        }
    }
}