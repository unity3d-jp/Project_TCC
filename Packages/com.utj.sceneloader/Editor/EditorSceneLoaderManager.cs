using Unity.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.SceneManagement
{
    /// <summary>
    /// A class for managing scenes within the editor.
    /// </summary>
    public static class EditorSceneLoaderManager
    {
        private static bool _initialized = false;

        /// <summary>
        /// Initialize during editor load.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;

            // Register events for scene open, close, and editor quitting
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosing += OnSceneClosing;
            EditorApplication.quitting += OnQuit;
            EditorApplication.playModeStateChanged += OnChangePlayModeState;
        }

        /// <summary>
        /// Handle editor quitting by unregistering events.
        /// </summary>
        private static void OnQuit()
        {
            // Unregister events
            EditorApplication.quitting -= OnQuit;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosing -= OnSceneClosing;
            EditorApplication.playModeStateChanged -= OnChangePlayModeState;

            _initialized = false;
        }

        /// <summary>
        /// Handle changes in play mode state.
        /// </summary>
        /// <param name="state">The play mode state.</param>
        private static void OnChangePlayModeState(PlayModeStateChange state)
        {
            // Process when exiting play mode
            if (state == PlayModeStateChange.ExitingEditMode &&
                SceneLoaderControl.instance.SceneLoadActionType != SceneLoadType.KeepAll)
            {
                if (SceneManager.sceneCount > 1)
                {
                    // Get currently open scenes and prompt for saving if changes are detected
                    var scenes = new Scene[SceneManager.sceneCount - 1];
                    for (var i = 1; i < scenes.Length; i++)
                        scenes[i] = SceneManager.GetSceneAt(i);

                    if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(scenes) == false)
                        EditorApplication.isPlaying = false;
                }
            }
        }

        /// <summary>
        /// Handle scene closing.
        /// </summary>
        /// <param name="scene">The scene being closed.</param>
        /// <param name="removingScene">Whether the scene is being removed.</param>
        private static void OnSceneClosing(Scene scene, bool removingScene)
        {
            // Don't process when in play mode, changing play mode, or if the scene is not loaded or valid
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying ||
                scene.isLoaded == false || scene.IsValid() == false)
                return;

            // Don't process if the scene is currently being reloaded
            if (EditorSceneManager.IsReloading(scene))
                return;

            // Close all SceneLoader components in the scene
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var childSubScene in root.GetComponentsInChildren<SceneLoader>(true))
                {
                    Close(childSubScene);
                }
            }
        }

        /// <summary>
        /// Handle scene opening.
        /// </summary>
        /// <param name="scene">The opened scene.</param>
        /// <param name="mode">The scene open mode.</param>
        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            // Don't process when in play mode, changing play mode, if the scene is not loaded or valid,
            // or if the scene is added additively without loading
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying ||
                scene.isLoaded == false || scene.IsValid() == false || mode == OpenSceneMode.AdditiveWithoutLoading)
                return;

            // Open all SceneLoader components in the scene
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var childSubScene in root.GetComponentsInChildren<SceneLoader>(true))
                {
                    Open(childSubScene);
                }
            }
        }

        /// <summary>
        /// Open a scene using the specified SceneLoader.
        /// </summary>
        /// <param name="subSceneLoader">The SceneLoader to use.</param>
        /// <param name="forceOpen">Whether to forcefully open the scene.</param>
        public static void Open(SceneLoader subSceneLoader, bool forceOpen = false)
        {
            // Don't proceed if no scene asset is set
            if (subSceneLoader.SceneAsset == null)
                return;

            // If not forcefully open and the scene state is "NotLoad," don't proceed
            if (forceOpen == false && subSceneLoader.state == SceneLoader.State.NotLoad)
                return;

            // Load the scene and recursively call Open on the loaded scene
            var scene = LoadScene(subSceneLoader);
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var childSubScene in root.GetComponentsInChildren<SceneLoader>(true))
                {
                    Open(childSubScene);
                }
            }
        }

        /// <summary>
        /// Load the scene associated with the specified SceneLoader.
        /// </summary>
        /// <param name="subSceneLoader">The SceneLoader for which to load the scene.</param>
        /// <returns>The loaded scene.</returns>
        private static Scene LoadScene(SceneLoader subSceneLoader)
        {
            // Get the scene name and return it if the scene is already loaded
            using var subSceneSo = new SerializedObject(subSceneLoader);
            using var sceneName = subSceneSo.FindProperty("_sceneName");

            var subSceneName = sceneName.stringValue;
            var loadedScene = SceneManager.GetSceneByName(subSceneName);

            if (loadedScene.isLoaded)
            {
                return loadedScene;
            }
            else
            {
                // If the scene is not loaded, open it using the scene asset's path
                var path = AssetDatabase.GetAssetPath(subSceneLoader.SceneAsset);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                return scene;
            }
        }

        /// <summary>
        /// Close the scene associated with the specified SceneLoader.
        /// </summary>
        /// <param name="subSceneLoader">The SceneLoader for which to close the scene.</param>
        public static void Close(SceneLoader subSceneLoader)
        {
            // Show a dialog to prompt for saving the scene
            // Get the scene name and close the loaded scene
            using var subSceneSo = new SerializedObject(subSceneLoader);
            using var sceneName = subSceneSo.FindProperty("_sceneName");
            var subSceneName = sceneName.stringValue;

            if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[] { SceneManager.GetSceneByName(subSceneName) }) == false)
                return;


            var scene = SceneManager.GetSceneByName(subSceneName);

            if (scene.IsValid() && scene.isLoaded)
            {
                // Close all SceneLoaders within the scene
                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var childSubScene in root.GetComponentsInChildren<SceneLoader>(true))
                    {
                        Close(childSubScene);
                    }
                }
            }

            // Unload the scene if in play mode, or close it if in edit mode
            if (EditorApplication.isPlaying)
                SceneManager.UnloadSceneAsync(scene);
            else
                EditorSceneManager.CloseScene(scene, true);

            // Update light probes
            LightProbes.Tetrahedralize();
        }
    }
}
