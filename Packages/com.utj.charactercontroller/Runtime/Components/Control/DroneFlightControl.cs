using UnityEngine;
using UnityEngine.Serialization;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// This component moves the character independently of the terrain.
    ///  Move the character with <see cref="MoveHorizontal"/> or <see cref="MoveVertical"/>.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(DroneFlightControl))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.DroneFlightController")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Control.DroneFlightControl")]
    public class DroneFlightControl : MonoBehaviour, 
        ITurn, 
        IMove, 
        IUpdateComponent
    {
        /// <summary>
        ///  Max Speed.
        /// </summary>
        [Header("Settings")]
        public float Speed = 5;
        
        /// <summary>
        /// acceleration amount.
        /// </summary>
        public float Accel = 10;

        /// <summary>
        /// Brake power amount.
        /// </summary>
        public float Brake = 1;
        
        /// <summary>
        /// Speed of turn.
        /// </summary>
        [Range(-1, 50)]
        public int TurnSpeed = 32;

        /// <summary>
        /// If True, faces in the direction entered.
        /// If False, faces the direction of movement.
        /// </summary>
        public LookType LookForward = LookType.MoveDirection;
        
        /// <summary>
        /// Ignore the pitch of the camera and only move horizontal.
        /// </summary>
        [FormerlySerializedAs("_isOnlyHorizontal")] 
        public bool IsOnlyHorizontal = false;
        
        /// <summary>
        /// Move Priority.
        /// </summary>
        [Header("Priorities")]
        public int MovePriority = 1;
        
        /// <summary>
        /// Turn Priority.
        /// </summary>
        public int TurnPriority = 1;

        private float _yawAngle;
        private Vector3 _moveVelocity;
        private Vector2 _inputHorizontal;
        private float _inputVertical;
        private CharacterSettings _settings;
        
        /// <summary>
        /// movement direction value.
        /// </summary>
        public Vector3 MoveDirection { get; private set; }

        /// <summary>
        /// Current movement speed. If priority is 0, the movement speed is also 0.
        /// </summary>
        public float CurrentSpeed => MovePriority > 0 ? _moveVelocity.sqrMagnitude : 0;

        private void Awake()
        {
            TryGetComponent(out _settings);
        }

        /// <summary>
        /// Move horizontal direction.
        /// The direction of movement is compensated by the camera direction.
        /// </summary>
        /// <param name="input">Direction of movement on screen, where X is sideways and Y is forward/backward</param>
        public void MoveHorizontal(Vector2 input)
        {
            _inputHorizontal = input;
        }

        /// <summary>
        /// Move vertically.
        /// </summary>
        /// <param name="input">Move up on a plus and down on a minus.</param>
        public void MoveVertical(float input)
        {
            _inputVertical = input;
        }
        
        /// <summary>
        /// If True, MoveHorizontal or MoveVertical requests a move.
        /// </summary>
        public bool HasRequestMove { get; private set; }

        int IPriority<ITurn>.Priority => _inputHorizontal != Vector2.zero ? TurnPriority : 0;

        int ITurn.TurnSpeed => TurnSpeed;

        float ITurn.YawAngle => _yawAngle;

        int IPriority<IMove>.Priority => MovePriority;

        Vector3 IMove.MoveVelocity => _moveVelocity;

        public Vector3 Velocity
        {
            get => _moveVelocity;
            set
            {
                _moveVelocity = value;
                MoveDirection = value.normalized;
            }
        }
        
        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            Vector3 direction;
            var input = new Vector3(_inputHorizontal.x, _inputVertical, _inputHorizontal.y);
            HasRequestMove = input.sqrMagnitude > 0;
            
            if (IsOnlyHorizontal == false)
            {
                // The direction of movement is compensated by the camera orientation.
                // However, the Y axis is added separately to avoid the vertical direction of movement being skewed by the camera orientation.
                if (HasRequestMove)
                {
                    input.y= 0f;
                    direction = _settings.CameraTransform.rotation * new Vector3(_inputHorizontal.x, 0, _inputHorizontal.y);
                    direction.y += _inputVertical;
                    direction.Normalize();
                }
                else
                {
                    direction = Vector3.zero;
                }
            }
            else
            {
                // Ignore the pitch of the camera and only move horizontal.
                // If it has not moved, skip the rotation process.
                direction = HasRequestMove
                    ? _settings.CameraYawRotation * input 
                    : Vector3.zero;
            }

            var maxDelta = deltaTime * (HasRequestMove ? Accel : Brake);
            MoveDirection = direction;
            _moveVelocity = Vector3.MoveTowards(_moveVelocity, direction * Speed, maxDelta);

            if (LookForward == LookType.InputDirection)
            {
                // Calculate angle, direction of movement, and movement vector.
                _yawAngle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
            }
            else
            {
                if (_moveVelocity != Vector3.zero)
                    _yawAngle = Vector3.SignedAngle(Vector3.forward, _moveVelocity.normalized, Vector3.up);
            }
        }

        int IUpdateComponent.Order => Order.Control;

        /// <summary>
        /// Specifies the direction in which the character faces
        /// </summary>
        public enum LookType
        {
            MoveDirection = 0, // look in the direction the character is moving
            InputDirection = 1 // look in the direction you input
        }
    }
}
