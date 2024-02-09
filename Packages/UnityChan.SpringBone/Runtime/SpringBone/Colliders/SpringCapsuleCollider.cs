using UnityEngine;

namespace UTJ
{
    // Up is y-axis
    public class SpringCapsuleCollider : MonoBehaviour
    {
        public float radius = 0.075f;
        public float height = 0.3f;
        
        // If linkedRenderer is not null, the collider will be enabled 
        // based on whether the renderer is
        public Renderer linkedRenderer;

        public SpringBone.CollisionStatus CheckForCollisionAndReact
        (
            Vector3 moverHeadPosition, 
            ref Vector3 moverPosition, 
            float moverRadius,
            ref Vector3 hitNormal
        )
        {
            const float RadiusThreshold = 0.0001f;
            if ((linkedRenderer != null && !linkedRenderer.enabled)
                || radius <= RadiusThreshold)
            {
                return SpringBone.CollisionStatus.NoCollision;
            }

            var worldToLocal = transform.worldToLocalMatrix;
            var radiusScale = worldToLocal.MultiplyVector(new Vector3(1f, 0f, 0f)).magnitude;

            // Lower than start cap
            var localHeadPosition = worldToLocal.MultiplyPoint3x4(moverHeadPosition);
            var localMoverPosition = worldToLocal.MultiplyPoint3x4(moverPosition);
            var localMoverRadius = moverRadius * radiusScale;

            var moverIsAboveTop = localMoverPosition.y >= height;
            var useSphereCheck = (localMoverPosition.y <= 0f) | moverIsAboveTop;
            if (useSphereCheck)
            {
                var sphereOrigin = new Vector3(0f, moverIsAboveTop ? height : 0f, 0f);
                var combinedRadius = localMoverRadius + radius;
                if ((localMoverPosition - sphereOrigin).sqrMagnitude >= combinedRadius * combinedRadius)
                {
                    // Not colliding
                    return SpringBone.CollisionStatus.NoCollision;
                }

                var originToHead = localHeadPosition - sphereOrigin;
                var isHeadEmbedded = originToHead.sqrMagnitude <= radius * radius;

#if UNITY_EDITOR
                RecordSphereCollision(
                    sphereOrigin,
                    localMoverPosition,
                    moverRadius,
                    isHeadEmbedded ? 
                        SpringBone.CollisionStatus.HeadIsEmbedded : 
                        SpringBone.CollisionStatus.TailCollision);
#endif

                if (isHeadEmbedded)
                {
                    // The head is inside the sphere, so just try to push the tail out
                    var localHitNormal = (localMoverPosition - sphereOrigin).normalized;
                    localMoverPosition = sphereOrigin + localHitNormal * combinedRadius;
                    moverPosition = transform.TransformPoint(localMoverPosition);
                    hitNormal = transform.TransformDirection(localHitNormal).normalized;
                    return SpringBone.CollisionStatus.HeadIsEmbedded;
                }

                var localHeadRadius = (localMoverPosition - localHeadPosition).magnitude;
                var intersection = new Circle3();
                if (SpringSphereCollider.ComputeIntersection(
                    localHeadPosition,
                    localHeadRadius,
                    sphereOrigin,
                    combinedRadius,
                    ref intersection))
                {
                    localMoverPosition = SpringSphereCollider.ComputeNewTailPosition(intersection, localMoverPosition);
                    moverPosition = transform.TransformPoint(localMoverPosition);
                    var localHitNormal = (localMoverPosition - sphereOrigin).normalized;
                    hitNormal = transform.TransformDirection(localHitNormal).normalized;
                }

                return SpringBone.CollisionStatus.TailCollision;
            }

            // Cylinder
            var collisionStatus = CheckForCylinderCollisionAndReact(
                localHeadPosition, ref moverPosition, localMoverRadius, localMoverPosition, ref hitNormal);
            return collisionStatus;
        }

        // private

        private SpringBone.CollisionStatus CheckForCylinderCollisionAndReact
        (
            Vector3 localHeadPosition,
            ref Vector3 worldMoverPosition,
            float localMoverRadius,
            Vector3 localSpherePosition,
            ref Vector3 hitNormal
        )
        {
            var originToMover = new Vector2(localSpherePosition.x, localSpherePosition.z);
            var combinedRadius = radius + localMoverRadius;
            var collisionStatus = SpringBone.CollisionStatus.NoCollision;
            var collided = originToMover.sqrMagnitude <= combinedRadius * combinedRadius;
            if (collided)
            {
                var normal = originToMover.normalized;
                originToMover = combinedRadius * normal;
                var newLocalMoverPosition = new Vector3(originToMover.x, localSpherePosition.y, originToMover.y);
                worldMoverPosition = transform.TransformPoint(newLocalMoverPosition);
                hitNormal = transform.TransformDirection(new Vector3(normal.x, 0f, normal.y)).normalized;

                var originToHead = new Vector2(localHeadPosition.x, localHeadPosition.z);
                collisionStatus = (originToHead.sqrMagnitude <= radius * radius) ?
                    SpringBone.CollisionStatus.HeadIsEmbedded :
                    SpringBone.CollisionStatus.TailCollision;
#if UNITY_EDITOR
                RecordCylinderCollision(
                    localSpherePosition, 
                    new Vector3(normal.x, 0f, normal.y), 
                    localMoverRadius, 
                    collisionStatus);
#endif
            }
            return collisionStatus;
        }

#if UNITY_EDITOR
        public bool shouldDrawGizmosThisFrame;

