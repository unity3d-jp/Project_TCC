using UnityEngine;

namespace Unity.TinyCharacterController.Interfaces.Components
{
    /// <summary>
    /// Interface for accessing wall detection.
    /// </summary>
    public interface IWallCheck
    {
        /// <summary>
        /// True if in contact with a wall.
        /// </summary>
        public bool IsContact { get; }
        
        /// <summary>
        /// The normal vector of the wall.
        /// </summary>
        public Vector3 Normal { get; }
    }
}