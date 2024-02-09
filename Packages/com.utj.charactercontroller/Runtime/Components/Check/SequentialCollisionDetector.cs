using System;
using Unity.Burst;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Components.Utility;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Subsystem;
using UnityEngine;

namespace Unity.TinyCharacterController.Utility
{
    /// <summary>
    ///     This class is a custom component designed for use in Unity. Its primary role is to detect collisions between
    ///     objects.
    ///     This component can utilize multiple detection points and perform collision detection in a step-by-step manner.
    ///     This is particularly useful, for example, when characters or objects in a game need to detect collisions with other
    ///     objects using multiple partial detection areas.
    /// </summary>
    [BurstCompile]
    [DefaultExecutionOrder(Order.Check)]
    [AddComponentMenu(MenuList.MenuCheck + nameof(SequentialCollisionDetector))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Utility.MultiStepCollisionDetector")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Utility.SequentialCollisionDetector")]    
    public class SequentialCollisionDetector : CollisionDetectorBase
    {
        /// <summary>
        ///     Detection frame range.
        /// </summary>
        [Header("Positions and frame")] [Range(0, 1)] [ReadOnlyOnRuntime] [SerializeField]
        private float _frame;

        /// <summary>
        ///     Configuration of detection positions and effective ranges.
        /// </summary>
        [SerializeField] private Data[] _hitPositions =
        {
            // To handle cases where initialization may not occur correctly, initial values should be set using this configuration.
            // Range 0 - 1
            // Initial settings
            // - Offset (0,0,0)
            // - Rotation (0,0,0)
            // - Scale (1,1,1)
            new()
            {
                Range = new Vector2(0, 1),
                Collisions = new[]
                {
                    new CollisionData
                    {
                        Offset = Vector3.zero,
                        Rotation = Vector3.zero,
                        Scale = Vector3.one
                    }
                }
            }
        };


        private Transform _transform;

        /// <summary>
        ///     Update the frame range for detection.
        /// </summary>
        public float Frame
        {
            get => _frame;
            set => _frame = value;
        }

        private void Awake()
        {
            TryGetComponent(out _transform);
        }

        private void OnEnable()
        {
            MultiStepCollisionDetectorSystem.Register(this, Timing);
            InitializeBufferOfCollidedCollision();
        }

        private void OnDisable()
        {
            MultiStepCollisionDetectorSystem.Unregister(this, Timing);
        }

        private void OnDrawGizmosSelected()
        {
            var trs = transform;
            var pos = trs.position;
            var rot = trs.rotation;

            foreach (var data in _hitPositions)
            {
                var isInRange = data.IsInRange(_frame);

                // Display detection range graphically.
                foreach (var col in data.Collisions)
                {
                    var worldPosition = pos + transform.TransformVector(col.Offset);
                    var rotation = rot * Quaternion.Euler(col.Rotation);

                    // Set Gizmos color to white if the component is enabled, or translucent white if disabled.
                    Gizmos.color = enabled ? Color.white : new Color(Color.white.r, Color.white.g, Color.white.b, 0.4f);

                    // Display range as a wireframe.
                    Gizmos.matrix = Matrix4x4.TRS(worldPosition, rotation, Vector3.one);
                    Gizmos.DrawWireCube(Vector3.zero, col.Scale);

                    // Display translucent yellow if in range and enabled.
                    if (isInRange && enabled)
                    {
                        Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.2f);
                        Gizmos.DrawCube(Vector3.zero, col.Scale);
                    }
                }
            }
        }


        private void OnUpdate(in Collider[] hitColliders)
        {
            // Cache transform position and rotation.
            var position = _transform.position;
            var rotation = _transform.rotation;

            foreach (var data in _hitPositions)
            {
                // Skip processing if out of range.
                if (data.IsInRange(_frame) == false)
                    continue;

                // Perform collision detection within the range.
                for (var index = 0; index < data.Collisions.Length; index++)
                {
                    // Perform collision detection.
                    var count = CalculateSphereCast(hitColliders, data, index, position, rotation);

                    // Register objects that are not in contact from the list obtained with OverlapBoxNonAlloc(...).
                    for (var hitIndex = 0; hitIndex < count; hitIndex++)
                    {
                        var hit = hitColliders[hitIndex];

                        // Exclude own Collider from the collision targets.
                        if (Owner != null && Owner.IsOwnCollider(hit))
                            continue;
        
                        // Get the object judged to have collided.
                        var hitObject = GetHitObject(hit);

                        // Skip if the GameObject was previously hit or if its tag is not in the _hitTags array.
                        // However, if nothing is set in _hitTags, it won't be skipped.
                        if (HitObjects.Contains(hitObject) ||
                            GameObjectUtility.ContainTag(hitObject, HitTags) == false)
                            continue;

                        // Register the collider.
                        HitObjects.Add(hitObject);
                        HitCollidersInThisFrame.Add(hit);
                        HitObjectsInThisFrame.Add(hitObject);
                    }
                }
            }
        }

        private int CalculateSphereCast(Collider[] hitColliders, Data data, int index, Vector3 position, Quaternion rotation)
        {
            var col = data.Collisions[index];

            var worldPosition = position + _transform.TransformVector(data.Collisions[index].Offset);
            var colRotation = rotation * Quaternion.Euler(col.Rotation);
            var count = Physics.OverlapBoxNonAlloc(worldPosition, col.Scale * 0.5f,
                hitColliders, colRotation, HitLayer, QueryTriggerInteraction.Ignore);
            return count;
        }


        /// <summary>
        ///     Initialize data at the start of processing.
        /// </summary>
        private void PrepareFrame()
        {
            HitCollidersInThisFrame.Clear();
            HitObjectsInThisFrame.Clear();
        }
        
        /// <summary>
        ///     Timing and range of detection.
        /// </summary>
        [Serializable]
        private struct Data
        {
            /// <summary>
            ///     Range of detection.
            /// </summary>
            [MinMax(0, 1)] public Vector2 Range;

            /// <summary>
            ///     Collision data for collision detection.
            /// </summary>
            [NonReorderable] public CollisionData[] Collisions;

            /// <summary>
            ///     Check if within the detection range.
            /// </summary>
            /// <param name="frame">Current frame.</param>
            /// <returns>True if within the detection range.</returns>
            public bool IsInRange(float frame)
            {
                return frame >= Range.x && frame <= Range.y;
            }
        }

        /// <summary>
        ///     Collision detection range.
        /// </summary>
        [Serializable]
        private struct CollisionData
        {
            /// <summary>
            ///     Relative coordinates.
            /// </summary>
            public Vector3 Offset;

            /// <summary>
            ///     Angle of the detection box. Euler angles.
            /// </summary>
            public Vector3 Rotation;

            /// <summary>
            ///     Collider size. Default is (1m, 1m, 1m).
            /// </summary>
            public Vector3 Scale;
        }

        /// <summary>
        ///     Class to control the <see cref="SequentialCollisionDetector" />.
        /// </summary>
        private class MultiStepCollisionDetectorSystem :
            SystemBase<SequentialCollisionDetector, MultiStepCollisionDetectorSystem>,
            IEarlyUpdate
        {
            /// <summary>
            ///     Maximum number of collisions that can be detected at once.
            /// </summary>
            private const int Capacity = 50;

            // Array to store detection results
            private readonly Collider[] _results = new Collider[Capacity];

            private void OnDestroy()
            {
                UnregisterAllComponents();
            }

            void IEarlyUpdate.OnUpdate()
            {
                // Initialize components
                foreach (var component in Components)
                    component.PrepareFrame();

                // Update collision detection using Physics
                foreach (var component in Components)
                    component.OnUpdate(_results);

                // Raise events related to collided colliders
                foreach (var component in Components)
                    if (component.IsHitInThisFrame)
                        component.OnHitObjects.Invoke(component.HitObjectsInThisFrame);
            }

            // Specify the execution order of the system
            int ISystemBase.Order => 0;
        }
    }
}