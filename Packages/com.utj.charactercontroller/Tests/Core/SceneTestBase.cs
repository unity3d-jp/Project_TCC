using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public abstract class SceneTestBase : IPrebuildSetup, IPostBuildCleanup
{
    private string _originalScene;
    
    protected abstract string ScenePath { get; }

    public static TComponent GetObjectAndComponentInRoot<TComponent>(string objectName) where TComponent : Component
    {
        var objectList = new List<GameObject>();
        for (var index = 0; index < SceneManager.sceneCount; index++)
        {
            var scene = SceneManager.GetSceneAt(index);
            scene.GetRootGameObjects(objectList);

            var result = objectList.Find(o => o.name.Equals(objectName));
            if (result != null && result.TryGetComponent(out TComponent component))
            {
                return component;
            }
        }

        return null;
    }
    
    public static T FindComponent<T>(string objectName) where T : Component
    {
        var obj = GameObject.Find(objectName);
        if (obj == null)
            return null;

        return obj.GetComponent<T>();
    }
    
    [UnitySetUp]
    public IEnumerator SetupBeforeTest()
    {
        _originalScene = SceneManager.GetActiveScene().path;
        SceneManager.LoadScene(ScenePath);
        yield return null;
        
        var scene = SceneManager.GetSceneByPath(ScenePath);
        Assert.That(scene.isLoaded, Is.True, "シーンのロードに失敗しています");
    }
   
    
    [TearDown]
    public void TeardownAfterTest()
    {
        SceneManager.LoadScene(_originalScene);
    }
    
    void IPrebuildSetup.Setup()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorBuildSettings.scenes.Any(scene => scene.path == ScenePath))
            return;

        var includedScenes = UnityEditor.EditorBuildSettings.scenes.ToList();
        includedScenes.Add(new UnityEditor.EditorBuildSettingsScene(ScenePath, true));
        UnityEditor.EditorBuildSettings.scenes = includedScenes.ToArray();
#endif
    }

    void IPostBuildCleanup.Cleanup()
    {
#if UNITY_EDITOR
        UnityEditor.EditorBuildSettings.scenes =  UnityEditor.EditorBuildSettings.scenes
            .Where(scene => scene.path != ScenePath)
            .ToArray();
#endif
    }
}