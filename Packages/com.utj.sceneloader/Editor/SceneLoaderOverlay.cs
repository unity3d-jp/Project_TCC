using System;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using SceneAsset = UnityEditor.SceneAsset;

namespace Unity.SceneManagement
{
    /// <summary>
    /// This overlay manipulates the behavior of the scene when the game is played.
    /// </summary>
    [Overlay(typeof(SceneView), ID, Title, true, defaultDockZone = DockZone.TopToolbar)]
    public class SceneLoaderOverlay : Overlay, ICreateHorizontalToolbar
    {
        // Constants for overlay settings.
        private const string ID = "SceneLoaderOverlay";
        private const string Title = "SceneLoader";

        // Constants for file paths used in the overlay.
        private const string IconGuid = "78d70be841cdde3489032af5538d631b";

        /// <summary>
        /// Method called when overlay is created, used to load icon.
        /// </summary>
        public override void OnCreated()
        {
            collapsedIcon = LoadIcon();
        }

        /// <summary>
        /// Creates the panel content for the overlay.
        /// </summary>
        /// <returns>overlay window</returns>
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();
        
            // Load and bind UI elements to the overlay.
            CreateOverlayUxml(ref root);
            Bind(root);
        
            return root;
        }
        /// <summary>
        /// Creates horizontal toolbar content, specific to this overlay.
        /// </summary>
        /// <returns>toolbar</returns>
        OverlayToolbar ICreateHorizontalToolbar.CreateHorizontalToolbarContent()
        {
            var root = new OverlayToolbar();
            CreateHorizontalUxml(ref root);
            Bind(root);
        
            return root;
        }
        
        /// <summary>
        /// Loads the icon for the overlay from the specified path.
        /// </summary>
        /// <returns>icon texture</returns>
        private static Texture2D LoadIcon()
        {
            var path = AssetDatabase.GUIDToAssetPath(IconGuid);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        
        /// <summary>
        /// Loads and clones the visual tree for the overlay.
        /// </summary>
        /// <param name="root">ui root</param>
        private static void CreateOverlayUxml(ref VisualElement root)
        {
            var objectField = new ObjectField("Start Scene");
            objectField.objectType = typeof(SceneAsset);
            objectField.value = SceneLoaderControl.instance.CurrentSceneAsset;

            var enumField = new EnumField("PlayMode Behaviour", SceneLoaderControl.instance.SceneLoadActionType);

            root.Add(objectField);
            root.Add(enumField);
        }
        
        /// <summary>
        /// Loads and clones the visual tree for the horizontal overlay.
        /// </summary>
        /// <param name="root">ui root</param>
        private static void CreateHorizontalUxml(ref OverlayToolbar root)
        {
            var objectField = new ObjectField();
            objectField.objectType = typeof(SceneAsset);
            objectField.value = SceneLoaderControl.instance.CurrentSceneAsset;
            objectField.style.width = 85;

            var enumField = new EnumField(SceneLoaderControl.instance.SceneLoadActionType);
            enumField.style.width = 85;

            root.Add(objectField);
            root.Add(enumField);
        }
        
        /// <summary>
        /// Binds functionality to the overlay's UI elements.
        /// </summary>
        /// <param name="root">ui root</param>
        private static void Bind(VisualElement root)
        {
            try
            {
                // Query UI elements from the root.
                var objectField = root.Q<ObjectField>();
                var enumField = root.Q<EnumField>();
        
                // Method to swap the state based on SceneLoadType.
                void SwapState(SceneLoadType newState)
                {
                    SceneLoaderControl.instance.SceneLoadActionType = newState;
                    var isShow = (newState == SceneLoadType.UseStart);
                    objectField.style.display =  isShow ? DisplayStyle.Flex : DisplayStyle.None;
                }
        
                // Setting up the object and enum fields with initial values and event listeners.
                
                objectField.value = SceneLoaderControl.instance.CurrentSceneAsset;
                objectField.RegisterValueChangedCallback(c => SceneLoaderControl.instance.CurrentSceneAsset = (SceneAsset)c.newValue);
                enumField.value = SceneLoaderControl.instance.SceneLoadActionType;
                enumField.RegisterValueChangedCallback(c =>SwapState((SceneLoadType)c.newValue));
        
                // Update UI based on initial state.
                SwapState(SceneLoaderControl.instance.SceneLoadActionType);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

    }
}
