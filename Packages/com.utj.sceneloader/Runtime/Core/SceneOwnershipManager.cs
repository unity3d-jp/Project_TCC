using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.SceneManagement
{
    /// <summary>
    /// A class that manages scene ownership.
    /// </summary>
    internal class SceneOwnershipManager : IInitializeOnEnterPlayMode
    {
        public SceneOwnershipManager()
        {
            SceneInitialization.Instance.Register(this);
        }
        
        void IInitializeOnEnterPlayMode.OnEnterPlayMode()
        {
            _sceneOwners.Clear();
        }
        
        private readonly Dictionary<PropertyName, GameObject> _sceneOwners = new();

        /// <summary>
        /// Add an owner to a scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <param name="owner">The owner's GameObject.</param>
        /// <returns></returns>
        public bool AddOwner(string sceneName, GameObject owner)
        {
            var id = new PropertyName(sceneName);
            var result = _sceneOwners.TryAdd(id, owner);
            return result;
        }

        /// <summary>
        /// Remove an owner from a scene.
        /// </summary>
        /// <param name="sceneName">The name of the owner.</param>
        public void RemoveOwner(string sceneName)
        {
            var id = new PropertyName(sceneName);
            _sceneOwners.Remove(id);
        }

        /// <summary>
        /// Get the owner of a scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns></returns>
        public GameObject GetOwner(Scene scene)
        {
            var id = new PropertyName(scene.name);

            // If an object is registered in advance, get the element.
            if (_sceneOwners.TryGetValue(id, out var sceneOwner))
                return sceneOwner;
            
            // Traverse the entire scene to get the element.
            // This process is mainly executed when there is a possibility of referencing the Owner before initialization in Awake, etc.
            var loaders = Object.FindObjectsByType<SceneLoader>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
            var loader = System.Array.Find(loaders, c => c.SceneName == scene.name);
            
            return loader != null ? loader.gameObject : null;
        }
    }
}
