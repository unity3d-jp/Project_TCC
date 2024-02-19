using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Unity.SceneManagement
{
    /// <summary>
    /// Manages handles for loading scenes.
    /// </summary>
    internal class SceneHandleManager : IInitializeOnEnterPlayMode
    {
        public SceneHandleManager()
        {
            SceneInitialization.Instance.Register(this);            
        }
        
        /// <summary>
        /// Initialization
        /// </summary>
        void IInitializeOnEnterPlayMode.OnEnterPlayMode()
        {
            _operationHandles.Clear();
        }
        
        /// <summary>
        /// Associates handles with AssetReferenceScene.
        /// </summary>
        private readonly Dictionary<AssetReferenceScene, AsyncOperationHandle<SceneInstance>> _operationHandles = new();

        /// <summary>
        /// Progress of the handles.
        /// </summary>
        public IEnumerable<AsyncOperationHandle<SceneInstance>> OperationHandles => _operationHandles.Values;

        /// <summary>
        /// Number of handles.
        /// </summary>
        public int OperationCount => _operationHandles.Count;

        /// <summary>
        /// Adds a handle.
        /// </summary>
        /// <param name="opHandle">Handle</param>
        /// <param name="scene">Scene to associate with</param>
        /// <param name="onComplete"></param>
        public void Add(AsyncOperationHandle<SceneInstance> opHandle, AssetReferenceScene scene, Action<Scene> onComplete)
        {
            _operationHandles.Add(scene, opHandle);
            
            opHandle.Completed += result =>
            {
                _operationHandles.Remove(scene);
                onComplete?.Invoke(result.Result.Scene);
            };
        }

        /// <summary>
        /// Gets the handle for a scene being loaded.
        /// </summary>
        /// <param name="scene">Scene to get the handle for</param>
        /// <param name="opHandle">Handle</param>
        /// <returns>True if the scene is being loaded</returns>
        public bool GetHandle(AssetReferenceScene scene, out AsyncOperationHandle<SceneInstance> opHandle)
        {
            return _operationHandles.TryGetValue(scene, out opHandle);
        } 
    }
}
