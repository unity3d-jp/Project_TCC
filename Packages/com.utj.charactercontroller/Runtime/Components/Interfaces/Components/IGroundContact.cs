using UnityEngine;

namespace Unity.TinyCharacterController.Interfaces.Components
{
    /// <summary>
    /// Interface for accessing components that determine whether the character is in contact with the ground.
    /// </summary>
    public interface IGroundContact 
    {
        /// <summary>
        /// True if in contact with the ground.
        /// This grounding detection uses ambiguous criteria.
        /// </summary>
        bool IsOnGround { get; }
        
        /// <summary>
        /// True if strictly in contact with the ground.
        /// This grounding detection uses strict criteria.
        /// </summary>
        bool IsFirmlyOnGround { get; }
        
        /// <summary>
        /// Relative distance from the ground.
        /// </summary>
        float DistanceFromGround { get; }
        
        /// <summary>
        /// Normal vector of the current ground.
        /// </summary>
        Vector3 GroundSurfaceNormal { get; }
        
        /// <summary>
        /// Point where contact with the ground is made.
        /// </summary>
        Vector3 GroundContactPoint { get; }
    }
}