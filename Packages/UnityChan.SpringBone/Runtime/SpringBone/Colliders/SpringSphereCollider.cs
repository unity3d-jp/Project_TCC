using UnityEngine;

namespace UTJ
{
    public class SpringSphereCollider : MonoBehaviour
    {
        public float radius = 0.1f;

        // If linkedRenderer is not null, the collider will be enabled 
        // based on whether the renderer is
        public Renderer linkedRenderer;

        public SpringBone.CollisionStatus CheckForCollisionAndReact
        (
            Vector3 headPosition,
            ref Vector3 tailPosition,
            float tailRadius,
            ref Vector3 hitNormal
        )
        {
            var localHeadPosition = transform.InverseTransformPoint(headPosition);
            var localTailPosition = transform.InverseTransformPoint(tailPosition);
            var localTailRadius = transform.InverseTransformDirection(tailRadius, 0f, 0f).magnitude;

#if UNITY_EDITOR
            var originalLocalTailPosition = localTailPosition;
#endif

            var result = CheckForCollisionAndReact(
                localHeadPosition,
                ref localTailPosition,
                localTailRadius,
                new Vector3(0f, 0f, 0f),
                radius);

            if (result != SpringBone.CollisionStatus.NoCollision)
            {
#if UNITY_EDITOR
                RecordCollision(originalLocalTailPosition, tailRadius, result);
#endif
                tailPosition = transform.TransformPoint(localTailPosition);
                hitNormal = transform.TransformDirection(localTailPosition.normalized).normalized;
            }

            return result;
        }

        public static SpringBone.CollisionStatus CheckForCollisionAndReact
        (
            Vector3 localHeadPosition,
            ref Vector3 localTailPosition,
            float localTailRadius,
            Vector3 sphereLocalOrigin,
            float sphereRadius
        )
        {
            var combinedRadius = sphereRadius + localTailRadius;
            if ((localTailPosition - sphereLocalOrigin).sqrMagnitude >= combinedRadius * combinedRadius)
            {
                // Not colliding
                return SpringBone.CollisionStatus.NoCollision;
            }

            var originToHead = localHeadPosition - sphereLocalOrigin;
            if (originToHead.sqrMagnitude <= sphereRadius * sphereRadius)
            {
                // The head is inside the sphere, so just try to push the tail out
                localTailPosition = 
                    sphereLocalOrigin + (localTailPosition - sphereLocalOrigin).normalized * combinedRadius;
                return SpringBone.CollisionStatus.HeadIsEmbedded;
            }

            var localHeadRadius = (localTailPosition - localHeadPosition).magnitude;
            var intersection = new Circle3();
            if (ComputeIntersection(
                localHeadPosition,
                localHeadRadius,
                sphereLocalOrigin,
                combinedRadius,
                ref intersection))
            {
                localTailPosition = ComputeNewTailPosition(intersection, localTailPosition);
            }

            return SpringBone.CollisionStatus.TailCollision;
        }

        // private

        // http://mathworld.wolfram.com/Sphere-SphereIntersection.html
        public static bool ComputeIntersection
        (
            Vector3 originA,
            float radiusA,
            Vector3 originB,
            float radiusB,
            ref Circle3 intersection
        )
        {
            var aToB = originB - originA;
            var dSqr = aToB.sqrMagnitude;
            var d = Mathf.Sqrt(dSqr);
            if (d <= 0f)
            {
                return false;
            }

            var radiusASqr = radiusA * radiusA;
            var radiusBSqr = radiusB * radiusB;

            // Assume a is at the origin and b is at (d, 0 0)
            var denominator = 0.5f / d;
            var subTerm = dSqr - radiusBSqr + radiusASqr;
            var x = subTerm * denominator;
            var squaredTerm = subTerm * subTerm;
            var intersectionRadius = Mathf.Sqrt(4f * dSqr * radiusASqr - squaredTerm) * denominator;

            var upVector = aToB / d;
            var origin = originA + x * upVector;

            intersection.origin = origin;
            intersection.upVector = upVector;
            intersection.radius = intersectionRadius;

            return true;
        }

        public static Vector3 ComputeNewTailPosition(Circle3 intersection, Vector3 tailPosition)
        {
            // http://stackoverflow.com/questions/300871/best-way-to-find-a-point-on-a-circle-closest-to-a-given-point
            // Project child's position onto the plane
            var newTailPosition = tailPosition
                - Vector3.Dot(intersection.upVector, tailPosition - intersection.origin) * intersection.upVector;
            var v = newTailPosition - intersection.origin;
            var newPosition = intersection.origin + intersection.radius * v.normalized;
            return newPosition;
        }

#if UNITY_EDITOR
        public bool shouldDrawGizmosThisFrame;

        public void DrawGizmos(Color drawColor)
        {
            var worldRadius = transform.TransformDirection(radius, 0f, 0f).magnitude;
            // For picking
            Gizmos.color = new Color(0f, 0f, 0f, 0f);
            Gizmos.DrawWireSphere(transform.position, worldRadius);

            UnityEditor.Handles.color = drawColor;
            UnityEditor.Handles.RadiusHandle(Quaternion.identity, transform.position, worldRadius);
            if (colliderDebug != null)
            {
                colliderDebug.DrawGizmosAndClear();
            }
        }

        private SpringManager manager;
        private SpringColliderDebug colliderDebug;

        private void OnDrawGizmos()
        {
            if (shouldDrawGizmosThisFrame || !SpringManager.onlyShowSelectedColliders)
            {
                if (manager == null) { manager = GetComponentInParent<SpringManager>(); }
                DrawGizmos((enabled && manager != null) ? manager.colliderColor : Color.gray);
                shouldDrawGizmosThisFrame = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos(enabled ? Color.white : Color.gray);
        }

        private void RecordCollision
        (
            Vector3 localMoverPosition,
            float worldMoverRadius,
            SpringBone.CollisionStatus collisionStatus
        )
        {
            if (!enabled) { return; }
            if (colliderDebug == null) { colliderDebug = new SpringColliderDebug(); }
            var localNormal = (localMoverPosition).normalized;
            var localContactPoint = localNormal * radius;
            var worldNormal = transform.TransformDirection(localNormal).normalized;
            var worldContactPoint = transform.TransformPoint(localContactPoint);
            colliderDebug.RecordCollision(worldContactPoint, worldNormal, worldMoverRadius, collisionStatus);
        }
#endif
    }
}
