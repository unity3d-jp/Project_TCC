using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Unity.SceneManagement.Test
{
    public class SceneLoaderTest : SceneTestBase
    {
#if UNITY_EDITOR
        [SetUp]
        public void テストを動作させるためにアドレスが登録されているかテスト()
        {
            var groupName = "SceneLoaderTest";
            
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.FindGroup(groupName);
            Assert.That(group, Is.Not.Null, "AddressableにSceneLoaderTestが登録されていません。" +
                                            "テストを動作させるためにはTests/SceneLoaderTest.assetを登録する必要があります。");
        }
#endif
        [UnityTest]
        [Timeout(5000)]
        public IEnumerator シーン読み込み後の進行度()
        {
            Assert.That(SceneLoaderManager.HasProgress, Is.False);

            
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            Assert.That(sceneLoader.InProgress, Is.False); 

            sceneLoader.enabled = true;

            Assert.That(sceneLoader.InProgress, Is.True); 
            
            Assert.That(sceneLoader.Progress, Is.GreaterThanOrEqualTo(0));
            Assert.That(SceneLoaderManager.HasProgress, Is.True);
            
            yield return sceneLoader.LoadHandle;

            Assert.That(sceneLoader.InProgress, Is.False); 
            Assert.That(sceneLoader.Progress, Is.Not.EqualTo(1f).Using(FloatEqualityComparer.Instance));
            Assert.That(SceneLoaderManager.HasProgress, Is.False);
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
                Assert.That(GameObject.Find("SpawnObject"), Is.Not.Null);
            });

            yield return sceneLoader.LoadHandle;

            Assert.That(sceneLoader.IsLoaded, Is.True);
            Assert.That(sceneLoader.LoadedScene.name, Is.EqualTo("LoadFromSceneLoader"));
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

            yield return sceneLoader.LoadHandle;

            sceneLoader.enabled = false;

            // Assert.That(GameObject.Find("SpawnObject"), Is.Null);
            
            yield return new WaitUntil(() => sceneLoader.IsLoaded == false);
            
            Assert.That(callEvent, Is.True);
            Assert.That(sceneLoader.InProgress, Is.False);
            Assert.That(sceneLoader.IsLoaded, Is.False);
            Assert.That(sceneLoader.LoadedScene, Is.EqualTo(default(Scene)));
            Assert.That(GameObject.Find("SpawnObject"), Is.Null);
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator ロードしたサブシーンをロード完了前に解放する()
        {
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            
            sceneLoader.enabled = true;
            sceneLoader.enabled = false;
            
            yield return new WaitUntil(() => sceneLoader.IsLoaded == false);

            sceneLoader.enabled = true;

            yield return sceneLoader.LoadHandle;

            var rootObjects = new List<GameObject>();
            sceneLoader.LoadedScene.GetRootGameObjects(rootObjects);
            Assert.That(rootObjects[0].name, Is.EqualTo("SpawnObject"));
            Assert.That(sceneLoader.InProgress, Is.False);
            Assert.That(rootObjects[0].GetOwner(), Is.EqualTo(sceneLoader.gameObject));
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator ロードしたサブシーンから呼び出したSceneLoaderを取得()
        {
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            sceneLoader.enabled = true;
            
            yield return new WaitUntil(() => sceneLoader.IsLoaded);

            var rootObjects = new List<GameObject>();
            sceneLoader.LoadedScene.GetRootGameObjects(rootObjects);
            
            Assert.That(rootObjects[0].name, Is.EqualTo("SpawnObject"));
            Assert.That(sceneLoader.InProgress, Is.False);
            Assert.That(rootObjects[0].GetOwner(), Is.EqualTo(sceneLoader.gameObject));
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator シーン解放前に即座にロード再開するもキャンセルする()
        {
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            var loadCount = 0;
            var unloadCount = 0;

            sceneLoader.OnLoaded.AddListener(scene => loadCount += 1);
            sceneLoader.OnUnloaded.AddListener(() => unloadCount += 1);

            sceneLoader.enabled = true;
            yield return sceneLoader.LoadHandle;
            
            sceneLoader.enabled = false;
            sceneLoader.enabled = true;
            sceneLoader.enabled = false;

            yield return new WaitForSeconds(1);
            
            Assert.That(loadCount, Is.EqualTo(1));
            Assert.That(unloadCount, Is.EqualTo(1));

        }

        [UnityTest]
        [Timeout(2000)]
        public IEnumerator ロード完了前にロードをキャンセルし際ロードした場合はキャンセルしない()
        {
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            var loadCount = 0;
            var unloadCount = 0;

            sceneLoader.OnLoaded.AddListener(scene => loadCount += 1);
            sceneLoader.OnUnloaded.AddListener(() => unloadCount += 1);

            sceneLoader.enabled = true;
            sceneLoader.enabled = false;
            sceneLoader.enabled = true;

            yield return sceneLoader.LoadHandle;
            
            Assert.That(loadCount, Is.EqualTo(1));
            Assert.That(unloadCount, Is.EqualTo(0));
        }
        
        [UnityTest]
        [Timeout(2000)]
        public IEnumerator ロードとアンロードが相互の場合は逐次処理する()
        {
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            var loadCount = 0;
            var unloadCount = 0;

            sceneLoader.OnLoaded.AddListener(scene => loadCount += 1);
            sceneLoader.OnUnloaded.AddListener(() => unloadCount += 1);

            sceneLoader.enabled = true;
            sceneLoader.enabled = false;

            yield return new WaitForSeconds(1);
            
            Assert.That(loadCount, Is.EqualTo(1));
            Assert.That(unloadCount, Is.EqualTo(1));
        }

        [UnityTest]
        [Timeout(30000)]
        public IEnumerator 同じフレームでロードとアンロードを繰り返す()
        {
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            var loadCount = 0;
            var unloadCount = 0;

            sceneLoader.OnLoaded.AddListener(scene => loadCount += 1);
            sceneLoader.OnUnloaded.AddListener(() => unloadCount += 1);

            for (var i = 0; i < 10; i++)
            {
                sceneLoader.enabled = true;
                sceneLoader.enabled = false;
            }
            sceneLoader.enabled = true;

            yield return new WaitForSeconds(1);
            
            Assert.That(sceneLoader.IsLoaded, Is.True);
            
            Assert.That(loadCount, Is.EqualTo(1));
            Assert.That(unloadCount, Is.EqualTo(0));
            
            for (var i = 0; i < 10; i++)
            {
                sceneLoader.enabled = false;
                sceneLoader.enabled = true;
            }
            sceneLoader.enabled = false;
            
            yield return new WaitForSeconds(1);
            
            Assert.That(sceneLoader.IsLoaded, Is.False);
            Assert.That(loadCount, Is.EqualTo(1));
            Assert.That(unloadCount, Is.EqualTo(1));
        }

        [UnityTest]
        [Timeout(30000)]
        public IEnumerator ロードとアンロードを繰り返す()
        {
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            sceneLoader.enabled = true;
            sceneLoader.OnLoaded.AddListener(_ =>
            {
                Assert.That(GameObject.Find("SpawnObject"), Is.Not.Null);
            });
            
            yield return new WaitForSeconds(1);


            Assert.That(GameObject.Find("SpawnObject"), Is.Not.Null);

            for (var i = 0; i < 10; i++)
            {
                Assert.That(sceneLoader.IsLoaded, Is.True);
                sceneLoader.enabled = false;
                yield return new WaitForSeconds(1);


                Assert.That(GameObject.Find("SpawnObject"), Is.Null);
                Assert.That(sceneLoader.IsLoaded, Is.False);
                sceneLoader.enabled = true;
                yield return sceneLoader.LoadHandle;
                
                Assert.That(GameObject.Find("SpawnObject"), Is.Not.Null);
            }

            sceneLoader.enabled = true;

            yield return sceneLoader.LoadHandle;

            Assert.That(GameObject.Find("SpawnObject"), Is.Not.Null);
        }

        [UnityTest]
        [Timeout(1000)]
        public IEnumerator ロードしたサブシーンのオーナーを取得する()
        {
            var sceneLoader = FindComponent<SceneLoader>("LoadFromSceneLoader");
            sceneLoader.enabled = true;
            sceneLoader.OnLoaded.AddListener(_ =>
            {
                var spawnObject = GameObject.Find("SpawnObject");
                var owner = SceneLoaderManager.GetOwner(spawnObject.gameObject);
                Assert.That(owner, Is.EqualTo(sceneLoader.gameObject));
            });

            yield return sceneLoader.LoadHandle;
            
            var spawnObject = GameObject.Find("SpawnObject");
            var owner = SceneLoaderManager.GetOwner(spawnObject.gameObject);
            Assert.That(owner, Is.EqualTo(sceneLoader.gameObject));
        }

        protected override string ScenePath =>
            "Packages/utj.com.utj.scene_loader/Tests/SceneLoaderTest.unity";
    }
}