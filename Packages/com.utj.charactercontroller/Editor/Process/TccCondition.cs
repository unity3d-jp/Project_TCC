using System;
using System.Collections.Generic;
using System.Text;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Unity.TinyCharacterControllerEditor
{
    /// <summary>
    /// Check TCC components and gather problematic components.
    /// </summary>
    internal class TccCondition
    {
        /// <summary>
        /// True if there are components with errors.
        /// </summary>
        public bool HasErrorMessage { get; private set; } = false;

        /// <summary>
        /// List of error messages.
        /// </summary>
        public List<string> ErrorMessages { get; } = new();

        /// <summary>
        /// Callback when settings of selected components change.
        /// </summary>
        public void GatherErrorMessage(Component target)
        {
            // Check the list of components and error messages.
            ErrorMessages.Clear();

            var components = ListPool<Component>.Get();
            
            try
            {
                // Components exist, but there is no Brain.
                if (target == null)
                {
                    HasErrorMessage = false;
                    return;
                }

                // Get a list of components attached to the object.
                target.GetComponents(components);

                MultiBrainCheck(components);
                ComponentDependenciesCheck(target, components);
                ComponentErrorCheck(components);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            finally
            {
                // Set the presence of error messages.
                HasErrorMessage = ErrorMessages.Count > 0;

                ListPool<Component>.Release(components);
            }
        }

        private void ComponentErrorCheck(in List<Component> components)
        {
            foreach (var component in components)
            {
                // There is some error in the condition.
                if (component is IComponentCondition condition)
                {
                    condition.OnConditionCheck(ErrorMessages);
                }
            }
        }

        private void ComponentDependenciesCheck(in Component target, in List<Component> components)
        {
            var message = new StringBuilder();

            // Check the contents of the collected components individually.
            foreach (var component in components)
            {
                // Required interfaces are not registered.
                var attributes = component.GetType()
                    .GetCustomAttributes(typeof(RequireInterfaceAttribute), false);
                foreach (RequireInterfaceAttribute attribute in attributes)
                {
                    var existComponent = target.TryGetComponent(attribute.InterfaceType, out _);
                    if (existComponent ) 
                        continue;
                    
                    message.Clear();
                    message.AppendFormat("<b>{0}</b> requires a component that implements <b>{1}</b>. <br> e.g. ", 
                        component.GetType().Name, attribute.InterfaceType.Name);
                        
                    foreach (var type in TypeCache.GetTypesDerivedFrom(attribute.InterfaceType))
                        message.AppendFormat("<b>{0}</b>, ", type.Name);

                    ErrorMessages.Add(message.ToString());
                }
            }
        }

        private void MultiBrainCheck(in List<Component> target)
        {
            // Multiple instances of IBrain are registered.
            var count = target.FindAll(component => component is IBrain).Count;
            
            switch (count)
            {
                case > 1:
                    ErrorMessages.Add("Multiple <b>IBrain</b> components are registered.");
                    break;
            }
        }
    }
}