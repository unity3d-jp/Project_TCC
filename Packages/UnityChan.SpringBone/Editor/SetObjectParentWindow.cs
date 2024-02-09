using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class SetObjectParentWindow : EditorWindow
    {
        [MenuItem("UTJ/親子付け")]
        public static void ShowWindow()
        {
            GetWindow<SetObjectParentWindow>("親子付け");
        }

        // private

        private Transform newParent;

        private void ReparentSelectedObjects()
        {
            var newChildren = Selection.gameObjects
                .Where(item => item.transform != newParent)
                .Select(gameObject => gameObject.transform)
                .ToArray();
            foreach (var child in newChildren)
            {
                Undo.SetTransformParent(child, newParent, "Set Parent");
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            newParent = EditorGUILayout.ObjectField("新しい親", newParent, typeof(Transform), true) as Transform;
            EditorGUILayout.Space();
            if (GUILayout.Button("親子付け"))
            {
                ReparentSelectedObjects();
            }
        }
   }
}