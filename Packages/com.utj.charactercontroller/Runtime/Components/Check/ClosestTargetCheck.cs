using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// This component retrieves the nearest target within a certain range.
    /// </summary>
    [AddComponentMenu(MenuList.MenuCheck + nameof(ClosestTargetCheck))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.ClosestTargetCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.ClosestTargetCheck")]
    public class ClosestTargetCheck : MonoBehaviour
    {
        /// <summary>
        /// The tag that will be targeted for the search.
        /// </summary>
        [SerializeField]
        [TagSelector] 
        private string _tag;
        
        /// <summary>
        /// The range of search.
        /// </summary>
        [SerializeField] private float _radius;

        /// <summary>
        /// The layer of search target.
        /// </summary>
        [SerializeField] private LayerMask _layer;

        private Transform _transform;

        /// <summary>
        /// The collider found as a result of the search.
        /// </summary>
        public Collider Target { get; private set; }

        /// <summary>
        /// Pre <see cref="Target"/>
        /// </summary>
        public Collider PreTarget { get; private set; }

        /// <summary>
        /// An event that is called when the target has changed.
        /// </summary>
        public UnityEvent OnChangeClosestTarget;

        private static readonly Collider[] Results = new Collider[100];
        private int _preInstanceId;

        private void Awake()
        {
            TryGetComponent(out _transform);
        }

        private void Update()
        {
            var position = _transform.position;

            // Cache the Collider info of the previous frame.
            PreTarget = Target;
            
            // Collect information of the colliders within the range, and select the closest collider among them.
            var count = Physics.OverlapSphereNonAlloc(position, _radius, Results, _layer, QueryTriggerInteraction.Ignore);
            Target = ClosestCollider(position, Results, count);

            // Compare the past and current colliders, and if the collider has changed, Invoke the UnityEvent.
            // This judgment is based on the InstanceID, taking into account cases where the collider has been deleted.
            var instanceId = Target != null ? Target.GetInstanceID() : 0;
            if (_preInstanceId != instanceId)
            {
                OnChangeClosestTarget?.Invoke();
            }
            _preInstanceId = instanceId;
        }


        private Collider ClosestCollider(Vector3 position, Collider[] colliders, int count)
        {
            Collider closest = null;
            float minDistance = float.MaxValue;
            for (var i = 0; i < count; i++)
            {
                var col = colliders[i];
                
                if( col.CompareTag(_tag) == false)
                    continue;
                
                var distance = Vector3.Distance(col.transform.position, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = col;
                }
            }

            return closest;
        }
        

        private void OnDrawGizmosSelected()
        {
            GizmoDrawUtility.DrawSphere(transform.position, _radius, Color.yellow);
        }
    }
}
