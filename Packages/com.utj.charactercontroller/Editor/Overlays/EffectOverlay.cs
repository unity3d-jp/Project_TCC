using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace TinyCharacterControllerEditor.Overlays
{
    /// <summary>
    ///     Displays a list of <see cref="IEffect"/> components held by a GameObject and the Velocity of each <see cref="IEffect"/>.
    ///     This Overlay is created by <see cref="TccEditorTool"/>.
    /// </summary>
    [Overlay(defaultDisplay = false, displayName = "Tcc Effect Velocity Debug")]
    public class EffectOverlay : Overlay
    {
        private readonly List<IEffect> _velocities = new();

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="obj">GameObject to collect Effect components from</param>
        public EffectOverlay(GameObject obj)
        {
            displayName = "TCC Effects";

            obj.GetComponents(_velocities);
        }

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();
            root.style.width = 200;

            foreach (var effect in _velocities)
            {
                // Add UI elements
                var velocityField = CreateVelocityElement(root, effect.GetType().Name);

                // Update UI
                UpdateVelocityElement(velocityField, effect);
                if (Application.isPlaying)
                    velocityField.schedule.Execute(() =>
                        UpdateVelocityElement(velocityField, effect)).Every(16);
            }

            return root;
        }

        /// <summary>
        ///     Update UI elements
        /// </summary>
        /// <param name="velocityField">Element to update</param>
        /// <param name="effect">Value used for updating</param>
        private static void UpdateVelocityElement(Vector3Field velocityField, IEffect effect)
        {
            velocityField.value = effect.Velocity;
        }

        /// <summary>
        ///     Add UI elements to represent Velocity
        /// </summary>
        /// <param name="root">Hierarchy to add to</param>
        /// <param name="effectName">Name of the Effect to display</param>
        /// <returns>Vector3Field to represent the effect</returns>
        private static Vector3Field CreateVelocityElement(VisualElement root, string effectName)
        {
            // Make UI collapsible with FoldOut
            var foldout = new Foldout();
            foldout.text = effectName;
            foldout.viewDataKey = $"tcc-effect-{effectName}";

            // Add field to represent velocity
            var velocityField = new Vector3Field();
            velocityField.SetEnabled(false); // Disable the field to prevent selection and editing

            // Build UI
            foldout.Add(velocityField);
            root.Add(foldout);
            
            return velocityField;
        }
    }
}
