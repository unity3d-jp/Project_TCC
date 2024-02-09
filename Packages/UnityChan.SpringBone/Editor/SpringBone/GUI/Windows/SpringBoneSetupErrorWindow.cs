using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class SpringBoneSetupErrorWindow : EditorWindow
    {
        public interface IConfirmAction
        {
            void Perform();
        }

        public static void ShowWindow
        (
            GameObject springBoneRoot,
            GameObject colliderRoot,
            string path, 
            IEnumerable<DynamicsSetup.ParseMessage> errors, 
            IConfirmAction onConfirm
        )
        {
            var window = GetWindow<SpringBoneSetupErrorWindow>("ダイナミクスセットアップ");
            window.springBoneRoot = springBoneRoot;
            window.colliderRoot = colliderRoot;
            window.filePath = path;
            window.onConfirmAction = onConfirm;
            window.errors = errors.ToArray();
        }

        // private

        private GameObject springBoneRoot;
        private GameObject colliderRoot;
        private string filePath;
        private IConfirmAction onConfirmAction;
        private DynamicsSetup.ParseMessage[] errors;
        private Vector2 scrollPosition;

        private void OnGUI()
        {
            EditorGUILayout.Space();
            GUILayout.Label("ダイナミクスセットアップに一部エラーが出ているものがあります。正常なものだけ作成しますか？");
            EditorGUILayout.Space();
            EditorGUILayout.ObjectField("スプリングボーンのルート", springBoneRoot, typeof(GameObject), true);
            EditorGUILayout.ObjectField("コライダーのルート", colliderRoot, typeof(GameObject), true);
            EditorGUILayout.TextField("パス", filePath);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("作成"))
            {
                onConfirmAction.Perform();
                Close();
            }
            if (GUILayout.Button("キャンセル")) { Close(); }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Label("エラー");
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
            foreach (var error in errors)
            {
                var errorString = error.Message;
                if (!string.IsNullOrEmpty(error.SourceLine))
                {
                    errorString += "\n" + error.SourceLine;
                }
                GUILayout.Label(errorString);
            }
            GUILayout.EndScrollView();
        }
    }
}