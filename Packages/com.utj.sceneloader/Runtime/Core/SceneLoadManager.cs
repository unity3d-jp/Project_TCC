using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Unity.SceneManagement
{
    /// <summary>
    /// A class that manages scene loading.
    /// </summary>
    internal class SceneLoadManager : IInitializeOnEnterPlayMode
    {
        public SceneLoadManager()
        {
            SceneInitialization.Instance.Register(this);
        }
        
        /// <summary>
        /// Initialization
        /// </summary>
        void IInitializeOnEnterPlayMode.OnEnterPlayMode()
        {
            _loadedHandles.Clear();
        }
        
        /// <summary>
        /// List of currently loaded scenes.
        /// </summary>
        private readonly List<AssetReferenceScene> _loadedHandles = new();

        /// <summary>
        /// Load a scene.
        /// </summary>
        /// <param name="scene">The scene to load.</param>
        /// <param name="priority">The priority of the scene.</param>
        /// <param name="isActive">Set the scene as active when loaded.</param>
        /// <param name="opHandle">The load handle.</param>
        /// <returns>True if the load operation was executed.</returns>
        public bool Load(AssetReferenceScene scene, int priority, bool isActive, out AsyncOperationHandle<SceneInstance> opHandle)
        {
            opHandle = default;
            if (_loadedHandles.Contains(scene))
                return false; 
            opHandle = scene.LoadSceneAsync(LoadSceneMode.Additive, true, priority);
            opHandle.Completed += sceneInstance =>
            {
                if (sceneInstance.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError("scene load failed");
                    return;
                }
                _loadedHandles.Add(scene);

                if (isActive)
                    SceneManager.SetActiveScene(sceneInstance.Result.Scene);
            };

            return true;
        }

        /// <summary>
        /// Unload a scene.
        /// </summary>
        /// <param name="scene">The scene to unload.</param>
        /// <param name="sceneName">The name of the scene.</param>
        /// <param name="onComplete">Callback when the release operation is completed.</param>
        public void Unload(AssetReferenceScene scene, string sceneName, Action onComplete)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif
            // Scenes loaded from SceneLoader (Addressable) are released using Addressable.
            // Scenes loaded from SceneManager are released using SceneManager.
            if (_loadedHandles.Contains(scene))
            {
                var op = scene.UnLoadScene();
                op.CompletedTypeless += (c) =>
                {
                    // Remove the loaded scene from the list.
                    _loadedHandles.Remove(scene);
                    onComplete?.Invoke();
                };
            }
            else
            {
                SceneManager.UnloadSceneAsync(sceneName).completed += _=> onComplete?.Invoke();
            }
        }
    }
}
