using System;
using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// A component that discovers the target with the lowest cost.
    /// The cost takes into account the angle to the target, the distance to the target, whether the target is visible, and whether it is captured by the MainCamera.
    /// The cost calculation can be customized.
    /// It is used, for example, to determine which enemy to attack once discovered.
    /// 
    /// The calculation is performed only when Find is executed.
    /// </summary>
    [AddComponentMenu(MenuList.MenuCheck + nameof(HighPriorityTargetSearch))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RenamedFrom("TinyCharacterController.HighPriorityTargetSearch")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.HighPriorityTargetSearch")]
    public class HighPriorityTargetSearch : MonoBehaviour
    {
        private const int Capacity = 100;

        /// <summary>
        /// Head height to determine visibility
        /// </summary>
        [FormerlySerializedAs("headOffset")] [Header("Sensor Settings")]
        public Vector3 HeadOffset = new Vector3(0, 0.5f, 0);
        
        /// <summary>
        /// Range of vision
        /// </summary>
        [FormerlySerializedAs("range")] public float Range = 5;
        
        /// <summary>
        /// Range of objects to include in the detection.
        /// Based on the Find direction, objects with an angle greater than MaxAngle are excluded from the list.
        /// </summary>
        [FormerlySerializedAs("maxAngle")] [Range(0, 360)]
        public float MaxAngle = 360;

        /// <summary>
        /// upper limit of the calculated cost.
        /// Subjects with a cost greater than this value will be removed from the list.
        /// </summary>
        public int CostLimit = 100;

        /// <summary>
        /// The width of the ray for calculate IsInsight checks.
        /// </summary>
        public float RadiusForIsInsightCheck = 1;
        
        /// <summary>
        /// The layer to include in the search target.
        /// </summary>
        [Header("Filter")]
        [FormerlySerializedAs("hitLayer")] 
        [FormerlySerializedAs("_hitLayer")]
        [SerializeField]
        public LayerMask HitLayer;
        /// <summary>
        /// The Tag to include in the search target.
        /// </summary>
        [FormerlySerializedAs("targetTags")] 
        [NonReorderable]
        [TagSelector]
        public List<string> TargetTags = new ();

        /// <summary>
        /// If True, calculate the distance determination to the character
        /// </summary>
        [FormerlySerializedAs("isCalculateDistance")] 
        public bool IsCalculateDistance = true;
        
        /// <summary>
        /// If True, calculate the angle from direction (about <see cref="Find(out UnityEngine.Transform)"/> ).
        /// </summary>
        [FormerlySerializedAs("isCalculateAngle")] 
        public bool IsCalculateAngle = true;

        /// <summary>
        /// If True, calculate the visibility passing through from the character.
        /// </summary>
        [FormerlySerializedAs("isCalculateIsInsight")] 
        public bool IsCalculateIsInsight = true;

        /// <summary>
        /// If True, limit inclusion to objects seen by the MainCamera
        /// </summary>
        [FormerlySerializedAs("isCalculateVisible")] 
        public bool IsCalculateVisible = true;
        
        /// <summary>
        /// This is a UnityEvent that calculates cost.
        /// If this event is not set, the default calculation formula will be used.
        /// The integer value contains the index of the discovered element, and the SearchTargetData includes a list of discovered elements.
        /// </summary>
        public Action<int,  SearchTargetData[]> OnCalculatePriority = null;


        /// <summary>
        /// Directly set the cost for the discovered element, primarily intended for use in VisualScripting.
        /// </summary>
        /// <param name="index">The index of element</param>
        /// <param name="cost">The cost used for the decision. The higher the cost, the more disadvantageous.</param>
        public void SetCost(int index, int cost)
        {
            _costs[index] = cost;
        }

        /// <summary>
        /// Discover the target with the lowest cost that matches the condition within the range.
        /// The judgment uses the Position and Forward of the Transform.
        /// </summary>
        /// <param name="target">Transform of the object with the lowest cost</param>
        /// <returns>The object is included in Target.</returns>
        public bool Find(out Transform target)
        {
            return Find(out target, _transform.forward);
        }

        /// <summary>
        /// Discover the target with the lowest cost that matches the condition within the range.
        /// The judgment uses the Position of the Transform.
        /// </summary>
        /// <param name="target">Transform of the object with the lowest cost</param>
        /// <param name="direction"></param>
        /// <returns>The object is included in Target.</returns>
        public bool Find(out Transform target, Vector3 direction)
        {
            var position = _transform.position + HeadOffset;
            var count = Physics.OverlapSphereNonAlloc(position, Range, Colliders, HitLayer, QueryTriggerInteraction.Ignore);
    
            if( IsCalculateVisible)
                GeometryUtility.CalculateFrustumPlanes(_settings.CameraMain, CameraPlanes);
            
            // search element.
            
            var findCount = 0;
            for (var index = 0; index < count; index++)
            {
                var hitCollider = Colliders[index];

                if (ContainTagList(hitCollider) == false)
                    continue;

                var isCalculatePosition =  IsCalculateDistance || IsCalculateIsInsight;
                var closestPoint = isCalculatePosition  ? hitCollider.ClosestPointOnBounds(position) : Vector3.zero;
                var angleToTarget = CalculateAngle(direction, position, closestPoint);

                if (angleToTarget > MaxAngle * 0.5f)
                    continue;
                
                var data = new SearchTargetData(
                        collider: hitCollider, 
                        distance: CalculateDistance(position, closestPoint) / Range,
                        isVisible: CalculateVisible(hitCollider),
                        angle:  angleToTarget / MaxAngle,
                        closestPoint: closestPoint,
                        isInsight:CalculateInsight(position, closestPoint));
                _targets[findCount] = data;
                findCount++;
            }

            if (findCount == 0)
            {
                // not found element.
                target = null;
#if UNITY_EDITOR
                CacheDebugData(null, 0, direction);
#endif
                return false;
            }

            // Set weights for targets 
            if( OnCalculatePriority != null)
                OnCalculatePriority.Invoke(findCount, _targets);
            else
                DefaultOrder(findCount, _targets);
            
            var lowestCostIndex = FindLowestCost(findCount);

            if (lowestCostIndex == -1)
            {
                // not found, all element cost over.
                target = null;
#if UNITY_EDITOR
                CacheDebugData(null, 0, direction);
#endif
                return false;
            }
            else
            {
                // select lighter ones
                target = _targets[lowestCostIndex].Collider.transform;

#if UNITY_EDITOR
                CacheDebugData(_targets[lowestCostIndex].Collider, findCount, direction);
#endif
                return true;
            }
        }
        
                
        private Transform _transform;
        private CharacterSettings _settings;
        private static readonly Collider[] Colliders = new Collider[Capacity];
        private static readonly Plane[] CameraPlanes = new Plane[6];
        private readonly SearchTargetData[] _targets = new SearchTargetData[Capacity];
        private readonly int[] _costs = new int[Capacity];

        private void Awake()
        {
            GatherComponents();
        }

        private void Reset()
        {
            HitLayer = 1 << LayerMask.NameToLayer("Default");
        }

        private void DefaultOrder(int count, SearchTargetData[] targets)
        {
            for (var i = 0; i < count; i++)
            {
                var target = targets[i];
                var cost = 0;
                cost += (int) (target.Angle * 0.2f);
                cost += target.IsInsight ? 0 : 50;
                cost += target.IsVisible ? 0 : 100;
                cost += (int)(target.Distance * 0.5f);
                _costs[i] = cost;
            }
        }

        private bool CalculateInsight(Vector3 position, Vector3 endPosition)
        {
            if (IsCalculateIsInsight == false)
                return true;
            
            var delta = endPosition - position;
            var distance = Vector3.Distance(position, endPosition);
            var ray = new Ray(position, delta.normalized);
            return !Physics.SphereCast(ray, 
                RadiusForIsInsightCheck,
                distance - 0.1f,
                _settings.EnvironmentLayer, 
                QueryTriggerInteraction.Ignore);
        }

        private int FindLowestCost(int count)
        {
            var lowestCostIndex = 0;
            var lowestCost = CostLimit;
            for (var index = 0; index < count; index++)
            {
                var cost = _costs[index];
                if (cost < lowestCost)
                {
                    lowestCost = cost;
                    lowestCostIndex = index;
                }
            }

            return CostLimit != lowestCost ?  lowestCostIndex : -1;
        }

        private bool ContainTagList(Component component)
        {
            foreach (var targetTag in TargetTags)
            {
                if (component.CompareTag(targetTag))
                    return true;
            }

            return false;
        }

        private bool CalculateVisible(Collider hitCollider)
        {
            if (IsCalculateVisible == false)
                return true;

            return GeometryUtility.TestPlanesAABB(CameraPlanes, hitCollider.bounds);
        }

        private float CalculateAngle(Vector3 direction, in Vector3 position, in Vector3 targetPosition)
        {
            if (IsCalculateAngle == false)
                return 0;
            
            var delta = (targetPosition - position).normalized;
            return Vector3.Angle(direction, delta);
        }

        private float CalculateDistance(in Vector3 position, in Vector3 targetPosition)
        {
            if (IsCalculateDistance == false)
                return 0;

            return Vector3.Distance(position, targetPosition);
        }

        private void GatherComponents()
        {
            TryGetComponent(out _transform);
            TryGetComponent(out _settings);
        }

        /// <summary>
        /// Data on the subject found.
        /// Used to determine cost.
        /// </summary>
        public struct SearchTargetData
        {
            /// <summary>
            /// Element Collider.
            /// </summary>
            public readonly Collider Collider;
            
            /// <summary>
            /// Amount about angle from direction.
            /// </summary>
            public readonly float Angle;
            
            /// <summary>
            /// Amount about distance from character.
            /// </summary>
            public readonly float Distance;
            
            /// <summary>
            /// Show in MainCamera
            /// </summary>
            public readonly bool IsVisible;
            
            /// <summary>
            /// Value of Closest Point of bounds.
            /// </summary>
            public readonly Vector3 ClosestPoint;
            
            /// <summary>
            /// Insight from character.
            /// </summary>
            public readonly bool IsInsight;

            public SearchTargetData(Collider collider, float distance, bool isVisible, float angle, Vector3 closestPoint, bool isInsight)
            {
                Collider = collider;
                Distance = distance;
                IsVisible = isVisible;
                Angle = angle;
                ClosestPoint = closestPoint;
                IsInsight = isInsight;
            }
        }

#if UNITY_EDITOR

        private Collider _debugClosest ;
        private Vector3 _debugDirection;
        private int _debugCapacity;

        void CacheDebugData(Collider closestTarget, int capacity, Vector3 direction)
        {
            _debugCapacity = capacity;
            _debugClosest = closestTarget;
            _debugDirection = direction;
        }

        private void OnDrawGizmosSelected()
        {
            var origin = transform.position + HeadOffset;
            GizmoDrawUtility.DrawSphere(origin, Range, Color.yellow, 0.1f);
            
            if (Application.isPlaying == false || _debugClosest == null)
                return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(origin, _debugDirection);
            Gizmos.DrawSphere(origin + _debugDirection, 0.1f);

            var max = 0f;
            for (var i = 0; i < _debugCapacity; i++)
            {
                max = Mathf.Max(max, _costs[i]);
            }
            
            for(var i=0; i<_debugCapacity; i++)
            {
                var bounds = _targets[i].Collider.bounds;
                var boundsTransform = _targets[i].Collider.transform;

                var late = _costs[i] / max;

                GizmoDrawUtility.DrawCube(bounds.center, bounds.size, 
                    boundsTransform.position, boundsTransform.rotation, boundsTransform.localScale, 
                    Color.yellow,  0.7f - late);
            }

            if (_debugClosest != null)
            {
                Gizmos.color = Color.red;
                var bounds1 = _debugClosest.bounds;
                Gizmos.DrawWireCube(bounds1.center, bounds1.size);
            }
        }
        
#endif
    }
}
