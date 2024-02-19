using UnityEngine;

namespace Unity.TinyCharacterController.Interfaces.Core
{
    /// <summary>
    /// Interface for adding acceleration to a character.
    /// </summary>
    public interface IEffect 
    {
        /// <summary>
        /// The acceleration to be added.
        /// </summary>
        Vector3 Velocity { get; }

        void ResetVelocity();
    }
}