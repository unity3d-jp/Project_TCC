using System;
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
    public class LerpMoveControlTest : SceneTestBase
    {
        [UnityTest]
        public IEnumerator Playで座標を更新する()
        {
            var control = FindComponent<LerpMoveControl>("Player");
            var pos = new Vector3(5, 0, 0);
            var rot = Quaternion.AngleAxis(90, Vector3.up);
            control.SetTarget("Target-A", pos, rot);
            control.Play("Target-A");

            for (var timeAmount = 0f; timeAmount < 1f; timeAmount += Time.deltaTime)
            {
                control.SetNormalizedTime(timeAmount, timeAmount);
                yield return null;
            }

            control.SetNormalizedTime(1, 1);
            yield return null;

            Assert.That(control.transform.position, Is.EqualTo(pos).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.transform.rotation, Is.EqualTo(rot).Using(QuaternionEqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator 途中でPriorityを失うと動作を停止する()
        {
            var pos = new Vector3(5, 0, 0);
            var control = FindComponent<LerpMoveControl>("Player");
            control.SetTarget("Target-A", pos);

            control.Play("Target-A");
            control.SetNormalizedTime(0.5f, 0);

            yield return null;

            control.Priority = 0;
            control.SetNormalizedTime(1f, 0);

            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(2.5f, 0, 0)).Using(Vector3EqualityComparer.Instance));

        }

        [UnityTest]
        public IEnumerator 途中でPriorityを停止後にStopしても座標の補正は行わない()
        {
            var pos = new Vector3(5, 0, 0);
            var control = FindComponent<LerpMoveControl>("Player");
            control.SetTarget("Target-A", pos);

            control.Play("Target-A");
            control.SetNormalizedTime(0.5f, 0);
            yield return null;

            control.Priority = 0;
            yield return null;

            control.SetNormalizedTime(1f, 0);
            yield return null;

            control.Stop("Target-A");
            yield return null;
            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(2.5f, 0, 0)).Using(Vector3EqualityComparer.Instance));

        }

        [UnityTest]
        public IEnumerator Play中ならIsPlayingはTrue()
        {
            var control = FindComponent<LerpMoveControl>("Player");
            control.SetTarget("Target-A", Vector3.zero);
            control.Play("Target-A");

            yield return null;

            Assert.That(control.IsPlaying, Is.True);


            control.Stop("Target-A");
        }

        [UnityTest]
        public IEnumerator Play中にターゲットが変化しても影響しない()
        {
            var control = FindComponent<LerpMoveControl>("Player");
            var id = new PropertyName("Target-A");
            control.SetTarget(id, new Vector3(10f, 0f, 0f));
            control.Play(id);

            yield return null;

            control.SetNormalizedTime(0.5f, 0);
            control.SetTarget(id, new Vector3(-10f, 0f, 0f));
            yield return null;

            control.SetNormalizedTime(0.7f, 0);
            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(5f, 0, 0)).Using(Vector3EqualityComparer.Instance));
            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(7f, 0, 0)).Using(Vector3EqualityComparer.Instance));
            control.Stop(id);
        }

        [UnityTest]
        public IEnumerator 再生中のPlayと異なるIdをStopされても停止しない()
        {
            var control = FindComponent<LerpMoveControl>("Player");
            control.SetTarget("Target-A", Vector3.zero);
            control.SetTarget("Target-B", Vector3.zero);

            control.Play("Target-A");

            yield return null;

            control.Stop("Target-B");

            yield return null;

            Assert.That(control.IsPlaying, Is.True);
        }

        [UnityTest]
        public IEnumerator 途中で他のPlayが実行された時は現在の座標からスタートする()
        {
            var control = FindComponent<LerpMoveControl>("Player");
            control.SetTarget("Target-A", new Vector3(5, 0, 0));
            control.SetTarget("Target-B", new Vector3(5, 0, 5));
            control.Play("Target-A");

            control.SetNormalizedTime(0.5f, 0.5f);

            yield return null;

            control.Play("Target-B");
            control.SetNormalizedTime(0.0f, 0.0f);

            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(2.5f, 0, 0)).Using(Vector3EqualityComparer.Instance));
            control.SetNormalizedTime(1f, 1f);

            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(5f, 0, 5f)).Using(Vector3EqualityComparer.Instance));
            control.Stop("Target-B");
        }

        [Test]
        public void 登録されていないキーをPlayするとエラー()
        {
            var control = FindComponent<LerpMoveControl>("Player");

            var ex = Assert.Throws<Exception>(() => { control.Play("Target-A"); });

            Assert.That(ex, Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator Cancelで処理を停止()
        {
            var control = FindComponent<LerpMoveControl>("Player");
            control.SetTarget("Target-A", new Vector3(10, 0, 0));
            control.Play("Target-A");

            for (var i = 0; i < 100; i++)
            {
                control.SetNormalizedTime(0.01f * i, 1);

                if (i == 50)
                    control.Cancel();
                
                yield return null;
            }
            
            // 最後のフレームが反映されないので4で停止
            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(4.9f, 0, 0)).Using(Vector3EqualityComparer.Instance));

            Assert.That(control.IsPlaying, Is.False);
        }
        

        [UnityTest]
        public IEnumerator Play中は座標は移動しない()
        {
            var control = FindComponent<LerpMoveControl>("Player");
            control.SetTarget("Target-A", Vector3.zero);
            control.SetTarget("Target-B", new Vector3(5, 0, 0));

            control.GetComponent<AdditionalVelocity>().Velocity = new Vector3(15, 0, 0);

            control.Play("Target-B");

            yield return new WaitForSeconds(1);

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0.0f, 0, 0)).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator Lerpで指定の座標へ移動する()
        {
            var control = FindComponent<LerpMoveControl>("Player");
            control.SetTarget("Target-A", Vector3.zero);
            control.SetTarget("Target-B", new Vector3(5, 0, 0));

            control.Play("Target-B");
            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0.0f, 0, 0)).Using(Vector3EqualityComparer.Instance));
            yield return null;

            control.SetNormalizedTime(0.5f, 0.5f);
            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(2.5f, 0, 0)).Using(Vector3EqualityComparer.Instance));
            yield return null;

            control.SetNormalizedTime(1f, 1f);
            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(5.0f, 0, 0)).Using(Vector3EqualityComparer.Instance));
            control.Stop("Target-B");
        }

        protected override string ScenePath =>
            "Packages/com.unity.tiny.character.controller/Tests/Scenes/LerpMoveControlTest.unity";
    }
}