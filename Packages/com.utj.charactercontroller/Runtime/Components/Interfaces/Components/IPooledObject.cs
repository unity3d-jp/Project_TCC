using Unity.TinyCharacterController.Utility;
using UnityEngine;

namespace Unity.TinyCharacterController.Interfaces.Components
{
    public interface IPooledObject
    {
        /// <summary>
        /// Initialize the object.
        /// Called by GameObjectPool.
        /// </summary>
        /// <param name="owner">The owner that generates the prefab.</param>
        /// <param name="hasRigidbody">True if the prefab has a <see cref="Rigidbody"/>.</param>
        void Initialize(IGameObjectPool owner, bool hasRigidbody);
        
        /// <summary>
        /// The corresponding GameObject.
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// An instance ID to identify the object.
        /// </summary>
        int InstanceId { get; }

        /// <summary>
        /// Called when the object is retrieved.
        /// </summary>
        void OnGet();
        
        /// <summary>
        /// True if the object is used.
        /// </summary>
        bool IsUsed { get; }
        
        /// <summary>
        /// Called when the object is released.
        /// </summary>
        void OnRelease();

        /// <summary>
        /// Release the component.
        /// </summary>
        void Release();
    }
}