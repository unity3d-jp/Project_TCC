using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    public static class SpringColliderSetup
    {
        public static IEnumerable<System.Type> GetColliderTypes()
        {
            return new System.Type[]
            {
                typeof(SpringSphereCollider),
                typeof(SpringCapsuleCollider),
                typeof(SpringPanelCollider)
            };
        }

        public static void DestroySpringColliders(GameObject colliderRoot)
        {
            DestroyComponentsOfType<SpringSphereCollider>(colliderRoot);
            DestroyComponentsOfType<SpringCapsuleCollider>(colliderRoot);
            DestroyComponentsOfType<SpringPanelCollider>(colliderRoot);

            var springBones = colliderRoot.GetComponentsInChildren<SpringBone>(true);
            foreach (var springBone in springBones)
            {
                springBone.sphereColliders = springBone.sphereColliders.Where(collider => collider != null).ToArray();
                springBone.capsuleColliders = springBone.capsuleColliders.Where(collider => collider != null).ToArray();
                springBone.panelColliders = springBone.panelColliders.Where(collider => collider != null).ToArray();
            }
        }

        // private

        private static void DestroyComponentsOfType<T>(GameObject rootObject) where T : Component
        {
            var components = rootObject.GetComponentsInChildren<T>(true);
            foreach (var component in components)
            {
                DestroyUnityObject(component);
            }
        }

        private static void DestroyUnityObject(UnityEngine.Object objectToDestroy)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.DestroyObjectImmediate(objectToDestroy);
#else
            Object.DestroyImmediate(objectToDestroy);
#endif
        }
    }
}