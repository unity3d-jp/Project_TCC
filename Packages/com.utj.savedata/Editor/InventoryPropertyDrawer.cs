using Unity.SaveData;
using UnityEditor;
using UnityEngine;

namespace Unity.SaveData
{
    [CustomPropertyDrawer(typeof(InventoryPropertyAttribute))]
    public class InventoryPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var nameProp = property.FindPropertyRelative("_name");
            using var countProp = property.FindPropertyRelative("Count");

            var namePosition = CalculateNamePosition(position);
            var countPosition = CalculateCountPosition(position);

            using var changed = new EditorGUI.ChangeCheckScope();
            var newName = DrawNameField(namePosition, nameProp.stringValue);
            var newCount = DrawCountField(countPosition, countProp.intValue);

            if (changed.changed)
            {
                ApplyChanges(nameProp, newName, countProp, newCount);
            }
        }

        /// <summary>
        /// Calculates the position of the name field.
        /// </summary>
        /// <param name="position">The overall Rect.</param>
        /// <returns>The Rect for the name field.</returns>
        private Rect CalculateNamePosition(in Rect position)
        {
            var namePosition = position;
            namePosition.width = position.width - 60;
            return namePosition;
        }

        /// <summary>
        /// Calculates the position of the count field.
        /// </summary>
        /// <param name="position">The overall Rect.</param>
        /// <returns>The Rect for the count field.</returns>
        private Rect CalculateCountPosition(Rect position)
        {
            var countPosition = position;
            countPosition.width = 50;
            countPosition.x = position.width - countPosition.width;
            return countPosition;
        }

        /// <summary>
        /// Draws the name field and gets the new value.
        /// </summary>
        /// <param name="position">The drawing position.</param>
        /// <param name="value">The current value.</param>
        /// <returns>The new value.</returns>
        private string DrawNameField(in Rect position, string value)
        {
            return EditorGUI.TextField(position, value);
        }

        /// <summary>
        /// Draws the count field and gets the new value.
        /// </summary>
        /// <param name="position">The drawing position.</param>
        /// <param name="value">The current value.</param>
        /// <returns>The new value.</returns>
        private int DrawCountField(in Rect position, int value)
        {
            return EditorGUI.IntField(position, value);
        }

        /// <summary>
        /// Applies the new values to SerializedProperties if changes were made.
        /// </summary>
        /// <param name="nameProp">The SerializedProperty for the name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="countProp">The SerializedProperty for the count.</param>
        /// <param name="newCount">The new count.</param>
        private void ApplyChanges(SerializedProperty nameProp, string newName, SerializedProperty countProp, int newCount)
        {
            nameProp.stringValue = newName;
            countProp.intValue = newCount;
        }
    }
}
