using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    /// <summary>
    /// The base class for performing batch processing. It collects and processes multiple components collectively.
    /// </summary>
    /// <typeparam name="TComponent">The component to register</typeparam>
    /// <typeparam name="TSystem">The class that performs batch processing</typeparam>
    public abstract class SystemBase<TComponent, TSystem> : UpdateTimingSingleton<TSystem>  
                        where TComponent : ComponentBase 
                        where TSystem : SystemBase<TComponent, TSystem>
    {
        /// <summary>
        /// Unregister components
        /// </summary>
        protected void UnregisterAllComponents()
        {
            // Unregister all components.
            var components = new List<TComponent>(Components);
            foreach (var component in components)
                Unregister(component, Timing);
            
        }

        /// <summary>
        /// Register a component with the system.
        /// </summary>
        /// <param name="component">The component to register</param>
        /// <param name="timing">The update timing</param>
        public static void Register(TComponent component, UpdateTiming timing)
        {
            if (((IComponentIndex)component).IsRegistered)
                return;

            var instance = GetInstance(timing);
            instance.RegisterInternal(component);
        }
        
        /// <summary>
        /// Unregister a component from the system.
        /// </summary>
        /// <param name="component">The component to unregister</param>
        /// <param name="timing">The update timing</param>
        public static void Unregister(TComponent component, UpdateTiming timing)
        {
            if (!IsCreated(timing) || ((IComponentIndex)component).IsRegistered == false) 
                return;
            
            var instance = GetInstance(timing);
            instance.UnregisterInternal(component, out var removeIndex);
            instance.OnUnregisterComponent(component, removeIndex);
        }

        /// <summary>
        /// Unregister an element.
        /// </summary>
        /// <param name="component">The component to unregister</param>
        /// <param name="removeIndex">The ID of the element to be removed</param>
        private void UnregisterInternal(IComponentIndex component, out int removeIndex)
        {
            
            removeIndex = component.Index;
            
            // Do nothing if the component doesn't exist
            if (removeIndex == -1)
                return;

            // Update the indexes
            var lastIndex = Components.Count - 1;
            var overwriteComponent = (IComponentIndex)Components[lastIndex];
            overwriteComponent.Index = removeIndex;
            component.Index = -1;

            // Overwrite the element to be removed with the last element and then remove the last element.
            Components[removeIndex] = Components[lastIndex];
            Components.RemoveAt(lastIndex);
        }

        /// <summary>
        /// Register an element.
        /// </summary>
        /// <param name="component">The component to register</param>
        private void RegisterInternal(TComponent component)
        {
            var index = Components.Count;
            Components.Add(component);

            // Update the index
            ((IComponentIndex)component).Index = index;
            
            // Notify the derived class of the added component's number
            OnRegisterComponent(component, index);
        }

        protected override void OnCreate(UpdateTiming timing)
        {
            // register callbacks. e.g. EarlyUpdate, PostUpdate.
            GameControllerManager.Register(this, timing);
        }

        /// <summary>
        /// Callback called when an element is registered.
        /// </summary>
        /// <param name="component">The registered element</param>
        /// <param name="index">The index of the element</param>
        protected virtual void OnRegisterComponent(TComponent component, int index) { }
        
        /// <summary>
        /// Callback called when an element is unregistered.
        /// </summary>
        /// <param name="component">The unregistered element</param>
        /// <param name="index">The index of the element</param>
        protected virtual void OnUnregisterComponent(TComponent component, int index) { }

        /// <summary>
        /// List of registered components.
        /// </summary>
        protected List<TComponent> Components { get; private set; } = new();
    }
}
