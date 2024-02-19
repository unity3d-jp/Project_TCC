using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Unity.SceneManagement
{
    /// <summary>
    /// Singleton class that manages scene loading settings.
    /// Controls the scene loading method based on the play mode state changes in the Unity editor.
    /// </summary>
    [FilePath("Library/SceneLoaderData.asset", FilePathAttribute.Location.ProjectFolder)]
    public class SceneLoaderControl : ScriptableSingleton<SceneLoaderControl>
    {
        [SerializeField] private SceneLoaderData _data = new ();

        /// <summary>
        /// Initialization method called when the editor's play mode state changes.
        /// Sets up event listeners.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged += SceneLoaderSwitcher.OnPlayModeStateChanged;
            EditorApplication.quitting += () => { instance.Save(true); };
        }

        // Property for the scene load action type
        public SceneLoadType SceneLoadActionType
        {
            get => _data.SceneLoadActionType;
            set => _data.SceneLoadActionType = value;
        }

        // Property for the current scene asset
        public SceneAsset CurrentSceneAsset
        {
            get => _data.CurrentSceneAsset;
            set => _data.CurrentSceneAsset = value;
        }
    }

    /// <summary>
    /// Class that holds scene loader configuration data.
    /// </summary>
    [Serializable]
    public class SceneLoaderData
    {
        public SceneAsset CurrentSceneAsset = null;
        public SceneLoadType SceneLoadActionType = SceneLoadType.RootOnly;
    }

    /// <summary>
    /// Enumeration defining scene load types.
    /// </summary>
    public enum SceneLoadType
    {
        RootOnly,
        UseStart,
        KeepAll,
    }

    /// <summary>
    /// Static class to switch scene loader behavior.
    /// </summary>
    public static class SceneLoaderSwitcher
    {
        /// <summary>
        /// Method called when the editor's play mode state changes.
        /// Switches the scene loading method.
        /// </summary>
        public static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            var instance = SceneLoaderControl.instance;
            
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode when instance.CurrentSceneAsset != null &&
                                                              instance.SceneLoadActionType == SceneLoadType.UseStart:
                    
                    // Set the scene to launch when playing the game to the one specified in SceneAsset
                    EditorSceneManager.playModeStartScene = instance.CurrentSceneAsset;
                    break;

                case PlayModeStateChange.ExitingEditMode when instance.SceneLoadActionType == SceneLoadType.RootOnly:
                    
                    // Set the scene to launch when playing the game to the currently highest scene in the hierarchy.
                    var rootScene = SceneManager.GetSceneAt(0);
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(rootScene.path);
                    EditorSceneManager.playModeStartScene = sceneAsset;
                    break;

                case PlayModeStateChange.ExitingEditMode when instance.SceneLoadActionType == SceneLoadType.KeepAll:
                    
                    // Clear the setting for the initial scene launch and do nothing.
                    EditorSceneManager.playModeStartScene = null;
                    break;

                case PlayModeStateChange.EnteredPlayMode when instance.SceneLoadActionType == SceneLoadType.KeepAll:
                    
                    // Expand scenes according to SceneLoader settings.
                    SceneManagement.EnableSceneLoaders();
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    
                    // Unregister the StartScene.
                    EditorSceneManager.playModeStartScene = null;
                    break;
            }
        }
    }

    /// <summary>
    /// Static class to manage scenes.
    /// </summary>
    public static class SceneManagement
    {
        /// <summary>
        /// Method to enable scene loader objects present in the scene.
        /// </summary>
        public static void EnableSceneLoaders()
        {
            var loaders = Object.FindObjectsByType<SceneLoader>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var loader = Array.Find(loaders, c => c.SceneName == scene.name);
                if (loader == null)
                    continue;

                loader.enabled = true;
                loader.gameObject.SetActive(true);
            }
        }
    }
}
