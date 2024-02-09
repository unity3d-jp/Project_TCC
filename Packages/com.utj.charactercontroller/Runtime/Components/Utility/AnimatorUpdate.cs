using System;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Subsystem;
using UnityEngine;

namespace Unity.TinyCharacterController.Utility
{
    /// <summary>
    /// Updates the Animator before EarlyUpdate.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("")]
    public class AnimatorUpdate : ComponentBase
    {
        private Animator _animator;
        private UpdateTiming _timing;

        private void Awake()
        {
            TryGetComponent(out _animator);
            
            // Disable Animator behavior to allow updates on the component side.
            _animator.enabled = false;

            // Update timing
            _timing = _animator.updateMode == AnimatorUpdateMode.Fixed
                ? UpdateTiming.FixedUpdate
                : UpdateTiming.Update;

            // Register the component with the system.
            AnimatorUpdateSystem.Register(this, _timing);
        }

        private void OnDestroy()
        {
            // Unregister from the system.
            AnimatorUpdateSystem.Unregister(this, _timing);
        }

        /// <summary>
        /// Updates the Animator.
        /// </summary>
        /// <param name="deltaTime">Time interval for the update.</param>
        private void OnUpdate(float deltaTime)
        {
            _animator.Update(deltaTime);
        }

        /// <summary>
        /// Updates the Animator before EarlyUpdate.
        /// </summary>
        private class AnimatorUpdateSystem : SystemBase<AnimatorUpdate, AnimatorUpdateSystem>, IEarlyUpdate
        {
            /// <summary>
            /// Specifies the execution order of the system.
            /// </summary>
            int ISystemBase.Order => -10;

            private void OnDestroy()
            {
                UnregisterAllComponents();
            }

            /// <summary>
            /// Component update method.
            /// </summary>
            void IEarlyUpdate.OnUpdate()
            {
                var deltaTime = Timing == UpdateTiming.FixedUpdate ? 
                    Time.fixedDeltaTime : Time.deltaTime;
                
                // Update the Animator.
                foreach(var component in Components)
                    component.OnUpdate(deltaTime);
            }
        }
    }
}
