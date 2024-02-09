using System.Collections;
using System.Collections.Generic;
using Unity.SaveData;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utj.SaveData
{
    [CustomEditor(typeof(SaveDataControl))]
    public class SaveDataControlEditor : Editor
    {
        // SaveDataControlEditorr.uxml
        private const string guid = "daf216a1431a04cd397eb8b871ac7193";
        
        public override VisualElement CreateInspectorGUI()
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var assetTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            var root = new VisualElement();

            assetTree.CloneTree(root);
            root.Bind(serializedObject);

            return root;
        }
    }
}
