using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    internal class TurnManager
    {
        /// <summary>
        /// Manages the current turn.
        /// </summary>
        public ITurn CurrentTurn { get; private set; }

        /// <summary>
        /// Indicates whether the highest priority turn is held.
        /// </summary>
        public bool HasHighestPriority { get; private set; } = false;

        /// <summary>
        /// Indicates the target yaw angle.
        /// </summary>
        public float TargetYawAngle { get; private set; }

        /// <summary>
        /// Indicates the difference from the previous yaw angle.
        /// </summary>
        public float DeltaTurnAngle { get; private set; }

        /// <summary>
        /// Calculates the next yaw angle.
        /// </summary>
        public float NextYawAngle { get; private set; }

        private ITransform _transform;
        private IPriorityLifecycle<ITurn> _turnLifecycle;
        private readonly List<ITurn> _turns = new();

        /// <summary>
        /// Initializes the TurnManager.
        /// </summary>
        /// <param name="obj">The GameObject to retrieve turns from.</param>
        /// <param name="transform">The object's transform.</param>
        public void Initialize(GameObject obj, ITransform transform)
        {
            _transform = transform;
            obj.GetComponents(_turns);
        }

        /// <summary>
        /// Updates the highest priority turn control.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last frame.</param>
        public void UpdateHighestTurnControl(float deltaTime)
        {
            using var _ = new ProfilerScope("Update Highest Turn Control");

            HasHighestPriority = _turns.GetHighestPriority(out var highestTurn);
            var isChangeHighestPriorityComponent = highestTurn != CurrentTurn;
            
            if (isChangeHighestPriorityComponent)
            {
                HandleLoseHighestPriority();
                HandleAcquireHighestPriority(highestTurn);
            }

            CurrentTurn = highestTurn;

            if(HasHighestPriority)
                _turnLifecycle?.OnUpdateWithHighestPriority(deltaTime);
        }

        /// <summary>
        /// Handles actions when the highest priority turn changes.
        /// </summary>
        private void HandleLoseHighestPriority()
        {
            _turnLifecycle?.OnLoseHighestPriority();
        }

        /// <summary>
        /// Handles acquiring the new highest priority turn when it changes.
        /// </summary>
        /// <param name="highestTurn">The new highest priority turn.</param>
        private void HandleAcquireHighestPriority(ITurn highestTurn)
        {
            var turnLifeCycle = highestTurn as IPriorityLifecycle<ITurn>;
            turnLifeCycle?.OnAcquireHighestPriority();
            _turnLifecycle = turnLifeCycle;
        }

        /// <summary>
        /// Calculates the angle.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last frame.</param>
        public void CalculateAngle(float deltaTime)
        {
            using var _ = new ProfilerScope("Calculate Rotation");
            
            if (HasHighestPriority)
            {
                NextYawAngle = CalculateNewAngle(CurrentTurn, _transform.Rotation, deltaTime);
                DeltaTurnAngle = Mathf.DeltaAngle(NextYawAngle, TargetYawAngle);
                TargetYawAngle = CurrentTurn.YawAngle;
            }
            else
            {
                DeltaTurnAngle = 0;
            }
        }

        /// <summary>
        /// Calculates the new angle.
        /// </summary>
        /// <param name="turn">The highest priority turn.</param>
        /// <param name="rotation">The current rotation.</param>
        /// <param name="deltaTime">Time elapsed since the last frame.</param>
        /// <returns>The calculated new angle.</returns>
        private float CalculateNewAngle(in ITurn turn, in Quaternion rotation, float deltaTime)
        {
            return turn.TurnSpeed < 0
                ? turn.YawAngle
                : Mathf.LerpAngle(rotation.eulerAngles.y, turn.YawAngle, turn.TurnSpeed * deltaTime);
        }
    }
}