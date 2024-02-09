using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Interfaces.Components;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Unity.TinyCharacterController.Test
{
    public class BrainTest : SceneTestBase
    {
        public static string[] BrainNames = new[]
        {
            "CharacterBrain",
            "RigidbodyBrain",
            "TransformBrain"
        };
        
        [UnityTest]
        public IEnumerator 移動しない([ValueSource(nameof(BrainNames))]string brainName)
        {
            var control = GetObjectAndComponentInRoot<ManualControl>(brainName);
            
            Assert.NotNull(control);
            
            control.gameObject.SetActive(true);

            control.MoveVelocity = new Vector3(1, 0, 0);
            control.MovePriority = 0;

            yield return new WaitForSeconds(1);


            Assert.That(Vector3.Distance(control.transform.position, new Vector3(0, 0, 0)),
                Is.LessThan(0.01f));
        }
        
        [UnityTest]
        public IEnumerator 移動([ValueSource(nameof(BrainNames))]string brainName)
        {
            var control = GetObjectAndComponentInRoot<ManualControl>(brainName);
            control.gameObject.SetActive(true);

            control.MoveVelocity = new Vector3(1, 0, 0);
            control.MovePriority = 1;

            yield return new WaitForSeconds(2);

            control.MovePriority = 0;

            yield return null;
            
            var diff = Vector3.Distance(new Vector3(2, 0, 0), control.transform.position);
            Assert.That(diff, Is.LessThan(0.25f));
        }

        [UnityTest]
        public IEnumerator 即回転([ValueSource(nameof(BrainNames))]string brainName)
        {
            var control = GetObjectAndComponentInRoot<ManualControl>(brainName);
            control.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            control.TurnSpeed = -1;
            control.TurnAngle = 90;
            control.TurnPriority = 1;

            yield return brainName == "RigidbodyBrain" ? new WaitForFixedUpdate() : null;

            Assert.That(control.transform.rotation.eulerAngles.y,
                Is.EqualTo(90));
        }

        [UnityTest]
        public IEnumerator 回転([ValueSource(nameof(BrainNames))]string brainName)
        {
            var control = GetObjectAndComponentInRoot<ManualControl>(brainName);
            control.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            control.TurnSpeed = 30;
            control.TurnAngle = 90;
            control.TurnPriority = 1;

            yield return new WaitForSeconds(1);
            
            Assert.That(control.transform.rotation.eulerAngles.y, 
                Is.EqualTo(90));
        }

        [UnityTest]
        public IEnumerator ワープ([ValueSource(nameof(BrainNames))]string brainName)
        {
            var control = GetObjectAndComponentInRoot<ManualControl>(brainName);
            control.gameObject.SetActive(true);
            var warp = control.gameObject.GetComponent<IWarp>();
            
            yield return new WaitForSeconds(0.1f);

            
            warp.Warp( new Vector3(-5, 0, 0) );

            yield return brainName == "RigidbodyBrain" ? new WaitForFixedUpdate() : null;


            
            Assert.That(control.transform.position.x, 
                Is.EqualTo(-5).Using(FloatEqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator ワープ移動([ValueSource(nameof(BrainNames))]string brainName)
        {
            var control = GetObjectAndComponentInRoot<ManualControl>(brainName);
            control.gameObject.SetActive(true);
            var warp = control.gameObject.GetComponent<IWarp>();
            var wall = GameObject.Find("Wall");
            wall.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.1f);


            warp.Move( new Vector3(-5, 0, 0) );


            yield return brainName == "RigidbodyBrain" ? new WaitForFixedUpdate() : null;

            
            Assert.That(control.transform.position.x, 
                Is.EqualTo(-5).Using(FloatEqualityComparer.Instance));
        }
        
        [UnityTest]
        public IEnumerator ワープ移動の途中で障害物CharacterBrain()
        {
            var control = GetObjectAndComponentInRoot<ManualControl>("CharacterBrain");
            control.gameObject.SetActive(true);
            var warp = control.gameObject.GetComponent<IWarp>();
            warp.Move( new Vector3(-5, 0, 0) );
            
            yield return null;

            var wallPos = -2;
            var pos = wallPos + control.GetComponent<CharacterSettings>().Radius;
            Assert.That(control.transform.position, 
                Is.EqualTo(new Vector3(pos, 0, 0)).Using(Vector3EqualityComparer.Instance));
        }
        
        [UnityTest]
        public IEnumerator ワープ移動の途中で障害物RigidBodyBrain()
        {
            var control = GetObjectAndComponentInRoot<ManualControl>("CharacterBrain");
            control.gameObject.SetActive(true);
            var warp = control.gameObject.GetComponent<IWarp>();
            warp.Move( new Vector3(-7, 0, 0) );
            
            yield return null;

            var wallPos = -2;
            var pos = wallPos + control.GetComponent<CharacterSettings>().Radius;
            Assert.That(control.transform.position, 
                Is.EqualTo(new Vector3(pos, 0, 0)).Using(Vector3EqualityComparer.Instance));
        }
        
        [UnityTest]
        public IEnumerator ワープ移動の途中で障害物TransformBrain()
        {
            var control = GetObjectAndComponentInRoot<ManualControl>("TransformBrain");
            control.gameObject.SetActive(true);
            var warp = control.gameObject.GetComponent<IWarp>();
            warp.Move( new Vector3(-5, 0, 0) );
            
            yield return null;

            Assert.That(control.transform.position, 
                Is.EqualTo(new Vector3(-5, 0, 0)).Using(Vector3EqualityComparer.Instance));
        }

        protected override string ScenePath => "Packages/com.unity.tiny.character.controller/Tests/Scenes/BrainTest.unity";
    }
}
