using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    internal class MoveManager
    {
        public bool HasHighestPriority { get; private set; }= false;

        public float CurrentSpeed { get; private set; }
        public IMove CurrentMove { get; private set; }
        public Vector3 Velocity { get; private set; }

        private IPriorityLifecycle<IMove> _moveLifeCycle;
        private readonly List<IMove> _moves = new();         // List of components for controlling character movement
        
        /// <summary>
        /// Initialize the MoveManager.
        /// </summary>
        /// <param name="obj"></param>
        public void Initialize(GameObject obj)
        {
            obj.GetComponents(_moves);

        }

        public void UpdateHighestMoveControl(float deltaTime)
        {
            using var _ = new ProfilerScope("Update Highest MoveControl");

            HasHighestPriority = _moves.GetHighestPriority(out var highestMove);
            var isChangeHighestPriorityComponent = CurrentMove != highestMove;
            
            // Action when a component with the highest priority changes
            if (isChangeHighestPriorityComponent)
            {
                HandleLoseHighestPriority();
                HandleAcquireHighestPriority(highestMove);
            }
            
            CurrentMove = highestMove;
            
            // Perform Update for the component with the highest priority
            if( HasHighestPriority )
                _moveLifeCycle?.OnUpdateWithHighestPriority(deltaTime);
        }

        private void HandleLoseHighestPriority()
        {
            _moveLifeCycle?.OnLoseHighestPriority();
        }

        private void HandleAcquireHighestPriority(IMove highestMove)
        {
            var moveLifeCycle = highestMove as IPriorityLifecycle<IMove>;
            moveLifeCycle?.OnAcquireHighestPriority();
            _moveLifeCycle = moveLifeCycle;
        }

        public void CalculateVelocity()
        {
            // Update movement vector with the highest priority component.
            using var _ = new ProfilerScope("Control Calculation");

            // Update Control information
            Velocity = HasHighestPriority ? CurrentMove.MoveVelocity : Vector3.zero;
            CurrentSpeed = HasHighestPriority ? Velocity.magnitude : 0;
        }
        
    }
}