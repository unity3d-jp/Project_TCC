using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class SpringBoneWindow : EditorWindow
    {
        [MenuItem("UTJ/スプリングボーン窓")]
        public static void ShowWindow()
        {
            var window = GetWindow<SpringBoneWindow>("スプリングボーン");
            window.OnShow();
        }

        // private

        private GUIElements.Column mainUI;
        private Vector2 scrollPosition;

        private Texture headerIcon;
        private Texture newDocumentIcon;
        private Texture openDocumentIcon;
        private Texture saveDocumentIcon;
        private Texture deleteIcon;
        private Texture pivotIcon;
        private Texture sphereIcon;
        private Texture capsuleIcon;
        private Texture panelIcon;

        private SpringBoneSettings settings;

        private static Texture LoadIcon(string iconDirectory, string filename)
        {
            var iconPath = PathUtil.CombinePath(iconDirectory, filename);
            var iconTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconPath);
            if (iconTexture == null)
            {
                Debug.LogWarning("アイコン読み込み失敗:\n" + iconPath);
            }
            return iconTexture;
        }

        private static string FindIconAssetDirectory()
        {
            // Try to find the icons in a way such that the user can put the Dynamics folder anywhere
            return DirectoryUtil.GetFilesRecursively(Application.dataPath, "SpringCapsuleIcon.tga")
                .Select(path => PathUtil.NormalizePath(path))
                .Where(path => path.ToLowerInvariant().Contains("editor/springbone/gui/icons/"))
                .Select(path => PathUtil.SystemPathToAssetPath(System.IO.Path.GetDirectoryName(path)))
                .FirstOrDefault();
        }

        private void InitializeIcons()
        {
            if (headerIcon != null) { return; }

            var iconDirectory = FindIconAssetDirectory();
            if (iconDirectory == null)
            {
                Debug.LogWarning("SpringBoneWindowのアイコンディレクトリーが見つかりません");
                return;
            }

            headerIcon = LoadIcon(iconDirectory, "SpringIcon.tga");
            newDocumentIcon = LoadIcon(iconDirectory, "NewDocumentHS.png");
            openDocumentIcon = LoadIcon(iconDirectory, "OpenHH.bmp");
            saveDocumentIcon = LoadIcon(iconDirectory, "SaveHH.bmp");
            deleteIcon = LoadIcon(iconDirectory, "Delete.png");
            pivotIcon = LoadIcon(iconDirectory, "Pivot.png");
            sphereIcon = LoadIcon(iconDirectory, "SpringSphereIcon.tga");
            capsuleIcon = LoadIcon(iconDirectory, "SpringCapsuleIcon.tga");
            panelIcon = LoadIcon(iconDirectory, "SpringPanelIcon.tga");
        }

        private void InitializeButtonGroups()
        {
            if (mainUI != null) { return; }

            const float BigButtonHeight = 60f;

            System.Func<GUIStyle> headerLabelStyleProvider = () => SpringBoneGUIStyles.HeaderLabelStyle;
            System.Func<GUIStyle> buttonLabelStyleProvider = () => SpringBoneGUIStyles.MiddleLeftJustifiedLabelStyle;

            mainUI = new GUIElements.Column(new GUIElements.IElement[]
            {
                new GUIElements.Column(new GUIElements.IElement[]
                {
                    new GUIElements.Label("ダイナミクスCSV", headerLabelStyleProvider),
                    new GUIElements.Row(new GUIElements.IElement[]
                    {
                        new GUIElements.Button("読み込む", LoadSpringBoneSetupWindow.ShowWindow, openDocumentIcon, buttonLabelStyleProvider),
                        new GUIElements.Button("保存", SaveSpringBoneSetupWindow.ShowWindow, saveDocumentIcon, buttonLabelStyleProvider)
                    },
                    BigButtonHeight)
                }),

                new GUIElements.Column(new GUIElements.IElement[]
                {
                    new GUIElements.Label("スプリングボーン", headerLabelStyleProvider),
                    new GUIElements.Row(new GUIElements.IElement[]
                    {
                        new GUIElements.Button("スプリング\nボーン追加", SpringBoneEditorActions.AssignSpringBonesRecursively, headerIcon, buttonLabelStyleProvider),
                        new GUIElements.Button("基点作成", SpringBoneEditorActions.CreatePivotForSpringBones, pivotIcon, buttonLabelStyleProvider)
                    },
                    BigButtonHeight),
                    new GUIElements.Button("マネージャーを作成／更新", SpringBoneEditorActions.AddToOrUpdateSpringManagerInSelection, newDocumentIcon, buttonLabelStyleProvider),
                    //new GUIElements.Button("初期セットアップを行う", SpringBoneAutoSetupWindow.ShowWindow, newDocumentIcon, buttonLabelStyleProvider),
                    //new GUIElements.Button("初期ボーンリストに合わせる", SpringBoneEditorActions.PromptToUpdateSpringBonesFromList, null, buttonLabelStyleProvider),
                    new GUIElements.Separator(),
                    new GUIElements.Button("スプリングボーンをミラー", MirrorSpringBoneWindow.ShowWindow, null, buttonLabelStyleProvider),
                    new GUIElements.Button("選択と子供のスプリングボーンを選択", SpringBoneEditorActions.SelectChildSpringBones, null, buttonLabelStyleProvider),
                    new GUIElements.Button("選択スプリングボーンを削除", SpringBoneEditorActions.DeleteSelectedBones, deleteIcon, buttonLabelStyleProvider),
                    new GUIElements.Button("選択と子供のマネージャーとボーンを削除", SpringBoneEditorActions.DeleteSpringBonesAndManagers, deleteIcon, buttonLabelStyleProvider),
                }),

                new GUIElements.Column(new GUIElements.IElement[]
                {
                    new GUIElements.Label("コリジョン", headerLabelStyleProvider),
                    new GUIElements.Row(new GUIElements.IElement[]
                    {
                        new GUIElements.Button("球体", SpringColliderEditorActions.CreateSphereColliderBeneathSelectedObjects, sphereIcon, buttonLabelStyleProvider),
                        new GUIElements.Button("カプセル", SpringColliderEditorActions.CreateCapsuleColliderBeneathSelectedObjects, capsuleIcon, buttonLabelStyleProvider),
                        new GUIElements.Button("板", SpringColliderEditorActions.CreatePanelColliderBeneathSelectedObjects, panelIcon, buttonLabelStyleProvider),
                    },
                    BigButtonHeight),
                    new GUIElements.Button("カプセルの位置を親に合わせる", SpringColliderEditorActions.AlignSelectedCapsulesToParents, capsuleIcon, buttonLabelStyleProvider),
                    new GUIElements.Button("スプリングボーンからコリジョンを外す", SpringColliderEditorActions.DeleteCollidersFromSelectedSpringBones, deleteIcon, buttonLabelStyleProvider),
                    new GUIElements.Button("選択と子供のコライダーを削除", SpringColliderEditorActions.DeleteAllChildCollidersFromSelection, deleteIcon, buttonLabelStyleProvider),
                    new GUIElements.Button("クリーンナップ", SpringColliderEditorActions.CleanUpDynamics, deleteIcon, buttonLabelStyleProvider)
                })
            },
            false,
            0f);
        }

        private Rect GetScrollContentsRect()
        {
            const int ScrollbarWidth = 24;
            var width = position.width - GUIElements.Spacing - ScrollbarWidth;
            var height = mainUI.Height;
            return new Rect(0f, 0f, width, height);
        }

        private void OnGUI()
        {
            if (settings == null) { LoadSettings(); }

            SpringBoneGUIStyles.ReacquireStyles();
            InitializeIcons();
            InitializeButtonGroups();

            var xPos = GUIElements.Spacing;
            var yPos = GUIElements.Spacing;
            var scrollContentsRect = GetScrollContentsRect();
            yPos = ShowHeaderUI(xPos, yPos, scrollContentsRect.width);
            var scrollViewRect = new Rect(0f, yPos, position.width, position.height - yPos);
            scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, scrollContentsRect);
            mainUI.DoUI(GUIElements.Spacing, 0f, scrollContentsRect.width);
            GUI.EndScrollView();

            ApplySettings();
        }

        private static void DrawHeaderIcon
        (
            ref Rect containerRect,
            Texture iconTexture,
            int iconDrawSize,
            int spacing = 4
        )
        {
            if (iconTexture != null
                && containerRect.width >= iconDrawSize * 3)
            {
                var iconYPosition = containerRect.y + (containerRect.height - iconDrawSize) / 2;
                var iconRect = new Rect(containerRect.x, iconYPosition, iconDrawSize, iconDrawSize);
                GUI.DrawTexture(iconRect, iconTexture);

                var xOffset = iconDrawSize + spacing;
                containerRect.x += xOffset;
                containerRect.width -= xOffset;
            }
        }

        private float ShowHeaderUI(float xPos, float yPos, float uiWidth)
        {
            var needToRepaint = false;
            System.Func<GUIStyle> headerLabelStyleProvider = () => SpringBoneGUIStyles.HeaderLabelStyle;
            System.Func<GUIStyle> toggleStyleProvider = () => SpringBoneGUIStyles.ToggleStyle;
            var headerColumn = new GUIElements.Column(
                new GUIElements.IElement[] {
                    new GUIElements.Label("表示", headerLabelStyleProvider),
                    new GUIElements.Row(new GUIElements.IElement[]
                        {
                            new GUIElements.Toggle("選択ボーンのみ表示", () => settings.onlyShowSelectedBones, newValue => { settings.onlyShowSelectedBones = newValue; needToRepaint = true; }, toggleStyleProvider),
                            new GUIElements.Toggle("ボーンのコリジョンを表示", () => settings.showBoneSpheres, newValue => { settings.showBoneSpheres = newValue; needToRepaint = true; }, toggleStyleProvider),
                        },
                        GUIElements.RowHeight),
                    new GUIElements.Row(new GUIElements.IElement[]
                        {
                            new GUIElements.Toggle("選択コライダーのみ表示", () => settings.onlyShowSelectedColliders, newValue => { settings.onlyShowSelectedColliders = newValue; needToRepaint = true; }, toggleStyleProvider),
                            new GUIElements.Toggle("ボーン名を表示", () => settings.showBoneNames, newValue => { settings.showBoneNames = newValue; needToRepaint = true; }, toggleStyleProvider)
                        },
                        GUIElements.RowHeight),
                },
                true, 4f, 0f);
            headerColumn.DoUI(xPos, yPos, uiWidth);
            if (needToRepaint)
            {
                ApplySettings();
                SaveSettings();
                SceneView.RepaintAll();
            }

            return yPos + headerColumn.Height + GUIElements.Spacing;
        }

        private void ApplySettings()
        {
            SpringManager.onlyShowSelectedBones = settings.onlyShowSelectedBones;
            SpringManager.showBoneSpheres = settings.showBoneSpheres;
            SpringManager.onlyShowSelectedColliders = settings.onlyShowSelectedColliders;
            SpringManager.showBoneNames = settings.showBoneNames;
        }

