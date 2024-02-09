using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Modifier;
using Unity.TinyCharacterController.Smb;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// Movement using root motion.
    /// This component is only active on frames when <see cref="WorkThisFrame()"/> is executed.<br/>
    /// Intended for use with <see cref="AnimatorMoveBehaviour"/>.
    /// The priority of this component is only active on the frame when <see cref="WorkThisFrame()"/> is called.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(AnimatorMoveControl))]
    [DisallowMultipleComponent]
    [RenamedFrom("TinyCharacterController.AnimatorMove")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Control.AnimatorMoveControl")]
    public class AnimatorMoveControl : MonoBehaviour,
        IPriorityLifecycle<IMove>,
        IMove,
        ITurn,
        IUpdateComponent
    {
        [Header("Priority")]
        public int MovePriority = 10;
        public int TurnPriority = 10;

        private bool _isFixedPosition = false; // Perform movement with Warp, preventing external influences.
        private bool _isWorkComponent; // When this setting is true, priorities are enabled.
        private Vector3 _velocity; // Character's movement vector.
        private float _turn; // Character's Y-axis direction.
        private bool _hasGroundCheck; // Enable GroundCheck correction.

        // Receive root motion movement.
        // This handles cases where objects from different hierarchies own the animator.
        private RootMotionReceiver _rootMotionReceiver;

        private Animator _animator;
        private Transform _transform;
        private IGroundContact _groundCheck;
        private IWarp _warp;
        private readonly Dictionary<int, List<AnimatorMoveBehaviour>> _behaviours = new();

        
        
        /// <summary>
        /// Whether to use ground normal for movement.
        /// </summary>
        public bool UseGroundNormal { get; set; } = true;

        /// <summary>
        /// Connect with the animator.
        /// Call this API after replacing the character model.
        /// </summary>
        public void Rebuild()
        {
            // Set up RootMotionReceiver component for child Animators.
            _animator = GetComponentInChildren<Animator>();
            if (_animator.TryGetComponent(out _rootMotionReceiver) == false)
                _rootMotionReceiver = _animator.gameObject.AddComponent<RootMotionReceiver>();
        }

        /// <summary>
        /// Set priorities for movement and direction.
        /// </summary>
        public int Priority
        {
            set
            {
                MovePriority = value;
                TurnPriority = value;
            }
        }

        #region ICharacterMove

        int IPriority<IMove>.Priority => _isWorkComponent ? MovePriority : 0;

        Vector3 IMove.MoveVelocity => _velocity;

        #endregion

        #region ICharacterTurn

        float ITurn.YawAngle => _turn;

        int IPriority<ITurn>.Priority => _isWorkComponent ? TurnPriority : 0;
        int ITurn.TurnSpeed => -1; // Direction is instantly reflected.

        #endregion

        private void Awake()
        {
            // Collect components.
            TryGetComponent(out _transform);
            TryGetComponent(out _warp);
            _hasGroundCheck = TryGetComponent(out _groundCheck);

            Rebuild();
        }

        void IUpdateComponent.OnUpdate(float deltaTime)
        {

            var isInProgress = IsInProgress(_animator.GetCurrentAnimatorStateInfo(0), out var behaviour);
            if (isInProgress == false)
                isInProgress = IsInProgress(_animator.GetNextAnimatorStateInfo(0), out behaviour);

            if (isInProgress == false)
            {
                _isWorkComponent = false;
                return;
            }

            UseGroundNormal = behaviour.UseGroundNormal;
            _isFixedPosition = behaviour.IsFixedPosition;
            _isWorkComponent = true;
            
            // Calculate character movement.
            if (UseGroundNormal)
            {
                // Calculate the vector considering the slope of the ground.
                // Treat as a flat plane if there is no GroundCheck.
                var velocity = _rootMotionReceiver.Velocity;
                var normal = _hasGroundCheck ? _groundCheck.GroundSurfaceNormal : Vector3.up;
                _velocity = Vector3.ProjectOnPlane(velocity, normal);
            }
            else
            {
                // Calculate the vector without considering the slope of the ground.
                _velocity = _rootMotionReceiver.Velocity;
            }

            // Calculate character direction.
            var angle = _transform.rotation * _rootMotionReceiver.Rotation;
            _turn = angle.eulerAngles.y;

            _isWorkComponent = true;
        }

        private bool IsInProgress(in AnimatorStateInfo current, out AnimatorMoveBehaviour result)
        {
            var currentHash = current.fullPathHash;
            if( _behaviours.ContainsKey(currentHash) == false)
                CacheBehaviour(currentHash);

            foreach (var behaviour in _behaviours[currentHash])
            {
                if (behaviour.IsInRange(current))
                {
                    result = behaviour;
                    return true;
                }
            }

            result = null;
            return false;
        }

        void IPriorityLifecycle<IMove>.OnAcquireHighestPriority() { }

        void IPriorityLifecycle<IMove>.OnLoseHighestPriority()  { }
        
        void IPriorityLifecycle<IMove>.OnUpdateWithHighestPriority(float deltaTime)
        {
            // When _isFixedPosition is enabled, character movement is done with Warp, not Control.
            if (_isFixedPosition)
            {
                // Update the position through Warp.
                _warp.Move(_transform.position + _velocity * deltaTime);
            }
        }
        
        private void CacheBehaviour(int hash)
        {
            var behaviours = new List<AnimatorMoveBehaviour>();
            foreach (var behaviour in _animator.GetBehaviours(hash, 0))
            {
                if (behaviour is AnimatorMoveBehaviour moveBehaviour)
                    behaviours.Add(moveBehaviour);
            }
            _behaviours.Add(hash, behaviours);
        }

        int IUpdateComponent.Order => Order.Control;

    }
}
