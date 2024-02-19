using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Unity.SceneManagement
{
    /// <summary>
    /// Class to extend the Hierarchy window.
    /// </summary>
    public static class SceneLoaderHierarchyExtensions
    {
        private static Texture2D _icon;
        private const string IconGuid = "78d70be841cdde3489032af5538d631b"; // Icon_SubScene.pngのGUID

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGui;
            EditorApplication.quitting += OnQuit;
        }

        private static void OnQuit()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGui;
        }

        private static void OnHierarchyGui(int instanceId, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            
            // Skip if the GameObject is null or doesn't have a SceneLoader component
            if (gameObject == null || !gameObject.TryGetComponent(out SceneLoader loader))  
                return;

            LoadIcon();
            DrawIcon(selectionRect);
            
            // Draw toggle only when the application is not playing
            if (Application.isPlaying == false)
                DrawToggle(loader, selectionRect);
        }

        private static void LoadIcon()
        {
            if (_icon != null) 
                return;
            
            var path = AssetDatabase.GUIDToAssetPath(IconGuid);
            _icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static void DrawIcon(Rect selectionRect)
        {
            var iconRect = new Rect(selectionRect.x, selectionRect.y, 16, 16);
            GUI.DrawTexture(iconRect, _icon);
        }

        private static void DrawToggle(SceneLoader loader, Rect selectionRect)
        {
            // Check if the scene associated with the loader is valid and loaded
            var scene = SceneManager.GetSceneByName(loader.SceneName);
            var loaded = scene.IsValid() && scene.isLoaded;

            // Position the toggle button
            var rect = new Rect(selectionRect.x + selectionRect.width - 16, selectionRect.y, 16, selectionRect.height);
            using var valueChanged = new EditorGUI.ChangeCheckScope();
            var newValue = EditorGUI.Toggle(rect, loaded);
            // Only proceed if the toggle value has changed
            if (!valueChanged.changed) 
                return;
            
            // Load or unload the scene based on the toggle value
            if (newValue)
                EditorSceneLoaderManager.Open(loader, true);
            else
                EditorSceneLoaderManager.Close(loader);
        }
    }
}