        public void DrawGizmos(Color drawColor)
        {
            const int PointCount = 16;

            UnityEditor.Handles.color = drawColor;

            if (ringPoints == null || ringPoints.Length != PointCount)
            {
                ringPoints = new Vector3[PointCount];
                endRingPoints = new Vector3[PointCount];
            }

            var worldRadius = transform.TransformDirection(radius, 0f, 0f).magnitude;

            var startCapOrigin = transform.position;
            var endCapOrigin = transform.TransformPoint(0f, height, 0f);
            AngleLimits.DrawAngleLimit(startCapOrigin, transform.up, transform.forward, -180f, worldRadius);
            AngleLimits.DrawAngleLimit(startCapOrigin, transform.up, transform.right, -180f, worldRadius);
            AngleLimits.DrawAngleLimit(endCapOrigin, transform.up, transform.forward, 180f, worldRadius);
            AngleLimits.DrawAngleLimit(endCapOrigin, transform.up, transform.right, 180f, worldRadius);

            GetRingPoints(startCapOrigin, transform.right, transform.forward, worldRadius, ref ringPoints);
            var startToEnd = endCapOrigin - startCapOrigin;
            for (int pointIndex = 0; pointIndex < PointCount; pointIndex++)
            {
                endRingPoints[pointIndex] = ringPoints[pointIndex] + startToEnd;
            }

            for (int pointIndex = 1; pointIndex < PointCount; pointIndex++)
            {
                UnityEditor.Handles.DrawLine(ringPoints[pointIndex - 1], ringPoints[pointIndex]);
                UnityEditor.Handles.DrawLine(endRingPoints[pointIndex - 1], endRingPoints[pointIndex]);
            }

            for (int pointIndex = 0; pointIndex < PointCount; pointIndex++)
            {
                UnityEditor.Handles.DrawLine(ringPoints[pointIndex], endRingPoints[pointIndex]);
            }

            if (colliderDebug != null)
            {
                colliderDebug.DrawGizmosAndClear();
            }
        }

        private SpringManager manager;
        private Vector3[] ringPoints;
        private Vector3[] endRingPoints;
        private SpringColliderDebug colliderDebug;

        private static Vector3 GetAngleVector(Vector3 sideVector, Vector3 forwardVector, float radians)
        {
            return Mathf.Sin(radians) * sideVector + Mathf.Cos(radians) * forwardVector;
        }

        private static void GetRingPoints
        (
            Vector3 origin,
            Vector3 sideVector,
            Vector3 forwardVector,
            float scale,
            ref Vector3[] ringPoints
        )
        {
            var lastPoint = origin + scale * forwardVector;
            var pointCount = ringPoints.Length;
            var deltaAngle = 2f * Mathf.PI / (pointCount - 1);
            var angle = deltaAngle;
            ringPoints[0] = lastPoint;
            for (var iteration = 1; iteration < pointCount; ++iteration)
            {
                var newPoint = origin + scale * GetAngleVector(sideVector, forwardVector, angle);
                ringPoints[iteration] = newPoint;
                lastPoint = newPoint;
                angle += deltaAngle;
            }
        }


        // Box for picking in the editor
        private void DrawPickBox()
        {
            Gizmos.color = new Color(0f, 0f, 0f, 0f);
            var start = transform.position;
            var end = transform.TransformPoint(0f, height, 0f);
            var center = 0.5f * (start + end);
            var worldRadius = 1.5f * transform.TransformDirection(radius, 0f, 0f).magnitude;
            var size = new Vector3(
                Mathf.Abs(end.x - start.x) + worldRadius,
                Mathf.Abs(end.y - start.y) + worldRadius,
                Mathf.Abs(end.z - start.z) + worldRadius);
            Gizmos.DrawCube(center, size);
        }

        private void OnDrawGizmos()
        {
            if (shouldDrawGizmosThisFrame || !SpringManager.onlyShowSelectedColliders)
            {
                if (manager == null) { manager = GetComponentInParent<SpringManager>(); }
                DrawPickBox();
                DrawGizmos((enabled && manager != null) ? manager.colliderColor : Color.gray);
                shouldDrawGizmosThisFrame = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos(enabled ? Color.white : Color.gray);
        }

        private void RecordCylinderCollision
        (
            Vector3 localMoverPosition,
            Vector3 localNormal,
            float localMoverRadius,
            SpringBone.CollisionStatus collisionStatus
        )
        {
            if (!enabled) { return; }
            if (colliderDebug == null) { colliderDebug = new SpringColliderDebug(); }
            var originToContactPoint = radius * localNormal;
            var localContactPoint = new Vector3(originToContactPoint.x, localMoverPosition.y, originToContactPoint.z);
            var worldContactPoint = transform.TransformPoint(localContactPoint);
            var worldNormal = transform.TransformDirection(localNormal).normalized;
            var worldRadius = transform.TransformDirection(localMoverRadius, 0f, 0f).magnitude;
            colliderDebug.RecordCollision(
                worldContactPoint,
                worldNormal,
                worldRadius,
                collisionStatus);
        }

        private void RecordSphereCollision
        (
            Vector3 localSphereOrigin,
            Vector3 localMoverPosition,
            float worldMoverRadius,
            SpringBone.CollisionStatus collisionStatus
        )
        {
            if (!enabled) { return; }
            if (colliderDebug == null) { colliderDebug = new SpringColliderDebug(); }
            var localNormal = (localMoverPosition - localSphereOrigin).normalized;
            var localContactPoint = localSphereOrigin + localNormal * radius;
            var worldNormal = transform.TransformDirection(localNormal).normalized;
            var worldContactPoint = transform.TransformPoint(localContactPoint);
            colliderDebug.RecordCollision(worldContactPoint, worldNormal, worldMoverRadius, collisionStatus);
        }
#endif
    }
}