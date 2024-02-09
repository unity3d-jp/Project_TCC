using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    public class SpringBone : MonoBehaviour
    {
        public enum CollisionStatus
        {
            NoCollision,
            HeadIsEmbedded,
            TailCollision
        }

        // Forces
        [Range(0f, 5000f)]
        public float stiffnessForce = 0.01f;
        [Range(0f, 1f)]
        public float dragForce = 0.4f;
        public Vector3 springForce = new Vector3(0.0f, -0.0001f, 0.0f);
        [Range(0f, 1f)]
        public float windInfluence = 1f;

        // Angle limits
        public Transform pivotNode;
        public float angularStiffness = 100f;
        public AngleLimits yAngleLimits = new AngleLimits();
        public AngleLimits zAngleLimits = new AngleLimits();

        // Length limits
        public Transform[] lengthLimitTargets;

        // Collision
        [Range(0f, 0.5f)]
        public float radius = 0.05f;
        public SpringSphereCollider[] sphereColliders;
        public SpringCapsuleCollider[] capsuleColliders;
        public SpringPanelCollider[] panelColliders;

        public Vector3 CurrentTipPosition { get { return currTipPos; } }

        // This should be called by the SpringManager in its Awake function before any updates
        public void Initialize(SpringManager owner)
        {
            manager = owner;

            var childPosition = ComputeChildPosition();
            var localChildPosition = transform.InverseTransformPoint(childPosition);
            boneAxis = localChildPosition.normalized;

            initialLocalRotation = transform.localRotation;
            actualLocalRotation = initialLocalRotation;

            sphereColliders = sphereColliders.Where(item => item != null).ToArray();
            capsuleColliders = capsuleColliders.Where(item => item != null).ToArray();
            panelColliders = panelColliders.Where(item => item != null).ToArray();

            lengthLimitTargets = (lengthLimitTargets != null)
                ? lengthLimitTargets.Where(target => target != null).ToArray()
                : new Transform[0];

            InitializeSpringLengthAndTipPosition();
        }
        
        public Vector3 ComputeChildPosition()
        {
            var children = GetValidChildren(transform);
            var childCount = children.Count;

            if (childCount == 0)
            {
                // This should never happen
                Debug.LogWarning("SpringBone「" + name + "」に有効な子供がありません");
                return transform.position + transform.right * -0.1f;
            }

            if (childCount == 1)
            {
                return children[0].position;
            }

            var initialTailPosition = new Vector3(0f, 0f, 0f);
            var averageDistance = 0f;
            var selfPosition = transform.position;
            for (int childIndex = 0; childIndex < childCount; childIndex++)
            {
                var childPosition = children[childIndex].position;
                initialTailPosition += childPosition;
                averageDistance += (childPosition - selfPosition).magnitude;
            }

            averageDistance /= childCount;
            initialTailPosition /= childCount;
            var selfToInitial = initialTailPosition - selfPosition;
            selfToInitial.Normalize();
            initialTailPosition = selfPosition + averageDistance * selfToInitial;
            return initialTailPosition;
        }

        public void RemoveAllColliders()
        {
            sphereColliders = new SpringSphereCollider[0];
            capsuleColliders = new SpringCapsuleCollider[0];
            panelColliders = new SpringPanelCollider[0];
        }

        public void UpdateSpring(float deltaTime, Vector3 externalForce)
        {
            skinAnimationLocalRotation = transform.localRotation;
    
            var baseWorldRotation = transform.parent.rotation * initialLocalRotation;
            var orientedInitialPosition = transform.position + baseWorldRotation * boneAxis * springLength;
            
            // Hooke's law: force to push us to equilibrium
            var force = stiffnessForce * (orientedInitialPosition - currTipPos);
            force += springForce + externalForce;
            var sqrDt = deltaTime * deltaTime;
            force *= 0.5f * sqrDt;

            // Verlet
            var temp = currTipPos;
            force += (1f - dragForce) * (currTipPos - prevTipPos);
            currTipPos += force;
            prevTipPos = temp;

            // Inlined because FixBoneLength is slow
            var headPosition = transform.position;
            var headToTail = currTipPos - headPosition;
            var magnitude = headToTail.magnitude;
            const float MagnitudeThreshold = 0.001f;
            headToTail = (magnitude <= MagnitudeThreshold)
                ? transform.TransformDirection(boneAxis)
                : headToTail / magnitude;
            currTipPos = headPosition + springLength * headToTail;
        }

        public void SatisfyConstraintsAndComputeRotation(float deltaTime, float dynamicRatio)
        {
            if (manager.enableLengthLimits)
            {
                currTipPos = ApplyLengthLimits(deltaTime);
            }

            var hadCollision = false;

            if (manager.collideWithGround)
            {
                hadCollision = CheckForGroundCollision();
            }

            if (manager.enableCollision & !hadCollision)
            {
                hadCollision = CheckForCollision();
            }

            if (manager.enableAngleLimits)
            {
                ApplyAngleLimits(deltaTime);
            }

            // ComputeRotation
            // Inlined for performance

            if (float.IsNaN(currTipPos.x)
                | float.IsNaN(currTipPos.y)
                | float.IsNaN(currTipPos.z))
            {
#if UNITY_EDITOR
                Debug.DebugBreak();
#endif
                var baseWorldRotation = transform.parent.rotation * initialLocalRotation;
                currTipPos = transform.position + baseWorldRotation * boneAxis * springLength;
                prevTipPos = currTipPos;
            }

            actualLocalRotation = ComputeRotation(currTipPos);
            transform.localRotation = Quaternion.Lerp(skinAnimationLocalRotation, actualLocalRotation, dynamicRatio);
        }

        public void ComputeRotation(float dynamicRatio)
        {
            if (float.IsNaN(currTipPos.x)
               | float.IsNaN(currTipPos.y)
               | float.IsNaN(currTipPos.z))
            {
#if UNITY_EDITOR
                Debug.DebugBreak();
#endif
                var baseWorldRotation = transform.parent.rotation * initialLocalRotation;
                currTipPos = transform.position + baseWorldRotation * boneAxis * springLength;
                prevTipPos = currTipPos;
            }

            actualLocalRotation = ComputeRotation(currTipPos);
            transform.localRotation = Quaternion.Lerp(skinAnimationLocalRotation, actualLocalRotation, dynamicRatio);
        }

        public Transform GetPivotTransform()
        {
            if (pivotNode == null)
            {
                pivotNode = transform.parent ?? transform;
            }
            return pivotNode;
        }

        // private

        private SpringManager manager;
        private Vector3 boneAxis = new Vector3(-1.0f, 0.0f, 0.0f);
        private float springLength;
        private Quaternion skinAnimationLocalRotation;
        private Quaternion initialLocalRotation;
        private Quaternion actualLocalRotation;
        private Vector3 currTipPos;
        private Vector3 prevTipPos;
        private float[] lengthsToLimitTargets;

        private static IList<Transform> GetValidChildren(Transform parent)
        {
            // Ignore SpringBonePivots
            var childCount = parent.childCount;
            var children = new List<Transform>(childCount);
            for (int childIndex = 0; childIndex < childCount; childIndex++)
            {
                var child = parent.GetChild(childIndex);
                if (child.GetComponent<SpringBonePivot>() == null)
                {
                    children.Add(child);
                }
            }
            return children;
        }

        private void ApplyAngleLimits(float deltaTime)
        {
            if ((!yAngleLimits.active && !zAngleLimits.active)
                || pivotNode == null)
            {
                return;
            }

            var origin = transform.position;
            var vector = currTipPos - origin;
            var pivot = GetPivotTransform();
            var forward = -pivot.right;

            if (yAngleLimits.active)
            {
                yAngleLimits.ConstrainVector(
                    -pivot.up, -pivot.forward, forward, angularStiffness, deltaTime, ref vector);
            }
            if (zAngleLimits.active)
            {
                zAngleLimits.ConstrainVector(
                    -pivot.forward, -pivot.up, forward, angularStiffness, deltaTime, ref vector);
            }

            currTipPos = origin + vector;
        }

        private bool CheckForCollision()
        {
            var desiredPosition = currTipPos;
            var headPosition = transform.position;
            var scaledRadius = transform.TransformDirection(radius, 0f, 0f).magnitude;
            var hitNormal = new Vector3(0f, 0f, 1f);

            var hadCollision = false;

            for (int i = 0; i < capsuleColliders.Length; ++i)
            {
                var collider = capsuleColliders[i];
                if (collider.enabled)
                {
                    var currentCollisionStatus = collider.CheckForCollisionAndReact(
                        headPosition, ref currTipPos, scaledRadius, ref hitNormal);
                    hadCollision |= currentCollisionStatus != CollisionStatus.NoCollision;
                }
            }

            for (int i = 0; i < sphereColliders.Length; ++i)
            {
                var collider = sphereColliders[i];
                if (collider.enabled)
                {
                    var currentCollisionStatus = collider.CheckForCollisionAndReact(
                        headPosition, ref currTipPos, scaledRadius, ref hitNormal);
                    hadCollision |= currentCollisionStatus != CollisionStatus.NoCollision;
                }
            }

            var colliderCount = panelColliders.Length;
            for (int colliderIndex = 0; colliderIndex < colliderCount; colliderIndex++)
            {
                var collider = panelColliders[colliderIndex];
                if (collider.enabled)
                {
                    var currentCollisionStatus = collider.CheckForCollisionAndReact(
                        headPosition, springLength, ref currTipPos, scaledRadius, ref hitNormal);
                    hadCollision |= currentCollisionStatus != CollisionStatus.NoCollision;
                }
            }

            if (hadCollision)
            {
                var incidentVector = desiredPosition - prevTipPos;
                var reflectedVector = Vector3.Reflect(incidentVector, hitNormal);

                // friction
                var upwardComponent = Vector3.Dot(reflectedVector, hitNormal) * hitNormal;
                var lateralComponent = reflectedVector - upwardComponent;

                var bounceVelocity = manager.bounce * upwardComponent + (1f - manager.friction) * lateralComponent;
                const float BounceThreshold = 0.0001f;
                if (bounceVelocity.sqrMagnitude > BounceThreshold)
                {
                    var distanceTraveled = (currTipPos - prevTipPos).magnitude;
                    prevTipPos = currTipPos - bounceVelocity;
                    currTipPos += Mathf.Max(0f, bounceVelocity.magnitude - distanceTraveled) * bounceVelocity.normalized;
                }
                else
                {
                    prevTipPos = currTipPos;
                }
            }
            return hadCollision;
        }

        private bool CheckForGroundCollision()
        {
            // Todo: this assumes a flat ground parallel to the xz plane
            var worldHeadPosition = transform.position;
            var worldTailPosition = currTipPos;
            var worldRadius = transform.TransformDirection(radius, 0f, 0f).magnitude;
            var worldLength = (currTipPos - worldHeadPosition).magnitude;
            var groundHeight = manager.groundHeight;
            worldHeadPosition.y -= groundHeight;
            worldTailPosition.y -= groundHeight;
            var collidingWithGround = SpringPanelCollider.CheckForCollisionWithAlignedPlaneAndReact(
                worldHeadPosition, worldLength, ref worldTailPosition, worldRadius, SpringPanelCollider.Axis.Y);
            if (collidingWithGround != CollisionStatus.NoCollision)
            {
                worldTailPosition.y += groundHeight;
                currTipPos = FixBoneLength(transform.position, worldTailPosition, 0.5f * springLength, springLength);
                // Todo: bounce, friction
                prevTipPos = currTipPos;
            }

            return collidingWithGround != CollisionStatus.NoCollision;
        }

        private Vector3 FixBoneLength
        (
            Vector3 headPosition,
            Vector3 tailPosition,
            float minLength,
            float maxLength
        )
        {
            var headToTail = tailPosition - headPosition;
            var magnitude = headToTail.magnitude;
            const float MagnitudeThreshold = 0.001f;
            if (magnitude <= MagnitudeThreshold)
            {
                return headPosition + transform.TransformDirection(boneAxis) * minLength;
            }
            var newMagnitude = (magnitude < minLength) ? minLength : magnitude;
            newMagnitude = (newMagnitude > maxLength) ? maxLength : newMagnitude;
            return headPosition + (newMagnitude / magnitude) * headToTail;
        }

        private void InitializeSpringLengthAndTipPosition()
        {
            var childPos = ComputeChildPosition();
            springLength = Vector3.Distance(transform.position, childPos);
            currTipPos = childPos;
            prevTipPos = childPos;

            var targetCount = lengthLimitTargets.Length;
            lengthsToLimitTargets = new float[targetCount];
            for (int targetIndex = 0; targetIndex < targetCount; targetIndex++)
            {
                lengthsToLimitTargets[targetIndex] =
                    (lengthLimitTargets[targetIndex].position - childPos).magnitude;
            }
        }

        private Quaternion ComputeRotation(Vector3 tipPosition)
        {
            var baseWorldRotation = transform.parent.rotation * initialLocalRotation;
            var worldBoneVector = tipPosition - transform.position;
            var localBoneVector = Quaternion.Inverse(baseWorldRotation) * worldBoneVector;
            localBoneVector.Normalize();

            var aimRotation = Quaternion.FromToRotation(boneAxis, localBoneVector);
            var outputRotation = initialLocalRotation * aimRotation;

            return outputRotation;
        }

        // Returns the new tip position
        private Vector3 ApplyLengthLimits(float deltaTime)
        {
            var targetCount = lengthLimitTargets.Length;
            if (targetCount == 0)
            {
                return currTipPos;
            }

            const float SpringConstant = 0.5f;
            var accelerationMultiplier = SpringConstant * deltaTime * deltaTime;
            var movement = new Vector3(0f, 0f, 0f);
            for (int targetIndex = 0; targetIndex < targetCount; targetIndex++)
            {
                var targetPosition = lengthLimitTargets[targetIndex].position;
                var lengthToLimitTarget = lengthsToLimitTargets[targetIndex];
                var currentToTarget = currTipPos - targetPosition;
                var currentDistanceSquared = currentToTarget.sqrMagnitude;

                // Hooke's Law
                var currentDistance = Mathf.Sqrt(currentDistanceSquared);
                var distanceFromEquilibrium = currentDistance - lengthToLimitTarget;
                movement -= accelerationMultiplier * distanceFromEquilibrium * currentToTarget.normalized;
            }

            return currTipPos + movement;
        }

#if UNITY_EDITOR
        public void DrawSpringBoneCollision()
        {
            var childPosition = ComputeChildPosition();
            var worldRadius = transform.TransformDirection(radius, 0f, 0f).magnitude;
            // For picking
            Gizmos.DrawSphere(childPosition, worldRadius);

            UnityEditor.Handles.DrawWireDisc(childPosition, Vector3.up, worldRadius);
            UnityEditor.Handles.DrawWireDisc(childPosition, Vector3.right, worldRadius);
            UnityEditor.Handles.DrawWireDisc(childPosition, Vector3.forward, worldRadius);
            //UnityEditor.Handles.RadiusHandle(Quaternion.identity, childPosition, worldRadius);
        }

        public void MarkCollidersForDrawing()
        {
            if (sphereColliders != null)
            {
                for (int colliderIndex = 0; colliderIndex < sphereColliders.Length; colliderIndex++)
                {
                    if (sphereColliders[colliderIndex] != null) { sphereColliders[colliderIndex].shouldDrawGizmosThisFrame = true; }
                }
            }
            if (capsuleColliders != null)
            {
                for (int colliderIndex = 0; colliderIndex < capsuleColliders.Length; colliderIndex++)
                {
                    if (capsuleColliders[colliderIndex] != null) { capsuleColliders[colliderIndex].shouldDrawGizmosThisFrame = true; }
                }
            }
            if (panelColliders != null)
            {
                for (int colliderIndex = 0; colliderIndex < panelColliders.Length; colliderIndex++)
                {
                    if (panelColliders[colliderIndex] != null) { panelColliders[colliderIndex].shouldDrawGizmosThisFrame = true; }
                }
            }
        }

        public void DrawAngleLimits(AngleLimits angleLimits, float drawScale)
        {
            if (angleLimits.active)
            {
                var pivot = GetPivotTransform();
                var forward = -pivot.right;
                var side = (angleLimits == yAngleLimits) ? -pivot.up : -pivot.forward;
                angleLimits.DrawLimits(transform.position, side, forward, drawScale);
            }
        }

        private void DrawLinesToLimitTargets()
        {
            if (lengthLimitTargets == null
                || lengthsToLimitTargets == null
                || lengthLimitTargets.Length != lengthsToLimitTargets.Length)
            {
                return;
            }

            var SelfToLimitColor = new Color(1f, 1f, 1f);
            var SelfToTargetColor = new Color(0.5f, 0.6f, 0.5f);
            var ExceededLimitColor = new Color(1f, 0.5f, 0.5f);

            var targetCount = lengthLimitTargets.Length;
            var selfPosition = ComputeChildPosition();
            for (int targetIndex = 0; targetIndex < targetCount; targetIndex++)
            {
                var target = lengthLimitTargets[targetIndex];
                var distance = lengthsToLimitTargets[targetIndex];
                if (target != null)
                {
                    var targetPosition = target.position;
                    var selfToTarget = targetPosition - selfPosition;
                    var limitPosition = selfPosition + distance * selfToTarget.normalized;

                    Gizmos.color = SelfToLimitColor;
                    Gizmos.DrawLine(limitPosition, selfPosition);
                    Gizmos.color = (selfToTarget.sqrMagnitude > distance * distance) ?
                        ExceededLimitColor : SelfToTargetColor;
                    Gizmos.DrawLine(targetPosition, limitPosition);
                }
            }
        }
#endif
    }
}
