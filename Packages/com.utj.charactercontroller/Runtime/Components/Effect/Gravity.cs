using System;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.TinyCharacterController.Effect
{
    /// <summary>
    /// A component that applies gravitational acceleration to the character.
    /// 
    /// It adds downward acceleration at the speed set in Gravity.
    /// The acceleration multiplier can be multiplied for each character.
    /// There is no acceleration while in contact with the ground.
    /// Events are executed at the timing of landing and takeoff.
    /// Components that move up and down, such as jumping, may manipulate this value.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(MenuList.MenuEffect + nameof(Gravity))]
    [RequireComponent(typeof(CharacterSettings))]
    [RequireInterface(typeof(IGroundContact))]
    [RenamedFrom("TinyCharacterController.Gravity")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Effect.Gravity")]
    public class Gravity : MonoBehaviour, 
        IGravity, 
        IGravityEvent,
        IEffect, 
        IEarlyUpdateComponent
    {
        /// <summary>
        /// Gravity multiplier.
        /// Multiply by the <see cref="Physics.gravity"/> value.
        /// </summary>
        [Tooltip("Gravity multiplier"), SerializeField, Range(0, 10)]
        private float _gravityScale = 1f;

        /// <summary>
        /// Event that invoke upon landing.
        /// </summary>
        [Header("events")]
        [SerializeField]
        private UnityEvent<float> _onLanding;

        /// <summary>
        /// Event that invoke when character leaves the ground.
        /// </summary>
        [SerializeField]
        private UnityEvent _onLeave;

        /// <summary>
        /// Event that invoke upon landing.
        /// </summary>
        public UnityEvent<float> OnLanding => _onLanding;
        
        /// <summary>
        /// Event that invoke when character leaves the ground.
        /// </summary>
        public UnityEvent OnLeave => _onLeave;

        /// <summary>
        /// Current fall speed.
        /// Negative value if falling, positive value if rising.
        /// </summary>
        public float FallSpeed => _velocity.y;

        /// <summary>
        /// Gravitational acceleration multiplier.
        /// 2 for a 2x faster fall, 0.5 for a lower gravity environment.
        /// </summary>
        public float GravityScale { get => _gravityScale; set => _gravityScale = value; }

        /// <summary>
        /// If True, character leaves the ground In this frame.
        /// 
        /// </summary>
        public bool IsLeaved { get; private set; }

        /// <summary>
        /// If True, character leaves the ground In this frame.
        /// </summary>
        public bool IsLanded { get; private set; }

        /// <summary>
        /// Current character status.
        /// Determines whether the character is in the air or on the ground.
        /// </summary>
        public State CurrentState => _state;

        /// <summary>
        /// Set the fall speed.
        /// For example, to stop the fall, specify Vector3.Zero.
        /// </summary>
        /// <param name="velocity">new velocity</param>
        public void SetVelocity(Vector3 velocity)
        {
            _velocity = velocity;
        }
        
        private IGroundContact _groundCheck;
        private CharacterSettings _settings;
        private State _state;
        private Vector3 _impactPower;
        private Vector3 _velocity;
        
        Vector3 IEffect.Velocity => _velocity;

        public void ResetVelocity()
        {
            SetVelocity(Vector3.zero);
        }

        private void Awake()
        {
            TryGetComponent(out _groundCheck);
            TryGetComponent(out _settings);
        }

 
        private void Start()
        {
            _state = IsGrounded ? State.Ground : State.Air;
        }

        private void OnDisable()
        {
            _velocity = Vector3.zero;
        }

        private bool IsGrounded => _groundCheck.IsOnGround && FallSpeed <= 0;
        
        private bool IsGroundedStrictly => _groundCheck.IsFirmlyOnGround && FallSpeed < 0;

        
        private void ApplyGravity(float deltaTime)
        {
            var fallSpeed = Physics.gravity * (_gravityScale * deltaTime);
            var angle = Vector3.Angle(Vector3.up, _groundCheck.GroundSurfaceNormal);
            if (angle > 45 && _velocity.y < 0 && _groundCheck.DistanceFromGround < _settings.Radius * 0.5f)
            {
                _velocity += Vector3.ProjectOnPlane(fallSpeed, _groundCheck.GroundSurfaceNormal);
            }
            else
                _velocity += fallSpeed;
        }

        private void CalculateGroundState()
        {
            var newState = GetCurrentState(_state);

            if (_state != newState)
            {
                switch (newState)
                {
                    case State.Ground:
                        IsLanded = true;
                        _onLanding?.Invoke(FallSpeed);
                        break;
                    case State.Air:
                        IsLeaved = true;
                        _onLeave?.Invoke();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _state = newState;
        }

        private State GetCurrentState(State preState)
        {
            return preState switch
            {
                State.Ground => IsGrounded ? State.Ground : State.Air,
                State.Air => IsGroundedStrictly ? State.Ground : State.Air,
                _ => throw new ArgumentOutOfRangeException(nameof(preState), preState, null)
            };
        }

        public enum State
        {
            Air = 0,
            Ground = 1,
        }

        void IEarlyUpdateComponent.OnUpdate(float deltaTime)
        {
            if (enabled == false)
                return;
            
            IsLeaved = false;
            IsLanded = false;

            
            ApplyGravity(deltaTime);
            CalculateGroundState();

            // If in contact with the ground, set acceleration to 0
            if( IsGroundedStrictly )
                _velocity = Vector3.zero;
        }

        int IEarlyUpdateComponent.Order => Order.Gravity;
    }
}
