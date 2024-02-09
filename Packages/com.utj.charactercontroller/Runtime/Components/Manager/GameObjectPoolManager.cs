using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Utility;

[assembly: RenamedNamespace("TinyCharacterController.Utility", "Utj.TinyCharacterController.Manager")]
namespace Unity.TinyCharacterController.Manager
{
    /// <summary>
    /// A class that registers and manages <see cref="IGameObjectPool"/>.
    /// It generates and retrieves new instances based on the key of the <see cref="IGameObjectPool"/>
    /// or retrieves <see cref="IGameObjectPool"/> instances in the scene based on the key.
    /// </summary>
    /// <seealso cref="GameObjectPool"/>
    [RenamedFrom("TinyCharacterController.Utility.GameObjectPoolManager")]
    [RenamedFrom("TinyCharacterController.Manager.GameObjectPoolManager")]
    public class GameObjectPoolManager : Utility.Singleton<GameObjectPoolManager>
    {
        /// <summary>
        /// List of components to register.
        /// </summary>
        private readonly List<IGameObjectPool> _components = new();

        /// <summary>
        /// Register a GameObjectPool.
        /// </summary>
        /// <param name="pool">The GameObjectPool to register.</param>
        /// <returns>Registration success</returns>
        public static bool Register(IGameObjectPool pool)
        {
            // Check if there are objects belonging to the same scene and registered with the same prefab.
            var alreadyRegistered = Instance._components.Exists(c =>
                c.PrefabID == pool.PrefabID && c.Scene == pool.Scene);

            // If not registered, then register it.
            if (alreadyRegistered == false)
                Instance._components.Add(pool);

            return alreadyRegistered == false;
        }

        /// <summary>
        /// Unregister a GameObjectPool.
        /// </summary>
        /// <param name="pool">The GameObjectPool to unregister.</param>
        public static void Unregister(IGameObjectPool pool)
        {
            if (IsCreated)
                Instance._components.Remove(pool);
        }

        /// <summary>
        /// Retrieve a GameObject from a <see cref="GameObjectPool"/> with the specified Prefab.
        /// </summary>
        /// <param name="prefab">The prefab to clone.</param>
        /// <returns>The reused GameObject.</returns>
        public static GameObject Get(IPooledObject prefab) => GetPool(prefab).Get().GameObject;

        /// <summary>
        /// Retrieve a GameObject from a <see cref="GameObjectPool"/> with the specified Prefab, position, and rotation.
        /// </summary>
        /// <param name="prefab">The prefab to clone.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The reused GameObject.</returns>
        public static GameObject Get(IPooledObject prefab, in Vector3 position, in Quaternion rotation) =>
            GetPool(prefab).Get(position, rotation).GameObject;

        /// <summary>
        /// Retrieve a GameObject from a <see cref="GameObjectPool"/> with the specified Prefab.
        /// </summary>
        /// <param name="prefab">The prefab to clone.</param>
        /// <param name="scene">Target Scene</param>
        /// <returns>The reused GameObject.</returns>
        public static GameObject Get(IPooledObject prefab, Scene scene) => GetPool(prefab, scene).Get().GameObject;

        /// <summary>
        /// Retrieve a GameObject from a <see cref="GameObjectPool"/> with the specified Prefab, position, and rotation.
        /// </summary>
        /// <param name="prefab">The prefab to clone.</param>
        /// <param name="scene">Target Scene</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The reused GameObject.</returns>
        public static GameObject Get(IPooledObject prefab, Scene scene, in Vector3 position, in Quaternion rotation) =>
            GetPool(prefab, scene).Get(position, rotation).GameObject;


        /// <summary>
        /// Release an instance.
        /// This API is maintained for backward compatibility and ease of use in Visual Scripting.
        /// </summary>
        /// <param name="instance">The instance to return.</param>
        public static void Release(IPooledObject instance) => instance.Release();

        /// <summary>
        /// Retrieve a GameObject from a <see cref="GameObjectPool"/> associated with the specified prefab.
        /// This API is maintained for backward compatibility and ease of use in VisualScripting.
        /// </summary>
        /// <param name="prefab">The prefab to clone.</param>
        /// <returns>The reused GameObject.</returns>
        public static GameObject Get(PooledGameObject prefab) => GetPool(prefab).Get().GameObject;


