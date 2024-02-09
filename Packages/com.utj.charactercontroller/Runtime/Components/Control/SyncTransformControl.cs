using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// A component that fixes the character's position to match a specific Transform's position.
    /// When <see cref="MovePriority"/> is high, the character transitions to the position that matches the <see cref="Target"/> coordinate.
    /// Also, if <see cref="TurnPriority"/> is high, it updates the character's orientation.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(SyncTransformControl))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RenamedFrom("TinyCharacterController.FixPositionControl")]
    [RenamedFrom("TinyCharacterController.Control.FixPositionControl")]
    [RenamedFrom("Utj.TinyCharacterController.Control.FixPositionControl")]
    public class SyncTransformControl : MonoBehaviour, 
        IUpdateComponent,
        IMove,
        ITurn,
        IPriorityLifecycle<IMove>,
        IPriorityLifecycle<ITurn>
    {
        /// <summary>
        /// The Transform to synchronize the position with.
        /// </summary>
        [SerializeField, ReadOnlyOnRuntime]
        [Header("Settings")]
        private Transform _targetTransform;
        
        /// <summary>
        /// The time it takes for the component to reach the target point when Move priority is higher than other components.
        /// After the specified time has passed, the component's position matches the target.
        /// </summary>
        public float MoveTransitionTime ;

        /// <summary>
        /// The time it takes for the component to change its orientation to the target direction when Turn priority is higher than other components.
        /// After the specified time has passed, the component's orientation matches the target.
        /// </summary>
        public float TurnTransitionTime;
        
        /// <summary>
        /// The movement priority of the character.
        /// </summary>
        [Header("Priority")]
        public int MovePriority;
        
        /// <summary>
        /// The priority to change the character's orientation.
        /// </summary>
        public int TurnPriority;

        // components.
        private IWarp _warp;
        private ITransform _transform;
        private bool _hasTargetTransform;
        
        // settings.
        private Vector3 _moveVelocity;
        private float _yawAngle;
        private Vector3 _deltaPosition;
        
        // transition settings.
        private Vector3 _startPosition = Vector3.zero;
        private float _startTurn;
        private float _currentMoveTransitionTime;
        private float _currentTurnTransitionTime;

        private void Awake()
        {
            GatherComponents();
            UpdateTransform();
        }

        private void OnValidate()
        {
            // clamp transition time amounts.
            TurnTransitionTime = Mathf.Max(0, TurnTransitionTime);
            MoveTransitionTime = Mathf.Max(0, MoveTransitionTime);
        }

        /// <summary>
        /// Collect the components.
        /// </summary>
        private void GatherComponents()
        {
            TryGetComponent(out _transform);
            TryGetComponent(out _warp);
        }

        /// <summary>
        /// The Transform to synchronize the character's position.
        /// </summary>
        [AllowsNull]
        public Transform Target
        {
            get => _targetTransform;
            set
            {
                var isChangeValue = _targetTransform != value;
                _targetTransform = value;
                
                if (isChangeValue)
                {
                    UpdateTransform();
                    InitializeMoveTransition();
                    InitializeTurnTransition();
                }
            }
        }

        /// <summary>
        /// Update the Transform settings.
        /// </summary>
        private void UpdateTransform()
        {
            _hasTargetTransform = _targetTransform != null;
        }

        /// <summary>
        /// The character's movement vector.
        /// </summary>
        public Vector3 Velocity => _moveVelocity;
        
        /// <summary>
        /// The character's current movement speed.
        /// </summary>
        public float CurrentSpeed => _moveVelocity.magnitude;

        /// <summary>
        /// Execution order.
        /// </summary>
        int IUpdateComponent.Order => Order.Control;

        /// <summary>
        /// The movement priority of the character.
        /// If there is no target, the priority becomes 0.
        /// </summary>
        int IPriority<IMove>.Priority => _hasTargetTransform ?  MovePriority : 0;

        /// <summary>
        /// Movement speed.
        /// </summary>
        Vector3 IMove.MoveVelocity => _moveVelocity;
        
        /// <summary>
        /// The priority to update the character's orientation.
        /// If there is no target, the priority becomes 0.
        /// </summary>
        int IPriority<ITurn>.Priority => _hasTargetTransform ? TurnPriority : 0;

        /// <summary>
        /// Turning speed.
        /// </summary>
        int ITurn.TurnSpeed => -1;
        
        /// <summary>
        /// The character's orientation.
        /// </summary>
        float ITurn.YawAngle => _yawAngle;
        
        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            if (NullCheckTargetTransform()) 
                return;

            _deltaPosition =  _targetTransform.position - _transform.Position;
        }

        void IPriorityLifecycle<ITurn>.OnAcquireHighestPriority()
        {
            InitializeTurnTransition();
        }

        void IPriorityLifecycle<ITurn>.OnLoseHighestPriority()　{ }

        void IPriorityLifecycle<ITurn>.OnUpdateWithHighestPriority(float deltaTime)
        {
            if (TurnTransitionTime > 0 && _currentTurnTransitionTime <= TurnTransitionTime)
            {
                // In transition.
                _currentTurnTransitionTime += deltaTime;
                _yawAngle = CalculateTurnTransitionAngle(_currentTurnTransitionTime);
            }
            else
            {
                // If the above conditions are not met, use the orientation of the TargetTransform.
                _yawAngle = _targetTransform.eulerAngles.y;
            }
        }

        void IPriorityLifecycle<IMove>.OnAcquireHighestPriority()
        {
            InitializeMoveTransition();
        }

        void IPriorityLifecycle<IMove>.OnLoseHighestPriority()　{}
                
        void IPriorityLifecycle<IMove>.OnUpdateWithHighestPriority(float deltaTime)
        {
            if (MoveTransitionTime > 0 && _currentMoveTransitionTime <= MoveTransitionTime)
            {
                // In transition.
                _currentMoveTransitionTime += deltaTime;
                var targetPosition = CalculateMoveTransitionPosition(_currentMoveTransitionTime);

                _moveVelocity = (_transform.Position - targetPosition) / deltaTime;
                _warp.Move(targetPosition);
            }
            else
            {
                // Calculate the vector and update the position in a way that matches the target's position.
                _moveVelocity = _deltaPosition / deltaTime;
                _warp.Move(_targetTransform.position);
            }
        }

        /// <summary>
        /// Check if the target object is Null, update HasTargetTransform, and abort the process if it is Null.
        /// </summary>
        /// <returns>True if the target is Null.</returns>
        private bool NullCheckTargetTransform()
        {
            // The object is not registered, so return True.
            if (_hasTargetTransform == false )
                return true;

            // The object has been deleted, so unregister it and return True.
            if (_targetTransform == null)
            {
                _hasTargetTransform = false;
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Calculate the position based on the elapsed time since the transition started.
        /// </summary>
        /// <param name="timeSinceTransitionStarted">Elapsed time</param>
        /// <returns>The position where the character should be</returns>
        private Vector3 CalculateMoveTransitionPosition(float timeSinceTransitionStarted)
        {
            var amount = timeSinceTransitionStarted / MoveTransitionTime;
            var targetPosition = Vector3.Lerp(_startPosition, _targetTransform.position, amount);
            return targetPosition;
        }

        /// <summary>
        /// Calculate the angle based on the elapsed time since the transition started.
        /// </summary>
        /// <param name="timeSinceTransitionStarted">Elapsed time</param>
        /// <returns>The angle to which the character should turn</returns>
        private float CalculateTurnTransitionAngle(float timeSinceTransitionStarted)
        {
            var amount = timeSinceTransitionStarted / TurnTransitionTime;
            var angle = Mathf.LerpAngle(_startTurn, _targetTransform.eulerAngles.y, amount);
            return angle;
        }

        /// <summary>
        /// Initialize when the transition starts.
        /// Does nothing if <see cref="MoveTransitionTime"/> is 0.
        /// </summary>
        private void InitializeMoveTransition()
        {
            if (MoveTransitionTime > 0 == false)
                return;

            _currentMoveTransitionTime = 0;
            _startPosition = _transform.Position;
        }

        /// <summary>
        /// Initialize when the transition starts.
        /// Does nothing if <see cref="TurnTransitionTime"/> is 0.
        /// </summary>
        private void InitializeTurnTransition()
        {
            if (TurnTransitionTime > 0 == false)
                return;

            _currentTurnTransitionTime = 0;
            _startTurn = _transform.Rotation.eulerAngles.y;
        }

    }
}
