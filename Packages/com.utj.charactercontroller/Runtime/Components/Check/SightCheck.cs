using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Components.Utility;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// This is a component that performs sight detection.
    /// It detects targets within a specified range from the viewpoint, considering obstacles.
    /// It calls <see cref="InsightTargets"/> when some targets are within the sight or when all objects have exited the sight.
    /// </summary>
    [AddComponentMenu(MenuList.MenuCheck + nameof(SightCheck))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RenamedFrom("TinyCharacterController.SightCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.SightCheck")]
    public class SightCheck : MonoBehaviour, IEarlyUpdateComponent
    {
        /// <summary>
        /// The position of the head to be used for detection.
        /// </summary>
        [Header("Sight Settings")]
        [RequestField]
        [SerializeField] private Transform _headTransform;

        /// <summary>
        /// The range of the sight.
        /// </summary>
        [FormerlySerializedAs("range")]
        [RenamedFrom("range")]
        public int Range = 10;

        /// <summary>
        /// The angle of the sight.
        /// </summary>
        [FormerlySerializedAs("angle")]
        [RenamedFrom("angle")]
        public int Angle = 30;

        /// <summary>
        /// The layer to use for detection. Objects in this layer will be visible.
        /// </summary>
        [FormerlySerializedAs("layer")]
        [RenamedFrom("layer")]
        public LayerMask VisibleLayerMask;

        /// <summary>
        /// The tags to use for detection. Objects with these tags will be visible.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("targetTagList")]
        [FormerlySerializedAs("TargetTagList")]
        private string[] _targetTagList;

        /// <summary>
        /// Gets a list of objects within the sight.
        /// </summary>
        public List<GameObject> InsightTargets { get; private set; } = new();
        
        /// <summary>
        /// If true, check for the presence of obstacles.
        /// Obstacle detection uses <see cref="CharacterSettings._environmentLayer"/>.
        /// </summary>
        [FormerlySerializedAs("raycastCheck")]
        [Header("Options")]
        [RenamedFrom("raycastCheck")]
        public bool RaycastCheck = true;

        /// <summary>
        /// Gets the first object found within the sight.
        /// </summary>
        public GameObject InsightTarget => InsightTargets.Count > 0 ? InsightTargets[0] : null;

        /// <summary>
        /// If true, there are objects within the sight.
        /// </summary>
        public bool IsInsightAnyTarget => InsightTargets.Count > 0;

        /// <summary>
        /// Calls an event when an object enters or exits the sight.
        /// </summary>
        public UnityEvent<bool> OnChangeInsightAnyTargetState;

        /// <summary>
        /// The maximum number of objects that can be detected at once.
        /// </summary>
        private const int Capacity = 100;
        
        private static readonly Collider[] Results = new Collider[Capacity];
        private CharacterSettings _settings;

        private void Reset()
        {
            // Set the default value of the layer to include in the sight.
            VisibleLayerMask = LayerMask.GetMask("Default");
        }

        private void OnValidate()
        {
            // Ensure that the maximum and minimum values are within valid ranges.
            Range = Mathf.Max(0, Range);
            Angle = Mathf.Clamp(Angle, 0, 360);
        }

        private void Awake()
        {
            // Collect a list of related components.
            GatherComponents();
        }

        /// <summary>
        /// Checks for obstacles between the target objects.
        /// Excludes the collider of the sensor and the target collider.
        /// </summary>
        /// <param name="position">The position of the sensor.</param>
        /// <param name="targetPosition">The closest position of the target.</param>
        /// <param name="targetCollider">The target object's collider.</param>
        /// <returns>True if obstructed by an obstacle.</returns>
        private bool IsCollideTarget(in Vector3 position, in Vector3 targetPosition, in Collider targetCollider)
        {
            var deltaPosition = (targetPosition - position);
            var direction = deltaPosition.normalized;
            var distance = deltaPosition.magnitude;
            
            // Allocate a buffer.
            var hits = ArrayPool<RaycastHit>.New(Capacity);
            
            // Check if the sight is clear towards the target from the sensor.
            var count = Physics.RaycastNonAlloc(position, direction, hits, distance, _settings.EnvironmentLayer,
                QueryTriggerInteraction.Ignore);

            // Skip processing if the detected collider is the target's collider or belongs to oneself.
            var isCollide = false;
            for (var i = 0; i < count; i++)
            {
                var hit = hits[i];
                // Skip processing if the collider belongs to the target or oneself.
                if (targetCollider == hit.collider || _settings.IsOwnCollider(hit.collider))
                    continue;

                // Interrupt the search process because it is obstructed by an obstacle.
                isCollide = true;
                break;
            }

            // Release the buffer.
            ArrayPool<RaycastHit>.Free(hits);
            
            // Return false if obstructed.
            return isCollide;
        }

        /// <summary>
        /// Collect a list of components.
        /// </summary>
        private void GatherComponents()
        {
            TryGetComponent(out _settings);
        }
        
        private void OnDrawGizmosSelected()
        {
            // Do nothing if the game is not playing.
            if (Application.isPlaying == false)
                return;

            // Represent objects within the sight using Gizmos.
            foreach (var obj in InsightTargets)
            {
                var position = obj.transform.position;
                GizmoDrawUtility.DrawSphere(position, 1f, Color.yellow);
            }
        }

        void IEarlyUpdateComponent.OnUpdate(float deltaTime)
        {
            // Cache the previous information to detect changes in the sight.
            var isAnyInsightTargetPreviousFrame = IsInsightAnyTarget;
            
            // Get the coordinates and direction of the sensor's position.
            var headPosition = _headTransform.position;
            var forward = _headTransform.forward;

            // Collect all colliders around the character.
            var count = Physics.OverlapSphereNonAlloc(headPosition, Range, Results, 
                VisibleLayerMask, QueryTriggerInteraction.Ignore);

            // Extract targets from the list of colliders.
            InsightTargets.Clear();
            for (var i = 0; i < count; i++)
            {
                var col = Results[i];

                // Skip processing if the detected object is one's own collider or not in the tag list.
                if (_settings.IsOwnCollider(col) ||
                    GameObjectUtility.ContainTag(col.gameObject, _targetTagList) == false)
                    continue;

                // Detect the position of the closest edge within the sight.
                var closestPoint = col.ClosestPointOnBounds(headPosition);
                var deltaPosition = closestPoint - headPosition;
                
                // Skip processing if the target is outside the sight.
                if (Vector3.Angle(forward, deltaPosition) > Angle * 0.5f)
                    continue;

                // Skip processing if RaycastCheck is enabled and the target is obstructed by an obstacle.
                if (RaycastCheck &&
                    IsCollideTarget(headPosition, closestPoint, col))
                    continue;

                // Add the object to the list of objects within the sight.
                if( InsightTargets.Contains(col.gameObject) == false)
                    InsightTargets.Add(col.gameObject);
            }

            // Notify if the sight state has changed.
            if (IsInsightAnyTarget != isAnyInsightTargetPreviousFrame)
                OnChangeInsightAnyTargetState.Invoke(IsInsightAnyTarget);
        }

        int IEarlyUpdateComponent.Order => Order.Check;
    }
}
