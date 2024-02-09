using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    internal class WarpManager
    {
        /// <summary>
        /// Gets the current position of the warp manager.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// Gets the current rotation of the warp manager.
        /// </summary>
        public Quaternion Rotation { get; private set; }

        /// <summary>
        /// Indicates whether the position has been warped.
        /// </summary>
        public bool WarpedPosition { get; private set; }

        /// <summary>
        /// Indicates whether the rotation has been warped.
        /// </summary>
        public bool WarpedRotation { get; private set; }
        
        /// <summary>
        /// Indicates whether the warp manager is in motion.
        /// </summary>
        public bool IsMove { get; private set; }

        /// <summary>
        /// Resets warp status, including position, rotation, and motion state.
        /// </summary>
        public void ResetWarp()
        {
            WarpedPosition = WarpedRotation = false;
            IsMove = false;
        }

        /// <summary>
        /// Warps the character to a new position and rotation.
        /// </summary>
        /// <param name="position">The new position to warp to.</param>
        /// <param name="rotation">The new rotation to warp to.</param>
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            WarpedPosition = true;
            WarpedRotation = true;
            Position = position;
            Rotation = rotation;
            IsMove = false;
        }

        /// <summary>
        /// Updates the position of the warp manager. Warp movement takes precedence over IMove.
        /// </summary>
        /// <param name="position">The new position to move to.</param>
        public void SetPosition(Vector3 position)
        {
            WarpedPosition = true;
            Position = position;
            IsMove = false;
        }

        /// <summary>
        /// Updates the rotation of the warp manager. Warp movement takes precedence over ITurn.
        /// </summary>
        /// <param name="rotation">The new rotation.</param>
        public void SetRotation(Quaternion rotation)
        {
            WarpedRotation = true;
            Rotation = rotation;
            IsMove = false;
        }

        /// <summary>
        /// Updates the position of the warp manager, considering obstacles.
        /// </summary>
        /// <param name="position">The new position, taking into account obstacles.</param>
        public void Move(Vector3 position)
        {
            WarpedPosition = true;
            IsMove = true;
            Position = position;
        }
    }
}