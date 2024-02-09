using UnityEngine;

namespace Unity.TinyCharacterController.Interfaces.Components
{
    /// <summary>
    /// Interface for controlling linear interpolation movement from the current position to a target position.
    /// </summary>
    internal interface ILinearInterpolationMove
    {
        /// <summary>
        /// Sets the progress of the linear interpolation movement.
        /// </summary>
        /// <param name="moveAmount">Progress of movement</param>
        /// <param name="turnAmount">Progress of turning</param>
        void SetNormalizedTime(float moveAmount, float turnAmount);

        /// <summary>
        /// Starts the linear interpolation movement transition.
        /// </summary>
        /// <param name="id">ID for state identification</param>
        void Play(PropertyName id);

        /// <summary>
        /// Ends the linear interpolation movement transition.
        /// </summary>
        /// <param name="id">ID for state identification</param>
        void Stop(PropertyName id);

        /// <summary>
        /// Moves to the target position using linear interpolation without performing Play.
        /// </summary>
        void FitTargetWithoutPlay(PropertyName id);
    }
}