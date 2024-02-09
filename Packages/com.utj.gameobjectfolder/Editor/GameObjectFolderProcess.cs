using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Unity.Utility
{
    /// <summary>
    /// During game build or play, this class will remove the parent-child hierarchy of objects under a <see cref="GameObjectFolder"/>.
    /// </summary>
    public class GameObjectFolderProcess : IProcessSceneWithReport
    {
        int IOrderedCallback.callbackOrder => 0;

        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
#if SKIP_OBJECT_FOLDER_STRIP         
            if (report == null)
                return;
#endif            
            
            var rootGameObjects = ListPool<GameObject>.Get();
            var gameObjectFolders = GenericPool<Queue<GameObjectFolder>>.Get();
            
            try
            {
                // Collect a list of objects in the scene
                scene.GetRootGameObjects(rootGameObjects);

                // Register folders in gameObjectFolders, maintaining the order of Hierarchy from the root objects.
                foreach (var root in rootGameObjects)
                {
                    var rootTransform = root.transform;
                    var allFolderObjects = GetFoldersObject(rootTransform);                    
                    Find(rootTransform, true, allFolderObjects, ref gameObjectFolders);
                }

                // Remove parent-child hierarchy of GameObjectFolders from top to bottom
                while (gameObjectFolders.TryDequeue(out var folder))
                {
                    var children = ListPool<Transform>.Get();
                    CollectionChildFolders(folder.transform, children);
                    DetachParentAndHide(folder, children);
                    ListPool<Transform>.Release(children);

                    // Only execute component removal during build
                    if (EditorApplication.isPlaying == false) 
                        StripFolderObject(folder);
                }

            }
            finally
            {
                GenericPool<Queue<GameObjectFolder>>.Release(gameObjectFolders);
                ListPool<GameObject>.Release(rootGameObjects);
            }

        }

        /// <summary>
        /// Remove the GameObjectFolder. If no other components are attached, the GameObject is also removed.
        /// </summary>
        /// <param name="folder">The GameObjectFolder to remove</param>
        private static void StripFolderObject(GameObjectFolder folder)
        {
            var components = ListPool<Component>.Get();
            folder.GetComponents(components);
            var onlyHasGameObjectFolder = components.Count <= 2;
            ListPool<Component>.Release(components);

            if (onlyHasGameObjectFolder)
            {
                Object.DestroyImmediate(folder);
            }
            else
            {
                Object.DestroyImmediate(folder.gameObject);
            }
        }

        /// <summary>
        /// Recursively retrieve GameObjects from folders already registered in the list.
        /// The objects in results are stored in Hierarchy order.
        /// </summary>
        /// <param name="parentTransform">The parent Transform</param>
        /// <param name="visible">Whether to show objects below the GameObjectFolder</param>
        /// <param name="folders">A list of already registered folders</param>
        /// <param name="results">The GameObjectFolders below parentTransform</param>
        private static void Find(Transform parentTransform, bool visible, in List<Transform> folders, ref Queue<GameObjectFolder> results)
        {
            var objIndex = folders.FindIndex(c => c == parentTransform);
            if (objIndex != -1 && parentTransform.TryGetComponent(out GameObjectFolder folder))
            {
                using var so = new SerializedObject(folder);
                using var isVisibleProperty = so.FindProperty("_isVisible");
                
                results.Enqueue(folder);
                visible = visible && isVisibleProperty.boolValue;
                isVisibleProperty.boolValue = visible;
            }
            
            for (var index = 0; index < parentTransform.childCount; index++)
            {
                var transform = parentTransform.GetChild(index);
                Find(transform,  visible, folders, ref results);
            }
        }

        /// <summary>
        /// Collect a list of child Transforms.
        /// </summary>
        /// <param name="transform">The parent Transform</param>
        /// <param name="children">The list of child Transforms</param>
        private static void CollectionChildFolders(in Transform transform, ICollection<Transform> children)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i));
            }
        }

        /// <summary>
        /// Remove the parent-child hierarchy of the GameObjectFolder and register the objects under it in <see cref="GameObjectFolder.ChildObjects"/>.
        /// Place the objects one level below the folder in Hierarchy.
        /// </summary>
        /// <param name="folder">The GameObjectFolder to process</param>
        /// <param name="children">A collection of child Transforms</param>
        private static void DetachParentAndHide( GameObjectFolder folder, in IEnumerable<Transform> children)
        {
            // Detach the parent-child hierarchy of the folder and obtain the Hierarchy index
            folder.transform.SetParent(null);
            var transformIndex = folder.transform.GetSiblingIndex();
            using var so = new SerializedObject(folder);
            using var isVisibleProperty = so.FindProperty("_isVisible");
            
            foreach (var child in children)
            {
                // Set the GameObject to be hidden or shown based on the setting.
                var childObj = child.gameObject;
                childObj.hideFlags = isVisibleProperty.boolValue ? HideFlags.None : HideFlags.HideInHierarchy;

                // Register the list of objects that were previously registered under the folder, which can be used to toggle visibility on the component side.
                folder.ChildObjects.Add(childObj);
                
                // Detach the parent-child hierarchy and move the object one level below the folder in Hierarchy.
                child.transform.SetParent(null);
                transformIndex += 1;
                child.SetSiblingIndex(transformIndex);
            }
        }

        /// <summary>
        /// Get a list of GameObjectFolders at the same hierarchy level.
        /// </summary>
        /// <param name="root">The root GameObject</param>
        /// <returns>A list of GameObjectFolders</returns>
        private static List<Transform> GetFoldersObject(in Component root)
        {
            var components = ListPool<GameObjectFolder>.Get();
            
            root.GetComponentsInChildren(true, components);
            
            var list = components.Select(c => c.transform).ToList();
            ListPool<GameObjectFolder>.Release(components);
            return list;
        }

    }
}
