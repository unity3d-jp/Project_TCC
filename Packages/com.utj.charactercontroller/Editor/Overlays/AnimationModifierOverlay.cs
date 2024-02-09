using System;
using System.Linq;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Smb;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace TinyCharacterControllerEditor.Overlays
{
    /// <summary>
    /// EditorTools for registering the <see cref="AnimationModifierOverlay"/>.
    /// </summary>
    [EditorTool("Animation Modifier", typeof(AnimatorModifierCheck))]
    public class AnimationModifierEditorTool : EditorTool
    {
        private AnimationModifierOverlay _overlay;
        public override bool IsAvailable() => false; // Do not display the icon in EditorTools

        private void Awake()
        {
            _overlay = new AnimationModifierOverlay((AnimatorModifierCheck)target);
        }

        public void OnEnable()
        {
            // Add Overlay to SceneView
            SceneView.AddOverlayToActiveView(_overlay);
        }

        public  void OnDisable()
        {
            // Remove Overlay from SceneView
            SceneView.RemoveOverlayFromActiveView(_overlay);
        }
    }
    
    /// <summary>
    /// Checks AnimationModifierCheck and displays a list of currently active Modifiers in an Overlay.
    /// </summary>
    [Overlay(defaultDisplay = true, displayName = "Tcc Animation Modifier Debug")]
    public class AnimationModifierOverlay : Overlay
    {
        // Resource paths
        private const string UxmlGuid = "7eb3a362e744c45b49e88ce03ba9f5f2"; // AnimationModifierOverlay.uxml
        private const string IconGuidD = "3d78d5df1c0ec42bb9f7a65fa35e242e"; // Animod_D.png
        private const string IconGuidL = "16078e3027b234c228784612c2138635"; // Animod_L.png
        
        private readonly AnimatorModifierCheck _modifierCheck;

        private static Texture2D _icon;
        private static bool _isProSkin;

        public AnimationModifierOverlay(AnimatorModifierCheck check)
        {
            _modifierCheck = check;
            displayName = "Animation Modifier";

            // Load and register the icon to display when the Overlay is stored in the toolbar.
            collapsedIcon = LoadTexture();
        }

        /// <summary>
        /// Loads the icon texture.
        /// The texture to load changes based on the skin.
        /// </summary>
        /// <returns>The icon texture</returns>
        private static Texture2D LoadTexture()
        {
            if (_icon != null || _isProSkin == EditorGUIUtility.isProSkin)
                return _icon;
            
            _isProSkin = EditorGUIUtility.isProSkin;
            var iconPath = AssetDatabase.GUIDToAssetPath(_isProSkin ? IconGuidD : IconGuidL);
            _icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            Assert.IsNotNull(_icon);
            return _icon;
        }
        
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            try
            {
                var path = AssetDatabase.GUIDToAssetPath(UxmlGuid);
                var treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                Assert.IsNotNull(treeAsset);
                treeAsset.CloneTree(root);

                var parent = root.Q("root");

                // Add a list of keys stored in the Animator to the UI
                // If there is no Animator, display "NO Animator"
                var animator = _modifierCheck.GetComponent<Animator>();
                if (animator != null)
                    AddModifierLabels(animator, parent);
                else
                    parent.Add(new Label("no animator"));
            }
            catch (Exception e)
            {
                // Always catch to avoid stopping SceneView updates.
                Debug.LogWarning(e);
            }

            return root;
        }

        /// <summary>
        /// Retrieves ModifierBehaviour set in the Animator and registers them in the UI.
        /// If the editor is playing the game, UI update processing is also registered.
        /// </summary>
        /// <param name="animator">Animator to get the Behaviour from</param>
        /// <param name="parent">Parent Element to register the UI</param>
        private void AddModifierLabels(Animator animator, VisualElement parent)
        {
            // Collect all Behaviours from the Animator and add a UI element to monitor each
            var behaviours = animator.GetBehaviours<AnimatorModifierBehaviour>();
            foreach (var key in behaviours
                         .Select(behaviour => behaviour.KeyName).Distinct().OrderBy(p => p))
            {
                // Construct UI
                var label = new Label(key);
                parent.Add(label);

                // Update UI
                if(EditorApplication.isPlaying)
                    parent.schedule.Execute(() => UpdateLabel(label, key)).Every(16);
                UpdateLabel(label, key);
            }
        }

        /// <summary>
        /// Checks if UI contains a list of Modifiers, and if so, enables the UI
        /// </summary>
        /// <param name="label">Target label for operation</param>
        /// <param name="key">Key to check the content</param>
        private void UpdateLabel(in Label label, PropertyName key)
        {
            label.SetEnabled(_modifierCheck.CurrentKeys.Contains(key));
        }
    }
}
