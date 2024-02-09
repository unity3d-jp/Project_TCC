using UnityEngine;

namespace Unity.TinyCharacterController.Interfaces.Core
{
    /// <summary>
    /// Interface for consistently accessing coordinates and orientations other than Transform.
    /// Values are cached in advance before Update, and cached values are used for Get operations.
    /// For Set operations, the changes are immediately reflected on the target component.
    /// </summary>
    public interface ITransform
    {
        /// <summary>
        /// Coordinates
        /// </summary>
        Vector3 Position { get; set; }
        
        /// <summary>
        /// Orientation
        /// </summary>
        Quaternion Rotation { get; set; }
    }
}