using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Unity.SceneManagement
{
    /// <summary>
    /// Manages SceneLoader.
    /// </summary>
    [RenamedFrom("Tcc.SceneManagement.SceneLoaderManager")]
    public class SceneLoaderManager
    {
        private static readonly SceneLoaderManager Instance = new();

        private readonly SceneLoadManager _sceneLoad ;
        private readonly SceneOwnershipManager _ownership ;
        private readonly SceneHandleManager _sceneHandle;
        private readonly SceneProgressTracker _sceneProgress;

        private SceneLoaderManager()
        {
            _sceneLoad = new SceneLoadManager();
            _ownership = new SceneOwnershipManager();
            _sceneHandle = new SceneHandleManager();
            _sceneProgress = new SceneProgressTracker(_sceneHandle);
        }
        
        /// <summary>
        /// Returns true if a scene loading process is currently running.
        /// </summary>
        public static bool HasProgress => Instance._sceneProgress.HasProgress;

        /// <summary>
        /// Overall progress of scene loading processes within SceneLoader.
        /// </summary>
        /// <returns></returns>
        public static float Progress() => Instance._sceneProgress.Progress();

        /// <summary>
        /// Gets the handle for a scene loading process.
        /// </summary>
        /// <param name="scene">Target scene</param>
        /// <param name="handle">Handle to manage the loading progress</param>
        /// <returns>True if the loading process is currently running</returns>
        public static bool GetHandle(AssetReferenceScene scene, out AsyncOperationHandle<SceneInstance> handle)
        {
            return Instance._sceneHandle.GetHandle(scene, out handle);
        }

        /// <summary>
        /// Loads a scene.
        /// </summary>
        /// <param name="scene">Scene asset</param>
        /// <param name="sceneName">Scene name</param>
        /// <param name="ownerObject">Object that will load the scene</param>
        /// <param name="priority">Priority for loading the scene</param>
        /// <param name="isActive">Whether the scene should be registered as active upon completion</param>
        /// <param name="onComplete">Callback when the loading is complete</param>
        /// <returns>True if the loading process has started</returns>
        public static bool Load(AssetReferenceScene scene, string sceneName,
            GameObject ownerObject, int priority, bool isActive, Action onComplete = null)
        {
            // Returns false if the scene is already loaded or if loading fails.
            if (Instance._ownership.AddOwner(sceneName, ownerObject) == false || 
                Instance._sceneLoad.Load(scene, priority, isActive, out var opHandle) == false)
            {
                return false;
            }
            Instance._sceneHandle.Add(opHandle, scene, onComplete);
            return true;
        }
        
        /// <summary>
        /// Unloads a scene.
        /// </summary>
        /// <param name="scene">Scene asset</param>
        /// <param name="sceneName">Scene name</param>
        /// <param name="onComplete">Callback when the unloading is complete</param>
        public static void Unload(AssetReferenceScene scene, string sceneName, Action onComplete = null)
        {
            Instance._ownership.RemoveOwner(sceneName);
            Instance._sceneLoad.Unload(scene, sceneName, onComplete);
        }

        /// <summary>
        /// Gets the owner of a scene.
        /// </summary>
        /// <param name="scene">Scene called by SceneLoader</param>
        /// <returns>The object that called SceneLoader for the scene. Returns null if it does not exist.</returns>
        public static GameObject GetOwner(Scene scene) => Instance._ownership.GetOwner(scene);

        /// <summary>
        /// Gets the owner of a scene.
        /// </summary>
        /// <param name="obj">GameObject within the scene called by SceneLoader</param>
        /// <returns>The object that called SceneLoader for the scene. Returns null if it does not exist.</returns>
        public static GameObject GetOwner(GameObject obj) => GetOwner(obj.scene);
    }
}
