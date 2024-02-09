using UnityEngine;

namespace UTJ
{
    public class SpringPanelCollider : MonoBehaviour
    {
        public float width = 0.25f;
        public float height = 0.25f;

        // If linkedRenderer is not null, the collider will be enabled 
        // based on whether the renderer is
        public Renderer linkedRenderer;

        public Vector3 GetPlaneNormal()
        {
            return transform.forward;
        }

        public SpringBone.CollisionStatus CheckForCollisionAndReact
        (
            Vector3 headPosition, 
            float length, 
            ref Vector3 tailPosition, 
            float tailRadius,
            ref Vector3 hitNormal
        )
        {
            if (linkedRenderer != null
                && !linkedRenderer.enabled)
            {
                return SpringBone.CollisionStatus.NoCollision;
            }

            var localTailPosition = transform.InverseTransformPoint(tailPosition);
            var localTailRadius = transform.InverseTransformDirection(tailRadius, 0f, 0f).magnitude;
            if (localTailPosition.z >= localTailRadius)
            {
                return SpringBone.CollisionStatus.NoCollision;
            }

            var localHeadPosition = transform.InverseTransformPoint(headPosition);
            var localLength = transform.InverseTransformDirection(length, 0f, 0f).magnitude;

            var halfWidth = 0.5f * width;
            var halfHeight = 0.5f * height;
            var adjustedWidth = halfWidth + localTailRadius;
            var adjustedHeight = halfHeight + localTailRadius;

            var tailOutOfBounds = Mathf.Abs(localTailPosition.y) >= adjustedHeight
                || Mathf.Abs(localTailPosition.x) >= adjustedWidth;

            if (tailOutOfBounds)
            {
                return SpringBone.CollisionStatus.NoCollision;
            }

            // Check edges
            var collisionStatus = SpringBone.CollisionStatus.TailCollision;
            if (localHeadPosition.z <= 0f && localTailPosition.z <= 0f)
            {
                if (Mathf.Abs(localHeadPosition.y) > halfHeight)
                {
                    halfHeight = (localTailPosition.y < 0f) ? -halfHeight : halfHeight;
                    localTailPosition = new Vector3(localTailPosition.x, halfHeight, localTailPosition.z);
                }
                else if (Mathf.Abs(localHeadPosition.x) > halfWidth)
                {
                    halfWidth = (localTailPosition.x < 0f) ? -halfWidth : halfWidth;
                    localTailPosition = new Vector3(halfWidth, localTailPosition.y, localTailPosition.z);
                }
                else
                {
                    collisionStatus = SpringBone.CollisionStatus.HeadIsEmbedded;
                    localTailPosition = localHeadPosition;
                    localTailPosition.z = localTailRadius;
                }
            }
            else
            {
                if (Mathf.Abs(localTailPosition.y) > halfHeight)
                {
                    halfHeight = (localTailPosition.y < 0f) ? -halfHeight : halfHeight;
                    var localNormal = new Vector3(0f, localTailPosition.y - halfHeight, localTailPosition.z).normalized;
                    localTailPosition = new Vector3(localTailPosition.x, halfHeight, 0f) + localTailRadius * localNormal;
                }
                else if (Mathf.Abs(localTailPosition.x) > halfWidth)
                {
                    halfWidth = (localTailPosition.x < 0f) ? -halfWidth : halfWidth;
                    var localNormal = new Vector3(localTailPosition.x - halfWidth, 0f, localTailPosition.z).normalized;
                    localTailPosition = new Vector3(halfWidth, localTailPosition.y, 0f) + localTailRadius * localNormal;
                }
                else
                {
                    collisionStatus = CheckForCollisionWithAlignedPlaneAndReact(
                        localHeadPosition, localLength, ref localTailPosition, localTailRadius, Axis.Z);
                }
            }

            if (collisionStatus != SpringBone.CollisionStatus.NoCollision)
            {
#if UNITY_EDITOR
                RecordCollision(tailPosition, tailRadius, collisionStatus);
#endif
                tailPosition = transform.TransformPoint(localTailPosition);
                hitNormal = transform.forward.normalized;
            }
            return collisionStatus;
        }

        public enum Axis
        {
            X = 0,
            Y,
            Z, 
            AxisCount
        }

