using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Utility
{
    /// <summary>
    /// Inspector extension for GameObjectFolder.
    /// </summary>
    [CustomEditor(typeof(GameObjectFolder))]
    internal class GameObjectFolderEditor : UnityEditor.Editor
    {
        private const string UxmlGuid = "e1147034edff44d72965812e5b49a6ed";
        
        public override VisualElement CreateInspectorGUI()
        {
            // Load the asset and clone the UI.
            var path = AssetDatabase.GUIDToAssetPath(UxmlGuid);
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            var root = asset.CloneTree();
            
            // Bind the UI to the object.
            root.Bind(serializedObject);

            return root;
        }

        [MenuItem("GameObject/Folder")]
        public static void CreateMenu()
        {
            // Create a GameObject and add a GameObjectFolder.
            var obj = new GameObject("Folder", typeof(GameObjectFolder));
            obj.transform.position = Vector3.zero;
            obj.transform.hideFlags = HideFlags.HideInInspector;
            
            // Select the created object.
            Selection.activeGameObject = obj;
            
            // Add Undo for the creation.
            Undo.RegisterCreatedObjectUndo(obj, "Create GameObjectFolder Object");
        }
    }
}