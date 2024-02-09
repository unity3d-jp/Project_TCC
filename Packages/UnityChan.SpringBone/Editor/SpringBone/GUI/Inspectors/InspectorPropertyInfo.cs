using UnityEditor;
using UnityEngine;

namespace UTJ
{
    namespace Inspector
    {
        public class PropertyInfo
        {
            public SerializedProperty serializedProperty;

            public PropertyInfo(string newName, string labelText)
            {
                name = newName;
                label = new GUIContent(labelText);
            }

            public void Initialize(SerializedObject serializedObject)
            {
                serializedProperty = serializedObject.FindProperty(name);
            }

            public virtual void Show()
            {
                EditorGUILayout.PropertyField(serializedProperty, label, true, null);
            }

            protected GUIContent label;

            private string name;
        }
    }
}