        /// <summary>
        /// Retrieve a GameObject from a <see cref="GameObjectPool"/> associated with the specified prefab, position, and rotation.
        /// This API is maintained for backward compatibility and ease of use in VisualScripting.
        /// </summary>
        /// <param name="prefab">The prefab to clone.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The reused GameObject.</returns>
        public static GameObject Get(PooledGameObject prefab, Vector3 position, Quaternion rotation) =>
            GetPool(prefab).Get(position, rotation).GameObject;

        /// <summary>
        /// Retrieve a GameObject from a <see cref="GameObjectPool"/> associated with the specified prefab.
        /// This API is maintained for backward compatibility and ease of use in VisualScripting.
        /// </summary>
        /// <param name="prefab">The prefab to clone.</param>
        /// <param name="spawnPosition">The Transform that serves as the origin for the generated position.</param>
        /// <returns>The reused GameObject.</returns>
        public static GameObject Get(PooledGameObject prefab, Transform spawnPosition)
        {
            return Get(prefab, spawnPosition.position, spawnPosition.rotation);
        }

        /// <summary>
        /// Return an instance.
        /// This API is maintained for backward compatibility and ease of use in VisualScripting.
        /// </summary>
        /// <param name="instance">The instance to return.</param>
        public static void Release(PooledGameObject instance) => instance.Release();


        /// <summary>
        /// Get a GameObjectPool associated with the specified prefab.
        /// An error will occur if the pool does not exist.
        /// </summary>
        /// <param name="prefab">The prefab associated with the GameObjectPool.</param>
        /// <returns>The GameObjectPool corresponding to the prefab.</returns>
        public static IGameObjectPool GetPool(IPooledObject prefab)
        {
            return Instance.GetPoolInternal(prefab);
        }

        /// <summary>
        /// Get a GameObjectPool associated with the specified prefab in the specified scene.
        /// An error will occur if the pool does not exist.
        /// </summary>
        /// <param name="prefab">The target prefab.</param>
        /// <param name="scene">The scene to search in.</param>
        /// <returns>The GameObjectPool corresponding to the prefab.</returns>
        public static IGameObjectPool GetPool(IPooledObject prefab, Scene scene)
        {
            return Instance.GetPoolInternal(prefab, scene);
        }

        /// <summary>
        /// Destroy the registration of a pool.
        /// </summary>
        /// <param name="prefab">The pool to remove.</param>
        public static void DestroyPool(PooledGameObject prefab)
        {
            if (IsCreated)
                Instance.DestroyPoolInternal(prefab);
        }


        /// <summary>
        /// Retrieve a GameObjectPool associated with the specified prefab in the specified scene.
        /// </summary>
        /// <param name="prefab">The key prefab.</param>
        /// <param name="scene">The scene to search in.</param>
        /// <returns>The GameObjectPool with the specified prefab.</returns>
        /// <exception cref="Exception">GameObjectPool not found.</exception>
        private IGameObjectPool GetPoolInternal(IPooledObject prefab, Scene scene)
        {
            // Get the prefab with a matching instance ID.
            var instanceID = prefab.InstanceId;
            var pool = _components
                .Find(c => c.Scene == scene && c.PrefabID == instanceID);

            if (pool == null)
                throw new Exception($"{((MonoBehaviour)prefab).name} prefab is not registered.");

            return pool;
        }

        /// <summary>
        /// Retrieve a GameObjectPool associated with the specified prefab.
        /// </summary>
        /// <param name="prefab">The prefab to search for.</param>
        /// <returns>The matching GameObjectPool.</returns>
        /// <exception cref="Exception">GameObjectPool not found.</exception>
        private IGameObjectPool GetPoolInternal(IPooledObject prefab)
        {
            var instanceID = prefab.InstanceId;
            var pool = _components.Find(c => c.PrefabID == instanceID);
            if (pool == null)
                throw new Exception($"{((MonoBehaviour)prefab).name} prefab is not registered.");

            return pool;
        }

        /// <summary>
        /// Destroy an instance of the specified PooledObject.
        /// </summary>
        /// <param name="prefab">The object that holds the PooledObject.</param>
        private void DestroyPoolInternal(IPooledObject prefab)
        {
            var instanceID = prefab.InstanceId;
            var pool = _components.Find(c => c.PrefabID == instanceID);
            if (pool != null)
            {
                _components.Remove(pool);
                Destroy((MonoBehaviour)pool);
            }
        }
    }
}