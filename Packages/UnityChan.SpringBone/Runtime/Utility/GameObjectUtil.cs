using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    namespace GameObjectExtensions
    {
        public static class GameObjectUtil
        {
            public enum SearchOptions
            {
                None,
                IgnoreNamespace // Maya風「namespace:objectname」の「namespace:」の部分を無視
            }

            // Includes inactive objects
            public static IEnumerable<T> FindComponentsOfType<T>() where T : Component
            {
                var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                var rootObjects = activeScene.GetRootGameObjects();
                var matchingObjects = new List<T>();
                foreach (var rootObject in rootObjects)
                {
                    matchingObjects.AddRange(rootObject.GetComponentsInChildren<T>(true));
                }
                return matchingObjects;
            }

            public static IEnumerable<GameObject> GetAllGameObjects()
            {
                return FindComponentsOfType<Transform>().Select(item => item.gameObject);
            }

            // 指定したクラスの名前→コンポーネントのマップを作成
            public static Dictionary<string, T> BuildNameToComponentMap<T>
            (
                this GameObject rootObject,
                bool includeInactive
            ) where T : Component
            {
                var components = rootObject.GetComponentsInChildren<T>(includeInactive);
                var map = new Dictionary<string, T>(components.Length);
                foreach (var component in components)
                {
                    map[component.name] = component;
                }
                return map;
                // This breaks on duplicate names
                //return components.ToDictionary(item => item.name, item => item);
            }

            public static IEnumerable<Transform> GetAllBones(this GameObject rootObject)
            {
                var skinnedMeshRenderers = rootObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                var bones = new HashSet<Transform>();
                foreach (var renderer in skinnedMeshRenderers)
                {
                    var rendererBones = renderer.bones;
                    foreach (var bone in rendererBones)
                    {
                        bones.Add(bone);
                    }
                }
                return bones;
            }

            public static Transform FindChildByName
            (
                this GameObject inRoot,
                string inName,
                SearchOptions searchOptions = SearchOptions.IgnoreNamespace
            )
            {
                return FindChildComponentByName<Transform>(inRoot, inName, searchOptions);
            }

            // 子供の指定した名前のオブジェクトを検出
            public static T FindChildComponentByName<T>
            (
                this GameObject inRoot,
                string inName,
                SearchOptions searchOptions = SearchOptions.IgnoreNamespace
            ) where T : Component
            {
                var lowerName = inName.ToLowerInvariant();
                if (searchOptions == SearchOptions.IgnoreNamespace)
                {
                    lowerName = RemoveNamespaceFromName(lowerName);
                }

                var children = inRoot.GetComponentsInChildren<T>();
                var childCount = children.Length;
                for (int childIndex = 0; childIndex < childCount; childIndex++)
                {
                    var child = children[childIndex];
                    var childName = child.gameObject.name.ToLowerInvariant();
                    if (searchOptions == SearchOptions.IgnoreNamespace)
                    {
                        childName = RemoveNamespaceFromName(childName);
                    }

                    if (childName == lowerName)
                    {
                        return child;
                    }
                }
                return null;
            }

            // 子供の指定した名前のオブジェクトを検出
            public static T[] FindChildComponentsByName<T>
            (
                this GameObject inRoot,
                string[] inNames,
                SearchOptions searchOptions = SearchOptions.IgnoreNamespace
            ) where T : Component
            {
                var children = inRoot.GetComponentsInChildren<T>();
                var outputList = new List<T>();
                var childCount = children.Length;
                for (var childIndex = 0; childIndex < childCount; ++childIndex)
                {
                    var child = children[childIndex];
                    var childName = child.gameObject.name.ToLowerInvariant();
                    if (searchOptions == SearchOptions.IgnoreNamespace)
                    {
                        childName = RemoveNamespaceFromName(childName);
                        if (System.Array.Exists(inNames,
                            searchName => RemoveNamespaceFromName(searchName.ToLowerInvariant()) == childName))
                        {
                            outputList.Add(child);
                        }
                    }
                    else
                    {
                        if (System.Array.Exists(inNames,
                            searchName => searchName.ToLowerInvariant() == childName))
                        {
                            outputList.Add(child);
                        }
                    }
                }
                return outputList.ToArray();
            }

            public static string RemoveNamespaceFromName(string inName)
            {
                var splitName = inName.Split(new char[] { ':' }, System.StringSplitOptions.None);
                return (splitName.Length > 0) ? splitName[splitName.Length - 1] : "";
            }

            public static int GetTransformDepth(Transform inObject)
            {
                var depth = 0;
                var currentObject = inObject;
                while (currentObject != null)
                {
                    currentObject = currentObject.parent;
                    ++depth;
                }
                return depth;
            }

            public static string GetUniqueName(string desiredName)
            {
                var existingNames = FindComponentsOfType<Transform>()
                    .Select(item => item.name);
                if (!existingNames.Contains(desiredName))
                {
                    return desiredName;
                }

                var newName = desiredName;
                const int AttemptCount = 10000;
                for (int index = 1; index <= AttemptCount; index++)
                {
                    newName = desiredName + "_" + index.ToString();
                    if (!existingNames.Contains(newName))
                    {
                        return newName;
                    }
                }

                Debug.LogError("Too many similar names exist: " + desiredName);
                return newName;
            }
        }
    }
}