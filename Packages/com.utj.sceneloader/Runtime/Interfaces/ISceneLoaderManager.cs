using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.SceneManagement
{
    public interface ISceneLoaderManager
    {
        /// <summary>
        /// Loads the scene referenced by sceneReference.
        /// Calls OnComplete after the scene is loaded.
        /// </summary>
        /// <param name="sceneReference">The scene to load</param>
        /// <param name="priority">Loading priority</param>
        /// <param name="isActive">Specifies if the scene should be set as the active scene</param>
        /// <param name="onComplete">Callback called after loading is complete</param>
        void Load(AssetReferenceScene sceneReference,  int priority, bool isActive, Action<Scene> onComplete);
        
        /// <summary>
        /// Unloads the scene specified by sceneName and removes its association with sceneReference.
        /// </summary>
        /// <param name="sceneReference">The key to get the handle</param>
        /// <param name="sceneName">The name of the scene to unload</param>
        /// <param name="onComplete">Callback called after unloading is complete</param>
        void Unload(AssetReferenceScene sceneReference, string sceneName, Action onComplete);
        
        /// <summary>
        /// Registers an owner
        /// </summary>
        /// <param name="sceneName">The scene name to associate</param>
        /// <param name="owner">The owner object</param>
        void Register(string sceneName, GameObject owner);
        
        /// <summary>
        /// Unregisters an owner
        /// </summary>
        /// <param name="sceneName">The scene name to associate</param>
        void Unregister(string sceneName);
    }
}