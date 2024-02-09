using UnityEngine;

namespace UTJ
{
    [System.Serializable]
    public class AngleLimits
    {
        public bool active = false;
        [Range(-180f, 0f)]
        public float min = 0f;
        [Range(0f, 180f)]
        public float max = 0f;

        public static Vector3 GetAngleVector(Vector3 sideVector, Vector3 forwardVector, float degrees)
        {
            var radians = Mathf.Deg2Rad * degrees;
            return Mathf.Sin(radians) * sideVector + Mathf.Cos(radians) * forwardVector;
        }

        public void CopyTo(AngleLimits target)
        {
            target.active = active;
            target.min = min;
            target.max = max;
        }

        private static float ComputeFalloff(float value, float range)
        {
            const float Threshold = 0.0001f;
            if (Mathf.Abs(range) <= Threshold) { return 0f; }

            var normalizedValue = value / range;
            normalizedValue = Mathf.Clamp01(normalizedValue);
            return Mathf.Min(normalizedValue, Mathf.Sqrt(normalizedValue));
        }

        // Returns true if exceeded bounds
        public bool ConstrainVector
        (
            Vector3 basisSide,
            Vector3 basisUp,
            Vector3 basisForward,
            float springStrength,
            float deltaTime,
            ref Vector3 vector
        )
        {
            var upProjection = Vector3.Project(vector, basisUp);
            var projection = vector - upProjection;
            var projectionMagnitude = projection.magnitude;
            var originalSine = Vector3.Dot(projection / projectionMagnitude, basisSide);
            // The above math might have a bit of floating point error 
            // so clamp the sine value into a valid range so we don't get NaN later
            originalSine = Mathf.Clamp(originalSine, -1f, 1f);

            // Use soft limits based on Hooke's Law to reduce jitter,
            // then apply hard limits
            var newAngle = Mathf.Rad2Deg * Mathf.Asin(originalSine);
            var acceleration = -newAngle * springStrength;
            newAngle += acceleration * deltaTime * deltaTime;

            var minAngle = min;
            var maxAngle = max;
            var preClampAngle = newAngle;
            newAngle = Mathf.Clamp(newAngle, minAngle, maxAngle);

            // Apply falloff
            var curveLimit = (newAngle < 0f) ? minAngle : maxAngle;
            newAngle = ComputeFalloff(newAngle, curveLimit) * curveLimit;

            var radians = Mathf.Deg2Rad * newAngle;
            var newProjection = Mathf.Sin(radians) * basisSide + Mathf.Cos(radians) * basisForward;
            newProjection *= projectionMagnitude;
            vector = newProjection + upProjection;

            return newAngle != preClampAngle;
        }

#if UNITY_EDITOR
        public void DrawLimits
        (
            Vector3 origin,
            Vector3 sideVector,
            Vector3 forwardVector,
            float drawScale
        )
        {
            DrawAngleLimit(origin, sideVector, forwardVector, min, drawScale);
            DrawAngleLimit(origin, sideVector, forwardVector, max, drawScale);
        }

        public static void DrawAngleLimit
        (
            Vector3 origin,
            Vector3 sideVector,
            Vector3 forwardVector,
            float angleLimit,
            float scale
        )
        {
            const int BaseIterationCount = 3;

            var lastPoint = origin + scale * forwardVector;
            var iterationCount = (Mathf.RoundToInt(Mathf.Abs(angleLimit) / 45f) + 1) * BaseIterationCount;
            var deltaAngle = angleLimit / iterationCount;
            var angle = deltaAngle;
            for (var iteration = 0; iteration < iterationCount; ++iteration)
            {
                var newPoint = origin + scale * GetAngleVector(sideVector, forwardVector, angle);
                UnityEditor.Handles.DrawLine(lastPoint, newPoint);
                lastPoint = newPoint;
                angle += deltaAngle;
            }
            UnityEditor.Handles.DrawLine(origin, lastPoint);
        }
#endif
    }
}