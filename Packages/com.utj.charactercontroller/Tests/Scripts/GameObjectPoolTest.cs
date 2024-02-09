using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Manager;
using Unity.TinyCharacterController.Utility;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.TinyCharacterController.Test
{
    public class GameObjectPoolTest : SceneTestBase
    {
        protected override string ScenePath => TestConstants.PackagePath +  "Scenes/GameObjectPoolTest.unity";

        // A Test behaves as an ordinary method
        [Test]
        public void PreWarmの動作()
        {
            var gameObjectPool = FindComponent<GameObjectPool>("Canvas");
            
            Assert.That(gameObjectPool.transform.childCount, Is.EqualTo(16));
        }

        [UnityTest]
        public IEnumerator PreWarmで準備したオブジェクトを使用する()
        {
            var gameObjectPool = FindComponent<GameObjectPool>("Canvas");

            gameObjectPool.Get();
            gameObjectPool.Get();
            gameObjectPool.Get();
            
            yield return null;
            
            Assert.That(gameObjectPool.transform.childCount, Is.EqualTo(16));
        }

        [UnityTest]
        public IEnumerator オブジェクトの取得と返却()
        {
            var gameObjectPool = FindComponent<GameObjectPool>("Canvas");

            var list = new List<IPooledObject>(15);

            yield return null;

            for (var i = 0; i < 15; i++)
                list.Add(((IGameObjectPool)gameObjectPool).Get());

            yield return null;

            for(var i=1; i<gameObjectPool.transform.childCount; i++)
                Assert.That(gameObjectPool.transform.GetChild(i).gameObject.activeInHierarchy, Is.True);

            foreach( var obj in list)
                obj.Release();

            yield return null;
            
            Assert.That(gameObjectPool.transform.childCount, Is.EqualTo(16));
            for(var i=1; i<gameObjectPool.transform.childCount; i++)
                Assert.That(gameObjectPool.transform.GetChild(i).gameObject.activeInHierarchy, Is.False);
        }
    }
}
