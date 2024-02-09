using Unity.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace Unity.SceneManagement
{
    /// <summary>
    /// Asset menu extension for SceneLoader.
    /// </summary>
    public static class SceneLoaderAssetMenuExtensions
    {
        /// <summary>
        /// Menu item to create a SceneLoader object.
        /// </summary>
        [MenuItem("GameObject/SceneLoader")]
        private static void CreateSceneLoader()
        {
            var obj = new GameObject("SceneLoader", typeof(SceneLoader));
            Selection.activeObject = obj;
            Undo.RegisterCreatedObjectUndo(obj, "Create Scene Loader Object");
        }
    }
}