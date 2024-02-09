using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.Utility
{
    /// <summary>
    /// Allows viewing components with GameObjectFolder set in the Hierarchy.
    /// </summary>
    public class GameObjectFolderHierarchyGUI
    {
        /// <summary>
        /// Label style.
        /// </summary>
        private GUIStyle _labelStyle;

        /// <summary>
        /// Texture for displaying Hierarchy background.
        /// </summary>
        private Texture _texture = null;
        
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            var drawer = new GameObjectFolderHierarchyGUI();
            
            EditorApplication.hierarchyWindowItemOnGUI += drawer.OnGui;
        }
        
        /// <summary>
        /// Creates a white texture.
        /// </summary>
        /// <returns>White texture</returns>
        private static Texture CreateWhiteTexture()
        {
            var texture = new Texture2D(1, 1);

            // Set color and apply to pixel
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            
            return texture;
        }

        private void OnGui(int instanceId, Rect selectionRect)
        {
            // Get GameObject from instance ID, and abort if there's no GameObject.
            if (TryGameObjectFromInstanceId(instanceId, out var gameObject) == false)
                return;

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            
            if (_texture == null)
                _texture = CreateWhiteTexture();

            if (gameObject.TryGetComponent(out GameObjectFolder component))
                LineMarkObjectWithChildren(ref component, selectionRect);
        }

        /// <summary>
        /// Get GameObject from InstanceID.
        /// </summary>
        /// <param name="instanceId">Instance ID</param>
        /// <param name="obj">GameObject</param>
        /// <returns>True if successful</returns>
        private static bool TryGameObjectFromInstanceId(int instanceId, out GameObject obj)
        {
            obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            return obj != null;
        }

        /// <summary>
        /// Draw the GUI.
        /// </summary>
        /// <param name="component">Component to display</param>
        /// <param name="selectionRect">UI area</param>
        private void LineMarkObjectWithChildren(ref GameObjectFolder component, in Rect selectionRect)
        {
            using var serializedObject = new SerializedObject(component);
            using var menuColorProperty = serializedObject.FindProperty("_menuColor");
            using var isVisibleProperty = serializedObject.FindProperty("_isVisible");
            
            // Fill the background with a color
            FillBackground(menuColorProperty.colorValue, selectionRect);
            
            // Display text in the Hierarchy label
            DrawLabel(menuColorProperty.colorValue, component.name, selectionRect);

            // Draw toggle for showing/hiding objects inside
            DrawToggle(component.ChildObjects, selectionRect, isVisibleProperty, serializedObject);
        }

        /// <summary>
        /// Draw a toggle to show/hide the UI.
        /// </summary>
        /// <param name="childObjects">Components to process</param>
        /// <param name="isVisible"></param>
        /// <param name="selectionRect">UI area</param>
        /// <param name="serializedObject"></param>
        private static void DrawToggle(in List<GameObject> childObjects, in Rect selectionRect,  
            SerializedProperty isVisible, SerializedObject serializedObject)
        {
            var rect = selectionRect;
            rect.width = 16;
            rect.x += 2;

            using var change = new EditorGUI.ChangeCheckScope();
            var newValue = GUI.Toggle(rect, isVisible.boolValue, "");
            if (change.changed == false)
                return;

            isVisible.boolValue = newValue;
            serializedObject.ApplyModifiedProperties();

            // If the check is changed during gameplay, switch the visibility in the Hierarchy
            if (Application.isPlaying == false)
                return;

            for (var i = 0; i < childObjects.Count; i++)
            {
                childObjects[i].hideFlags = isVisible.boolValue ? HideFlags.None : HideFlags.HideInHierarchy;
            }
        }

        /// <summary>
        /// Fill the background with a solid color.
        /// </summary>
        /// <param name="menuColor">Menu color</param>
        /// <param name="selectionRect">UI area</param>
        private void FillBackground(in Color menuColor, in Rect selectionRect)
        {
            GUI.DrawTexture(selectionRect, _texture, ScaleMode.StretchToFill,
                false, 0, menuColor, 0, 0);
        }

        /// <summary>
        /// Display text with a color that is easy to read from the MenuColor background.
        /// </summary>
        /// <param name="menuColor">Menu color</param>
        /// <param name="label">Text to display</param>
        /// <param name="selectionRect">UI area</param>
        private void DrawLabel(in Color menuColor, in string label, in Rect selectionRect)
        {
            var fontColor = menuColor.grayscale < 0.5f ? Color.white : Color.black;
            var hoverFontColor = Color.Lerp(fontColor, new Color(0.8f, 0.8f, 0.8f), 0.5f);
            _labelStyle.normal.textColor = fontColor;
            _labelStyle.hover.textColor = hoverFontColor;
            GUI.Label(selectionRect, label, _labelStyle);
        }
    }
}
