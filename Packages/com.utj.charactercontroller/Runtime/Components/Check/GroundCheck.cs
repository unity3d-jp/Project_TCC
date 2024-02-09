using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// This component performs collision detection with the ground.
    /// It determines whether the ground is present or not, the orientation of the surface in contact,
    /// information about the object in contact, and notifies about any changes in the ground object.
    ///
    /// This component's calculation is performed at the timing of OnUpdate.
    /// </summary>
    [RequireComponent(typeof(CharacterSettings))]
    [AddComponentMenu(MenuList.MenuCheck + nameof(GroundCheck))]
    [DisallowMultipleComponent]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.GroundCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.GroundCheck")]
    public class GroundCheck : MonoBehaviour, 
        IGroundContact, 
        IGroundObject, 
        IEarlyUpdateComponent
    {
        /// <summary>
        /// The maximum distance at which the character can be recognized as being on the ground.
        /// This distance is used for ambiguous ground detection.
        /// </summary>
        [FormerlySerializedAs("_localOffset")] 
        [SerializeField, Range(0, 2f)]
        private float _ambiguousDistance = 0.2f;

        /// <summary>
        /// The distance at which the character can be recognized as being on the ground.
        /// This distance is used for strict ground detection.
        /// </summary>
        [FormerlySerializedAs("_strictOffset")] 
        [SerializeField, Range(0, 0.5f)]
        private float _preciseDistance = 0.02f;
        
        /// <summary>
        /// This is the maximum slope at which the ground is recognized as "ground".
        /// If the slope of the nearest ground is greater than this angle, IsGround will be set to false.
        /// </summary>
        [FormerlySerializedAs("_limitGroundAngle")] 
        [SerializeField, Range(0, 90)]
        private int _maxSlope = 60;
        
        /// <summary>
        /// This Unity Event Invoke callbacks when the ground collider changes.
        /// It is useful mainly when the behavior of a character is affected by the ground settings.
        /// </summary>
        [Header("Callbacks")]
        [SerializeField]
        private UnityEvent<GameObject> _onChangeGroundObject;

        /// <summary>
        /// This Unity Event Invoke callbacks when the ground collider changes.
        /// It is useful mainly when the behavior of a character is affected by the ground settings.
        /// </summary>
        public UnityEvent<GameObject> OnChangeGroundObject => _onChangeGroundObject;

        /// <summary>
        /// This checks for ground detection.
        /// Returns true if there is a collider within range.
        /// This calculation result used when you want to avoid small fluctuations in ground detection, such as for between Animator state.
        /// </summary>
        [RenamedFrom("IsGrounded")]
        public bool IsOnGround { get; private set; }
        
        /// <summary>
        /// Returns true if the character is in contact with the ground.
        /// This property is stricter than IsGrounded. This property is mainly used for positioning the character.
        /// </summary>
        [RenamedFrom("IsGroundedStrictly")]
        public bool IsFirmlyOnGround { get; private set; }

        /// <summary>
        /// Returns the current ground collider. If the object is not grounded, returns null.
        /// </summary>
        public Collider GroundCollider { get; private set; }

        /// <summary>
        /// Returns the orientation of the ground surface. If not grounded, it returns Vector3.Up.
        /// </summary>
        [RenamedFrom("GroundNormal")]
        public Vector3 GroundSurfaceNormal { get; private set; } = Vector3.up;

        /// <summary>
        /// Get the grounded position.
        /// If not grounded, return Vector3.Zero.
        /// </summary>
        [RenamedFrom("ContactPoint")]
        public Vector3 GroundContactPoint { get; private set; }

        /// <summary>
        /// Get the distance to the ground.
        /// If not grounded, return the maximum distance.
        /// </summary>
        public float DistanceFromGround { get; private set; }
        
        /// <summary>
        /// Get the object of the ground.
        /// If not grounded, return Null.
        /// </summary>
        public GameObject GroundObject { get; private set; } = null;
        
        /// <summary>
        /// If the ground object has changed in the current frame, then it is True.
        /// </summary>
        public bool IsChangeGroundObject { get; private set; } = false;

        /// <summary>
        /// Performs a Raycast that ignores the Collider attached to the character.
        /// This API is used, for example, to detect a step in front of the character.
        /// </summary>
        /// <param name="position">start position.</param>
        /// <param name="distance">range of ray.</param>
        /// <param name="hit">return raycast hit.</param>
        /// <returns>Return true when hit collider.</returns>
        public bool Raycast(Vector3 position, float distance, out RaycastHit hit)
        {
            var groundCheckCount = Physics.RaycastNonAlloc(position, Vector3.down, _hits, distance,
                _settings.EnvironmentLayer, QueryTriggerInteraction.Ignore);
            
            return _settings.ClosestHit(_hits, groundCheckCount, distance, out hit);
        }

        UnityEvent<GameObject> IGroundObject.OnChangeGroundObject => _onChangeGroundObject;

        private const int CollectionColliderSize = 5;
        private readonly RaycastHit[] _hits = new RaycastHit[CollectionColliderSize];        // 
        private ITransform _transform;
        private CharacterSettings _settings;
        private RaycastHit _groundCheckHit;

        private void Reset()
        {
            _ambiguousDistance = GetComponent<CharacterSettings>().Height * 0.35f;
        }

        private void Awake()
        {
            GatherComponents();
        }
        
        private void GatherComponents()
        {
            TryGetComponent(out _transform);
            TryGetComponent(out _settings);
        }
        
        void IEarlyUpdateComponent.OnUpdate(float deltaTime)
        {
            using var _ = new ProfilerScope("Ground Check");
            var preGroundObject = GroundObject;
            var offset = _settings.Radius * 2;
            var origin = new Vector3(0, offset, 0) + _transform.Position;
            var groundCheckDistance = _ambiguousDistance + offset;
            
            // Perform ground detection while ignoring the character's own collider.
            var groundCheckCount = Physics.SphereCastNonAlloc(origin, _settings.Radius, Vector3.down, _hits, groundCheckDistance,
                _settings.EnvironmentLayer, QueryTriggerInteraction.Ignore);
            var isHit = ClosestHit(_hits, groundCheckCount, groundCheckDistance, out _groundCheckHit);
            
            // fill the properties of the component based on the information of the ground.
            if (isHit )
            {
                var inLimitAngle = Vector3.Angle(Vector3.up, _groundCheckHit.normal) < _maxSlope;
                
                DistanceFromGround = _groundCheckHit.distance - (offset - _settings.Radius );
                IsOnGround =  DistanceFromGround  <  _ambiguousDistance;
                IsFirmlyOnGround =  DistanceFromGround  <= _preciseDistance &&  inLimitAngle;
                GroundContactPoint = _groundCheckHit.point;
                GroundSurfaceNormal = _groundCheckHit.normal;
                GroundCollider = _groundCheckHit.collider;
                GroundObject = IsOnGround ? _groundCheckHit.collider.gameObject : null;
            }
            else
            {
                DistanceFromGround = _ambiguousDistance;
                IsOnGround = false;
                IsFirmlyOnGround = false;
                GroundContactPoint = Vector3.zero;
                GroundSurfaceNormal = Vector3.zero;
                GroundCollider = null;
                GroundObject = null;
            }

            // If the object has changed, invoke _onChangeGroundObject.
            IsChangeGroundObject = preGroundObject != GroundObject;
            if (IsChangeGroundObject)
            {
                using var invokeProfile = new ProfilerScope("Ground Check.Invoke");
                _onChangeGroundObject?.Invoke(GroundObject);
            }
        }
        
        private bool ClosestHit(IReadOnlyList<RaycastHit> hits,  int count, float maxDistance, out RaycastHit closestHit)
        {
            var min = maxDistance;
            closestHit = new RaycastHit();
            var isHit = false;

            for (var i = 0; i < count; i++)
            {
                var hit = hits[i];
                var isOverOriginHeight = hit.distance == 0; // Ignore contacts higher than ORIGIN.
                if (isOverOriginHeight || hit.distance > min ||  _settings.IsOwnCollider(hit.collider)  || hit.collider == null )
                    continue;

                min = hit.distance;
                closestHit = hit;
                isHit = true;
            }

            return isHit ;
        }

        int IEarlyUpdateComponent.Order => Order.Check;
        
        
        private void OnDrawGizmosSelected()
        {
            void DrawHitRangeGizmos(Vector3 startPosition, Vector3 endPosition)
            {
                var leftOffset = new Vector3(_settings.Radius, 0, 0);
                var rightOffset = new Vector3(-_settings.Radius, 0, 0);
                var forwardOffset = new Vector3(0, 0, _settings.Radius);
                var backOffset = new Vector3(0, 0, -_settings.Radius);
                Gizmos.DrawLine(startPosition + leftOffset, endPosition + leftOffset);
                Gizmos.DrawLine(startPosition + rightOffset, endPosition + rightOffset);
                Gizmos.DrawLine(startPosition + forwardOffset, endPosition + forwardOffset);
                Gizmos.DrawLine(startPosition + backOffset, endPosition + backOffset);
                GizmoDrawUtility.DrawSphere(startPosition, _settings.Radius, Color.yellow);
                GizmoDrawUtility.DrawSphere(endPosition, _settings.Radius, Color.yellow);
            }
            
            if (_settings == null)
                TryGetComponent(out _settings);
            
            var position = transform.position;
            var offset = _settings.Height / 2;



            if (Application.isPlaying)
            {
                Gizmos.color = IsOnGround ? Color.red : Gizmos.color;
                Gizmos.color = IsFirmlyOnGround ? Color.blue : Gizmos.color;

                var topPosition = new Vector3 { y = _settings.Radius - _preciseDistance };
                var bottomPosition = IsOnGround
                    ? new Vector3 { y = _settings.Radius - DistanceFromGround }
                    : new Vector3 { y = _settings.Radius - _ambiguousDistance };
                
                DrawHitRangeGizmos(position + topPosition, position + bottomPosition);
                
                if (IsOnGround)
                {
                    GizmoDrawUtility.DrawCollider(GroundCollider, Color.green, 0f);
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(_groundCheckHit.point, 0.1f);
                    Gizmos.DrawRay(_groundCheckHit.point, GroundSurfaceNormal * offset);
                }
            }
            else
            {
                var topPosition = new Vector3 { y = _settings.Radius - _preciseDistance };
                var bottomPosition = new Vector3 { y = _settings.Radius - _ambiguousDistance };
                DrawHitRangeGizmos( position + topPosition, position + bottomPosition);
            }
        }
    }
}