        public static SpringBone.CollisionStatus CheckForCollisionWithAlignedPlaneAndReact
        (
            Vector3 localHeadPosition,
            float localLength,
            ref Vector3 localTailPosition,
            float localTailRadius,
            Axis upAxis
        )
        {
            var zIndex = (int)upAxis;
            if (localTailPosition[zIndex] >= localTailRadius)
            {
                return SpringBone.CollisionStatus.NoCollision;
            }

            var collisionStatus = SpringBone.CollisionStatus.TailCollision;
            var newLocalTailPosition = localHeadPosition;
            if (localHeadPosition[zIndex] + localLength <= localTailRadius)
            {
                // Bone is completely embedded
                newLocalTailPosition[zIndex] += localLength;
                collisionStatus = SpringBone.CollisionStatus.HeadIsEmbedded;
            }
            else
            {
                var xIndex = (zIndex + 1) % (int)Axis.AxisCount;
                var yIndex = (zIndex + 2) % (int)Axis.AxisCount;

                var heightAboveRadius = localHeadPosition[zIndex] - localTailRadius;
                var projectionLength = Mathf.Sqrt(localLength * localLength - heightAboveRadius * heightAboveRadius);
                var localBoneVector = localTailPosition - localHeadPosition;
                var projectionVector = new Vector2(localBoneVector[xIndex], localBoneVector[yIndex]);
                var projectionVectorLength = projectionVector.magnitude;
                if (projectionVectorLength > 0.001f)
                {
                    var projection = (projectionLength / projectionVectorLength) * projectionVector;
                    newLocalTailPosition[xIndex] += projection.x;
                    newLocalTailPosition[yIndex] += projection.y;
                    newLocalTailPosition[zIndex] = localTailRadius;
                }
            }
            localTailPosition = newLocalTailPosition;
            return collisionStatus;
        }

#if UNITY_EDITOR
        public bool shouldDrawGizmosThisFrame;

        public void DrawGizmos(Color color)
        {
            const int PointCount = 4;

            if (localGizmoPoints == null
                || worldGizmoPoints == null
                || localGizmoPoints.Length < PointCount
                || worldGizmoPoints.Length < PointCount)
            {
                localGizmoPoints = new Vector3[PointCount];
                worldGizmoPoints = new Vector3[PointCount];
            }

            var halfWidth = 0.5f * width;
            var halfHeight = 0.5f * height;
            localGizmoPoints[0] = new Vector3(-halfWidth, -halfHeight, 0f);
            localGizmoPoints[1] = new Vector3( halfWidth, -halfHeight, 0f);
            localGizmoPoints[2] = new Vector3( halfWidth,  halfHeight, 0f);
            localGizmoPoints[3] = new Vector3(-halfWidth,  halfHeight, 0f);

            for (int pointIndex = 0; pointIndex < PointCount; pointIndex++)
            {
                worldGizmoPoints[pointIndex] = transform.TransformPoint(localGizmoPoints[pointIndex]);
            }

            UnityEditor.Handles.color = color;
            for (int pointIndex = 0; pointIndex < PointCount; pointIndex++)
            {
                var endPointIndex = (pointIndex + 1) % PointCount;
                UnityEditor.Handles.DrawLine(worldGizmoPoints[pointIndex], worldGizmoPoints[endPointIndex]);
                UnityEditor.Handles.DrawLine(worldGizmoPoints[pointIndex], worldGizmoPoints[pointIndex] - 0.15f * transform.forward);
            }

            HandlesUtil.DrawArrow(transform.position, transform.position + transform.forward * 0.1f, color, 0.1f);

            if (colliderDebug != null)
            {
                colliderDebug.DrawGizmosAndClear();
            }
        }

        // private

        private SpringManager manager;
        private Vector3[] localGizmoPoints;
        private Vector3[] worldGizmoPoints;
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
            Vector3 tailPosition, 
            float tailRadius, 
            SpringBone.CollisionStatus collisionStatus
        )
        {
            if (!enabled) { return; }
            if (colliderDebug == null) { colliderDebug = new SpringColliderDebug(); }
            var planeNormal = GetPlaneNormal();
            var planeOrigin = transform.position;
            var planeToCollision = tailPosition - planeOrigin;
            var normalProjection = Vector3.Dot(planeToCollision, planeNormal) * planeNormal;
            var projectedCollisionPoint = tailPosition - normalProjection;
            colliderDebug.RecordCollision(
                projectedCollisionPoint, planeNormal, tailRadius, collisionStatus);
        }
#endif
    }
}