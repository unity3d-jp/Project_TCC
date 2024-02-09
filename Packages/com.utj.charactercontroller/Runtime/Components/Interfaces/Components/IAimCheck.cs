using UnityEngine;

namespace Unity.TinyCharacterController.Interfaces.Components
{
    /// <summary>
    /// Interface for accessing information about objects in the line of sight.
    /// </summary>
    public interface IAimCheck
    {
        /// <summary>
        /// True if there is an object in the line of sight.
        /// </summary>
        bool IsHit { get; }
        
        /// <summary>
        /// Starting point for issuing a RayCast.
        /// </summary>
        Vector3 Origin { get; }
        
        /// <summary>
        /// Direction from Origin to the point in the line of sight.
        /// </summary>
        Vector3 Direction { get; }
        
        /// <summary>
        /// Position of the point in the line of sight.
        /// </summary>
        Vector3 Point { get; }
        
        /// <summary>
        /// Distance to the point in the line of sight.
        /// </summary>
        float Distance { get; }
        
        /// <summary>
        /// Normal vector at the point in the line of sight.
        /// </summary>
        Vector3 Normal { get; }
    }
}