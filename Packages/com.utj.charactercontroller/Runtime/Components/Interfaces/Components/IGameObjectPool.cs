using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.TinyCharacterController.Interfaces.Components
{
    /// <summary>
    /// An interface for registration in <see cref="TinyCharacterController.Manager.GameObjectPoolManager"/>.
    /// Used for constructing GameObject pools.
    /// Utilized for object acquisition, return, destruction, and also for searching objects from the manager.
    /// </summary>
    [RenamedFrom("Unity.TinyCharacterController.Interfaces.Components.IGameObjectPool")]
    public interface IGameObjectPool
    {
        /// <summary>
        /// Prefab ID (used for searching).
        /// </summary>
        int PrefabID { get; }

        /// <summary>
        /// The scene to which the component belongs.
        /// </summary>
        Scene Scene { get; }

        /// <summary>
        /// Retrieve an object from the object pool.
        /// </summary>
        /// <returns>An object reused from the cache.</returns>
        IPooledObject Get();

        /// <summary>
        /// Retrieve an object from the object pool.
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="rotation">Rotation</param>
        /// <returns>An object reused from the cache.</returns>
        IPooledObject Get(in Vector3 position, in Quaternion rotation);

        /// <summary>
        /// Release a cached object.
        /// </summary>
        /// <param name="obj">Object to release</param>
        void Release(IPooledObject obj);
    }
}