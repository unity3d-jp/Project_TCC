using System;
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
        public float StepTime = 0.01f;

        /// <summary>
        /// Step movement curve
        /// </summary>
        [SerializeField] private AnimationCurve _curve;

        /// <summary>
        /// Event when the character arrives at the start or end of the Ladder
        /// </summary>
        [SerializeField] private UnityEvent _onComplete;

        [SerializeField] private UnityEvent _onCompleteStep;

        public UnityEvent OnComplete => _onComplete;

        public UnityEvent OnCompleteStep => _onCompleteStep;

        private float _yawAngle;
        private ITransform _transform;
        private IWarp _warp;
        private Vector3 _velocity;
        private float _currentPoint;
        private float _nextPoint;
        private float _timeAmount;
        private int _inputDirection;
        private IBrain _brain;
        private bool _invokedCompleteStep;

        /// <summary>
        /// Grab the Ladder.
        /// </summary>
        /// <param name="ladder">Ladder to be grabbed.</param>
        public void GrabLadder([AllowsNull] Ladder ladder)
        {
            CurrentLadder = ladder;

            // reset values.
            IsComplete = false;
            Direction = 0;
            _timeAmount = 0;
            _inputDirection = 0;

            // set turn direction.
            _yawAngle = CurrentLadder.transform.eulerAngles.y;
            // update current position.
            _currentPoint = _nextPoint = CurrentLadder.ClosePoint(_transform.Position);
            _warp.Move(CurrentLadder.GetPosition(_currentPoint));
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
            _inputDirection = Mathf.RoundToInt(direction);
        }

        private void OnValidate()
        {
            StepTime = Mathf.Max(0.01f, StepTime);
        }

        /// <summary>
        /// Currently connected Ladder
        /// </summary>
        public Ladder CurrentLadder { get; private set; }

        int IPriority<IMove>.Priority => (CurrentLadder != null) ? Priority : 0;
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
            var next = CurrentLadder.CloseStepPosition(_nextPoint);
            Gizmos.DrawWireCube(position, Vector3.one * 0.4f);
            Gizmos.DrawWireCube(next, Vector3.one * 0.4f);
        }

        private float CalculateNextPoint(float direction)
        {
            var stepSize = CurrentLadder.GetStepSize();
            return CurrentLadder.CloseStepPoint(_currentPoint + Mathf.Round(direction) * stepSize);
        }

        int IUpdateComponent.Order => Order.Control;

        /// <summary>
        /// Adjust the position of the character.
        /// </summary>
        [RenamedFrom("LocationCorrection")]
        public void AdjustPosition()
        {
            _timeAmount = 0;
            _nextPoint = _currentPoint = CurrentLadder.ClosePoint(_transform.Position);

            _warp.Move(CurrentLadder.GetPosition(_nextPoint));
        }

        public void AdjustStepPosition()
        {
            _timeAmount = 0;
            _currentPoint = CurrentLadder.ClosePoint(_transform.Position);
            _nextPoint = CalculateNextPoint(_inputDirection);

            _warp.Move(CurrentLadder.GetPosition(_currentPoint));
        }

        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            if (CurrentLadder == null)
                return;

            var currentPoint = CurrentLadder.ClosePoint(_transform.Position);

            // Adjust the coordinates as the target point has been reached
            IsComplete = currentPoint + _brain.ControlVelocity.y * Time.deltaTime < 0f ||
                         currentPoint + _brain.ControlVelocity.y * Time.deltaTime >
                         CurrentLadder.Length - CurrentLadder.GetStepSize();
            if (IsComplete)
            {
                if (currentPoint > CurrentLadder.Length * 0.5f)
                    _warp.Warp(CurrentLadder.TopStartPosition, CurrentLadder.transform.forward);
                else
                    _warp.Warp(CurrentLadder.BottomStartPosition, CurrentLadder.transform.forward);

                _onComplete.Invoke();
                ReleaseLadder();
            }
        }

        void IPriorityLifecycle<IMove>.OnAcquireHighestPriority()
        {
            AdjustStepPosition();
            _timeAmount = 0;
        }

        void IPriorityLifecycle<IMove>.OnLoseHighestPriority()
        {
        }

        void IPriorityLifecycle<IMove>.OnUpdateWithHighestPriority(float deltaTime)
        {
            // Check if the next step has been reached in terms of time or distance

            _timeAmount += deltaTime;
            IsCompleteStep = StepTime - _timeAmount < 0 ||
                             Math.Abs(_currentPoint - _nextPoint) < 0.01f;

            // Since the priority is elevated, movement is handled by LadderMoveControl.
            // Movement with Animator, etc., doesn't have higher priority.

            if (IsCompleteStep)
            {
                // Adjustment of position as the step movement is complete
                Direction = _inputDirection;
                _timeAmount = 0;

                _warp.Move(CurrentLadder.CloseStepPosition(_nextPoint));

                if (_invokedCompleteStep == false)
                {
                    _invokedCompleteStep = true;
                    _onCompleteStep.Invoke();
                }

                // Calculate the next vector from the destination
                _currentPoint = _nextPoint;

                // Calculate the destination if there is input
                if (Mathf.Abs(_inputDirection) > 0)
                {
                    _invokedCompleteStep = false;
                    _nextPoint = CalculateNextPoint(_inputDirection);
                }
            }
            else
            {
                // During step movement

                // Calculate the movement amount based on the difference between elapsed time and movement time
                var delta = StepTime - _timeAmount;
                var percent = _curve.Evaluate(delta / StepTime);
                // Calculate the arrival point from the current position and destination
                var targetPosition = CurrentLadder.GetPosition(Mathf.Lerp(_nextPoint, _currentPoint, percent));

                // Calculate the movement vector based on the comparison between the arrival point and current position
                _velocity = (targetPosition - _transform.Position) / deltaTime;

                // Warp to the arrival point
                _warp.Move(targetPosition);
            }
        }
    }
}