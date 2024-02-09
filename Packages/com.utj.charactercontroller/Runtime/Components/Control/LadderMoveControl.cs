using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Modifier;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// Component that implements the process of climbing a ladder.
    /// 
    /// Moves the character to the movement range specified by the Ladder component separately.
    /// If the Priority is high, the character will move onto the path of the ladder.
    /// This priority is valid only when the ladder to be moved is registered to the character.
    ///
    /// Move on the path of the connected <see cref="Ladder"/> with the <see cref="GrabLadder"/>.
    /// Ladder automatically releases the connection when it is reached, and can also be released
    /// voluntarily with <see cref="ReleaseLadder"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [AddComponentMenu(MenuList.MenuControl + nameof(LadderMoveControl))]
    [RenamedFrom("TinyCharacterController.Control.LadderMoveControl")]
    [RenamedFrom("TinyCharacterController.LadderMoveControl")]
    public class LadderMoveControl : MonoBehaviour, 
        ITurn, 
        IUpdateComponent, 
        IMove,
        IPriorityLifecycle<IMove>
    {
        /// <summary>
        /// Move and Turn priority
        /// </summary>
        public int Priority;

        /// <summary>
        /// Time to reach the next step
        /// </summary>
        public float StepTime;

        /// <summary>
        /// Step movement curve
        /// </summary>
        [SerializeField] private AnimationCurve _curve;

        /// <summary>
        /// Event when the character arrives at the start or end of the Ladder
        /// </summary>
        [SerializeField] private UnityEvent _onComplete;
        
        private float _yawAngle;
        private ITransform _transform;
        private IWarp _warp;
        private Vector3 _velocity;
        private float _current;
        private float _nextAmount;
        private float _timeAmount;
        private IBrain _brain;
        private int _inputDirection;

        /// <summary>
        /// Grab the Ladder.
        /// </summary>
        /// <param name="ladder">Ladder to be grabbed.</param>
        public void GrabLadder([AllowsNull] Ladder ladder)
        {
            IsComplete = false;
            CurrentLadder = ladder;
            Direction = 0;
            _current = CurrentLadder.ClosePoint(transform.position);
            _nextAmount = _current;
            _inputDirection = 0;
            _timeAmount = 0;
            _yawAngle = CurrentLadder.transform.eulerAngles.y;
        }

        /// <summary>
        /// Release the Ladder from itself.
        /// </summary>
        public void ReleaseLadder()
        {
            CurrentLadder = null;
        }

        /// <summary>
        /// Reaching the end of the Ladder path.
        /// </summary>
        public bool IsComplete { get; private set; }
        
        /// <summary>
        /// Complete move step.
        /// </summary>
        public bool IsCompleteStep { get; private set; }

        /// <summary>
        /// Direction of travel.
        /// If positive, move in the direction of Goal; if negative, move in the direction of Start.
        /// </summary>
        public float Direction { get; private set; }

        /// <summary>
        /// If True, it is connected to Ladder.
        /// </summary>
        public bool IsGrabLadder => CurrentLadder != null;

        /// <summary>
        /// Move the character based on the Ladder information.
        /// If the path is not connected, ignore it.
        /// </summary>
        /// <param name="direction"></param>
        public void Move(float direction)
        {
            if (CurrentLadder == null)
                return;

            _inputDirection = Mathf.RoundToInt(direction);
            // var stepSize = _ladder.GetStepSize();
            // var offset = Direction * stepSize;
            // var closePoint = _ladder.CloseStepPoint(_transform.position) + offset;
        }
        
        /// <summary>
        /// Currently connected Ladder
        /// </summary>
        public Ladder CurrentLadder { get; private set; }
        
        int IPriority<IMove>.Priority => CurrentLadder != null ? Priority : 0;
        Vector3 IMove.MoveVelocity => _velocity;
        int IPriority<ITurn>.Priority => CurrentLadder != null ? Priority : 0;
        int ITurn.TurnSpeed => -1;
        float ITurn.YawAngle => _yawAngle;

        private void Awake()
        {
            TryGetComponent(out _transform);
            TryGetComponent(out _warp);
            TryGetComponent(out _brain);
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == false || CurrentLadder == null)
                return;


            Gizmos.color = IsCompleteStep ? Color.red : Color.green;
            var position = CurrentLadder.CloseStepPosition(transform.position);
            var next = CurrentLadder.CloseStepPosition(_nextAmount);
            // Gizmos.DrawWireCube(position, Vector3.one * 0.4f);
            // Gizmos.DrawWireCube(next, Vector3.one * 0.4f);
            var delta =  StepTime - _timeAmount;
            var targetPosition = CurrentLadder.GetPoint(Mathf.Lerp(_current, _nextAmount, delta));

        }

        int IUpdateComponent.Order => Order.Control;
        
        /// <summary>
        /// Adjust the position of the character.
        /// </summary>
        [RenamedFrom("LocationCorrection")]
        public void AdjustCharacterPosition()
        {
            var current = CurrentLadder.ClosePoint(_transform.Position);
            _timeAmount = 0;
            _current = current;
            _nextAmount = CurrentLadder.CloseStepPoint(current );
        }
        
        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            if (CurrentLadder == null)
                return;
            
            // var closePoint = _ladder.ClosePoint(position);
            //
            // var closePosition = _ladder.CloseStepPosition(closePoint);
            // var line = closePosition - position;
            // _velocity = line / (deltaTime * Speed);

            var current = CurrentLadder.ClosePoint(transform.position);

            // IsComplete = current + Direction * Time.deltaTime < 0f || 
            //              current + Direction  * Time.deltaTime > CurrentLadder.Length ;

            
            if (current + Direction * Time.deltaTime < 0f  || 
                current + Direction  * Time.deltaTime > CurrentLadder.Length - CurrentLadder.GetStepSize())
            {
                IsComplete = true;
                if( current > CurrentLadder.Length * 0.5f)
                    _warp.Warp(CurrentLadder.TopStartPosition, CurrentLadder.transform.forward);
                else
                    _warp.Warp(CurrentLadder.BottomStartPosition, CurrentLadder.transform.forward);
                
                _onComplete.Invoke();
                ReleaseLadder();
            }
        }

        void IPriorityLifecycle<IMove>.OnAcquireHighestPriority()
        {
            AdjustCharacterPosition();
        }

        void IPriorityLifecycle<IMove>.OnLoseHighestPriority()
        {
        }

        void IPriorityLifecycle<IMove>.OnUpdateWithHighestPriority(float deltaTime)
        {
            _timeAmount += deltaTime;
            var delta =  StepTime - _timeAmount;
            var position = _transform.Position;

            IsCompleteStep =  delta < 0 || Math.Abs(_current - _nextAmount) < 0.01f;
            
            if (IsCompleteStep)
            {
                Direction = _inputDirection;
                var stepSize = CurrentLadder.GetStepSize();
                _warp.Warp(CurrentLadder.CloseStepPosition(_nextAmount), CurrentLadder.transform.forward);
                _timeAmount = 0;
                _current = CurrentLadder.CloseStepPoint(position);
                _nextAmount = CurrentLadder.CloseStepPoint(_current + Mathf.Round(Direction) * stepSize);

            }else
            {
                var percent = _curve.Evaluate(delta / StepTime) ;
                 var targetPosition = CurrentLadder.GetPoint(Mathf.Lerp(_nextAmount, _current, percent));
                 _velocity = (targetPosition - position) / deltaTime;
                 _warp.Warp(targetPosition, CurrentLadder.transform.forward);
            }
        }
    }
}