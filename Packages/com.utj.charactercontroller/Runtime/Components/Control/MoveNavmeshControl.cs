using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Interfaces.Utility;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Unity.TinyCharacterController.Interfaces.Components;

namespace Unity.TinyCharacterController.Control
{

    /// <summary>
    /// Component for Navmesh-based movement of a character.
    ///
    /// Uses the component specified by <see cref="_agent"/> to perform a path search to
    /// the coordinates specified by <see cref="SetTargetPosition(Vector3)"/> and move the character
    /// in the context of <see cref="IMove"/>.
    /// 
    /// If MovePriority is high, the character moves on the shortest path set by NavmeshAgent.
    /// /// If TurnPriority is high, the character will turn in the direction of the destination.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(MoveNavmeshControl))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(CharacterSettings))]
    [RequireInterface(typeof(IBrain))]
    [RenamedFrom("TinyCharacterController.Control.MoveNavmeshControl")]
    [RenamedFrom("TinyCharacterController.MoveByNavigationControl")]
    public class MoveNavmeshControl : MonoBehaviour, 
        IMove, ITurn, IUpdateComponent,
        IComponentCondition
    {
        /// <summary>
        /// Agent used to control character movement.
        /// Used to calculate paths asynchronously.
        /// Also, the component used in this setting cannot be shared by multiple components.
        /// The Agent set here should be registered as a child object of the character.
        /// </summary>
        [SerializeField]
        private NavMeshAgent _agent;

        /// <summary>
        ///　Maximum character movement speed
        /// </summary>
        [Header("Settings")]
        [SerializeField]
        [FormerlySerializedAs("Speed")] 
        private float _speed = 4;

        /// <summary>
        /// Character turn speed.
        /// </summary>
        [FormerlySerializedAs("_turnSpeed")] [Range(-1, 50)]
        public int TurnSpeed = 8;

        /// <summary>
        /// Character move priority.
        /// </summary>
        [FormerlySerializedAs("_movePriority")] 
        [Header("movement and orientation")]
        public int MovePriority = 1;

        /// <summary>
        /// Character Turn Priority.
        /// </summary>
        [FormerlySerializedAs("_turnPriority")] 
        public int TurnPriority = 1;

        /// <summary>
        /// Callback when destination is reached
        /// </summary>
        public UnityEvent OnArrivedAtDestination;
        
        private ITransform _transform;
        private float _yawAngle;
        private Vector3 _moveVelocity;
        
        private void Awake()
        {
            TryGetComponent(out _transform);

            if (_agent == null)
            {
                var agent = new GameObject("agent", typeof(NavMeshAgent));
                agent.transform.SetParent(transform);
                agent.TryGetComponent(out _agent);
            }

            _agent.transform.localPosition = Vector3.zero;
            _agent.speed = _speed;
            _agent.updatePosition = false;
            _agent.updateRotation = false;
        }
#if UNITY_EDITOR

        private void Reset()
        {
            if (_agent == null)
            {
                var agent = new GameObject("Agent (NavigationControl)", typeof(NavMeshAgent));
                agent.transform.SetParent(transform, false);
                agent.TryGetComponent(out _agent);
                _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                _agent.speed = 0;
                _agent.acceleration = 0;
                _agent.stoppingDistance = 0;
                _agent.autoBraking = false;
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == false)
                return;

            var position = _agent.destination;
            var center = position + new Vector3(0, 1, 0);
            var cubeSize = new Vector3(0.5f, 2f, 0.5f);
            
            GizmoDrawUtility.DrawCube(center, cubeSize, Color.yellow);

            if (_agent.path.status == NavMeshPathStatus.PathComplete)
            {
                var corners = _agent.path.corners;
                if (corners.Length > 0)
                {
                    for (var i = 1; i < corners.Length; i++)
                    {
                        var start = corners[i - 1];
                        var next = corners[i];
                        Gizmos.DrawLine(start, next);
                    }
                }
            }
        }

#endif
        
        /// <summary>
        /// True if the character has reached the target point.
        /// </summary>
        public bool IsArrived { get; private set; } = true;

        /// <summary>
        /// Character movement speed
        /// </summary>
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                _agent.speed = _speed;
            }
        }
        
        /// <summary>
        /// Set a target point to move to.
        /// </summary>
        /// <param name="position">Target position</param>
        public void SetTargetPosition(Vector3 position)
        {
            _agent.isStopped = false;
            _agent.SetDestination(position);
            IsArrived = false;
        }

        /// <summary>
        /// Set the target point to move to.
        /// and, the character maintains the <see cref="distance"/> distance.
        /// Use when you do not necessarily want to move to the same coordinates as the target, for example in melee combat.
        /// </summary>
        /// <param name="position">Position of target</param>
        /// <param name="distance">distance from the target</param>.
        public void SetTargetPosition(Vector3 position, float distance)
        {
            var deltaPosition =  _transform.Position - position;
            deltaPosition.y = 0;
            var direction = deltaPosition.normalized * distance;
            SetTargetPosition(position + direction);
        }
        
        /// <summary>
        /// Current Speed.
        /// </summary>
        public float CurrentSpeed => IsArrived ? 0 : _speed;

        int IPriority<IMove>.Priority => MovePriority;

        /// <summary>
        /// Movement vector
        /// </summary>
        public Vector3 MoveVelocity => _moveVelocity;

        int IPriority<ITurn>.Priority => TurnPriority;

        int ITurn.TurnSpeed => TurnSpeed;

        float ITurn.YawAngle => _yawAngle;
        
        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            using var profiler = new ProfilerScope(nameof(MoveNavmeshControl));

            if (_agent.pathPending )
                return;
            var distance = _agent.remainingDistance;
            
            if (_agent.isOnOffMeshLink)
            {
                var deltaPosition =   _agent.currentOffMeshLinkData.endPos - _transform.Position;
                deltaPosition.y = 0;
                var direction = deltaPosition.normalized;
                
                if( deltaPosition.sqrMagnitude < 1)
                    _agent.CompleteOffMeshLink();
                
                _moveVelocity = direction * _speed;
                _agent.nextPosition = _transform.Position + _moveVelocity * deltaTime;
                
            }
            else
            {
                var deltaPosition =   _agent.steeringTarget - _transform.Position;
                deltaPosition.y = 0;
                
                var isMoving = distance  - deltaTime * _speed >  0;
                if (isMoving && IsArrived == false)
                {
                    _yawAngle = Vector3.SignedAngle(Vector3.forward, deltaPosition, Vector3.up);
                }
                else
                {
                    // Process stopped because speed is 0 or target point has been reached.
                    if (IsArrived == false )
                    {
                        OnArrivedAtDestination?.Invoke();
                        IsArrived = true;
                        _agent.isStopped = true;
                    }
                }
                // var speed = _agent.desiredVelocity.magnitude;
                var currentSpeed = distance < Speed * deltaTime ? distance / deltaTime : Speed;
                // _moveVelocity = direction * currentSpeed;
                _moveVelocity = _agent.desiredVelocity.normalized * currentSpeed;
                _agent.nextPosition = _transform.Position + _moveVelocity * deltaTime;
                
            }
        }

        int IUpdateComponent.Order => Order.Control;
        
        void IComponentCondition.OnConditionCheck(List<string> messageList)
        {
            if (_agent != null && _agent.transform.parent != transform)
            {
                messageList.Add("Please place the agent as a child object of the character.");
            }
        }

    }
}
