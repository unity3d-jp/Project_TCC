using UnityEditor;
using UnityEngine;
using Unity.TinyCharacterController.Attributes;

namespace TinyCharacterControllerEditor.Attributes
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        private const int NumWidth = 50;
        private const int Padding = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // EditorGUI.BeginProperty(position, label, property);

            var minRect = new Rect(position.x, position.y, NumWidth, position.height);
            var sliderRect = new Rect(minRect.x + minRect.width + Padding, position.y, position.width - NumWidth * 2 - Padding * 2, position.height);
            var maxRect = new Rect(sliderRect.x + sliderRect.width + Padding, position.y, NumWidth, position.height);

            var minmaxAttribute = (MinMaxAttribute)attribute;
            var minProperty = property.FindPropertyRelative("x");
            var maxProperty = property.FindPropertyRelative("y");

            var min = minProperty.floatValue;
            var max = maxProperty.floatValue;

            min = Mathf.Clamp(EditorGUI.FloatField(minRect, min), minmaxAttribute.Min, max);
            max = Mathf.Clamp(EditorGUI.FloatField(maxRect, max), min, minmaxAttribute.Max);
            EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, minmaxAttribute.Min, minmaxAttribute.Max);

            // Limit the number of float digits.
            min = Mathf.Round(min * 1000) / 1000;
            max = Mathf.Round(max * 1000) / 1000;

            minProperty.floatValue = min;
            maxProperty.floatValue = max;

            // EditorGUI.EndProperty();
        }
    }
}
