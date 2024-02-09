using UnityEngine.Events;

namespace Unity.TinyCharacterController.Interfaces.Components
{
    /// <summary>
    /// Callback events for landing and leaving
    /// </summary>
    public interface IGravityEvent
    {
        /// <summary>
        /// Event for landing
        /// Receives the fall speed.
        /// </summary>
        UnityEvent<float> OnLanding { get; }
        
        /// <summary>
        /// Event for leaving the ground
        /// </summary>
        UnityEvent OnLeave { get; }
    }
}