using System;
using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// This component considers line of sight and obstacles to retrieve objects with specific tags within its range.
    /// The component runs every frame and invoke a callback if there are any changes in its content.
    /// This component is mainly used to retrieve objects within a certain range.
    /// </summary>
    [AddComponentMenu(MenuList.MenuCheck + nameof(RangeTargetCheck))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RenamedFrom("TinyCharacterController.RangeTargetCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.RangeTargetCheck")]
    public class RangeTargetCheck : MonoBehaviour, IEarlyUpdateComponent
    {
        private const int Capacity = 100;
        
        /// <summary>
        /// The center point of the sensor.
        /// </summary>
        [SerializeField] private Vector3 _sensorOffset = new Vector3(0, 0.5f, 0);
        
        /// <summary>
        /// The layers that the sensor can detect.
        /// </summary>
        [SerializeField] private LayerMask _hitLayer;
        
        /// <summary>
        /// This is a layer that ignores in Environment Layer.
        /// It is used to specify objects that should be movable but not detectable, such as transparent windows.
        /// </summary>
        [SerializeField] private LayerMask _transparentLayer;
        
        /// <summary>
        /// This is a property for specifying the target that the sensor will search for.
        /// It allows you to set properties such as the target's tag, search range, and whether to consider visibility.
        ///
        /// This setting determines the range of the component's sensor.
        /// </summary>
        [SerializeField] private SearchRangeSettings[] _searchData;
        
        private readonly List<string> _tags = new ();
        private SearchedTarget[] _searchTargets;
        private static readonly Collider[] OverlapSphereResult = new Collider[Capacity];
        private static readonly RaycastHit[] Hits = new RaycastHit[30];
        private static readonly Plane[] CameraPlanes = new Plane[6];
        private float _maxDistance = -1;
        private bool _useScreenCheck = true;
        private LayerMask _raycastHitLayer;
        private CharacterSettings _settings;
        private ITransform _transform;

        public SearchRangeSettings GetSearchRangeSettings(int index) => _searchData[index];

        public SearchRangeSettings GetSearchRangeSettings(string tagName)
        {
            var index = GetTagIndex(tagName);
            if (index == -1)
                throw new Exception("SearchData not found!");
            
            return GetSearchRangeSettings(index);
        }
        
        /// <summary>
        /// Get a list of objects that possess certain tags within a specific range.
        /// The gathering of this collection is based on an aggregation that takes place at the time of the Check.
        /// Any tags that are not found in the _searchData will return Null.
        /// </summary>
        /// <param name="tagName">Tag index</param>
        /// <returns>A list of Transforms within the range.</returns>
        public List<Transform> GetTargets(string tagName)
        {
            var tagIndex = GetTagIndex(tagName);
            return GetTargets(tagIndex);
        }
        
        /// <summary>
        /// Get a list of objects that possess certain tags within a specific range.
        /// The gathering of this collection is based on an aggregation that takes place at the time of the Check.
        /// Any tags that are not found in the _searchData will return Null.
        /// </summary>
        /// <param name="index">Index of tag.</param>
        /// <returns>A list of Transforms within the range.</returns>
        public List<Transform> GetTargets(int index)
        {
            return index == -1 ? null : _searchTargets[index].Targets;
        }

        /// <summary>
        /// Check if there are no objects with the specified tag.
        /// </summary>
        /// <param name="tagIndex">Index of tag.</param>
        /// <returns>Returns True if there is no object with the specified tag, or if the tag is not included in the search target.</returns>
        public bool IsEmpty(int tagIndex)
        {
            if (tagIndex < _searchTargets.Length && tagIndex >= 0)
                return _searchTargets[tagIndex].Targets.Count == 0;
            return
                true;
        }
        
        /// <summary>
        /// Check if there are no objects with the specified tag.
        /// </summary>
        /// <param name="tagName">Name of tag.</param>
        /// <returns>Returns True if there is no object with the specified tag, or if the tag is not included in the search target.</returns>
        public bool IsEmpty(string tagName) 
        {
            var tagIndex = GetTagIndex(tagName);
            return IsEmpty(tagIndex);
        }

        /// <summary>
        /// Get a nearest target with a specified tag using pre-calculated results.
        /// It returns False if the requested tag is not included in the range or if there are no targets within the range.
        /// </summary>
        /// <param name="tagName">Name of tag.</param>
        /// <param name="target">Nearest target.</param>
        /// <returns>Target was found.</returns>
        public bool TryGetClosestTarget(string tagName, out Transform target)
        {
            var tagIndex = GetTagIndex(tagName);
            if (tagIndex == -1)
            {
                target = null;
                return false;
            }

            target = _searchTargets[tagIndex].ClosestTarget.CurrentTransform;
            return target != null;
        }

        /// <summary>
        /// Get the index where the tag is stored.
        /// </summary>
        /// <param name="tagName">Name of tag.</param>
        /// <returns>Index of tag.</returns>
        public int GetTagIndex(string tagName)
        {
            return _tags.IndexOf(tagName);
        }

        /// <summary>
        /// Get a nearest target with a specified tag using pre-calculated results.
        /// It returns False if the requested tag is not included in the range or if there are no targets within the range.
        /// </summary>
        /// <param name="tagName">Name of tag.</param>
        /// <param name="target">Nearest target.</param>
        /// <param name="preTarget">Pre nearest target.</param>
        /// <returns>Target was found.</returns>
        public bool TryGetClosestTarget(string tagName, out Transform target, out Transform preTarget)
        {
            var tagIndex = GetTagIndex(tagName);;
            if (tagIndex == -1)
            {
                target = null;
                preTarget = null;
                return false;
            }

            var item = _searchTargets[tagIndex].ClosestTarget;
            target = item.CurrentTransform;
            preTarget = item.PreTransform;
            return target != null;
        }

        /// <summary>
        /// Get a list of newly added or removed targets within the specified tag range.
        /// It returns False if there is no change in the list, or if the specified tag is not included in the search target.
        /// If the specified tag is not included in the search target, Added and Removed will be Null.
        /// </summary>
        /// <param name="tagName">Name of tag.</param>
        /// <param name="added">List of objects in range</param>
        /// <param name="removed">List of objects out of range</param>
        /// <returns>Objects have been added or removed.</returns>
        public bool ChangedValues(string tagName, out List<Transform> added, out List<Transform> removed)
        {
            var tagIndex = GetTagIndex(tagName);
            return ChangedValues(tagIndex, out added, out removed);
        }

        /// <summary>
        /// Get a list of newly added or removed targets within the specified tag range.
        /// It returns False if there is no change in the list, or if the specified tag is not included in the search target.
        /// If the specified tag is not included in the search target, Added and Removed will be Null.
        /// </summary>
        /// <param name="tagIndex">Index of tag.</param>
        /// <param name="added">List of objects in range</param>
        /// <param name="removed">List of objects out of range</param>
        /// <returns>Objects have been added or removed.</returns>
        public bool ChangedValues(int tagIndex, out List<Transform> added, out List<Transform> removed)
        {
            if (tagIndex == -1)
            {
                added = null;
                removed = null;
                return false;
            }

            added = _searchTargets[tagIndex].Added;
            removed = _searchTargets[tagIndex].Removed;

            return added.Count + removed.Count > 0;
            
        }

        private Transform GetClosest(Vector3 position, List<Transform> targets)
        {
            float minDistance = float.MaxValue;
            Transform nearest = null;
            foreach (var target in targets)
            {
                var distance = Vector3.Distance(position, target.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = target;
                }
            }

            return nearest;
        }



        private void Awake()
        {
            GatherComponents();

            _raycastHitLayer =_settings.EnvironmentLayer & ~_transparentLayer;
            
            foreach (var data in _searchData)
            {
                _tags.Add(data.Tag);
                _useScreenCheck |= data.ExcludeHiddenFromCamera;
                if (data.Range > _maxDistance)
                    _maxDistance = data.Range;
            }

            _searchTargets = new SearchedTarget[_tags.Count];
            for (var i = 0; i < _tags.Count; i++)
            {
                _searchTargets[i] = new SearchedTarget();
            }
        }


        void IEarlyUpdateComponent.OnUpdate(float deltaTime)
        {
            using var profiler = new ProfilerScope("Range Check");

            // Get a list of Colliders with the specified layer centered on the sensor position.
            
            var sensorPosition = _transform.Position + _sensorOffset;
            var cameraPosition = _settings.CameraTransform.position;
            var count = Physics.OverlapSphereNonAlloc(
                sensorPosition, 
                _maxDistance, 
                OverlapSphereResult, 
                _hitLayer, 
                QueryTriggerInteraction.Collide);
 
            if( _useScreenCheck)
                GeometryUtility.CalculateFrustumPlanes(_settings.CameraMain, CameraPlanes);

            // Get Collider, Object Coordinates, Distance to Collider bounds.
            
            using var hitStatePo = UnityEngine.Pool.ListPool<ValueTuple<Collider, Vector3, float>>
                .Get(out var hitState);
            
            for (var index = 0; index < count; index++)
            {
                var col = OverlapSphereResult[index];
                var closePoint = col.ClosestPointOnBounds(sensorPosition);
                var distance = Vector3.Distance(sensorPosition, closePoint);
                hitState.Add(new ValueTuple<Collider, Vector3, float>(col, closePoint, distance));
            }

            // Allocate each tag from the list, and select newly added elements and deleted elements.
            
            for (var dataIndex = 0; dataIndex < _searchData.Length; dataIndex++)
            {
                var data = _searchData[dataIndex];
                var target = _searchTargets[dataIndex];

                using var preTransformPo = UnityEngine.Pool.ListPool<Transform>.Get(out var preTransforms);
                preTransforms.AddRange(target.Targets);
                target.Colliders.Clear();
                target.Targets.Clear();
                target.Added.Clear();
                target.Removed.Clear();
                
                foreach (var hit in hitState)
                {
                    if( data.Range < hit.Item3 ||
                        hit.Item1.CompareTag(data.Tag) == false)
                        continue;

                    if (target.Targets.Contains(hit.Item1.transform))
                        continue;
                    
                    if( data.ExcludeOutOfView && GeometryUtility.TestPlanesAABB(CameraPlanes, hit.Item1.bounds) == false)
                        continue;
                    
                    if (data.ExcludeHiddenFromPlayer && RaycastCheck(sensorPosition, hit.Item2, hit.Item1))
                        continue;

                    if (data.ExcludeHiddenFromCamera &&  RaycastCheck(cameraPosition, hit.Item2, hit.Item1) )
                        continue;
                    
                    target.Colliders.Add(hit.Item1);
                    target.Targets.Add(hit.Item1.transform);
                }

                foreach (var newTransform in target.Targets)
                {
                    if (newTransform != null && !preTransforms.Contains(newTransform))
                        target.Added.Add(newTransform);
                }
                foreach (var preTransform in preTransforms)
                {
                    if (preTransform != null && !target.Targets.Contains(preTransform))
                        target.Removed.Add(preTransform);
                    
                    if( preTransform == null )
                        target.Removed.Add(null);
                } 
                
                if( target.Added.Count != 0 || target.Removed.Count != 0)
                    data.OnChangeValue?.Invoke(new ValueTuple<List<Transform>, List<Transform>>(target.Added, target.Removed));
            }


            // 
            
            for (var index = 0; index < _searchData.Length; index++)
            {
                var data = _searchData[index];
                if (data.CalculateNearest == false)
                    continue;

                var tagIndex = _tags.FindIndex(c => c == data.Tag);
                var isEmpty = IsEmpty(tagIndex);
                var closest = isEmpty ? null : GetClosest(sensorPosition, _searchTargets[index].Targets);

                var item = _searchTargets[tagIndex].ClosestTarget;

                var id = isEmpty ? 0 : closest.GetInstanceID();

                if (id != item.PreTransformInstance)
                {
                    _searchTargets[tagIndex].ClosestTarget = new TargetData
                    {
                        CurrentTransform = closest,
                        PreTransform = item.CurrentTransform,
                        PreTransformInstance = id
                    };
                    data.OnChangeClosestTarget.Invoke();
                }
            }
        }

        int IEarlyUpdateComponent.Order => Order.Check;
        


        private bool RaycastCheck(Vector3 position, Vector3 closePoint, Collider targetCollider)
        {
            const float offset = 0.1f;

            for (var i = 0; i < Hits.Length; i++)
                Hits[i] = default;

            var distance = Vector3.Distance(position, closePoint) - offset;
            var ray = new Ray(position, closePoint - position);
            var count = Physics.RaycastNonAlloc(ray, Hits,distance,  _raycastHitLayer, QueryTriggerInteraction.Ignore);
            count -= Array.Exists(Hits, h => h.collider == targetCollider) ? 1 : 0;
            return count > 0;
        }

        private void GatherComponents()
        {
            TryGetComponent(out _transform);
            _settings = GetComponentInParent<CharacterSettings>();
        }

        private class SearchedTarget
        {
            public readonly List<Collider> Colliders = new();
            public readonly List<Transform> Targets = new();
            public readonly List<Transform> Added = new();
            public readonly List<Transform> Removed = new();

            
            public TargetData ClosestTarget;
        }

        private struct TargetData
        {
            public Transform PreTransform;
            public Transform CurrentTransform;
            public int PreTransformInstance;
        }
        

        [System.Serializable]
        public class SearchRangeSettings
        {
            /// <summary>
            /// The tag of the target objects.
            /// </summary>
            [TagSelector]
            [Header("Settings")]
            public string Tag;
    
            /// <summary>
            /// Radius of the search range.
            /// </summary>
            public float Range;
    
            /// <summary>
            /// Exclude objects that are out of the camera's view.
            /// </summary>
            [Header("Options")]
            public bool ExcludeOutOfView;

            /// <summary>
            /// Exclude objects with obstructions between them and the camera.
            /// </summary>
            public bool ExcludeHiddenFromCamera;

            /// <summary>
            /// Exclude objects with obstructions between them and the player.
            /// </summary>
            public bool ExcludeHiddenFromPlayer;

            /// <summary>
            /// Calculate the nearest object. Enabled by default.
            /// </summary>
            public bool CalculateNearest = true;

            /// <summary>
            /// Event triggered when the closest target changes.
            /// </summary>
            [Header("Event")]
            public UnityEvent OnChangeClosestTarget;

            /// <summary>
            /// Event triggered when the list of detected targets changes.
            /// </summary>
            public UnityEvent<(List<Transform>, List<Transform>)> OnChangeValue;
        }


#if UNITY_EDITOR
        private readonly Color[] _colors = new[]
        {
            Color.yellow, Color.red, Color.blue, Color.green, 
            Color.cyan, Color.white, Color.magenta, 
        };
        
        private void OnDrawGizmosSelected()
        {
            var center = _sensorOffset + transform.position;


            for (var i = 0; i < _searchData.Length; i++)
            {
                var data = _searchData[i];
                var currentColor = _colors[i % _colors.Length];
                if (Mathf.Approximately(0, data.Range) || data.Range < 0 )
                    continue;
                
                Gizmos.color = currentColor;
                Gizmos.DrawWireSphere(center, data.Range);
                Gizmos.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.2f);
                Gizmos.DrawSphere(center, data.Range);
            }
            if (Application.isPlaying == false)
                return;
            
            for (var i = 0; i < _searchData.Length; i++)
            {
                var currentColor = _colors[i % _colors.Length];
                var targets = _searchTargets[i].Targets;
                // UnityEditor.Handles.DrawOutline(targets.Select(c => c.gameObject).ToArray(), currentColor, 0.4f);

                var colliders = _searchTargets[i].Colliders;
                var closest = _searchTargets[i].ClosestTarget.CurrentTransform;
                for (var index = 0; index < targets.Count; index++)
                {
                    var target = targets[index];
                    Gizmos.color = target == closest ? Color.red : Color.white;
                    Gizmos.DrawWireCube(colliders[index].bounds.center, colliders[index].bounds.size);
                    Gizmos.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
                    Gizmos.DrawCube(colliders[index].bounds.center, colliders[index].bounds.size);
                }
            }
        }
#endif
        
        
    }
}
