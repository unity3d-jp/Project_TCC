using UnityEngine;
using UnityEngine.Events;

namespace Unity.TinyCharacterController.Interfaces.Components
{
    /// <summary>
    /// Interface for accessing information about ground objects.
    /// </summary>
    public interface IGroundObject
    {
        /// <summary>
        /// True when the grounded GameObject has changed.
        /// </summary>
        bool IsChangeGroundObject { get; }
        
        /// <summary>
        /// Currently recognized ground GameObject.
        /// </summary>
        GameObject GroundObject { get; }
        
        /// <summary>
        /// Currently recognized ground Collider.
        /// </summary>
        Collider GroundCollider { get; }
        
        /// <summary>
        /// Callback when the ground object changes.
        /// </summary>
        UnityEvent<GameObject> OnChangeGroundObject { get; }
    }
}