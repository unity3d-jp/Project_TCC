using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.TinyCharacterController.Check;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.TinyCharacterController.Test
{
    public class SightCheckTest : SceneTestBase
    {
        protected override string ScenePath => TestConstants.PackagePath + "Scenes/SightCheckTest.unity";


        [UnityTest]
        public IEnumerator 視界内にオブジェクトがあれば見つける()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_1");
            
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.True); 
        }
        
        [UnityTest]
        public IEnumerator タグが設定されている場合はタグのみを見つける()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_2");
            var player2 = GameObject.Find("Player_2");
            var player3 = GameObject.Find("Player_3");
            
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.True); 
            Assert.That(sightCheck.InsightTargets.Count, Is.EqualTo(2));
            Assert.That(sightCheck.InsightTargets.Contains(player2), Is.True);
            Assert.That(sightCheck.InsightTargets.Contains(player3), Is.True);
        }

        [UnityTest]
        public IEnumerator 遮蔽物で視界を遮る()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_3");
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.False);
        }
        
        [UnityTest]
        public IEnumerator 遮蔽物で視界を遮るがRayCastがFalse()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_4");
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.True);
        }
        
        [UnityTest]
        public IEnumerator オブジェクトはあるがタグが不一致()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_5");
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.False);
        }
        
        [UnityTest]
        public IEnumerator VisibleLayerMaskに含まれる()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_6");
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.True);
        }
        
        [UnityTest]
        public IEnumerator EnvironmentLayerで遮蔽されている()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_7");
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.False);
        }
     
        [UnityTest]
        public IEnumerator EnvironmentLayerに含まれるがVisibleLayerMaskに含まれていない()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_8");
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.False);
        }
        
        [UnityTest]
        public IEnumerator 視界内の対象の数が変化した()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_9");
            var player10 = GameObject.Find("Player_10");
            var isChangeInsightAnyTargetState = false;
            
            sightCheck.OnChangeInsightAnyTargetState.AddListener((c) =>
            {
                isChangeInsightAnyTargetState = true;
            });
            
            player10.SetActive(false);
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.True);
            yield return null;
            
            // Assert.That(sightCheck.IsInsightAnyTarget, Is.False);
            Assert.That(isChangeInsightAnyTargetState, Is.True);
        }
        
        [UnityTest]
        public IEnumerator 視界内の距離が低下する()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_9");
            sightCheck.Range = 1;

            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.True);
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.False);
        }
        
        [UnityTest]
        public IEnumerator 視界内の範囲が低下する()
        {
            var sightCheck = FindComponent<SightCheck>("Enemy_9");
            sightCheck.Angle = 10;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.True);
            yield return null;
            
            Assert.That(sightCheck.IsInsightAnyTarget, Is.False);
        }
    }
}
