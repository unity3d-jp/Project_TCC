using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Effect;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Unity.TinyCharacterController.Test
{
    public class SyncTransformControlTest : SceneTestBase
    {
        [UnityTest]
        public IEnumerator TargetTransformを設定すると同じ座標へ移動する()
        {
            var control = FindComponent<SyncTransformControl>("Player");
            var target = GameObject.Find("Target").transform;
            control.Target = target;
            control.MovePriority = 10;
            control.TurnPriority = 10;

            var pos = new Vector3(3, 0, 0);
            var rot = Quaternion.AngleAxis(45, Vector3.up);
            target.position = pos;
            target.rotation = rot;
            
            yield return null;
            
            Assert.That(control.transform.position, Is.EqualTo(pos).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.transform.rotation, Is.EqualTo(rot).Using(QuaternionEqualityComparer.Instance));
        }
        
        [UnityTest]
        public IEnumerator TargetTransformを解除すると追跡しない()
        {
            var control = FindComponent<SyncTransformControl>("Player");
            var target = GameObject.Find("Target").transform;
            control.Target = target;
            control.MovePriority = 10;
            control.TurnPriority = 10;

            var pos = new Vector3(3, 0, 0);
            var rot = Quaternion.AngleAxis(45, Vector3.up);
            target.position = pos;
            target.rotation = rot;

            yield return null;

            control.Target = null;
            target.position = new Vector3(5, 0, 0);
            target.rotation = Quaternion.identity;

            yield return null;

            
            Assert.That(control.transform.position, Is.EqualTo(pos).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.transform.rotation, Is.EqualTo(rot).Using(QuaternionEqualityComparer.Instance));
        }
        
        [UnityTest]
        public IEnumerator Transitionが設定されている場合はTargetTransformへの到達に時間がかかる()
        {
            var control = FindComponent<SyncTransformControl>("Player");
            var target = GameObject.Find("Target").transform;
            control.Target = target;
            control.MovePriority = 10;
            control.TurnPriority = 10;

            control.TurnTransitionTime = 1;
            control.MoveTransitionTime = 1;

            var pos = new Vector3(3, 0, 0);
            var rot = Quaternion.AngleAxis(45, Vector3.up);
            target.position = pos;
            target.rotation = rot;

            yield return new WaitForSeconds(0.5f);
            
            Assert.That(control.transform.position, Is.Not.EqualTo(pos).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.transform.rotation, Is.Not.EqualTo(rot).Using(QuaternionEqualityComparer.Instance));
            
            yield return new WaitForSeconds(1f);

            
            Assert.That(control.transform.position, Is.EqualTo(pos).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.transform.rotation, Is.EqualTo(rot).Using(QuaternionEqualityComparer.Instance));
        }
        
        [UnityTest]
        public IEnumerator Transition中にTargetTransformが外されれば追跡は停止する()
        {
            var control = FindComponent<SyncTransformControl>("Player");
            var target = GameObject.Find("Target").transform;
            control.Target = target;
            control.MovePriority = 10;
            control.TurnPriority = 10;

            control.TurnTransitionTime = 1;
            control.MoveTransitionTime = 1;

            var pos = new Vector3(3, 0, 0);
            var rot = Quaternion.AngleAxis(45, Vector3.up);
            target.position = pos;
            target.rotation = rot;

            yield return new WaitForSeconds(0.5f);

            control.Target = null;
            
            yield return new WaitForSeconds(1f);
            
            Assert.That(control.transform.position, Is.Not.EqualTo(pos).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.transform.rotation, Is.Not.EqualTo(rot).Using(QuaternionEqualityComparer.Instance));
        }
        
        [UnityTest]
        public IEnumerator Priorityが無効の場合は移動しない()
        {
            var control = FindComponent<SyncTransformControl>("Player");
            var target = GameObject.Find("Target").transform;
            control.Target = target;
            control.MovePriority = 0;
            control.TurnPriority = 0;

            var pos = new Vector3(3, 0, 0);
            var rot = Quaternion.AngleAxis(45, Vector3.up);
            target.position = pos;
            target.rotation = rot;
            
            yield return new WaitForSeconds(1f);
            
            Assert.That(control.transform.position, Is.EqualTo(Vector3.zero).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.transform.rotation, Is.EqualTo(Quaternion.identity).Using(QuaternionEqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator 他のEffectを無視する()
        {
            var control = FindComponent<SyncTransformControl>("Player");
            var target = GameObject.Find("Target").transform;
            control.Target = target;
            control.MovePriority = 10;
            control.TurnPriority = 10;
            control.GetComponent<ExtraForce>().AddForce(new Vector3(15, 0, 0));
            
            var pos = new Vector3(3, 0, 0);
            var rot = Quaternion.AngleAxis(45, Vector3.up);
            target.position = pos;
            target.rotation = rot;

            yield return new WaitForSeconds(1);
            
            Assert.That(control.transform.position, Is.EqualTo(pos).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.transform.rotation, Is.EqualTo(rot).Using(QuaternionEqualityComparer.Instance));

        }
        
        protected override string ScenePath => "Packages/com.unity.tiny.character.controller/Tests/Scenes/SyncTransformControlTest.unity";
    }
}