using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Smb;
using Unity.TinyCharacterController.Utility;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// This component is used to move the character from the current position to a target position.
    /// It is used for actions where the character needs to move to specific locations, such as jumping or moving to a specified location.
    /// The target position is set using SetTarget.
    /// It is used in conjunction with <see cref="MatchTargetBehaviour" /> within the Animator.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(LerpMoveControl))]
    [RequireComponent(typeof(CharacterSettings))]
    [DisallowMultipleComponent]
    [RenamedFrom("TinyCharacterController.MatchTargetMove")]
    [RenamedFrom("TinyCharacterController.Effect.MatchTargetMoveControl")]
    [RenamedFrom("TinyCharacterController.Control.MatchTargetMoveControl")]
    [RenamedFrom("TinyCharacterController.Control.LerpMoveControl")]
    public class LerpMoveControl : MonoBehaviour,
        ILinearInterpolationMove,
        IMove,
        ITurn,
        IPriorityLifecycle<IMove>,
        IPriorityLifecycle<ITurn>
    {
        /// <summary>
        /// Priority for Move and Turn.
        /// </summary>
        public int Priority = 100;

        /// <summary>
        /// Update the character's position.
        /// </summary>
        private bool _usePosition = true;

        /// <summary>
        /// Update the character's rotation.
        /// </summary>
        private bool _useRotation = true;

        private Vector3 _currentTargetPosition;
        private Quaternion _currentTargetRotation;
        private PropertyName _id; // Prevents cancellation from other states

        private float _moveAmount, _turnAmount;

        private Vector3 _moveVelocity;
        private Vector3 _startPosition;

        private Quaternion _startRotation;
        private List<TargetInfo> _targets = new();
        private Transform _transform;
        private IWarp _warp;
        private float _yawAngle;

        private bool _isHighestMovePriority = false;
        private bool _isHighestTurnPriority = false;

        /// <summary>
        /// If true, MatchTargetMoveControl is currently in progress.
        /// </summary>
        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            TryGetComponent(out _transform);
            TryGetComponent(out _warp);
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == false)
                return;

            if (IsPlaying)
            {
                var targetPosition = _currentTargetPosition;
                // Line from the starting position where the tween begins to the target position.
                Gizmos.DrawLine(_startPosition, targetPosition);

                // Character's desired center position.
                var centerPosition = Vector3.Lerp(_startPosition, targetPosition, _moveAmount);

                // Draw white wireframes for the character's desired center position and target position.
                Gizmos.DrawWireSphere(centerPosition, 0.1f);
                Gizmos.DrawWireSphere(targetPosition, 0.1f);

                // Fill center position and target position with yellow and green colors, respectively.
                Gizmos.color = new Color(1, 1, 0, 1);
                Gizmos.DrawSphere(centerPosition, 0.1f);
                Gizmos.color = new Color(0, 1, 0, 0.4f);
                Gizmos.DrawSphere(targetPosition, 0.1f);
            }

            // Use Gizmos to represent target points.
            foreach (var target in _targets)
            {
                var eachTargetPosition = target.Position + target.Offset;
                Gizmos.color = new Color(0, 1, 0, 1);
                Gizmos.DrawWireSphere(eachTargetPosition, 0.1f);
                Gizmos.color = new Color(0, 1, 0, 0.4f);
                Gizmos.DrawSphere(eachTargetPosition, 0.1f);
#if UNITY_EDITOR
                UnityEditor.Handles.Label(eachTargetPosition, $"{target.Name}");
#endif
            }
        }

        int IPriority<IMove>.Priority => IsPlaying && _usePosition ? Priority : 0;

        Vector3 IMove.MoveVelocity => _moveVelocity;

        int IPriority<ITurn>.Priority => IsPlaying && _useRotation ? Priority : 0;

        int ITurn.TurnSpeed => -1; // Always turn in the direction specified by Control

        float ITurn.YawAngle => _yawAngle;

        /// <summary>
        /// Start the matching target process. It overwrites the process if it is already running.
        /// </summary>
        /// <param name="id">Key to identify the target.</param>
        public void Play(PropertyName id)
        {
            var index = _targets.FindIndex(c => c.Id == id);
            if (index == -1)
                throw new Exception($"Id[{id}] not found");

            _id = id;
            IsPlaying = true;
            _moveAmount = 0;
            _turnAmount = 0;
            _startPosition = _transform.position;
            _startRotation = _transform.rotation;

            var target = _targets[index];
            _currentTargetPosition = target.Position + target.Offset;
            _currentTargetRotation = target.Rotation;
            _usePosition = target.UsePosition;
            _useRotation = target.UseRotation;
        }

        /// <summary>
        /// Stop the matching target process. Only valid if it is currently running.
        /// The character will move to the position specified by SetTarget.
        /// It is ignored if the current process has a different ID.
        /// </summary>
        /// <param name="id">Unique ID of the matching target, used for cancellation or overwriting.</param>
        public void Stop(PropertyName id)
        {
            if (IsPlaying == false || id != _id)
                return;

            ApplyPositionAndRotation(_currentTargetPosition, _currentTargetRotation);
            IsPlaying = false;
        }

        /// <summary>
        /// Set the normalized time for the matching target position.
        /// 0 is the starting point, and 1 is the ending point.
        /// </summary>
        /// <param name="moveAmount">Position</param>
        /// <param name="turnAmount">Rotation</param>
        public void SetNormalizedTime(float moveAmount, float turnAmount)
        {
            _moveAmount = moveAmount;
            _turnAmount = turnAmount;
        }

        /// <summary>
        /// Immediately move to the target position without playing.
        /// </summary>
        /// <param name="id">Key to identify the target.</param>
        void ILinearInterpolationMove.FitTargetWithoutPlay(PropertyName id)
        {
            var index = _targets.FindIndex(c => c.Id == id);
            if ( index == -1)
                return;

            var target = _targets[index];
            var targetPosition = target.Position + target.Offset;
            ApplyPositionAndRotation(targetPosition, target.Rotation);
            IsPlaying = false;
        }

        void IPriorityLifecycle<ITurn>.OnAcquireHighestPriority()
        {
            _isHighestTurnPriority = true;
        }

        void IPriorityLifecycle<ITurn>.OnLoseHighestPriority()
        {
            _isHighestTurnPriority = false;
        }

        void IPriorityLifecycle<ITurn>.OnUpdateWithHighestPriority(float deltaTime)
        {
            // Rotate towards the target direction.
            var rotation = Quaternion.Lerp(_startRotation, _currentTargetRotation, _turnAmount);
            _warp.Warp(rotation);
            _yawAngle = rotation.eulerAngles.y;
        }

        void IPriorityLifecycle<IMove>.OnAcquireHighestPriority()
        {
            _isHighestMovePriority = true;
        }

        void IPriorityLifecycle<IMove>.OnLoseHighestPriority()
        {
            _isHighestMovePriority = false;
        }

        void IPriorityLifecycle<IMove>.OnUpdateWithHighestPriority(float deltaTime)
        {
            var position = Vector3.Lerp(_startPosition, _currentTargetPosition, _moveAmount);
            _moveVelocity = (position - _transform.position) / deltaTime;
            _warp.Move(position);
        }

        /// <summary>
        /// Set the target position.
        /// </summary>
        /// <param name="rotation">Target rotation</param>
        /// <param name="id">Key to identify the target.</param>
        public void SetTarget(PropertyName id, in Quaternion rotation)
        {
            SetTargetInternal(id, false, Vector3.zero, true, rotation, Vector3.zero);
        }

        /// <summary>
        /// Set the target position.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="id">Key to identify the target.</param>
        public void SetTarget(PropertyName id, in Vector3 position)
        {
            SetTargetInternal(id, true, position, false, quaternion.identity, Vector3.zero);
        }

        /// <summary>
        /// Set the target position.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="rotation">Target rotation</param>
        /// <param name="id">Key to identify the target.</param>
        public void SetTarget(PropertyName id, in Vector3 position, in Quaternion rotation)
        {
            SetTargetInternal(id, true, position, true, rotation, Vector3.zero);
        }

        /// <summary>
        /// Set the target position.
        /// Reach a position with a relative offset set from the current position.
        /// For example, if the offset is set to (0, 0, -1), it will reach one meter in front of the target point.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="offset">Offset for the target position</param>
        /// <param name="id">Key to identify the target.</param>
        public void SetTarget(PropertyName id, in Vector3 position, in Vector3 offset)
        {
            SetTargetInternal(id, true, position, false, quaternion.identity, offset);
        }

        /// <summary>
        /// Set the target position.
        /// Reach a position with a relative offset set from the current position.
        /// For example, if the offset is set to (0, 0, -1), it will reach one meter in front of the target.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="rotation">Target rotation</param>
        /// <param name="offset">Offset for the target position</param>
        /// <param name="id">Key to identify the target.</param>
        public void SetTarget(PropertyName id, in Vector3 position, in Quaternion rotation, in Vector3 offset)
        {
            SetTargetInternal(id, true, position, true, rotation, offset);
        }

        private void SetTargetInternal(
            PropertyName id, bool usePosition, Vector3 position, 
            bool useRotation, Quaternion rotation, in Vector3 offset)
        {
            var index = _targets.FindIndex(c => c.Id == id);

            TargetInfo target;
            if (index == -1)
            {
                target = new TargetInfo(id);
                _targets.Add(target);
            }
            else
            {
                target = _targets[index];
            }

            target.UsePosition = usePosition;
            target.Position = position;
            target.UseRotation = useRotation;
            target.Rotation = Quaternion.AngleAxis(rotation.eulerAngles.y, Vector3.up);
            target.Offset = rotation * offset;
        }

        /// <summary>
        /// Remove target position information.
        /// </summary>
        /// <param name="id">Key to identify the target.</param>
        public void RemoveTarget(PropertyName id)
        {
            var index = _targets.FindIndex(c => c.Id == id);
            if( index != -1)
                _targets.RemoveAtSwapBack(index);
        }

        /// <summary>
        /// Cancel the currently running matching target process.
        /// </summary>
        /// <param name="id">Unique ID of the matching target, used for cancellation or overwriting.</param>
        public void Cancel(PropertyName id)
        {
            if (_id != id)
                return;

            IsPlaying = false;
        }

        /// <summary>
        /// Cancel the currently running process.
        /// </summary>
        public void Cancel()
        {
            IsPlaying = false;
        }

        /// <summary>
        /// Apply position and rotation.
        /// However, it does not work if UsePosition or UseRotation is disabled.
        /// </summary>
        /// <param name="position">New position</param>
        /// <param name="rotation">New rotation</param>
        private void ApplyPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            if (_usePosition && _isHighestMovePriority)
                _warp.Warp(position);
            if (_useRotation && _isHighestTurnPriority)
                _warp.Warp(rotation);
        }

        /// <summary>
        /// Information about the target position.
        /// </summary>
        private class TargetInfo
        {
            public TargetInfo(PropertyName id)
            {
                Id = id;
#if UNITY_EDITOR
                Name = Id.ToString().Split(":")[0];
#endif
            }
                
                
            public Vector3 Position;
            public Vector3 Offset;
            public Quaternion Rotation;
            public PropertyName Id { get; private set; }

            public bool UsePosition;
            public bool UseRotation;
            
#if UNITY_EDITOR
            
            // debug info
            public string Name { get; private set; }
#endif
        }
    }
}
