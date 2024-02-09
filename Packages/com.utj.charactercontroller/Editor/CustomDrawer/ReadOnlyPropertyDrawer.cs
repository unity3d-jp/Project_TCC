using Unity.TinyCharacterController.Attributes;
using UnityEditor;
using UnityEngine;

namespace TinyCharacterControllerEditor.Attributes
{
    [CustomPropertyDrawer(typeof(ReadOnlyOnRuntimeAttribute))]
    internal class ReadOnlyPropertyDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}