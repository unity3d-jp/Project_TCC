using Unity.TinyCharacterController.Interfaces.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// A component that performs collision detection with walls is implemented.
    /// It detects walls in the direction of the character's movement.
    /// When a collision with a wall occurs, callbacks are triggered during the collision,
    /// while in contact with the wall, and when the character moves away from the wall.
    /// </summary>
    [AddComponentMenu(MenuList.MenuCheck + nameof(WallCheck))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.WallCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.WallCheck")]
    public class WallCheck : MonoBehaviour, 
        IEarlyUpdateComponent, IWallCheck
    {
        [Header("Settings")]
        [FormerlySerializedAs("_angleMinMax")] 
        [SerializeField, MinMax(15, 165)] 
        private Vector2 _wallAngleRange = new Vector2(75, 115);

        [FormerlySerializedAs("_maxDistance")]
        [Range(0.01f, 1f)]
        [SerializeField] private float _wallDetectionDistance　= 0.1f;
        
        // callbacks
        
        /// <summary>
        /// Invoke when touch with a wall.
        /// </summary>
        [Header("Callbacks")]
        [FormerlySerializedAs("OnTouchWall")] 
        public UnityEvent OnWallContacted;
        
        /// <summary>
        /// Invoke when leave from a wall.
        /// </summary>
        [FormerlySerializedAs("OnLeaveWall")] 
        public UnityEvent OnWallLeft;
        
        /// <summary>
        /// Invoke when contact with a wall.
        /// </summary>
        [FormerlySerializedAs("OnStickWall")] 
        public UnityEvent OnWallStuck;

        private int _order;
        private IBrain _brain;
        private ITransform _transform;
        private CharacterSettings _settings;
        
        private Vector3 _normal;
        private Vector3 _hitPoint;
        private Collider _contactObject ;
        private static readonly RaycastHit[] Hits = new RaycastHit[5];
        
        /// <summary>
        /// If there is contact, it returns True.
        /// </summary>
        public bool IsContact { get; private set; }

        /// <summary>
        /// Returns normal vector of the contact surface. If there is no contact, it returns Vector3.Zero.
        /// </summary>
        public Vector3 Normal => _normal;

        public GameObject ContactObject => _contactObject.gameObject;

        public Vector3 HitPoint => _hitPoint;

        private void Awake()
        {
            TryGetComponent(out _brain);
            TryGetComponent(out _transform);
            TryGetComponent(out _settings);
        }

        void IEarlyUpdateComponent.OnUpdate(float deltaTime)
        {
            // If the component is invalid, do not update the process.
            if (enabled == false)
                return;

            var preContact = IsContact;
            var direction = _brain.ControlVelocity.normalized;

            IsContact = HitCheck(direction, out _normal, out _hitPoint, out _contactObject);
            
            if (IsContact && !preContact )
            {
                OnWallContacted?.Invoke();
            }
            
            if (IsContact)
            {
                OnWallStuck?.Invoke();
            }
            
            if (!IsContact && preContact)
            {
                OnWallLeft?.Invoke();
            }
        }

        /// <summary>
        /// Immediately performs wall determination.
        /// The results of this calculation are processed independently of the component calculations. This means that the calculation results are not saved.
        /// The calculation ignores colliders in the same component.
        /// </summary>
        /// <param name="direction">direction</param>
        /// <param name="normal">result normal</param>
        /// <param name="point">result hit point</param>
        /// <param name="contactCollider">return contact object.  if no contact return null.</param>
        /// <returns>is contact any collider</returns>
        public bool HitCheck(Vector3 direction, out Vector3 normal, out Vector3 point, out Collider contactCollider)
        {
            var distance = _settings.Radius + _wallDetectionDistance;
            var halfHeight = _settings.Height * 0.5f;
            var centerPosition = _transform.Position + Vector3.up * halfHeight;
            var ray = new Ray(centerPosition, direction);
            var count = Physics.SphereCastNonAlloc(ray, _settings.Radius, Hits, distance, _settings.EnvironmentLayer,
                QueryTriggerInteraction.Ignore);

            // find most closest target.
            var hasClosestHit = _settings.ClosestHit(Hits, count, distance, out var hit);
            if (hasClosestHit)
            {
                // apply limit angle.
                var angle = Vector3.Angle(Vector3.up, hit.normal);
                if (angle > _wallAngleRange.x && angle < _wallAngleRange.y &&
                    Vector3.Distance(hit.point, centerPosition) < distance)
                {
                    normal = hit.normal;
                    point = hit.point;
                    contactCollider = hit.collider;
                    return true;
                }
            }

            normal = Vector3.zero;
            point = Vector3.zero;
            contactCollider = null;
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == false)
                return;
            
            if( IsContact)
                GizmoDrawUtility.DrawCollider(_contactObject, Color.yellow, 0);
            
            var distance = _settings.Radius + 0.1f;
            var halfHeight = _settings.Height * 0.5f;
            var centerPosition = _transform.Position + Vector3.up * halfHeight;
            var direction = _brain.ControlVelocity.normalized;
            var position = centerPosition + direction * distance;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(position,  _settings.Radius);
            
            Gizmos.color = IsContact ? Color.red : Color.white;
            Gizmos.DrawWireSphere(position, _settings.Radius);
        }

        int IEarlyUpdateComponent.Order => Order.Check;
    }
}
