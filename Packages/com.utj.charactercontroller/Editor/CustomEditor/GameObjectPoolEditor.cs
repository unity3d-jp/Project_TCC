using Unity.TinyCharacterController.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.TinyCharacterControllerEditor
{
    [CustomEditor(typeof(GameObjectPool))]
    public class GameObjectPoolEditor : UnityEditor.Editor
    {
        // GameObjectPoolEditor.uxml path
        private const string UiDocumentGuid = "11f5f31428330466eb72f4db9cf34668";

        public override VisualElement CreateInspectorGUI()
        {
            var path = AssetDatabase.GUIDToAssetPath(UiDocumentGuid);
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);

            var root = new VisualElement();
            visualTree.CloneTree(root);
            root.Bind(serializedObject);
            ErrorCheck(root);

            RegisterCallbacks(root);

            return root;
        }

        /// <summary>
        /// Register callbacks for each element.
        /// </summary>
        /// <param name="root">The root element.</param>
        private void RegisterCallbacks(VisualElement root)
        {
            root.Q<PropertyField>("prefab").RegisterValueChangeCallback(c => ErrorCheck(root));
            root.Q<PropertyField>("isActiveOnGet").RegisterValueChangeCallback(c => ErrorCheck(root));
            root.Q<PropertyField>("parent").RegisterValueChangeCallback(c => ErrorCheck(root));
        }

        /// <summary>
        /// Check the content and display error messages in the UI.
        /// </summary>
        /// <param name="root">The root element.</param>
        private void ErrorCheck(VisualElement root)
        {
            // Update the data.
            serializedObject.Update();

            // Collect information on necessary properties.
            var prefabProperty = serializedObject.FindProperty("_prefab");
            var isActiveOnGetProperty = serializedObject.FindProperty("_isActiveOnGet");
            var parentProperty = serializedObject.FindProperty("_parent");
            var prefab = prefabProperty.objectReferenceValue as MonoBehaviour;

            // Get the UI element to display the error message.
            var errorMessage = root.Q<Label>("ErrorMessage");

            if (prefab == null)
            {
                // Prefab is not set.
                errorMessage.style.display = DisplayStyle.Flex;
                errorMessage.text = "Please set an object in the Prefab field.";
            }
            else if (prefab.gameObject.activeSelf && isActiveOnGetProperty.boolValue == false)
            {
                // IsActiveOnGet is false, but the GameObject is active.
                errorMessage.style.display = DisplayStyle.Flex;
                errorMessage.text = "Set the Prefab object to be inactive or set IsActiveOnGet to true.";
            }
            else if (HasGraphic(prefab) && HasCanvas(parentProperty) == false)
            {
                // UI is not set under a Canvas.
                errorMessage.style.display = DisplayStyle.Flex;
                errorMessage.text = "If using UI, please register an object with a Canvas as the parent in the Parent field.";
            }
            else
            {
                errorMessage.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// Check if the Prefab contains UI graphics components.
        /// </summary>
        /// <param name="prefab">The object to check.</param>
        /// <returns>True if itself or its child objects have Graphics components.</returns>
        private static bool HasGraphic(in Component prefab)
        {
            return prefab.GetComponentInChildren<CanvasRenderer>() != null;
        }

        /// <summary>
        /// Check if the parent object has a <see cref="Canvas"/>.
        /// </summary>
        /// <param name="parentProperty">The property to check.</param>
        /// <returns>True if the parent object exists and has a Canvas component.</returns>
        private static bool HasCanvas(SerializedProperty parentProperty)
        {
            var hasCanvas = false;
            // Check the object.
            var parentObject = parentProperty.objectReferenceValue as Transform;

            // Check if the parent object has a Canvas component.
            if (parentObject != null)
                hasCanvas = parentObject.GetComponentInParent<Canvas>(true) != null;

            return hasCanvas;
        }
    }
}
