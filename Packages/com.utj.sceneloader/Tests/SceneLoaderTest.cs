using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Unity.SceneManagement.Test
{
    
    
    public class SceneLoaderTest : SceneTestBase
    {
        [SetUp]
        public void テストを動作させるためにアドレスが登録されているかテスト()
        {
            var groupName = "SceneLoaderTest";
            
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.FindGroup(groupName);
            Assert.That(group, Is.Not.Null, "AddressableにSceneLoaderTestが登録されていません。" +
                                            "テストを動作させるためにはTests/SceneLoaderTest.assetを登録する必要があります。");
        }
        
        [UnityTest]
        [Timeout(5000)]
        public IEnumerator サブシーンを読み込む()
        {
            var scene = default(Scene);
            var callEvent = false;
            
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            sceneLoader.enabled = true;
            sceneLoader.OnLoaded.AddListener(s =>
            {
                scene = s;
                callEvent = true;
            });

            yield return sceneLoader.Handle;

            Assert.That(sceneLoader.IsLoaded, Is.True);
            Assert.That(sceneLoader.Scene.name, Is.EqualTo("LoadFromSceneLoader"));
            Assert.That(callEvent, Is.True);
            Assert.That(scene.name, Is.EqualTo(SceneManager.GetSceneByName("LoadFromSceneLoader").name));
            Assert.That(scene, Is.EqualTo( SceneManager.GetSceneByName("LoadFromSceneLoader")));
        }
        
        [UnityTest]
        [Timeout(5000)]
        public IEnumerator サブシーンを解放する()
        {
            var callEvent = false;
            
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            sceneLoader.OnUnloaded.AddListener(() =>
            {
                callEvent = true;
            });
            sceneLoader.enabled = true;

            yield return sceneLoader.Handle;

            sceneLoader.enabled = false;

            // Assert.That(GameObject.Find("SpawnObject"), Is.Null);
            
            yield return new WaitUntil(() => sceneLoader.IsLoaded == false);
            
            Assert.That(callEvent, Is.True);
            Assert.That(sceneLoader.InProgress, Is.False);
            Assert.That(sceneLoader.IsLoaded, Is.False);
            Assert.That(sceneLoader.Scene, Is.EqualTo(default(Scene)));
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator ロードしたサブシーンから呼び出したSceneLoaderを取得()
        {
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            sceneLoader.enabled = true;
            
            yield return new WaitUntil(() => sceneLoader.IsLoaded);

            var rootObjects = new List<GameObject>();
            sceneLoader.Scene.GetRootGameObjects(rootObjects);
            
            Assert.That(rootObjects[0].name, Is.EqualTo("SpawnObject"));
            Assert.That(sceneLoader.InProgress, Is.False);
            Assert.That(rootObjects[0].GetOwner(), Is.EqualTo(sceneLoader.gameObject));
        }
        
        

        protected override string ScenePath =>
            "Packages/utj.com.utj.scene_loader/Tests/SceneLoaderTest.unity";
    }
}