using Unity.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Unity.SceneManagement
{
    [UnityEditor.CustomEditor(typeof(SceneLoader))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset _tree;

        private SceneLoader Component => target as SceneLoader;
        
        private Button _closeButton;
        private Button _editButton;

        private Button _showExplorer;

        private PropertyField _sceneAsset;
        // private Button _showButton;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = _tree.CloneTree();
            
            root.Bind(serializedObject);
            BindUI(root);

            BindButtonEvent(root);

            UpdateUIVisual();
            
            return root;
        }

        private void AddChangeSceneAction(VisualElement root)
        {
            var sceneAsset = root.Q<PropertyField>("SceneAsset");
            sceneAsset.RegisterValueChangeCallback(c =>
            {
                // EditorSceneLoaderManager.Open(Component, false, true);
                UpdateUIVisual();
            });
        }

        private void BindUI(VisualElement root)
        {
            _sceneAsset = root.Q<PropertyField>("SceneAsset");
            _closeButton = root.Q<Button>("CloseButton");
            _editButton = root.Q<Button>("EditButton");
            _showExplorer = root.Q<Button>("ShowExplorer");
            // _showButton = root.Q<Button>("ShowButton");

        }
        

        private void UpdateUIVisual()
        {
            var scene = SceneManager.GetSceneByName(Component.SceneName);
            var isLoaded = scene.isLoaded;
            var hasValue = Component.SceneAsset != null;

            if (isLoaded)
            {
                var isSubScene = scene.isSubScene;
                // _showButton.SetEnabled(!isSubScene);
                _editButton.SetEnabled(isSubScene);
            }
            else
            {
                // _showButton.SetEnabled(true);
                _editButton.SetEnabled(true);
            }
            _closeButton.SetEnabled(isLoaded);
            
            
            _sceneAsset.style.backgroundColor = hasValue ? Color.clear : Color.red;
        }
        

        private void BindButtonEvent(VisualElement element)
        {

            _closeButton.clicked += () =>
            {

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    var scene = SceneManager.GetSceneByName(Component.SceneName);
                    EditorSceneLoaderManager.Close(Component);
                    EditorSceneManager.SaveScene(scene);
                    UpdateUIVisual();
                }
            };

            
            _editButton.clicked += () =>
            {
                EditorSceneLoaderManager.Open(Component, true);
                UpdateUIVisual();
            };

            _showExplorer.clicked += () =>
            {
                SceneLoaderExplorer.Initialize();
            };
        }
    }
}