using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace TinyCharacterControllerEditor.Overlays
{
    /// <summary>
    /// Displays an Overlay showing a list of priorities for components with ICharacterMove and ICharacterTurn interfaces,
    /// collected from the components of a GameObject. 
    /// This Overlay is registered by <see cref="TccEditorTool"/>.
    /// </summary>
    [Overlay(defaultDisplay = true, displayName = "Tcc Priority Debug")]
    public class ControlPriorityOverlay : Overlay
    {
        // List of components with priorities
        private readonly List<IPriority<IMove>> _moves = new ();
        private readonly List<IPriority<ITurn>> _turns = new();
        
        // Paths to resources
        private const string UxmlGuid = "cc1c6b0008738472e9014e8f35755170"; // ControlPriorityOverlay.uxml
        private const string IconGuidL = "d079a553c22ff463eab7113da8af026e"; // Priority_L.png
        private const string IconGuidD = "09025c02795724bf79e9172389841e2d"; // Priority_D.png

        // Cached information for the icon
        private static Texture2D _icon;
        private static bool _isProSkin;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject">The GameObject component to bind with the Overlay</param>
        public ControlPriorityOverlay(GameObject gameObject)
        {
            displayName = "TCC Priority";
            
            // Gather components
            gameObject.GetComponents(_moves);
            gameObject.GetComponents(_turns);
            
            // Update icon
            collapsedIcon = LoadIcon();
        }

        /// <summary>
        /// Loads the icon. Uses the cached value if the icon is already loaded or if the skin has not changed.
        /// </summary>
        /// <returns>Icon image</returns>
        private static Texture2D LoadIcon()
        {
            // Use the cached value if the icon is already loaded or if the skin has not changed
            if (_icon != null || _isProSkin == EditorGUIUtility.isProSkin)
                return _icon;
            
            // Load the icon
            var iconPath = AssetDatabase.GUIDToAssetPath(EditorGUIUtility.isProSkin ? IconGuidD : IconGuidL);
            _icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            _isProSkin = EditorGUIUtility.isProSkin;
            Assert.IsNotNull(_icon);
            return _icon;
        }

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();
            var path = AssetDatabase.GUIDToAssetPath(UxmlGuid);
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            
            Assert.IsNotNull(tree);
            tree.CloneTree(root);

            // Set the minimum height when stored in the bar
            if (collapsed)
                root.style.minHeight = 100;
            
            // Add elements
            AddPriorityElements(root.Q<VisualElement>("move-element") , _moves);
            AddPriorityElements(root.Q<VisualElement>("turn-element"), _turns);

            
            return root;
        }

        /// <summary>
        /// Adds UI elements to indicate priorities to the specified Element
        /// </summary>
        /// <param name="root">The root Element to add to</param>
        /// <param name="components">List of components with priorities</param>
        /// <typeparam name="T">Component type with priorities</typeparam>
        private static void AddPriorityElements<T>(VisualElement root, List<IPriority<T>> components) where T : class, IPriority<T>
        {
            foreach (var move in components)
            {
                AddPriorityElementLabel(move, root);
            }
        }

        /// <summary>
        /// Adds an element to display the priority
        /// </summary>
        /// <param name="control">Control with a priority</param>
        /// <param name="parent">The parent element to add to</param>
        /// <typeparam name="T">Component type with priorities</typeparam>
        private static void AddPriorityElementLabel<T>(IPriority<T> control, VisualElement parent) where T : class, IPriority<T>
        {
            // Create the element
            var elementRoot = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween
                }
            };
            var priority = control.Priority;
            var label = new Label( control.GetType().Name);
            var intField = new IntegerField()
            {
                value = priority,
                isReadOnly = true
            };

            // Construct UI
            elementRoot.Add(label);
            elementRoot.Add(intField);
            parent.Add(elementRoot);

            // Update UI content when priority changes
            intField.schedule.Execute((t) =>
            {
                if (priority == control.Priority)
                    return;

                priority = control.Priority;
                UpdateElement(priority);
            }).Every(16);
            UpdateElement(priority);
                
            return;

            void UpdateElement(int i) 
            {
                var enable = i > 0;
                intField.value = i;
                intField.SetEnabled(enable);
                label.SetEnabled(enable);
            }
        }
    }
}
