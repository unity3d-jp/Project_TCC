using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.TinyCharacterController.Test
{
    public class RangeCheckTest : SceneTestBase
    {
        // A Test behaves as an ordinary method
        [Test]
        public void RangeCheckTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        [UnityTest]
        public IEnumerator 最初から複数のオブジェクトがある()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_1");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");

            yield return null;
            
            var targets = rangeTargetCheck.GetTargets(0);
            Assert.That(targets.Count, Is.EqualTo(4));
            Assert.That(targets.Exists(item => item.name == "Sphere_1"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_2"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_3"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_4"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_5"), Is.False);
        }
        
        [UnityTest]
        public IEnumerator カメラの範囲内のオブジェクトを探す()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_2");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");
            var targets = rangeTargetCheck.GetTargets(0);

            yield return null;
            
            Assert.That(targets.Count, Is.EqualTo(3));
            Assert.That(targets.Exists(item => item.name == "Sphere_1"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_2"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_3"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_4"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_5"), Is.False);
        }
        
        [UnityTest]
        public IEnumerator カメラから障害物で隠れていないオブジェクトを探す()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_3");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");
            var targets = rangeTargetCheck.GetTargets(0);

            yield return null;
            
            Assert.That(targets.Count, Is.EqualTo(3));
            Assert.That(targets.Exists(item => item.name == "Sphere_1"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_2"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_3"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_4"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_5"), Is.False);
        }

        [UnityTest]
        public IEnumerator プレイヤーから障害物で隠れていないオブジェクトを探す()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_4");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");
            var targets = rangeTargetCheck.GetTargets(0);

            yield return null;
            
            Assert.That(targets.Count, Is.EqualTo(3));
            Assert.That(targets.Exists(item => item.name == "Sphere_1"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_2"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_3"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_4"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_5"), Is.False);
        }

        [UnityTest]
        public IEnumerator プレイヤーからもカメラからも隠れていないオブジェクトを探す()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_5");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");
            var targets = rangeTargetCheck.GetTargets(0);

            yield return null;
            
            Assert.That(targets.Count, Is.EqualTo(2));
            Assert.That(targets.Exists(item => item.name == "Sphere_1"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_2"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_3"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_4"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_5"), Is.False);
        }
        
        [UnityTest]
        public IEnumerator キャラクターを移動させる()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_6");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");
            var targets = rangeTargetCheck.GetTargets(0);
            
            rangeTargetCheck.GetComponent<ITransform>().Position = (new Vector3(-3, 0, -3));

            yield return null;
            
            Assert.That(targets.Count, Is.EqualTo(1));
            Assert.That(targets.Exists(item => item.name == "Sphere_1"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_2"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_3"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_4"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_5"), Is.False);
        }
        [UnityTest]
        public IEnumerator キャラクターを移動して増減したオブジェクトを探す()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_7");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");

            yield return null;
            
            rangeTargetCheck.GetComponent<ITransform>().Position = (new Vector3(1, 0, -5));
            var targets = rangeTargetCheck.GetTargets(0);

            yield return null;

            var result = rangeTargetCheck.ChangedValues(0, out var add, out var remove);
            
            Assert.That(result, Is.True, "値が変化なかった");
            Assert.That(targets.Count, Is.EqualTo(3));
            Assert.That(add.Count, Is.EqualTo(1));
            Assert.That(remove.Count, Is.EqualTo(1));

            Assert.That(add.Exists(item => item.name == "Sphere_3"), Is.True);
            Assert.That(remove.Exists(item => item.name == "Sphere_1"), Is.True);
        }
        
        
        
        [UnityTest]
        public IEnumerator オブジェクトを無効にする()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_8");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");
            var targets = rangeTargetCheck.GetTargets(0);

            yield return null;
            
            var sphere = GameObject.Find("Sphere_1");
            sphere.SetActive(false);

            yield return null;
            
            var result = rangeTargetCheck.ChangedValues(0, out var add, out var remove);
            Assert.That(result, Is.True, "値が変化なかった");

            Assert.That(remove.Count, Is.EqualTo(1));
            Assert.That(remove.Exists(item => item.name == "Sphere_1"));
            
            Assert.That(targets.Count, Is.EqualTo(3));
            Assert.That(targets.Exists(item => item.name == "Sphere_1"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_2"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_3"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_4"), Is.True);
        }
        
        [UnityTest]
        public IEnumerator オブジェクトを削除する()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_8");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");
            
            var targets = rangeTargetCheck.GetTargets(0);

            yield return null;
            
            var sphere = GameObject.Find("Sphere_1");
            Object.Destroy(sphere);

            yield return null;
            
            
            
            var result = rangeTargetCheck.ChangedValues(0, out var add, out var remove);
            Assert.That(result, Is.True, "値が変化なかった");

            Assert.That(remove.Count, Is.EqualTo(1));
            
            Assert.That(targets.Count, Is.EqualTo(3));
            Assert.That(targets.Exists(item => item.name == "Sphere_1"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_2"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_3"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_4"), Is.True);
        }
        
        [UnityTest]
        public IEnumerator オブジェクトを削除した時のイベント()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_8");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");
            var isCallEvent = false;
            
            rangeTargetCheck.GetSearchRangeSettings(0).OnChangeValue.AddListener(result =>
            {
                isCallEvent = true;
            });

            yield return null;
            
            var sphere = GameObject.Find("Sphere_1");
            Object.Destroy(sphere);

            yield return null;
            
            Assert.That(isCallEvent, Is.True);
        }
        
        [Test]
        public void タグを取得()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_9");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");
            
            Assert.That(rangeTargetCheck.GetTagIndex("Player"), Is.EqualTo(0) );
            Assert.That(rangeTargetCheck.GetTagIndex("MainCamera"), Is.EqualTo(1) );
            Assert.That(rangeTargetCheck.GetTagIndex("Respawn"), Is.EqualTo(2) );
        }

        [UnityTest]
        public IEnumerator Transparentの壁は無視する()
        {
            var root = GetObjectAndComponentInRoot<Transform>("Test_10");
            root.gameObject.SetActive(true);
            var rangeTargetCheck = FindComponent<RangeTargetCheck>("Character_1");

            yield return null;
            
            var targets = rangeTargetCheck.GetTargets(0);
            Assert.That(targets.Count, Is.EqualTo(3));
            Assert.That(targets.Exists(item => item.name == "Sphere_1"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_2"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_3"), Is.True);
            Assert.That(targets.Exists(item => item.name == "Sphere_4"), Is.False);
            Assert.That(targets.Exists(item => item.name == "Sphere_5"), Is.False);
        }

        protected override string ScenePath => TestConstants.PackagePath +  "Scenes/RangeCheckTest.unity";
    }
}
