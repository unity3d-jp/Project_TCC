using System.Collections;
using NUnit.Framework;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Unity.TinyCharacterController.Test
{

    public class GroundHeightCheckのテスト : SceneTestBase
    {
        protected override string ScenePath => TestConstants.PackagePath + "Scenes/GroundHeightCheck.unity";

        [UnityTest]
        public IEnumerator 地面と接地している()
        {
            // position.y = 0.0f, _toleranceHeight = 0.5f
            var groundCheck = FindComponent<GroundHeightCheck>("Player_1");

            yield return null;

            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.True);
        }


        [UnityTest]
        public IEnumerator 地面と少し離れている()
        {
            // position.y = 0.2f, _toleranceHeight = 0.5f
            var groundCheck = FindComponent<GroundHeightCheck>("Player_2");
            
            yield return null;

            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.False);
        }

        [UnityTest]
        public IEnumerator 地面から離れている()
        {
            // position.y = 1f, _toleranceHeight = 0.5f
            var groundCheck = FindComponent<GroundHeightCheck>("Player_3");

            yield return null;

            Assert.That(groundCheck.IsOnGround, Is.False);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.False);
        }

        
        [UnityTest]
        public IEnumerator GroundHeightを変更して地面が少し離す()
        {
            var groundCheck = FindComponent<GroundHeightCheck>("Player_1");
            groundCheck.GroundHeight = -0.1f;

            yield return null;

            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.False);
        }
        
        [UnityTest]
        public IEnumerator GroundHeightを変更して地面から大きく離す()
        {
            var groundCheck = FindComponent<GroundHeightCheck>("Player_1");
            groundCheck.GroundHeight = -1f;

            yield return null;

            Assert.That(groundCheck.IsOnGround, Is.False);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.False);
        }
        
        [UnityTest]
        public IEnumerator GroundHeightを変更して地面にめりこませる()
        {
            var groundCheck = FindComponent<GroundHeightCheck>("Player_1");
            groundCheck.GroundHeight = 0.1f;

            yield return null;


            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.True);
        }


        [UnityTest]
        public IEnumerator 座標を少し離す()
        {
            var groundCheck = FindComponent<GroundHeightCheck>("Player_1");
            groundCheck.GetComponent<ITransform>().Position = (new Vector3(0, 0.1f, 0));

            yield return null;

            
            Assert.That(groundCheck.IsOnGround, Is.True);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.False);
        }
        
        [UnityTest]
        public IEnumerator 座標を大きく離す()
        {
            var groundCheck = FindComponent<GroundHeightCheck>("Player_1");

            yield return null;

            groundCheck.GetComponent<ITransform>().Position = (new Vector3(0, 1f, 0));

            yield return null;

            Assert.That(groundCheck.IsOnGround, Is.False);
            Assert.That(groundCheck.IsFirmlyOnGround, Is.False);
        }

        [UnityTest]
        public IEnumerator 地面までの距離([Values(0, 0.1f, -0.1f, -1f, 1f)] float height)
        {
            var groundCheck = FindComponent<GroundHeightCheck>("Player_1");
            groundCheck.GroundHeight = height;

            yield return null;
            
            Assert.That(
                groundCheck.DistanceFromGround, 
                Is.EqualTo(-height).Using(FloatEqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator 地面の位置()
        {
            // position : (2, 1, 0)
            var groundCheck = FindComponent<GroundHeightCheck>("Player_3");
            groundCheck.GroundHeight = -1;

            yield return null;

            Assert.That( 
                groundCheck.GroundContactPoint, 
                Is.EqualTo(new Vector3(2, -1, 0)).Using(Vector3EqualityComparer.Instance));
        }
    }
}