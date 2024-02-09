using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class LoadSpringBoneSetupWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            var editorWindow = GetWindow<LoadSpringBoneSetupWindow>(
                "スプリングボーンセットアップを読み込む");
            if (editorWindow != null)
            {
                editorWindow.SelectObjectsFromSelection();
            }
        }

        public static T DoObjectPicker<T>
        (
            string label,
            T currentObject,
            int uiWidth,
            int uiHeight,
            ref int yPos
        ) where T : UnityEngine.Object
        {
            var uiRect = new Rect(UISpacing, yPos, LabelWidth, uiHeight);
            GUI.Label(uiRect, label, SpringBoneGUIStyles.LabelStyle);
            uiRect.x = LabelWidth + UISpacing;
            uiRect.width = uiWidth - uiRect.x + UISpacing;
            yPos += uiHeight + UISpacing;
            return EditorGUI.ObjectField(uiRect, currentObject, typeof(T), true) as T;
        }

        // private

        private const string StopPlayModeMessage = "再生モードでセットアップしないでください。";
        private const string SelectObjectRootsMessage = "スプリングボーンの親オブジェクトを指定してください。";
        private const int UIRowHeight = 24;
        private const int UISpacing = 8;
        private const int LabelWidth = 200;

        private GameObject springBoneRoot;
        private DynamicsSetup.ImportSettings importSettings;

        private void SelectObjectsFromSelection()
        {
            springBoneRoot = null;
            if (Selection.objects.Length > 0)
            {
                springBoneRoot = Selection.objects[0] as GameObject;
            }

            if (springBoneRoot == null)
            {
                var characterRootComponentTypes = new System.Type[] {
                    typeof(SpringManager),
                    typeof(Animation),
                    typeof(Animator)
                };
                springBoneRoot = characterRootComponentTypes
                    .Select(type => FindObjectOfType(type) as Component)
                    .Where(component => component != null)
                    .Select(component => component.gameObject)
                    .FirstOrDefault();
            }
        }

        private void ShowImportSettingsUI(ref Rect uiRect)
        {
            if (importSettings == null)
            {
                importSettings = new DynamicsSetup.ImportSettings();
            }

            GUI.Label(uiRect, "読み込み設定", SpringBoneGUIStyles.HeaderLabelStyle);
            uiRect.y += uiRect.height;
            importSettings.ImportSpringBones = GUI.Toggle(uiRect, importSettings.ImportSpringBones, "スプリングボーン", SpringBoneGUIStyles.ToggleStyle);
            uiRect.y += uiRect.height;
            importSettings.ImportCollision = GUI.Toggle(uiRect, importSettings.ImportCollision, "コライダー", SpringBoneGUIStyles.ToggleStyle);
            uiRect.y += uiRect.height;
        }

        private void OnGUI()
        {
            SpringBoneGUIStyles.ReacquireStyles();

            const int ButtonHeight = 30;

            var uiWidth = (int)position.width - UISpacing * 2;
            var yPos = UISpacing;
            springBoneRoot = DoObjectPicker("スプリングボーンのルート", springBoneRoot, uiWidth, UIRowHeight, ref yPos);
            var buttonRect = new Rect(UISpacing, yPos, uiWidth, ButtonHeight);
            if (GUI.Button(buttonRect, "選択からルートを取得", SpringBoneGUIStyles.ButtonStyle))
            {
                SelectObjectsFromSelection();
            }
            yPos += ButtonHeight + UISpacing;
            buttonRect.y = yPos;

            ShowImportSettingsUI(ref buttonRect);

            string errorMessage;
            if (IsOkayToSetup(out errorMessage))
            {
                if (GUI.Button(buttonRect, "CSVを読み込んでセットアップ", SpringBoneGUIStyles.ButtonStyle))
                {
                    BrowseAndLoadSpringSetup();
                }
            }
            else
            {
                const int MessageHeight = 24;
                var uiRect = new Rect(UISpacing, buttonRect.y, uiWidth, MessageHeight);
                GUI.Label(uiRect, errorMessage, SpringBoneGUIStyles.HeaderLabelStyle);
            }
        }

        private bool IsOkayToSetup(out string errorMessage)
        {
            errorMessage = "";
            if (EditorApplication.isPlaying)
            {
                errorMessage = StopPlayModeMessage;
                return false;
            }

            if (springBoneRoot == null)
            {
                errorMessage = SelectObjectRootsMessage;
                return false;
            }
            return true;
        }

        private static T FindHighestComponentInHierarchy<T>(GameObject startObject) where T : Component
        {
            T highestComponent = null;
            if (startObject != null)
            {
                var transform = startObject.transform;
                while (transform != null)
                {
                    var component = transform.GetComponent<T>();
                    if (component != null) { highestComponent = component; }
                    transform = transform.parent;
                }
            }
            return highestComponent;
        }

        private class BuildDynamicsAction : SpringBoneSetupErrorWindow.IConfirmAction
        {
            public BuildDynamicsAction
            (
                DynamicsSetup newSetup,
                string newPath,
                GameObject newSpringBoneRoot
            )
            {
                setup = newSetup;
                path = newPath;
                springBoneRoot = newSpringBoneRoot;
            }

            public void Perform()
            {
                setup.Build();
                AssetDatabase.Refresh();

                const string ResultFormat = "セットアップ完了: {0}\nボーン数: {1} コライダー数: {2}";
                var boneCount = springBoneRoot.GetComponentsInChildren<SpringBone>(true).Length;
                var colliderCount = SpringColliderSetup.GetColliderTypes()
                    .Sum(type => springBoneRoot.GetComponentsInChildren(type, true).Length);
                var resultMessage = string.Format(ResultFormat, path, boneCount, colliderCount);
                Debug.Log(resultMessage);
            }

            private DynamicsSetup setup;
            private string path;
            private GameObject springBoneRoot;
        }

        private void BrowseAndLoadSpringSetup()
        {
            string checkErrorMessage;
            if (!IsOkayToSetup(out checkErrorMessage))
            {
                Debug.LogError(checkErrorMessage);
                return;
            }

            // var initialPath = "";
            var initialDirectory = ""; // System.IO.Path.GetDirectoryName(initialPath);
            var fileFilters = new string[] { "CSVファイル", "csv", "テキストファイル", "txt" };
            var path = EditorUtility.OpenFilePanelWithFilters(
                "スプリングボーンセットアップを読み込む", initialDirectory, fileFilters);
            if (path.Length == 0) { return; }

            var sourceText = FileUtil.ReadAllText(path);
            if (string.IsNullOrEmpty(sourceText)) { return; }

            var parsedSetup = DynamicsSetup.ParseFromRecordText(springBoneRoot, springBoneRoot, sourceText, importSettings);
            if (parsedSetup.Setup != null)
            {
                var buildAction = new BuildDynamicsAction(parsedSetup.Setup, path, springBoneRoot);
                if (parsedSetup.HasErrors)
                {
                    SpringBoneSetupErrorWindow.ShowWindow(springBoneRoot, springBoneRoot, path, parsedSetup.Errors, buildAction);
                }
                else
                {
                    buildAction.Perform();
                }
            }
            else
            {
                const string ErrorFormat =
                    "スプリングボーンセットアップが失敗しました。\n"
                    + "元データにエラーがあるか、もしくは\n"
                    + "キャラクターにデータが一致しません。\n"
                    + "詳しくはConsoleのログをご覧下さい。\n\n"
                    + "キャラクター: {0}\n\n"
                    + "パス: {1}";
                var resultErrorMessage = string.Format(ErrorFormat, springBoneRoot.name, path);
                EditorUtility.DisplayDialog("スプリングボーンセットアップ", resultErrorMessage, "OK");
                Debug.LogError("スプリングボーンセットアップ失敗: " + springBoneRoot.name + "\n" + path);
            }
            Close();
        }
    }
}