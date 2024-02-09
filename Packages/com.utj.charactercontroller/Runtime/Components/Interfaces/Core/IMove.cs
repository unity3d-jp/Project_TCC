using UnityEngine;

namespace Unity.TinyCharacterController.Interfaces.Core
{
    /// <summary>
    /// Interface for controlling character movement.
    /// </summary>
    public interface IMove : IPriority<IMove>
    {
        /// <summary>
        /// Movement vector.
        /// </summary>
        Vector3 MoveVelocity { get; }
    }
}