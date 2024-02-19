using System.Collections;
using NUnit.Framework;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Modifier;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Unity.TinyCharacterController.Test
{
    public class LadderMoveTest : SceneTestBase
    {
        protected override string ScenePath
            => "Packages/com.unity.tiny.character.controller/Tests/Scenes/LadderMoveControl.unity";

        [UnityTest]
        public IEnumerator 掴むとプライオリティが有効になる()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            control.Priority = 10;

            Assert.That(((IMove)control).Priority, Is.EqualTo(0));
            control.GrabLadder(ladder);
            Assert.That(((IMove)control).Priority, Is.EqualTo(10));

            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 1, -0.5f)).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.IsGrabLadder, Is.True);
            Assert.That(control.IsComplete, Is.False);
        }

        [UnityTest]
        public IEnumerator 掴んだ後に離すとプライオリティを失う()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            control.Priority = 10;

            Assert.That(((IMove)control).Priority, Is.EqualTo(0));
            control.GrabLadder(ladder);
            Assert.That(((IMove)control).Priority, Is.EqualTo(10));

            yield return null;

            control.ReleaseLadder();
            Assert.That(((IMove)control).Priority, Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator プライオリティが無くても掴むと座標を更新するがMoveで移動しない()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            control.Priority = 0;
            control.GrabLadder(ladder);
            control.Move(1);

            yield return new WaitForSeconds(1);

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 1, -0.5f)).Using(Vector3EqualityComparer.Instance));
        }


        [UnityTest]
        public IEnumerator プライオリティが無くても掴むと座標を更新する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            control.Priority = 0;

            control.GrabLadder(ladder);

            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 1, -0.5f)).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.IsGrabLadder, Is.True);
            Assert.That(control.IsComplete, Is.False);
        }

        [UnityTest]
        public IEnumerator 移動はステップ単位で行う()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            control.Priority = 10;
            control.GrabLadder(ladder);
            control.Move(1);

            yield return null;
            yield return null;
            control.Move(0);
            Assert.That(control.IsCompleteStep, Is.False);


            yield return new WaitForSeconds(1);

            // Assert.That(control.IsCompleteStep, Is.True);
            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 2, -0.5f)).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.IsCompleteStep, Is.True);
            Assert.That(control.IsComplete, Is.False);
            Assert.That(control.IsGrabLadder, Is.True);
        }

        [UnityTest]
        public IEnumerator ステップ完了時にOnCompleteStepを呼ぶ()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            control.Priority = 10;
            control.GrabLadder(ladder);
            control.Move(1);
            var callCount = 0;
            control.OnCompleteStep.AddListener(() => callCount += 1);

            yield return new WaitForSeconds(0.8f);
            control.Move(0);

            yield return new WaitForSeconds(1);
            
            Assert.That(callCount, Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator 押しっぱなしは連続で移動する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            control.Priority = 10;
            control.GrabLadder(ladder);
            control.Move(1);

            yield return null; // 実行順の問題で二回必要
            yield return null;

            Assert.That(control.IsComplete, Is.False);
            Assert.That(control.IsCompleteStep, Is.False);

            yield return new WaitForSeconds(0.8f);

            control.Move(0);
            Assert.That(control.IsCompleteStep, Is.False);


            yield return new WaitForSeconds(0.4f);

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 3, -0.5f)).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.IsCompleteStep, Is.True);
        }

        [UnityTest]
        public IEnumerator 掴んだ後にプライオリティをあげると直近のステップへTransition時間かけて移動する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            var transform = control.GetComponent<ITransform>();
            transform.Position = new Vector3(0, 2.6f, -0.5f);

            yield return null;
            control.GrabLadder(ladder);

            yield return null;
            
            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 2.6f, -0.5f)).Using(Vector3EqualityComparer.Instance));

            control.Priority = 10;

            yield return new WaitForSeconds(1);

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 3f, -0.5f)).Using(Vector3EqualityComparer.Instance));
            
            control.Priority = 0;
            yield return null;
            
            transform.Position = new Vector3(0, 2.3f, -0.5f);
            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 2.3f, -0.5f)).Using(Vector3EqualityComparer.Instance));
            control.Priority = 10;
            yield return new WaitForSeconds(1);

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 2.0f, -0.5f)).Using(Vector3EqualityComparer.Instance));

        }

        [UnityTest]
        public IEnumerator 掴んだ時は直近のStepPointに移動する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            var transform = control.GetComponent<ITransform>();
            transform.Position = new Vector3(0, 2.3f, 0);

            yield return null;
            yield return null;

            control.Priority = 10;
            control.GrabLadder(ladder);

            yield return null; // 実行順の問題で二回必要
            yield return new WaitForSeconds(0.6f);
            yield return null;

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 2.0f, -0.5f)).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator 掴んだ時に上を押してると即座に移動する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            var transform = control.GetComponent<ITransform>();
            transform.Position = new Vector3(0, 2.3f, 0);

            yield return null;
            yield return null;

            control.Priority = 10;
            control.GrabLadder(ladder);
            control.Move(1);

            yield return null; // 実行順の問題で二回必要

            control.Move(0);
            yield return new WaitForSeconds(1f);
            
            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 3f, -0.5f)).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator 下に到達すればBottomPointへ移動する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");
            var calledEvent = false;
            control.OnComplete.AddListener(() => calledEvent = true);

            var transform = control.GetComponent<ITransform>();
            transform.Position = new Vector3(0, 2.0f, 0);

            control.Priority = 10;
            control.GrabLadder(ladder);
            control.Move(-1);

            yield return null; // 実行順の問題で二回必要
            yield return null;

            yield return new WaitForSeconds(2f);

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 0, -1f)).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.IsComplete);
            Assert.That(calledEvent, Is.True);
        }


        [UnityTest]
        public IEnumerator 上に到達すればTopPointへ移動する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");

            var transform = control.GetComponent<ITransform>();
            transform.Position = new Vector3(0, 4.0f, 0);
            var calledEvent = false;
            control.OnComplete.AddListener(() => calledEvent = true);

            control.Priority = 10;
            control.GrabLadder(ladder);
            control.Move(1);

            yield return null; // 実行順の問題で二回必要
            yield return null;

            yield return new WaitForSeconds(1f);

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 5, 1f)).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.IsComplete);
            Assert.That(calledEvent, Is.True);
        }


        [UnityTest]
        public IEnumerator 他のControlで下に到達した場合もBottomPointへ移動する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");
            var manualControl = control.GetComponent<ManualControl>();

            var transform = control.GetComponent<ITransform>();
            transform.Position = new Vector3(0, 2.0f, 0);
            control.OnComplete.AddListener(() => manualControl.MovePriority = 0);

            control.Priority = 0;
            control.GrabLadder(ladder);
            manualControl.MovePriority = 10;
            manualControl.MoveVelocity = new Vector3(0, -1, 0);

            yield return null; // 実行順の問題で二回必要
            yield return null;

            yield return new WaitForSeconds(2f);

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 0, -1f)).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.IsComplete);
        }

        [UnityTest]
        public IEnumerator 他のControlで上に到達した場合もTopPointへ移動する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");
            var manualControl = control.GetComponent<ManualControl>();

            var transform = control.GetComponent<ITransform>();
            transform.Position = new Vector3(0, 3.0f, 0);
            control.OnComplete.AddListener(() => manualControl.MovePriority = 0);

            control.Priority = 0;
            control.GrabLadder(ladder);
            manualControl.MovePriority = 10;
            manualControl.MoveVelocity = new Vector3(0, 1, 0);

            yield return null; // 実行順の問題で二回必要
            yield return null;

            yield return new WaitForSeconds(2f);

            Assert.That(control.transform.position,
                Is.EqualTo(new Vector3(0, 5, 1f)).Using(Vector3EqualityComparer.Instance));
            Assert.That(control.IsComplete);
        }

        [UnityTest]
        public IEnumerator ステップの移動時間はStepTimeに依存する()
        {
            var control = FindComponent<LadderMoveControl>("Player_1");
            var ladder = FindComponent<Ladder>("Ladder");
            control.Priority = 10;
            control.StepTime = 1.5f;
            control.GrabLadder(ladder);
            control.Move(1);

            yield return null;

            control.Move(0);

            yield return new WaitForSeconds(1);

            Assert.That(control.IsCompleteStep, Is.False);

            yield return new WaitForSeconds(1);

            Assert.That(control.IsCompleteStep, Is.True);
        }
    }
}