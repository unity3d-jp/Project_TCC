using System;
using System.Buffers;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.TinyCharacterController.Components.Utility;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Subsystem;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Utility
{
    /// <summary>
    ///     This component is designed to detect and notify collision events between actions such as swings, attacks, and
    ///     movement,
    ///     as well as collisions with other colliders. It performs collision detection based on a defined range and the
    ///     movement path
    ///     of the contact point. It is primarily intended for use in scenarios such as weapon swing attacks and collision
    ///     detection
    ///     with traps.
    /// </summary>
    [BurstCompile]
    [DefaultExecutionOrder(Order.Check)]
    [AddComponentMenu(MenuList.MenuCheck + nameof(SwingHitDetector))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Utility.SwingHitDetector")]   
    public class SwingHitDetector : CollisionDetectorBase
    {
        /// <summary>
        ///     Number of colliders that can be hit in a single collision check.
        /// </summary>
        private const int HitCapacity = 15;

        /// <summary>
        ///     Positions and sizes for collision detection.
        /// </summary>
        [FormerlySerializedAs("_hitPositions")] [Header("Positions")] [SerializeField]
        private LineHitData[] _detectorPositions;

        private readonly List<Vector3> _hitPositions = new();

        private NativeArray<Vector3> _currentPositions;
        private NativeArray<Vector3> _offsets;
        private NativeArray<Vector3> _previousPositions;

        private Transform _transform;

        private void Awake()
        {
            InitializeBuffers();
            TryGetComponent(out _transform);
        }

        private void OnEnable()
        {
            // Register with the subsystem.
            LineHitSystem.Register(this, Timing);

            // Update PreparePositions for the current frame.
            ManualCalculatePreparePositions();

            // Initialize the list for multiple-hit avoidance.
            InitializeBufferOfCollidedCollision();
        }

        private void OnDisable()
        {
            // Unregister from the subsystem.
            LineHitSystem.Unregister(this, Timing);
        }

        private void OnDestroy()
        {
            _offsets.Dispose();
            _currentPositions.Dispose();
            _previousPositions.Dispose();
        }

        protected override void Reset()
        {
            base.Reset();

            _detectorPositions = new LineHitData[1];
            _detectorPositions[0] = new LineHitData
            {
                Radius = 1,
            };
        }


        private void OnDrawGizmosSelected()
        {
            if (_detectorPositions == null || _detectorPositions.Length == 0)
                return;

            using var positions = GetWorldPosition(transform.position);

            // Draw a wire sphere at _hitPositions.
            // If the component is enabled, draw a filled sphere.
            DrawWireSphere(positions);
            if (enabled)
                DrawFilledSphere(positions);

            // If the application is not playing or the component is disabled, exit the function.
            if (Application.isPlaying == false || enabled == false)
                return;

            for (var i = 0; i < _detectorPositions.Length; i++)
            {
                // Draw a red line between the current position and the previous position.
                var position = _currentPositions[i];
                var previousPosition = _previousPositions[i];
                Debug.DrawLine(position, previousPosition, Color.red, 1);
            }

            return;


            NativeArray<Vector3> GetWorldPosition(Vector3 position)
            {
                var nativeArray = new NativeArray<Vector3>(_detectorPositions.Length, Allocator.Temp);
                for (var i = 0; i < _detectorPositions.Length; i++)
                    nativeArray[i] = position + transform.TransformVector(_detectorPositions[i].Offset);
                return nativeArray;
            }

            void DrawWireSphere(in NativeArray<Vector3> spherePositions)
            {
                Gizmos.color = Color.white;
                for (var i = 0; i < _detectorPositions.Length; i++)
                    Gizmos.DrawWireSphere(spherePositions[i], _detectorPositions[i].Radius);
            }

            void DrawFilledSphere(in NativeArray<Vector3> spherePositions)
            {
                var color = HitCollidersInThisFrame.Count == 0 ? Color.yellow : Color.red;
                color.a = 0.4f;
                Gizmos.color = color;
                for (var i = 0; i < _detectorPositions.Length; i++)
                    Gizmos.DrawSphere(spherePositions[i], _detectorPositions[i].Radius);
            }
        }

        /// <summary>
        ///     Get the position where the object made contact in the past.
        /// </summary>
        /// <param name="obj">The object to check for contact.</param>
        /// <returns>The position of contact. Returns <see cref="Vector3.zero" /> if there was no previous contact.</returns>
        public Vector3 GetContactPosition(GameObject obj)
        {
            var index = HitObjects.IndexOf(obj);
            if (index != -1) return _hitPositions[index];

            Debug.LogError($"{obj} not found!");
            return Vector3.zero;
        }

        /// <summary>
        ///     Get the position where the object made contact in the past.
        /// </summary>
        /// <param name="col">The collider to check for contact.</param>
        /// <returns>The position of contact. Returns <see cref="Vector3.zero" /> if there was no previous contact.</returns>
        public Vector3 GetContactPosition(Collider col)
        {
            var index = HitColliders.IndexOf(col);
            if (index != -1) return _hitPositions[index];
            Debug.LogError($"{col} not found!");
            return Vector3.zero;
        }


        /// <summary>
        ///     Initializes the buffers.
        /// </summary>
        private void InitializeBuffers()
        {
            var count = _detectorPositions.Length;
            _offsets = new NativeArray<Vector3>(count, Allocator.Persistent);
            for (var i = 0; i < _detectorPositions.Length; i++)
                _offsets[i] = _detectorPositions[i].Offset;

            _currentPositions = new NativeArray<Vector3>(count, Allocator.Persistent);
            _previousPositions = new NativeArray<Vector3>(count, Allocator.Persistent);
        }

        /// <summary>
        ///     Manually calculates the prepare positions.
        /// </summary>
        private void ManualCalculatePreparePositions()
        {
            // Allocate a temporary NativeArray<Vector3> of the same length as _hitPositions.
            using var worldPositions = new NativeArray<Vector3>(_detectorPositions.Length, Allocator.Temp);

            // Transform local coordinates in _offsets to world coordinates and store them in worldPositions.
            _transform.TransformVectors(_offsets.AsReadOnlySpan(), worldPositions.AsSpan());

            // Get the current position of the object.
            var position = _transform.position;

            // Calculate new positions and update _previousPositions.
            CalculateNewPositions(ref _previousPositions, in worldPositions, in position);

            // Copy _previousPositions to _currentPositions.
            _previousPositions.CopyTo(_currentPositions);
        }


        /// <summary>
        ///     Updates HitObjectsInThisFrame and HitObjects, and also updates <see cref="_hitPositions" />.
        /// </summary>
        /// <param name="col">The collider that was hit.</param>
        /// <param name="position">The contact position.</param>
        private void RegisterHitColliders(in Collider col, in Vector3 position)
        {
            // Skip registration if the contacted object is owned by the collider's owner.
            if (Owner.IsOwnCollider(col))
                return;

            // Get the cached object from the collided collider.
            var obj = GetHitObject(col);

            // Skip processing if the collider is already registered or if its tag is not in the _hitTags array.
            if (HitObjects.Contains(obj) || GameObjectUtility.ContainTag(obj, HitTags) == false)
                return;

            // Cache the collider until the component is disabled.
            HitColliders.Add(col);
            HitObjects.Add(obj);
            _hitPositions.Add(position);

            // Cache only for the current frame.
            HitObjectsInThisFrame.Add(obj);
            HitCollidersInThisFrame.Add(col);
        }

        /// <summary>
        ///     Performs sphere-based collision detection within the specified range.
        /// </summary>
        private void CalculateSphereHit()
        {
            var result = ArrayPool<Collider>.Shared.Rent(HitCapacity);

            for (var i = 0; i < _detectorPositions.Length; i++)
            {
                // Detect colliders within the range.
                var hitCount = Physics.OverlapSphereNonAlloc(_currentPositions[i], _detectorPositions[i].Radius,
                    result, HitLayer, QueryTriggerInteraction.Ignore);

                // Register hit colliders.
                for (var hitIndex = 0; hitIndex < hitCount; hitIndex++)
                    RegisterHitColliders(result[hitIndex], _currentPositions[i]);
            }

            ArrayPool<Collider>.Shared.Return(result);
        }

        /// <summary>
        ///     Line-based collision detection between the previous frame and the current frame.
        ///     Only active when the detection coordinates have moved.
        /// </summary>
        private void CalculateLineHit()
        {
            var lineData = new NativeArray<(Vector3, float)>(_detectorPositions.Length, Allocator.Temp);
            var hits = ArrayPool<RaycastHit>.Shared.Rent(HitCapacity);

            try
            {
                // Calculate world positions for the previous and current frames.
                CalculateLineData(ref lineData, in _currentPositions, in _previousPositions);

                for (var sphereIndex = 0; sphereIndex < _detectorPositions.Length; sphereIndex++)
                {
                    var distance = lineData[sphereIndex].Item2;
                    var direction = lineData[sphereIndex].Item1;

                    // Skip processing if no movement has occurred.
                    if (Mathf.Approximately(0, distance))
                        continue;

                    // Cast a ray from the previous frame to detect midpoints during movement.
                    var hitCount = Physics.SphereCastNonAlloc(
                        _previousPositions[sphereIndex],
                        _detectorPositions[sphereIndex].Radius,
                        direction.normalized,
                        hits, distance, HitLayer, QueryTriggerInteraction.Ignore);

                    // Register hit colliders.
                    for (var hitIndex = 0; hitIndex < hitCount; hitIndex++)
                        RegisterHitColliders(hits[hitIndex].collider, hits[hitIndex].point);
                }
            }
            finally
            {
                lineData.Dispose();
                ArrayPool<RaycastHit>.Shared.Return(hits);
            }
        }

        /// <summary>
        ///     Calculate delta positions and movement distances between the previous frame and the current frame.
        /// </summary>
        /// <param name="data">Direction and distance data.</param>
        /// <param name="positions">List of current frame positions.</param>
        /// <param name="prePositions">List of previous frame positions.</param>
        [BurstCompile]
        private static void CalculateLineData(ref NativeArray<(Vector3, float)> data, in NativeArray<Vector3> positions,
            in NativeArray<Vector3> prePositions)
        {
            for (var i = 0; i < positions.Length; i++)
            {
                var direction = positions[i] - prePositions[i];
                var distance = direction.magnitude;

                data[i] = new ValueTuple<Vector3, float>
                {
                    Item1 = direction,
                    Item2 = distance
                };
            }
        }

        /// <summary>
        ///     Calculate new positions.
        /// </summary>
        /// <param name="newPositions">Final positions.</param>
        /// <param name="worldPositions">Positions considering offsets.</param>
        /// <param name="position">Reference position for collision detection.</param>
        [BurstCompile]
        private static void CalculateNewPositions(ref NativeArray<Vector3> newPositions,
            in NativeArray<Vector3> worldPositions,
            in Vector3 position)
        {
            for (var i = 0; i < newPositions.Length; i++) newPositions[i] = position + worldPositions[i];
        }

        /// <summary>
        ///     Update the offsets of the detection in world coordinates.
        /// </summary>
        private void UpdateCurrentPositions()
        {
            using var worldPositions = new NativeArray<Vector3>(_detectorPositions.Length, Allocator.Temp);
            _transform.TransformVectors(_offsets, worldPositions);
            CalculateNewPositions(ref _currentPositions, in worldPositions, _transform.position);
        }

        /// <summary>
        ///     Struct representing the position of the collision points.
        /// </summary>
        [Serializable]
        public class LineHitData
        {
            public float Radius = 1;
            public Vector3 Offset;
        }

        /// <summary>
        ///     System to control the <see cref="SwingHitDetector" /> component.
        /// </summary>
        public class LineHitSystem : SystemBase<SwingHitDetector, LineHitSystem>, IEarlyUpdate
        {
            private static readonly CustomSampler UpdatePositionSample = CustomSampler.Create("Update Positions");
            private static readonly CustomSampler PhysicsTestSample = CustomSampler.Create("Physics Tests");
            private static readonly CustomSampler InvokeEventSample = CustomSampler.Create("Invoke Event");

            int ISystemBase.Order => 0;

            private void OnDestroy()
            {
                UnregisterAllComponents();
            }

            void IEarlyUpdate.OnUpdate()
            {
                CleanUp();

                CalculatePositions();

                CalculateCollisionTest();

                InvokeEvents();
            }

            /// <summary>
            ///     Clear the buffers.
            /// </summary>
            private void CleanUp()
            {
                foreach (var component in Components)
                {
                    component.HitObjectsInThisFrame.Clear();
                    component.HitCollidersInThisFrame.Clear();
                    component._currentPositions.CopyTo(component._previousPositions);
                }
            }

            /// <summary>
            ///     Update the positions within the components.
            /// </summary>
            private void CalculatePositions()
            {
                UpdatePositionSample.Begin();
                foreach (var component in Components)
                    component.UpdateCurrentPositions();
                UpdatePositionSample.End();
            }

            /// <summary>
            ///     Perform collision tests using paths and ranges.
            /// </summary>
            private void CalculateCollisionTest()
            {
                PhysicsTestSample.Begin();

                foreach (var component in Components)
                    component.CalculateLineHit();

                foreach (var component in Components)
                    component.CalculateSphereHit();

                PhysicsTestSample.End();
            }

            /// <summary>
            ///     Invoke <see cref="CollisionDetectorBase.OnHitObjects" /> if colliders were detected in this frame.
            /// </summary>
            private void InvokeEvents()
            {
                foreach (var component in Components)
                {
                    if (component.IsHitInThisFrame == false)
                        continue;

                    InvokeEventSample.Begin(component.gameObject);
                    component.OnHitObjects.Invoke(component.HitObjectsInThisFrame);
                    InvokeEventSample.End();
                }
            }
        }
    }
}