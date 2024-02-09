using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Modifier;
using Unity.TinyCharacterController.Utility;
using UnityEngine;

namespace Unity.TinyCharacterController.Effect
{
    /// <summary>
    /// This component that moves a character riding on a moving object.
    /// If an object with a ground collider has the MovePlatform component, it will follow the movement.
    /// When leaves from the ground, the movement amount is converted to acceleration and set to the character.
    /// </summary>
    [AddComponentMenu(MenuList.MenuEffect + nameof(MoveWithPlatform))]
    [DisallowMultipleComponent]
    [RequireInterface(typeof(IGroundContact))]
    [RequireInterface(typeof(IGroundObject))]
    [RequireInterface(typeof(IGravity))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.MoveWithPlatform")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Effect.MoveWithPlatform")]
    public class MoveWithPlatform : MonoBehaviour, 
        IEffect, 
        IEarlyUpdateComponent
    {
        /// <summary>
        /// Friction with the ground
        /// </summary>
        [SerializeField, Range(0f, 1f)] 
        private float _friction = 0.9f;
        
        /// <summary>
        /// Aerodynamic drag
        /// </summary>
        [SerializeField, Range(0f, 1f)] 
        private float _drag = 0f;
        
        /// <summary>
        /// Reset additional vectors by MoveWithPlatform.
        /// For example, use to reset the vector when jumping in the air.
        /// </summary>
        public void ResetVelocity()
        {
            _velocity = Vector3.zero;
        }
        
        /// <summary>
        /// True if on any platform.
        /// </summary>
        public bool OnPlatform { get; private set; }
        
        private IGroundContact _groundCheck;
        private IGroundObject _groundObject;
        private IGravity _gravity;
        private Vector3 _velocity;
        private Platform _platform;
        private MovePlatform _movePlatform;
        private ITransform _transform;
        private float _speed;

        Vector3 IEffect.Velocity => _velocity;

        private void Awake()
        {
            TryGetComponent(out _groundCheck);
            TryGetComponent(out _groundObject);
            TryGetComponent(out _gravity);
            TryGetComponent(out _transform);
            _platform = new Platform();
        }

        private void UpdateState()
        {
            // initialize
            var hasGroundObject = _groundObject.GroundObject != null;
            
            if ((!_gravity.IsLanded || !hasGroundObject) &&
                (!_groundObject.IsChangeGroundObject || !hasGroundObject)) 
                return;

            // Update any changes in ground information
            var ground = _groundObject.GroundObject.TryGetComponent(out _movePlatform);
            if( ground )
                _platform.Activate(_groundObject.GroundObject.transform, _transform.Position, _transform.Rotation);
            else
                _platform.Inactive();
        }

        void IEarlyUpdateComponent.OnUpdate(float deltaTime)
        {
            UpdateState();

            OnPlatform = _groundCheck.IsFirmlyOnGround && _movePlatform != null ;
    
            // Update character position based on updated ground coordinates
            if (_platform.IsActive)
            {
                _platform.UpdatePositionAndRotation(_transform.Position, _transform.Rotation);
        
                _transform.Position += _platform.DeltaPosition;
                _transform.Rotation *= _platform.DeltaRotation;
            }
    
            if ( _gravity.IsLeaved )
            {
                _velocity = _platform.DeltaPosition.normalized * _speed ;
                _platform.Inactive();
            }
    
            // Attenuates movement speed
            var dump = _groundCheck.IsFirmlyOnGround ? _friction : _drag;
            _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, dump);
            _speed = Mathf.MoveTowards(_speed, _platform.DeltaPosition.magnitude / deltaTime, 0.1f);
        }

        
        int IEarlyUpdateComponent.Order => Order.Effect - 1; // 他のコンポーネントより先に実行
       
        
#if UNITY_EDITOR
        
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            if (_platform.IsActive)
                DrawPlatformGizmos();

            if (_movePlatform != null && _movePlatform.TryGetComponent(out Collider col))
                GizmoDrawUtility.DrawCollider(col, Color.blue);
        }

        private void DrawPlatformGizmos()
        {
            var offset = Vector3.up * GetComponent<CharacterSettings>().Height * 0.5f;
            var trsPosition = transform.position;
            var direction = _platform.DeltaPosition.normalized;
            var drawPoint = trsPosition + offset + direction * _speed * 0.5f;

            Gizmos.DrawRay(trsPosition + offset, direction * (_speed * 0.5f - 0.25f * 0.5f));
            GizmoDrawUtility.DrawSphere(drawPoint, 0.25f, Color.blue);
        }


#endif

        
        /// <summary>
        /// Platform Information
        /// </summary>
        private class Platform
        {
            private Transform _activePlatform;
            private Vector3 _previousPoint;
            private Vector3 _localPoint;
            private Quaternion _previousRotation;
            private Quaternion _localRotation;
            
            public bool IsActive { get; private set; } 
            
            public void Activate(Transform value, in Vector3 position, in Quaternion rotation)
            {
                _activePlatform = value;
                IsActive = _activePlatform != null;
                if (IsActive)
                {
                    UpdatePreviousPointAndRotation(position, rotation);
                }
                else
                {
                    DeltaPosition = Vector3.zero;
                }
            }

            public void Inactive()
            {
                IsActive = false;
                _activePlatform = null;
                DeltaPosition = Vector3.zero;
            }
            /// <summary>
            /// Delta movement compared to the previous state.
            /// </summary>
            public Vector3 DeltaPosition { get; private set; }

            /// <summary>
            /// Delta rotation compared to the previous state.
            /// </summary>
            public Quaternion DeltaRotation { get; private set; }

            public void UpdatePositionAndRotation(in Vector3 position, in Quaternion rotation)
            {
                if (IsActive == false) return;
    
                CalculateNewPointAndRotation();
                UpdatePreviousPointAndRotation(position, rotation);
            }
            
            private void CalculateNewPointAndRotation()
            {    
                var currentPoint = _activePlatform.TransformPoint(_localPoint);
                var currentRotation = _activePlatform.rotation * _localRotation;
                var inverseRotation = Quaternion.Inverse(_previousRotation);
    
                DeltaPosition = currentPoint - _previousPoint;

                var angel = Quaternion.Angle(currentRotation, inverseRotation);
                DeltaRotation = Mathf.Approximately(angel, 0)
                    ? Quaternion.identity
                    : currentRotation * Quaternion.Inverse(_previousRotation);
            }
    
            private void UpdatePreviousPointAndRotation(in Vector3 position, Quaternion rotation)
            {
                _previousPoint = position;
                _previousRotation = rotation;
                _localPoint = _activePlatform.InverseTransformPoint(_previousPoint);
                _localRotation = Quaternion.Inverse(_activePlatform.rotation) * _previousRotation;
            }        
        }
    }

}
