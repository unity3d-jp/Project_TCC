using UTJ.GameObjectExtensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public static class SpringColliderEditorActions
    {
        public static IList<GameObject> CreateObjectsWithComponentBeneathSelectedObjects<T>(string suffix) where T : Component
        {
            if (Application.isPlaying)
            {
                Debug.LogError("再生モードを止めてください。");
                return new List<GameObject>();
            }

            var newObjects = new List<GameObject>(Selection.gameObjects.Length);
            foreach (var parent in Selection.gameObjects)
            {
                var childName = GameObjectUtil.GetUniqueName(parent.name + suffix);
                var newChild = CreateChildObject(childName, parent.transform);
                newChild.AddComponent<T>();
                newObjects.Add(newChild);
            }
            Selection.objects = newObjects.ToArray();
            return newObjects;
        }

        public static void AlignSelectedCapsulesToParents()
        {
            var capsules = Selection.gameObjects
                .SelectMany(item => item.GetComponents<SpringCapsuleCollider>());
            AlignCapsulesToParents(capsules);
        }

        public static void AlignCapsulesToParents(IEnumerable<SpringCapsuleCollider> capsules)
        {
            var childComponentsToIgnore = new List<System.Type> { typeof(SpringBonePivot) };
            childComponentsToIgnore.AddRange(SpringColliderSetup.GetColliderTypes());

            var undoObjects = new List<Object>();
            undoObjects.AddRange(capsules.Select(item => (Object)item));
            undoObjects.AddRange(capsules.Select(item => (Object)item.transform));
            Undo.RecordObjects(undoObjects.ToArray(), "Align capsules to parents");
            foreach (var capsule in capsules)
            {
                capsule.transform.localPosition = Vector3.zero;
                var parent = capsule.transform.parent;
                if (parent.childCount >= 1)
                {
                    var childPositions = Enumerable.Range(0, parent.childCount)
                        .Select(index => parent.GetChild(index))
                        .Where(child => !GameObjectHasComponentOfAnyType(child.gameObject, childComponentsToIgnore))
                        .Select(child => child.position);
                    var tailPosition = Vector3.zero;
                    foreach (var position in childPositions)
                    {
                        tailPosition += position;
                    }
                    tailPosition /= childPositions.Count();

                    var length = (tailPosition - parent.position).magnitude;
                    const float Threshold = 0.0001f;
                    if (length > Threshold)
                    {
                        capsule.transform.LookAt(tailPosition);
                        capsule.transform.Rotate(90f, 0f, 0f, Space.Self);
                        capsule.height = length;
                    }
                }
            }
        }

        public static void CreateSphereColliderBeneathSelectedObjects()
        {
            CreateObjectsWithComponentBeneathSelectedObjects<SpringSphereCollider>("SphereCollider");
        }

        public static void CreateCapsuleColliderBeneathSelectedObjects()
        {
            var newGameObjects = CreateObjectsWithComponentBeneathSelectedObjects<SpringCapsuleCollider>("CapsuleCollider");
            var newCapsules = newGameObjects
                .Select(item => item.GetComponent<SpringCapsuleCollider>())
                .Where(item => item != null);
            AlignCapsulesToParents(newCapsules);
        }

        public static void CreatePanelColliderBeneathSelectedObjects()
        {
            CreateObjectsWithComponentBeneathSelectedObjects<SpringPanelCollider>("PanelCollider");
        }

        public static void DeleteCollidersFromSelectedSpringBones()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("再生モードを止めてください。");
                return;
            }

            var selectedBones = Selection.gameObjects
                .Select(gameObject => gameObject.GetComponent<SpringBone>())
                .Where(bone => bone != null);
            if (!selectedBones.Any()) { return; }

            var queryMessage = "本当に選択SpringBoneのコリジョンを削除しますか？";
            if (EditorUtility.DisplayDialog("コリジョンを削除", queryMessage, "削除", "キャンセル"))
            {
                Undo.RecordObjects(selectedBones.ToArray(), "コリジョンを削除");
                foreach (var springBone in selectedBones)
                {
                    springBone.RemoveAllColliders();
                }
            }
        }

        public static void DeleteAllChildCollidersFromSelection()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("再生モードを止めてください。");
                return;
            }

            if (Selection.gameObjects.Length == 0) { return; }

            var queryMessage = "本当に選択中のオブジェクトの全子供のコライダーを削除しますか？";
            if (!EditorUtility.DisplayDialog("選択コライダーを削除", queryMessage, "削除", "キャンセル"))
            {
                return;
            }

            var charaBones = Selection.gameObjects
                .SelectMany(gameObject => GameObjectUtil.GetAllBones(gameObject.transform.root.gameObject))
                .Distinct();

            var colliderTypes = SpringColliderSetup.GetColliderTypes();
            var deadColliders = new List<Component>();
            foreach (var gameObject in Selection.gameObjects)
            {
                foreach (var type in colliderTypes)
                {
                    deadColliders.AddRange(gameObject.GetComponentsInChildren(type, true));
                }
            }
            deadColliders = deadColliders.Distinct().ToList();

            var probablyDeadGameObjects = deadColliders.Select(collider => collider.gameObject)
                .Distinct()
                .Where(gameObject => !charaBones.Contains(gameObject.transform)
                    && gameObject.GetComponents<Component>().Count() <= 1
                    && gameObject.transform.childCount == 0)
                .ToArray();

            var springBones = GameObjectUtil.FindComponentsOfType<SpringBone>();
            var undoObjects = new List<Object>(springBones.Select(item => (Object)item));
            undoObjects.AddRange(deadColliders.Select(item => (Object)item));
            undoObjects.AddRange(probablyDeadGameObjects.Select(item => (Object)item));
            Undo.RecordObjects(undoObjects.ToArray(), "Remove all selected child colliders");

            foreach (var springBone in springBones)
            {
                springBone.sphereColliders = springBone.sphereColliders.Where(collider => !deadColliders.Contains(collider)).ToArray();
                springBone.capsuleColliders = springBone.capsuleColliders.Where(collider => !deadColliders.Contains(collider)).ToArray();
                springBone.panelColliders = springBone.panelColliders.Where(collider => !deadColliders.Contains(collider)).ToArray();
            }

            foreach (var deadCollider in deadColliders)
            {
                SpringBoneSetup.DestroyUnityObject(deadCollider);
            }

            foreach (var gameObject in probablyDeadGameObjects)
            {
                SpringBoneSetup.DestroyUnityObject(gameObject);
            }
        }

        public static void CleanUpDynamics()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("再生モードを止めてください。");
                return;
            }

            var springManagers = GameObjectUtil.FindComponentsOfType<SpringManager>();
            if (!springManagers.Any()) { return; }

            var queryMessage = "本当にダイナミクスのクリーンナップを行いますか？";
            if (EditorUtility.DisplayDialog("ダイナミクスクリーンナップ", queryMessage, "削除", "キャンセル"))
            {
                RemoveDuplicateComponents<SpringBone>();
                RemoveDuplicateComponents<DynamicsNull>();
                RemoveDuplicateComponents<SpringManager>();

                springManagers = GameObjectUtil.FindComponentsOfType<SpringManager>();
                foreach (var springManager in springManagers)
                {
                    var springBones = springManager.GetComponentsInChildren<SpringBone>(true);
                    var allBones = GameObjectUtil.GetAllBones(springManager.gameObject);

                    var maybeUnusedGameObjects = new HashSet<GameObject>();

                    var unusedSpheres = springManager.GetComponentsInChildren<SpringSphereCollider>(true)
                        .Where(collider => !springBones.Any(springBone => springBone.sphereColliders.Contains(collider)));
                    foreach (var collider in unusedSpheres)
                    {
                        maybeUnusedGameObjects.Add(collider.gameObject);
                        SpringBoneSetup.DestroyUnityObject(collider);
                    }

                    var unusedCapsules = springManager.GetComponentsInChildren<SpringCapsuleCollider>(true)
                        .Where(collider => !springBones.Any(springBone => springBone.capsuleColliders.Contains(collider)));
                    foreach (var collider in unusedCapsules)
                    {
                        maybeUnusedGameObjects.Add(collider.gameObject);
                        SpringBoneSetup.DestroyUnityObject(collider);
                    }

                    var unusedPanels = springManager.GetComponentsInChildren<SpringPanelCollider>(true)
                        .Where(collider => !springBones.Any(springBone => springBone.panelColliders.Contains(collider)));
                    foreach (var collider in unusedPanels)
                    {
                        maybeUnusedGameObjects.Add(collider.gameObject);
                        SpringBoneSetup.DestroyUnityObject(collider);
                    }

                    var gameObjectsToDelete = maybeUnusedGameObjects
                        .Where(gameObject => !allBones.Contains(gameObject.transform));
                    foreach (var gameObject in gameObjectsToDelete)
                    {
                        if (gameObject.GetComponents<Component>().Length <= 1)
                        {
                            SpringBoneSetup.DestroyUnityObject(gameObject);
                        }
                    }

                    // Next remove all empty entries from SpringBones
                    Undo.RecordObjects(springBones.ToArray(), "SpringBone cleanup");
                    foreach (var springBone in springBones)
                    {
                        springBone.capsuleColliders = springBone.capsuleColliders.Where(item => item != null).ToArray();
                        springBone.panelColliders = springBone.panelColliders.Where(item => item != null).ToArray();
                        springBone.sphereColliders = springBone.sphereColliders.Where(item => item != null).ToArray();
                        springBone.lengthLimitTargets = springBone.lengthLimitTargets.Where(item => item != null).ToArray();
                    }
                }
            }
        }

        // private

        private static bool GameObjectHasComponentOfAnyType(GameObject gameObject, IEnumerable<System.Type> types)
        {
            return types.Any(type => gameObject.GetComponent(type) != null);
        }

        private static void RemoveDuplicateComponents<T>() where T : Component
        {
            // Delete all components of the same type after the first
            var duplicateObjects = GameObjectUtil.FindComponentsOfType<Transform>()
                .Where(item => item.GetComponents<T>().Length > 1);
            if (duplicateObjects.Any())
            {
                Undo.RecordObjects(duplicateObjects.ToArray(), "Remove duplicate components");
                foreach (var transform in duplicateObjects)
                {
                    var components = transform.GetComponents<T>();
                    for (int componentIndex = 1; componentIndex < components.Length; componentIndex++)
                    {
                        SpringBoneSetup.DestroyUnityObject(components[componentIndex]);
                    }
                }
            }
        }

        private static GameObject CreateChildObject(string name, Transform parent)
        {
            var newChild = new GameObject(name);
            newChild.transform.parent = parent;
            newChild.transform.localScale = Vector3.one;
            newChild.transform.localRotation = Quaternion.identity;
            newChild.transform.localPosition = Vector3.zero;
            return newChild;
        }
    }
}