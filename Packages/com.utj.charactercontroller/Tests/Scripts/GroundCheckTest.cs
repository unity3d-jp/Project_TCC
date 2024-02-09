using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.TinyCharacterController.Check;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Unity.TinyCharacterController.Test
{
    public class GroundCheckTest : SceneTestBase
    {
        protected override string ScenePath => TestConstants.PackagePath + "Scenes/GroundCheck.unity";
        
        
        [Test]
        public void 起動直後の地面判定()
        {
            // position = (2,0,0)
            var groundCheck = FindComponent<GroundCheck>("Player_1");
            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.True);
            Assert.That(groundCheck.IsChangeGroundObject, Is.True);
        }
        
        [UnityTest]
        public IEnumerator 地面と接地している()
        {
            // position = (2,0,0)
            var groundCheck = FindComponent<GroundCheck>("Player_1");
            
            yield return null;

            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.True);
        }

        [UnityTest]
        public IEnumerator 少し浮いている()
        {
            // position = (0, 0.3, 0)
            // ambiguousDistance = 0.49
            var groundCheck = FindComponent<GroundCheck>("Player_2");
            
            yield return null;

            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.False);
        }


        [UnityTest]
        public IEnumerator 浮いている()
        {
            // position = (-3, 1, 0)
            // ambiguousDistance = 0.49
            var groundCheck = FindComponent<GroundCheck>("Player_3");
            
            yield return null;

            Assert.That(groundCheck.IsOnGround, Is.False);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.False);
        }

        [UnityTest]
        public IEnumerator 地面が傾いてるがMaxSlopより小さい()
        {
            var groundCheck = FindComponent<GroundCheck>("Player_4");
            
            yield return null;
            
            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.True);
        }
        
        [UnityTest]
        public IEnumerator 地面の傾きがMaxSlopeより大きい()
        {
            var groundCheck = FindComponent<GroundCheck>("Player_5");
            
            yield return null;
            
            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.False);
        }
        
        [UnityTest]
        public IEnumerator 地面オブジェクトが変化する()
        {
            var groundCheck = FindComponent<GroundCheck>("Player_6");
            groundCheck.transform.position = new Vector3(8, 0, 3);
            
            yield return null;
            
            Assert.That(groundCheck.IsChangeGroundObject, Is.True);
        }
        
        [UnityTest]
        public IEnumerator IsChangeGroundObjectは次のフレームで解除される()
        {
            var groundCheck = FindComponent<GroundCheck>("Player_6");
            groundCheck.transform.position = new Vector3(8, 0, 3);
            
            yield return null;
            yield return null;
            
            Assert.That(groundCheck.IsChangeGroundObject, Is.False);
        }
        
        [UnityTest]
        public IEnumerator 地面が変化する度にイベントは呼ばれる()
        {
            var groundCheck = FindComponent<GroundCheck>("Player_6");
            groundCheck.transform.position = new Vector3(8, 0, 3);
            
            yield return null;

            groundCheck.transform.position = new Vector3(2, 0, 3);
            yield return null;
            
            Assert.That(groundCheck.IsChangeGroundObject, Is.True);
        }

        [UnityTest]
        public IEnumerator オブジェクトの変更時にイベントを呼ぶ()
        {
            var plane2 = GameObject.Find("Plane_2");
            var groundCheck = FindComponent<GroundCheck>("Player_6");
            groundCheck.transform.position = new Vector3(8, 0, 3);

            var isInvoked = false;
            GameObject obj = null;
            
            groundCheck.OnChangeGroundObject.AddListener(go =>
            {
                isInvoked = true;
                obj = go;
            });
            
            yield return null;
            
            Assert.That(isInvoked, Is.True);
            Assert.That(obj , Is.EqualTo(plane2));
        }

        [UnityTest]
        public IEnumerator 地面との距離([Values(0, 0.1f, 0.2f, -0.1f, -0.2f)] float y)
        {
            // position = (2,0,0)
            var groundCheck = FindComponent<GroundCheck>("Player_1");
            groundCheck.transform.position = new Vector3(2, y, 0);
            
            yield return null;

            Assert.That(
                groundCheck.DistanceFromGround, 
                Is.EqualTo(y).Using(FloatEqualityComparer.Instance));
        }

    }
}