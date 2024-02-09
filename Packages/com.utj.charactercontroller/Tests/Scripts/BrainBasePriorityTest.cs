using System.Collections;
using NUnit.Framework;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.TinyCharacterController.Test
{
    public class BrainBaseのプライオリティの動作テスト : SceneTestBase
    {
        protected override string ScenePath => TestConstants.PackagePath + "Scenes/HighestPriorityTest.unity";

        [UnityTest]
        public IEnumerator 起動直後にコンポーネントを更新した場合はbrainControlはnull()
        {
            var instance = GameObject.Find("Target_1");
            var control = instance.GetComponent<HighestPriorityTestControl>();
            var brain = instance.GetComponent<IBrain>();

            IMove previousMove = null;
            ITurn previousTurn = null;

            control.OnAcquireHighestMovePriority.AddListener(previous => previousMove = previous);
            control.OnAcquireHighestTurnPriority.AddListener(previous => previousTurn = previous);

            yield return null;

            control.MovePriority = 50;
            control.MovePriority = 50;

            yield return null;

            Assert.AreEqual(null, previousMove);
            Assert.AreEqual(null, previousTurn);
        }

        [UnityTest]
        public IEnumerator コンポーネントを更新した時のコールバックは呼ばれている()
        {
            var instance = GameObject.Find("Target_1");

            var control = instance.GetComponent<HighestPriorityTestControl>();
            var manualControl = instance.GetComponent<ManualControl>();

            var isCallTurnCallback = false;
            var isCallMoveCallback = false;

            control.OnAcquireHighestMovePriority.AddListener(previous => isCallMoveCallback = true);
            control.OnAcquireHighestTurnPriority.AddListener(previous => isCallTurnCallback = true);

            manualControl.MovePriority = 10;
            manualControl.TurnPriority = 10;
            yield return null;

            control.MovePriority = 50;
            control.TurnPriority = 50;

            yield return null;

            Assert.IsTrue(isCallTurnCallback);
            Assert.IsTrue(isCallMoveCallback);
        }

        [UnityTest]
        public IEnumerator コンポーネントを更新した時のコールバックではControlは更新されていない()
        {
            var instance = GameObject.Find("Target_1");

            var control = instance.GetComponent<HighestPriorityTestControl>();
            var brain = instance.GetComponent<IBrain>();
            var manualControl = instance.GetComponent<ManualControl>();

            IMove previousMove = null;
            ITurn previousTurn = null;

            control.OnAcquireHighestMovePriority.AddListener(previous => previousMove = previous);
            control.OnAcquireHighestTurnPriority.AddListener(previous => previousTurn = previous);

            manualControl.MovePriority = 10;
            manualControl.TurnPriority = 10;
            yield return null;

            control.MovePriority = 50;
            control.TurnPriority = 50;

            yield return null;

            Assert.AreEqual(manualControl, previousMove);
            Assert.AreEqual(manualControl, previousTurn);
        }

        [UnityTest]
        public IEnumerator Updateの更新()
        {
            var instance = GameObject.Find("Target_1");

            var control = instance.GetComponent<HighestPriorityTestControl>();
            control.MovePriority = 10;

            for (var i = 0; i < 5; i++)
                yield return null;

            Assert.AreEqual(control.MoveUpdateCount, 5);
            Assert.AreEqual(control.TurnUpdateCount, 0);
            control.TurnPriority = 10;

            for (var i = 0; i < 5; i++)
                yield return null;

            Assert.AreEqual(control.MoveUpdateCount, 10);
            Assert.AreEqual(control.TurnUpdateCount, 5);
            control.MovePriority = 0;

            for (var i = 0; i < 5; i++)
                yield return null;

            Assert.AreEqual(control.MoveUpdateCount, 10);
            Assert.AreEqual(control.TurnUpdateCount, 10);
        }

        [UnityTest]
        public IEnumerator 他のコンポーネントのプライオリティが0の時の動作()
        {
            var instance = GameObject.Find("Target_1");

            var control = instance.GetComponent<HighestPriorityTestControl>();
            var manualControl = instance.GetComponent<ManualControl>();

            control.MovePriority = 5;
            yield return null;

            Assert.True(control.HasMoveHighestPriority, "プライオリティをあげた後");
            control.MovePriority = 0;
            yield return null;

            Assert.False(control.HasMoveHighestPriority, "プライオリティを0にした後");
        }


        [UnityTest]
        public IEnumerator 他のコンポーネントがある状態で回転のプライオリティを下げる()
        {
            var instance = GameObject.Find("Target_1");

            var control = instance.GetComponent<HighestPriorityTestControl>();
            var manualControl = instance.GetComponent<ManualControl>();
            manualControl.TurnPriority = 5;
            control.TurnPriority = 10;
            yield return null;

            Assert.True(control.HasTurnHighestPriority, "プライオリティを上げる");
            control.TurnPriority = 4;
            yield return null;

            Assert.IsFalse(control.HasTurnHighestPriority, "起動直後");
        }

        [UnityTest]
        public IEnumerator 他のコンポーネントがある状態でプライオリティを下げる()
        {
            var instance = GameObject.Find("Target_1");

            var control = instance.GetComponent<HighestPriorityTestControl>();
            var manualControl = instance.GetComponent<ManualControl>();
            control.TurnPriority = 15;
            control.MovePriority = 15;
            manualControl.TurnPriority = 10;
            manualControl.MovePriority = 10;
            yield return null;

            Assert.True(control.HasTurnHighestPriority);
            Assert.True(control.HasMoveHighestPriority);
            control.TurnPriority = 5;
            control.MovePriority = 5;
            yield return null;

            Assert.False(control.HasTurnHighestPriority);
            Assert.False(control.HasMoveHighestPriority);
        }

        [UnityTest]
        public IEnumerator 他のコンポーネントがある状態でプライオリティを上げる()
        {
            var instance = GameObject.Find("Target_1");

            var control = instance.GetComponent<HighestPriorityTestControl>();
            var manualControl = instance.GetComponent<ManualControl>();

            manualControl.TurnPriority = 5;
            manualControl.MovePriority = 5;
            yield return null;

            control.TurnPriority = 10;
            control.MovePriority = 10;
            yield return null;

            Assert.True(control.HasTurnHighestPriority);
            Assert.True(control.HasMoveHighestPriority);
        }


        [UnityTest]
        public IEnumerator プライオリティをあげた後に0にする()
        {
            var instance = GameObject.Find("Target_1");

            var control = instance.GetComponent<HighestPriorityTestControl>();
            control.TurnPriority = 10;
            control.MovePriority = 10;
            yield return null;

            control.TurnPriority = 0;
            control.MovePriority = 0;
            yield return null;

            Assert.IsFalse(control.HasMoveHighestPriority);
            Assert.IsFalse(control.HasTurnHighestPriority);
        }

        [UnityTest]
        public IEnumerator プライオリティを上げる()
        {
            var instance = GameObject.Find("Target_1");

            var control = instance.GetComponent<HighestPriorityTestControl>();
            control.TurnPriority = 10;
            control.MovePriority = 10;

            yield return null;

            Assert.IsTrue(control.HasTurnHighestPriority);
            Assert.IsTrue(control.HasMoveHighestPriority);
        }
    }
}