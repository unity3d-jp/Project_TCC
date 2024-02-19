using System;
using System.Collections;
using System.Collections.Generic;
using Unity.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.SceneManagement
{
    /// <summary>
    /// A class that provides a window for the Unity Editor and explores the loading status of scenes.
    /// </summary>
    public class SceneLoaderExplorer : EditorWindow
    {
        /// <summary>
        /// Initializes the window and adds it to the menu bar.
        /// </summary>
        [MenuItem("Window/Analysis/SceneLoaderExplorer")]
        public static void Initialize()
        {
            SceneLoaderExplorer.CreateWindow<SceneLoaderExplorer>();
            // FocusWindowIfItsOpen<SubSceneViewer>();
        }

        // A list to hold SceneLoader objects and their indices.
        private readonly List<(SceneLoader, int)> _sceneLoaders = new();

        /// <summary>
        /// Called when the window gains focus.
        /// </summary>
        private void OnFocus()
        {
            EditorSceneManager.sceneClosed += OnChangeScene;
            EditorSceneManager.sceneOpened += OnSceneOpen;

            UpdateSceneLoaderList();
        }

        /// <summary>
        /// Called when the window loses focus.
        /// </summary>
        private void OnLostFocus()
        {
            EditorSceneManager.sceneClosed -= OnChangeScene;
            EditorSceneManager.sceneOpened -= OnSceneOpen;
        }

        /// <summary>
        /// Called when a scene is opened.
        /// </summary>
        /// <param name="scene">The opened scene.</param>
        /// <param name="mode">The mode of opening the scene.</param>
        private void OnSceneOpen(Scene scene, OpenSceneMode mode)
        {
            UpdateSceneLoaderList();
        }

        /// <summary>
        /// Called when a scene is closed.
        /// </summary>
        /// <param name="scene">The closed scene.</param>
        private void OnChangeScene(Scene scene)
        {
            UpdateSceneLoaderList();
        }

        /// <summary>
        /// Updates the list of scene loaders.
        /// </summary>
        private void UpdateSceneLoaderList()
        {
            var topScene = SceneManager.GetSceneAt(0);

            _sceneLoaders.Clear();
            GatherSceneLoader(topScene, 0);
        }

        /// <summary>
        /// Gathers SceneLoader components in the specified scene.
        /// </summary>
        /// <param name="scene">The target scene.</param>
        /// <param name="index">The index of the scene.</param>
        private void GatherSceneLoader(Scene scene, int index)
        {
            if (index > 100 || scene.IsValid() == false || scene.isLoaded == false)
                return;

            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var sceneLoader in root.GetComponentsInChildren<SceneLoader>(true))
                {
                    _sceneLoaders.Add(new(sceneLoader, index));
                    var sceneLoaderScene = SceneManager.GetSceneByName(sceneLoader.SceneName);
                    if (sceneLoaderScene.isLoaded)
                        GatherSceneLoader(sceneLoaderScene, index + 1);
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"{SceneManager.GetSceneAt(0).name}");
            var closeRequest = new Queue<SceneLoader>();
            var openRequest = new Queue<SceneLoader>();

            foreach (var loader in _sceneLoaders)
            {
                RenderSceneLoader(loader, closeRequest, openRequest);
            }

            ProcessSceneLoaders(closeRequest, EditorSceneLoaderManager.Close);
            ProcessSceneLoaders(openRequest, loader => EditorSceneLoaderManager.Open(loader, true));
        }

        /// <summary>
        /// Renders the UI for scene loaders and adds close or open requests as needed.
        /// </summary>
        /// <param name="loader">The SceneLoader and its index.</param>
        /// <param name="closeRequest">The queue for close requests.</param>
        /// <param name="openRequest">The queue for open requests.</param>
        private void RenderSceneLoader((SceneLoader, int) loader, Queue<SceneLoader> closeRequest,
            Queue<SceneLoader> openRequest)
        {
            var scene = SceneManager.GetSceneByName(loader.Item1.SceneName);
            var isLoaded = scene.isLoaded;
            var isSubScene = scene.isSubScene;

            using (new GUILayout.HorizontalScope())
            {
                DrawActiveToggle(loader.Item1);
                EditorGUI.indentLevel = loader.Item2;

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(loader.Item1, typeof(SceneLoader), true);
                }

                RenderSceneLoaderButtons(loader.Item1, isLoaded, isSubScene, openRequest, closeRequest);
            }

            EditorGUI.indentLevel = 0;
            EditorGUILayout.Space(3);
        }

        /// <summary>
        /// Draws the active/inactive toggle for scene loaders.
        /// </summary>
        /// <param name="loader">The SceneLoader object.</param>
        private static void DrawActiveToggle(in SceneLoader loader)
        {
            using var isChange = new EditorGUI.ChangeCheckScope();

            var newValue = EditorGUILayout.Toggle(loader.gameObject.activeSelf, GUILayout.Width(15));
            if (isChange.changed == false)
                return;

            Undo.RecordObject(loader.gameObject, "change active");
            loader.gameObject.SetActive(newValue);
        }

        /// <summary>
        /// Renders the buttons for scene loaders.
        /// </summary>
        private void RenderSceneLoaderButtons(in SceneLoader loader, bool isLoaded, bool isSubScene,
            Queue<SceneLoader> openRequest, Queue<SceneLoader> closeRequest)
        {
            if (ButtonWithDisabledScope("Edit", !((isLoaded && isSubScene) || !isLoaded), 50))
            {
                openRequest.Enqueue(loader);
            }

            if (ButtonWithDisabledScope("Close", !isLoaded, 50))
            {
                closeRequest.Enqueue(loader);
            }
        }

        /// <summary>
        /// Draws a button and disables it based on the specified condition.
        /// </summary>
        /// <returns>True if the button is pressed; otherwise, false.</returns>
        private static bool ButtonWithDisabledScope(string text, bool disabled, float width)
        {
            using (new EditorGUI.DisabledScope(disabled))
            {
                return GUILayout.Button(text, GUILayout.Width(width));
            }
        }

        /// <summary>
        /// Processes the requests for scene loaders.
        /// </summary>
        /// <param name="queue">The queue of scene loaders to process.</param>
        /// <param name="action">The action to execute for each scene loader.</param>
        private static void ProcessSceneLoaders(Queue<SceneLoader> queue, Action<SceneLoader> action)
        {
            while (queue.TryDequeue(out var loader))
            {
                action(loader);
            }
        }
    }
}
