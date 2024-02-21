using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.TinyCharacterController.Brain;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Unity.TinyCharacterController.Test
{
    public class BrainUpdateTest : SceneTestBase
    {

        [UnityTest]
        public IEnumerator CharacterBrain時のDeltaTimeの更新()
        {
            var obj = GetObjectAndComponentInRoot<CharacterBrain>("CharacterBrain");
            var mock = obj.gameObject.AddComponent<MockCheck>();
            obj.gameObject.SetActive(true);

            yield return new WaitForSeconds(1);

            var delta = Mathf.Abs(mock.TimeAmount - 1);
            
            Assert.That(mock.DeltaTime, Is.EqualTo(Time.deltaTime).Using(FloatEqualityComparer.Instance));
            Assert.That(delta, Is.LessThanOrEqualTo(Time.deltaTime));
            Debug.Log(mock.DeltaTime);
        }
        
        [UnityTest]
        public IEnumerator RigidBodyBrain時のDeltaTimeの更新()
        {
            var obj = GetObjectAndComponentInRoot<RigidbodyBrain>("RigidbodyBrain");
            var mock = obj.gameObject.AddComponent<MockCheck>();
            obj.gameObject.SetActive(true);

            yield return new WaitForSeconds(1);

            var delta = Mathf.Abs(mock.TimeAmount - 1);
            
            Assert.That(mock.DeltaTime, Is.EqualTo(Time.fixedDeltaTime).Using(FloatEqualityComparer.Instance));
            Assert.That(delta, Is.LessThanOrEqualTo(Time.fixedDeltaTime));
            Debug.Log(mock.DeltaTime);
        }

        protected override string ScenePath =>
            "Packages/com.unity.tiny.character.controller/Tests/Scenes/BrainUpdateTest.unity";
        
        [AddComponentMenu("")]
        internal class MockCheck : MonoBehaviour, IEarlyUpdateComponent
        {
            public float TimeAmount;
            public float DeltaTime;
        
            void IEarlyUpdateComponent.OnUpdate(float deltaTime)
            {
                TimeAmount += deltaTime;
                DeltaTime = deltaTime;
            }

            int IEarlyUpdateComponent.Order => 0;
        }

    }
}