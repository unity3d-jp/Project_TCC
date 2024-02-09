using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// Component to move a character to a specified position.
    /// Moves the character to the coordinates specified by <see cref="SetTargetPosition(Vector3)"/>.
    /// Operates within the context of <see cref="IMove"/>.
    ///
    /// If <see cref="TurnPriority"/> is high, the character will face the movement direction.
    /// If <see cref="MovePriority"/> is high, the character will move toward the specified position.
    /// If <see cref="_ignoreYAxis"/> is set to false, the character will move along the terrain.
    /// This movement does not avoid obstacles.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(MovePositionControl))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(CharacterSettings))]
    [RequireInterface(typeof(IBrain))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Control.MovePositionControl")]
    [RenamedFrom("TinyCharacterController.MovePositionControl")]
    public class MovePositionControl : MonoBehaviour, 
        IMove, ITurn, IUpdateComponent
    {
        /// <summary>
        /// The maximum movement speed of the character.
        /// </summary>
        [Header("Move Settings")]
        [FormerlySerializedAs("_speed")] 
        public float Speed = 4;

        /// <summary>
        /// The rotation speed of the character.
        /// </summary>
        [FormerlySerializedAs("_turnSpeed")]
        [Range(-1, 50)]
        public int TurnSpeed = 3;

        /// <summary>
        /// The angle of slopes that the character can move on.
        /// If the terrain has an angle less than this, the character moves along it.
        /// Only considered if <see cref="_ignoreYAxis"/> is disabled.
        /// </summary>
        [FormerlySerializedAs("_slope")]
        [FormerlySerializedAs("_angle")] 
        [Tooltip("Angle of slopes that can be traversed"), SerializeField] 
        private float _maxSlope = 45;
        
        /// <summary>
        /// The movement priority of the character.
        /// </summary>
        [FormerlySerializedAs("_movePriority")]
        [Header("Priorities")]
        public int MovePriority = 1;
        
        /// <summary>
        /// The rotation priority of the character.
        /// </summary>
        [FormerlySerializedAs("_turnPriority")] [SerializeField]
        public int TurnPriority = 1;

        /// <summary>
        /// Callback when the destination is reached.
        /// </summary>
        [Header("Callbacks")]
        public UnityEvent OnArrivedAtDestination;

        /// <summary>
        /// A boolean indicating whether the character has reached the target destination.
        /// </summary>
        public bool IsArrived { get; private set; } = true;

        /// <summary>
        /// Get the current movement speed of the character.
        /// </summary>
        public float CurrentSpeed => IsArrived ? 0 : _currentSpeed;
        
        public float DistanceToTargetPosition { get; private set; }
        
        // Components.
        private IGroundContact _groundCheck;
        private ITransform _transform;
        
        private Vector3 _moveVelocity;
        private Vector3 _targetPosition;
        private Vector3 _direction;
        private float _brakeDistance;
        private float _yawAngle;
        private float _currentSpeed;
        private bool _hasGroundCheck;
        private bool _ignoreYAxis;
        private int _cycleAround;

        private void OnValidate()
        {
            MovePriority = Mathf.Max(0, MovePriority);
            TurnPriority = Mathf.Max(0, TurnPriority);
            _maxSlope = Mathf.Max(0, _maxSlope);
            TurnSpeed = Mathf.Max(0, TurnSpeed);
            Speed = Mathf.Max(0, Speed);
        }

        /// <summary>
        /// Set the target point for movement.
        /// </summary>
        /// <param name="position">Target position</param>
        public void SetTargetPosition(Vector3 position)
        {
            SetTargetPosition(position, false);
        }
        
        /// <summary>
        /// Set the target point for movement.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="ignoreYAxis">Ignore terrain</param>
        public void SetTargetPosition(Vector3 position, bool ignoreYAxis)
        {
            SetTargetPosition(position, 0, ignoreYAxis);
        }

        /// <summary>
        /// Set the target point for movement while maintaining a specified distance from the character.
        /// Used, for example, when the character does not need to move to the exact coordinate, such as during combat.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="distance">Stop distance</param>
        public void SetTargetPosition(Vector3 position, float distance)
        {
            SetTargetPosition(position, distance, false);
        }

        /// <summary>
        /// Set the target point for movement while maintaining a specified distance from the character.
        /// Used, for example, when the character does not need to move to the exact coordinate, such as during combat.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="distance">Stop distance</param>
        /// <param name="ignoreYAxis">Ignore terrain</param>
        public void SetTargetPosition(Vector3 position, float distance, bool ignoreYAxis)
        {
            SetTargetPosition(position, distance, ignoreYAxis, 0);
        }
        

        /// <summary>
        /// Set the target point for movement while maintaining a specified distance from the character.
        /// Used, for example, when the character does not need to move to the exact coordinate, such as during combat.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="distance">Stop distance</param>
        /// <param name="cycleAround">Cycle around the object</param>
        public void SetTargetPosition(Vector3 position, float distance, int cycleAround)
        {
            SetTargetPosition(position, distance, false, cycleAround);
        }

        /// <summary>
        /// Set the target point for movement while maintaining a specified distance from the character.
        /// Used, for example, when the character does not need to move to the exact coordinate, such as during combat.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="distance">Stop distance</param>
        /// <param name="ignoreYAxis">Ignore terrain</param>
        /// <param name="cycleAround">Cycle around the object</param>
        public void SetTargetPosition(Vector3 position, float distance, bool ignoreYAxis, int cycleAround)
        {
            var deltaPosition =  position - _transform.Position; 
            if( ignoreYAxis == false )
                deltaPosition.y = 0;

            // Don't move if no movement is needed.
            if (deltaPosition == Vector3.zero)
                return;

            var direction = deltaPosition.normalized;
            _brakeDistance = distance;
            _direction = direction;
            _ignoreYAxis = ignoreYAxis;
            _targetPosition = position ;
            IsArrived = false;
            _cycleAround = cycleAround;

        }
        
        /// <summary>
        /// Get a list of components.
        /// </summary>
        private void GatherComponents()
        {
            TryGetComponent(out _transform);
            _hasGroundCheck = TryGetComponent(out _groundCheck);
        }
        
        private void Awake()
        {
            GatherComponents();

            var trs = transform;
            _targetPosition = trs.position;
            _yawAngle = CalculateYawAngle(trs.forward);
        }

        int IPriority<IMove>.Priority => IsArrived == false ? MovePriority : 0;
        int IPriority<ITurn>.Priority => IsArrived == false ?  TurnPriority : 0;
        Vector3 IMove.MoveVelocity => _moveVelocity;
        int ITurn.TurnSpeed => TurnSpeed;
        float ITurn.YawAngle => _yawAngle;

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == false)
                return;

            var center = _targetPosition;
            center.y = _ignoreYAxis == false ? _transform.Position.y + 1 : center.y + 1;

            var cubeSize = new Vector3(0.5f, 2f, 0.5f);
            
            // Draw a line from the current position to the target position.
            Gizmos.DrawLine(transform.position, _targetPosition);
            
            // Represent the target position with a cube.
            GizmoDrawUtility.DrawCube(center, cubeSize, Color.yellow);
        }

        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            using var profiler = new ProfilerScope(nameof(MovePositionControl));
            
            // Adjust the target position based on the brake distance.
            var targetPosition = CalculateTargetPosition();

            // Get the difference, distance, and direction between the current position and the target position.
            var delta = DeltaVelocity(_transform.Position, targetPosition);
            var distanceToTarget = delta.magnitude;
            var direction = delta.normalized;

            DistanceToTargetPosition = distanceToTarget;
            
            // Determine if the character is moving based on the distance to the target.
            var isMoving = distanceToTarget - deltaTime * Speed > 0 ;
            
            if (isMoving && IsArrived == false)
                CalculateMove(deltaTime, distanceToTarget, direction);
            else
                CalculateArrive();
        }

        /// <summary>
        /// Calculate the character's movement.
        /// </summary>
        /// <returns>The target position</returns>
        private Vector3 CalculateTargetPosition()
        {
            // If not moving, use the current position.
            if (Speed <= 0)
                return _transform.Position;

            // There is no correction in position due to _brakeDistance or CycleAround, so use the target position.
            if (_cycleAround == 0 && Mathf.Approximately(0, _brakeDistance))
                return _targetPosition;

            var offset = new Vector3(_direction.x, 0, _direction.z) * -_brakeDistance;

            // When not moving on a circular path, consider only the offset position.
            if (_cycleAround == 0)
                return offset + _targetPosition;

            // Consider both _brakeDistance and the direction of rotation.
            var cycleAround = Mathf.Clamp(_cycleAround, -1, 1);
            var turn = Quaternion.AngleAxis(Speed / _brakeDistance * cycleAround, Vector3.up);
            return turn * offset + _targetPosition;
        }

        /// <summary>
        /// Processing when arriving at the destination.
        /// </summary>
        private void CalculateArrive()
        {
            // The parameters are updated before the event to allow
            // for the possibility of SetTarget being executed in OnArrivedAtDestination.
            // The decision to execute the event is made before the parameters are updated.
            var needInvokeEvent = IsArrived == false;

            // Update parameters.
            _currentSpeed = 0;
            IsArrived = true;
            _moveVelocity = Vector3.zero;

            // Trigger the event only once when arriving at the target position.
            if (needInvokeEvent)
                OnArrivedAtDestination?.Invoke();
        }

        /// <summary>
        /// Processing while in motion.
        /// </summary>
        /// <param name="deltaTime">Delta time</param>
        /// <param name="distanceToTarget">Distance to the target position</param>
        /// <param name="direction">Direction of movement</param>
        private void CalculateMove(float deltaTime, float distanceToTarget, Vector3 direction)
        {
            // Limit the speed to ensure the character does not move beyond the specified distance.
            _currentSpeed = distanceToTarget < Speed * deltaTime ? distanceToTarget / deltaTime : Speed;
            _yawAngle = CalculateYawAngle(direction);

            
            if (_ignoreYAxis == false)
            {
                // Generate the character's movement vector to align with the terrain.
                var normal = _hasGroundCheck ? _groundCheck.GroundSurfaceNormal : Vector3.up;
                normal = Vector3.Angle(Vector3.up, normal) < _maxSlope ? normal : Vector3.up;
                direction = Vector3.ProjectOnPlane(direction, normal);
                _moveVelocity = direction * _currentSpeed;
            }
            else
            {
                // Generate the movement vector toward the target position.
                _moveVelocity = direction * _currentSpeed;
            }
        }

        /// <summary>
        /// Calculate the angle of the character's orientation excluding the Y-axis.
        /// </summary>
        /// <param name="direction">Movement direction</param>
        /// <returns>Character's orientation</returns>
        private static float CalculateYawAngle(Vector3 direction)
        {
            direction.y = 0;
            return Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        }

        /// <summary>
        /// Calculate the movement vector.
        /// Y-axis is disabled if <see cref="_ignoreYAxis"/> is enabled.
        /// </summary>
        /// <param name="from">Starting position</param>
        /// <param name="to">End position</param>
        /// <returns>Vector</returns>
        private Vector3 DeltaVelocity(in Vector3 from, in Vector3 to)
        {
            var delta = to - from;
            if( _ignoreYAxis == false)
                delta.y = 0;
            return delta;
        }

        int IUpdateComponent.Order => Order.Control;
    }
}
