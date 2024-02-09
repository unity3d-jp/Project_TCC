using System;
using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Utility;
using Unity.TinyCharacterController.Interfaces;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Unity.TinyCharacterController.Manager
{
    /// <summary>
    /// System to check the current state of animations and detect the acquisition and release of Modifiers.
    /// </summary>
    public class AnimationModifierSystem : Singleton<AnimationModifierSystem>
    {
        
        private readonly List<IAnimationModifierUpdate> _updateComponents = new ();
        private readonly List<IAnimationModifierUpdate> _fixedUpdateComponents = new();

        
        private void Awake()
        {
            RegisterSystem();
        }

        private void OnDestroy()
        {
            UnregisterSystem();
        }

        /// <summary>
        /// Add a component to the system.
        /// </summary>
        /// <param name="component">The component to add.</param>
        /// <param name="isFixedUpdate">True if the operation interval is FixedUpdate.</param>
        public static void Add(IAnimationModifierUpdate component, bool isFixedUpdate = false)
        {
            if (isFixedUpdate)
            {
                Instance._fixedUpdateComponents.Add(component);
            }
            else
            {
                Instance._updateComponents.Add(component);
            }
        }

        /// <summary>
        /// Remove a component from the system.
        /// </summary>
        /// <param name="component">The component to remove.</param>
        /// <param name="isFixedUpdate">True if the operation interval is FixedUpdate.</param>
        public static void Remove(IAnimationModifierUpdate component, bool isFixedUpdate = false)
        {
            if (IsCreated == false)
                return;
            
            if (isFixedUpdate)
                Instance._fixedUpdateComponents.Remove(component);
            else
                Instance._updateComponents.Remove(component);
        }


        /// <summary>
        /// Register the system to interrupt after Animator operations.
        /// </summary>
        private void RegisterSystem()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            {
                var index = Array.FindIndex(loop.subSystemList, c => c.type == typeof(PreLateUpdate));
                var subIndex = Array.FindIndex(loop.subSystemList[index].subSystemList, c => c.type == typeof(PreLateUpdate.DirectorUpdateAnimationEnd));
                var subsystem = new PlayerLoopSystem { type = typeof(AnimationModifierSystem), updateDelegate = OnUpdate };

                var subsystems = new List<PlayerLoopSystem>(loop.subSystemList[index].subSystemList);
                subsystems.Insert(subIndex, subsystem);
                loop.subSystemList[index].subSystemList = subsystems.ToArray();
            }

            {
                var index = Array.FindIndex(loop.subSystemList, c => c.type == typeof(FixedUpdate));
                var subsystem = new PlayerLoopSystem { type = typeof(AnimationModifierSystem), updateDelegate = OnFixedUpdate };
                var subsystems = new List<PlayerLoopSystem>(loop.subSystemList[index].subSystemList);
                subsystems.Add(subsystem);
                loop.subSystemList[index].subSystemList = subsystems.ToArray();
            }

            PlayerLoop.SetPlayerLoop(loop);
        }

        /// <summary>
        /// Unregister the system from interrupting after Animator operations.
        /// </summary>
        private void UnregisterSystem()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            {
                var index = Array.FindIndex(loop.subSystemList, c => c.type == typeof(PreLateUpdate));
                var subsystems = new List<PlayerLoopSystem>(loop.subSystemList[index].subSystemList);
                subsystems.RemoveAll(c => c.type == typeof(AnimationModifierSystem));
                loop.subSystemList[index].subSystemList = subsystems.ToArray();
            }

            {
                var index = Array.FindIndex(loop.subSystemList, c =>  c.type == typeof(FixedUpdate));
                var subsystems = new List<PlayerLoopSystem>(loop.subSystemList[index].subSystemList);
                subsystems.RemoveAll(c => c.type == typeof(AnimationModifierSystem));
                loop.subSystemList[index].subSystemList = subsystems.ToArray();
            }
            PlayerLoop.SetPlayerLoop(loop);
        }

        /// <summary>
        /// Interrupt Animator operations at the Update timing.
        /// </summary>
        private void OnUpdate()
        {
            foreach (var component in _updateComponents)
                component.OnUpdate();
        }

        /// <summary>
        /// Interrupt Animator operations at the FixedUpdate timing.
        /// </summary>
        private void OnFixedUpdate()
        {
            foreach (var component in _fixedUpdateComponents)
                component.OnUpdate();
        }
    }
}
