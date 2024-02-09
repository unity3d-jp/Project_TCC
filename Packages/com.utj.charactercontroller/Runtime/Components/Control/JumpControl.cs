using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// This component controls the behavior of jumping.
    /// When the Jump method is executed, it controls the Gravity and moves upwards.
    /// The Priority only works during the jump operation.
    /// The TurnPriority is only effective when the movement is set to the horizontal direction.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(JumpControl))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RequireInterface(typeof(IGravity))]
    [RequireInterface(typeof(IGroundContact))]
    [RequireInterface(typeof(IOverheadDetection))]
    [RenamedFrom("TinyCharacterController.JumpControl")]
    [RenamedFrom("TinyCharacterController.Control.JumpControl")]
    public class JumpControl : MonoBehaviour, 
        IUpdateComponent, 
        IMove, 
        ITurn
    {
        /// <summary>
        /// Jump height.
        /// </summary>
        [FormerlySerializedAs("_jumpHeight")]
        [Unity.VisualScripting.RenamedFrom("_jumpHeight")]
        [Header("Settings")]
        [Tooltip("JumpHeight"), SerializeField]
        public float JumpHeight = 3;

        /// <summary>
        /// Number of jumps in the air
        /// </summary>
        [FormerlySerializedAs("_maxAerialJumpCount")] 
        [Tooltip("Areal Jump Count."), SerializeField]
        public int MaxAerialJumpCount = 0;

        /// <summary>
        /// Aerodynamic drag
        /// </summary>
        public float Drag = 0;
        
        /// <summary>
        /// The speed at which the character will change direction.
        /// If this value is -1, the character will change direction immediately.
        /// </summary>
        [SerializeField]
        [Range(-1, 50)]
        private int _turnSpeed;

        /// <summary>
        /// The time that <see cref="Jump"/> can be entered ahead of time.
        /// If a jump is possible within the time, it will be automatically jumped.
        /// </summary>
        [Header("Input Settings")]
        [SerializeField, Range(0, 1)]
        private float _standbyTime = 0.05f;

        /// <summary>
        /// Move Priority
        /// </summary>
        [Header("Priority Settings")]
        public int MovePriority;

        /// <summary>
        /// Turn Priority
        /// </summary>
        public int TurnPriority;
        
        /// <summary>
        /// Callback called when a jump is requested.
        /// Called if the jump is feasible, regardless of <see cref="IsAllowJump"/>.
        /// </summary>
        [Header("Callbacks")]
        public UnityEvent OnReadyToJump;
        
        /// <summary>
        /// Callback called just before jumping.
        /// </summary>
        public UnityEvent OnJump;
        
        /// <summary>
        /// Callback called just before jumping.
        /// </summary>
        public int AerialJumpCount { get; private set; } = 0;
        
        /// <summary>
        /// True if a jump starts on this frame
        /// </summary>
        public bool IsJumpStart { get; private set; } = false;

        /// <summary>
        /// True if a jump starts on this frame
        /// </summary>
        public bool IsReadyToJumpStart { get; private set; }

        /// <summary>
        /// True if ready to jump
        /// </summary>
        public bool IsReadyToJump { get; private set; } = false;

        /// <summary>
        /// True if jumping
        /// </summary>
        public bool IsJumping { get; private set; } = false;

        /// <summary>
        /// The elapsed time between when the jump is ready and when Allow becomes True.
        /// Use this when you want to change the intensity of the jump with the delay time.
        /// </summary>
        public float TimeSinceReady => IsReadyToJump ? _readyTime : 0;
        

        /// <summary>
        /// The elapsed time between when the jump is ready and when Allow becomes True.
        /// Use this when you want to change the intensity of the jump with the delay time.
        /// </summary>
        public bool IsAllowJump { get; set; } = true;

             
        /// <summary>
        /// Direction of jump.
        /// Usually use up (0,1,0).
        /// If you are doing a wall jump or dash jump, set a vector in that direction.
        /// This setting does not take the character's direction into account.
        /// This value is normalized when used.
        /// </summary>
        public Vector3 JumpDirection { get; set; } = Vector3.up;

        // components
        private IGroundContact _groundCheck;
        private IGravity _gravity;
        private IOverheadDetection _head;
        private CharacterSettings _settings;
        
        private float _requestJump = -1; // Time at which the jump was requested. Request invalid state at -1
        private bool _requestJumpIncrement = false; // Determines if the number of jumps should be increased.
        private float _leaveTime = 0; // Time away from the ground. Used to determine that the ground is still ground even if it leaves the _standbyTime time.
        private float _readyTime = 0; // Time after which a jump is possible.
        private Vector3 _velocity;
        private float _yawAngle;

        private void Awake()
        {
            TryGetComponent(out _gravity);
            TryGetComponent(out _groundCheck);
            TryGetComponent(out _head);
            TryGetComponent(out _settings);
        }
        
        /// <summary>
        /// Requests a jump and jumps at the timing when a jump becomes possible.
        /// If a request comes in at a timing when jumping is not possible, maintain the jump request for the time of _standbyTime. (In other words, the jump is processed at the moment it becomes possible to jump.)
        /// Note that it does not jump immediately.
        /// </summary>
        /// <param name="incrementJumpCount">Count the number of jumps</param>.
        [RenamedFrom("RequestJump")]
        public void Jump(bool incrementJumpCount = true)
        {
            _requestJumpIncrement = incrementJumpCount;
            _requestJump = Time.time + _standbyTime;
        }

        /// <summary>
        /// Forces a jump, ignoring AllowJump and JumpCount.
        /// This process is executed immediately.
        /// </summary>
        /// <param name="incrementJumpCount">The number of jumps is +1. </param>
        [RenamedFrom("Jump")]
        public void ForceJump(bool incrementJumpCount = true)
        {
            // +1 to the number of jumps if already in the air; if not off the ground,
            // the number of jumps in the air is not counted.
            if ( incrementJumpCount && _leaveTime > 0)
                AerialJumpCount += 1;
            
            IsJumpStart = true;
            OnJump?.Invoke();
            
            // Initialize
            _requestJump = -1;
            IsReadyToJump = false;
            _readyTime = 0;


            // Calculate jump strength based on jump direction and jump force.
            // Vectors are for XZ axis only; use Gravity values for Y axis.
            var direction = JumpDirection.normalized;
            _velocity = new Vector3(direction.x, 0, direction.z) * HeightToJumpPower;
            _gravity.SetVelocity( new Vector3(0, direction.y, 0) * HeightToJumpPower );

            // If the velocity is not zero, calculate the yaw angle
            if (_velocity != Vector3.zero)
                _yawAngle = Vector3.SignedAngle(Vector3.forward, _velocity, Vector3.up);
            
            IsJumping = true;
        }

        /// <summary>
        /// Reset the number of jumps, the vector, and the decision during a jump
        /// </summary>
        public void ResetJump()
        {
            AerialJumpCount = 0;
            IsJumping = false;
            _velocity = Vector3.zero;
            IsReadyToJump = false;
            IsReadyToJumpStart = false;
        }

        private void ReadyJump()
        {
            IsReadyToJump = true;
            IsReadyToJumpStart = true;
            OnReadyToJump?.Invoke();
        }

        private void CalculateContactEnvironment()
        {
            // If anything comes in contact with the head, the speed is reduced to zero.
            if ( _head.IsHeadContact && _gravity.FallSpeed > 0)
            {
                _gravity.SetVelocity(Vector3.zero);
            }

            // Set the number of jumps to 0 when you land.
            if (_groundCheck.IsFirmlyOnGround && _gravity.FallSpeed <= 0)
            {
                ResetJump();
            }
        }
        
        private bool IsCanJump => _groundCheck.IsFirmlyOnGround || AerialJumpCount < MaxAerialJumpCount;

        private float HeightToJumpPower => Mathf.Sqrt(JumpHeight * -2.0f * _gravity.GravityScale * Physics.gravity.y);   

        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            // Initialize
            IsJumpStart = false;
            IsReadyToJumpStart = false;
            
            // Calculates time off the ground.　Used for judging aerial jumps.
            _leaveTime = (_groundCheck.IsFirmlyOnGround && _gravity.FallSpeed <= 0) ? 0 : _leaveTime + deltaTime;
            if( IsReadyToJump )
                _readyTime += deltaTime;

            // Damping jump vectors
            _velocity = _leaveTime == 0 ? Vector3.zero : Vector3.Lerp(_velocity, Vector3.zero, Drag * Time.deltaTime);
            
            CalculateContactEnvironment();
            
            // Prepare to jump if possible
            if (Time.time < _requestJump && IsCanJump && IsReadyToJump == false)
            {
                ReadyJump();
                _readyTime = 0;
            }

            // Jump if jumping is allowed
            if (IsReadyToJump && IsAllowJump)
            {
                ForceJump(_requestJumpIncrement);
            }
        }

        int IUpdateComponent.Order => Order.Control;

        int IPriority<IMove>.Priority => IsJumping ?  MovePriority : 0;

        Vector3 IMove.MoveVelocity => _velocity ;

        int IPriority<ITurn>.Priority => IsJumping && _velocity.sqrMagnitude > 0  ? TurnPriority : 0;

        int ITurn.TurnSpeed => _turnSpeed;

        float ITurn.YawAngle => _yawAngle;

        public Vector3 Velocity => _velocity;
        
        private void OnDrawGizmosSelected()
        {
            const float cursorRadius = 0.1f;
            if( _settings == null)
                TryGetComponent(out _settings);

            var position = transform.position;
            var width = _settings.Radius;

            if (_leaveTime > 0)
            {
                var characterCenter = position + new Vector3(0, _settings.Height * 0.5f, 0);
                var velocityOffset = _velocity + new Vector3(0, _gravity.FallSpeed, 0);
                var velocityPosition = characterCenter + velocityOffset * 0.3f;
                Gizmos.DrawLine(characterCenter, velocityPosition);
                GizmoDrawUtility.DrawSphere(velocityPosition, cursorRadius, Color.blue, 1);
            }
            else
            {
                var top = position + new Vector3(0, JumpHeight, 0);
                var size = new Vector3(_settings.Radius, 0, _settings.Radius);
                GizmoDrawUtility.DrawCube(top, size, Color.blue);
                Gizmos.DrawLine(position, top);
            }
        }
    }
}