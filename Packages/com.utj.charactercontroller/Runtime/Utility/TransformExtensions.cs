using UnityEngine;

namespace Unity.TinyCharacterController.Utility
{
    /// <summary>
    /// Handy class for Transform operations.
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Get rotation towards a specified direction while ignoring the Y-axis.
        /// </summary>
        /// <param name="self">Self</param>
        /// <param name="target">Transform to face towards</param>
        /// <returns>Rotation to face the target</returns>
        public static Quaternion GetYawRotationToTarget(this Transform self, Transform target)
        {
            return GetYawRotationToPosition(self, target.position);
        }
        
        /// <summary>
        /// Get rotation towards a specified direction while ignoring the Y-axis.
        /// </summary>
        /// <param name="self">Self</param>
        /// <param name="position">Position to face towards</param>
        /// <returns>Rotation to face the target</returns>
        public static Quaternion GetYawRotationToPosition(this Transform self, Vector3 position)
        {
            var delta = position - self.position;
            delta.y = 0;
            return Quaternion.LookRotation(delta, Vector3.up);
        }

        /// <summary>
        /// Get the direction vector towards a target.
        /// </summary>
        /// <param name="self">Self</param>
        /// <param name="target">Target Transform</param>
        /// <returns>Direction vector</returns>
        public static Vector3 GetDirectionToTarget(this Transform self, Transform target)
        {
            return GetDirectionToPosition(self, target.position);
        }

        /// <summary>
        /// Extract only the XZ components from a Vector3.
        /// For Vector3.One, it becomes (1, 0, 1).
        /// </summary>
        /// <param name="self">Self</param>
        /// <returns>Value with Y-axis set to 0</returns>
        public static Vector3 GetXZ(this Vector3 self)
        {
            return new Vector3(self.x, 0, self.z);
        }

        /// <summary>
        /// Extract only the XZ components from a Vector3Int.
        /// For Vector3.One, it becomes (1, 0, 1).
        /// </summary>
        /// <param name="self">Self</param>
        /// <returns>Value with Y-axis set to 0</returns>
        public static Vector3Int GetXZ(this Vector3Int self)
        {
            return new Vector3Int(self.x, 0, self.z);
        }

        /// <summary>
        /// Get the direction vector towards a specified position.
        /// </summary>
        /// <param name="self">Self</param>
        /// <param name="position">Target position</param>
        /// <returns>Direction vector</returns>
        public static Vector3 GetDirectionToPosition(this Transform self, Vector3 position)
        {
            var delta = position - self.position;

            return delta.sqrMagnitude > 0 ? delta.normalized : Vector3.zero;
        }

        /// <summary>
        /// Get the distance to a target Transform.
        /// </summary>
        /// <param name="self">Self</param>
        /// <param name="target">Target Transform</param>
        /// <returns>Distance</returns>
        public static float GetDistanceFromTransform(this Transform self, Transform target)
        {
            return GetDistanceFromPosition(self, target.position);
        }
        
        /// <summary>
        /// Get the distance to a target position.
        /// </summary>
        /// <param name="self">Self</param>
        /// <param name="position">Target position</param>
        /// <returns>Distance</returns>
        public static float GetDistanceFromPosition(this Transform self, Vector3 position)
        {
            return Vector3.Distance(self.position, position);
        }

        /// <summary>
        /// Get the angle between Transform's forward and a vector.
        /// </summary>
        /// <param name="self">Self</param>
        /// <param name="direction">Vector</param>
        /// <param name="ignoreY">Ignore Y-axis</param>
        /// <returns>Angle</returns>
        public static float GetDeltaAngle(this Transform self, Vector3 direction, bool ignoreY = true)
        {
            var forward = self.forward;
            if (ignoreY)
            {
                forward = Vector3.ProjectOnPlane(forward, Vector3.up);
                direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            }
            
            return Vector3.SignedAngle(forward, direction, Vector3.up);
        }
        
        /// <summary>
        /// Get the angle between Transform's forward and a rotation.
        /// </summary>
        /// <param name="self">Self</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="ignoreY">Ignore Y-axis</param>
        /// <returns>Angle</returns>
        public static float GetDeltaAngle(this Transform self, Quaternion rotation, bool ignoreY = true)
        {
            var direction = rotation * Vector3.forward;
            return GetDeltaAngle(self, direction, ignoreY);
        }
    }
}
