using Unity.TinyCharacterController.Attributes;
using UnityEditor;
using UnityEngine;

namespace TinyCharacterControllerEditor.Attributes
{
    [CustomPropertyDrawer(typeof(RequestField))]
    public class RequestFieldEditor : PropertyDrawer
    {
        private static readonly Color ErrorFieldColor = new Color(0.7f, 0, 0);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if( IsShowError(property))
                EditorGUI.DrawRect(position, ErrorFieldColor);

            base.OnGUI(position, property, label);
            EditorGUI.PropertyField(position, property);
        }

        private static bool IsShowError(SerializedProperty property)
        {
            return property.objectReferenceValue == null;
        }
    }
}