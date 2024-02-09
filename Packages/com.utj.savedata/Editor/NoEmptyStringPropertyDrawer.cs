using Unity.SaveData;
using UnityEditor;
using UnityEngine;

namespace Unity.SaveData
{
    [CustomPropertyDrawer(typeof(NoEmptyStringAttribute))]
    public class NoEmptyStringPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var value = property.stringValue;

            var style = GUIStyle.none;
            if (string.IsNullOrEmpty(value))
            {
                EditorGUI.DrawRect(position, Color.red);
            }
            EditorGUI.PropertyField(position, property);
        }
    }
}