using Unity.SaveData;
using UnityEditor;
using UnityEngine;

namespace Unity.SaveData
{
    [CustomPropertyDrawer(typeof(FlagCollectionPropertyAttribute))]
    public class FlagCollectionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var id = property.FindPropertyRelative("_id");
            using var value = property.FindPropertyRelative("_value");

            var valueRect = GetValueRect(position);
            var idRect = GetIDRect(position, valueRect);

            DrawValueField(valueRect, value);
            DrawIDField(idRect, id);
        }

        private Rect GetValueRect(Rect position)
        {
            var rect = position;
            rect.width = 15;
            return rect;
        }

        private Rect GetIDRect(Rect position, Rect valueRect)
        {
            var rect = position;
            rect.width = position.width - valueRect.width;
            rect.x = valueRect.width;
            return rect;
        }

        private void DrawValueField(Rect rect, SerializedProperty value)
        {
            EditorGUI.PropertyField(rect, value, GUIContent.none);
        }

        private void DrawIDField(Rect rect, SerializedProperty id)
        {
            EditorGUI.PropertyField(rect, id, GUIContent.none);
        }
    }
}