#if false
        private static string GetSettingsFilePath()
        {
            const string SettingsFileName = "SpringBoneWindow.json";
            return ProjectPaths.GetUserPreferencesPath(SettingsFileName);
        }

        private void LoadSettings()
        {
            var settingPath = GetSettingsFilePath();
            if (System.IO.File.Exists(settingPath))
            {
                var settingText = FileUtil.ReadAllText(settingPath);
                if (settingText.Length > 0)
                {
                    settings = JsonUtility.FromJson<SpringBoneSettings>(settingText);
                }
            }
            if (settings == null)
            {
                settings = SpringBoneSettings.GetDefaultSettings();
            }
        }

        private void SaveSettings()
        {
            if (settings == null) { return; }
            var settingText = JsonUtility.ToJson(settings);
            FileUtil.WriteAllText(GetSettingsFilePath(), settingText);
        }

        private void OnDestroy()
        {
            SaveSettings();
        }
#else
        // Todo: Get a good settings path
        private void LoadSettings()
        {
            if (settings == null)
            {
                settings = SpringBoneSettings.GetDefaultSettings();
            }
        }

        private void SaveSettings()
        {
            // NYI
        }
#endif

        private void OnShow()
        {
            LoadSettings();
        }

        [System.Serializable]
        private class SpringBoneSettings
        {
            public bool onlyShowSelectedBones;
            public bool onlyShowSelectedColliders;
            public bool showBoneSpheres;
            public bool showBoneNames;

            public static SpringBoneSettings GetDefaultSettings()
            {
                return new SpringBoneSettings
                {
                    onlyShowSelectedBones = true,
                    onlyShowSelectedColliders = true,
                    showBoneSpheres = true,
                    showBoneNames = false
                };
            }
        }
    }
}