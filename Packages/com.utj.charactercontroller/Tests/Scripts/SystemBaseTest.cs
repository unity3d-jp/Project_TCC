using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.TinyCharacterController.Test
{
    public class SystemBaseTest
    {
        [SetUp]
        public void SetUp()
        {
            BatchDummySystem.GetInstance(UpdateTiming.Update);

        }
        
        [TearDown]
        public void TearDown()
        {
            var system =  BatchDummySystem.GetInstance(UpdateTiming.Update);
            Object.DestroyImmediate(system);
        }

        [Test]
        public void システムを作って削除する()
        {
            var system = BatchDummySystem.GetInstance(UpdateTiming.FixedUpdate);
            var isDeleted = false;
            system.OnDeleteInstance.AddListener(() => isDeleted = true);
            Object.DestroyImmediate(system);
            
            Assert.That(isDeleted, Is.True);
            Assert.That(system == null, Is.True);
        }

        [Test]
        public void オブジェクトを削除して要素をリセットする()
        {
            var system = BatchDummySystem.GetInstance(UpdateTiming.Update);
            var obj1 = new GameObject();
            var component1 = obj1.AddComponent<BatchDummyComponent>();
            
            var obj2 = new GameObject();
            var component2 = obj2.AddComponent<BatchDummyComponent>();
            
            BatchDummySystem.Register(component1, UpdateTiming.Update);
            BatchDummySystem.Register(component2, UpdateTiming.Update);
            
            
            Assert.That(system.Count, Is.EqualTo(2));
            Assert.That(((IComponentIndex)component1).IsRegistered, Is.True);
            
            Object.DestroyImmediate(system);
            Assert.That(system.Count, Is.EqualTo(0));
            Assert.That(((IComponentIndex)component1).Index, Is.EqualTo(-1));
            Assert.That(((IComponentIndex)component1).IsRegistered, Is.False);
        }
        
        [Test]
        public void オブジェクトを登録()
        {
            var obj = new GameObject();
            var component = obj.AddComponent<BatchDummyComponent>();
            
            var system = BatchDummySystem.GetInstance(UpdateTiming.Update);
            
            BatchDummySystem.Register(component, UpdateTiming.Update);

            Assert.That(system.Count, Is.EqualTo(1));
            Assert.That(((IComponentIndex)component).Index, Is.EqualTo(0));
        }

        [Test]
        public void オブジェクトを削除()
        {
            var obj = new GameObject();
            var component = obj.AddComponent<BatchDummyComponent>();
            
            var system = BatchDummySystem.GetInstance(UpdateTiming.Update);
            
            BatchDummySystem.Register(component, UpdateTiming.Update);
            BatchDummySystem.Unregister(component, UpdateTiming.Update);

            Assert.That(system.Count, Is.EqualTo(0));
            Assert.That(((IComponentIndex)component).Index, Is.EqualTo(-1));

        }
        
        [Test]
        public void オブジェクトを複数登録してから削除()
        {
            var obj1 = new GameObject();
            var component1 = obj1.AddComponent<BatchDummyComponent>();

            var obj2 = new GameObject();
            var component2 = obj2.AddComponent<BatchDummyComponent>();
            
            var system = BatchDummySystem.GetInstance(UpdateTiming.Update);
            
            BatchDummySystem.Register(component1, UpdateTiming.Update);
            BatchDummySystem.Register(component2, UpdateTiming.Update);
            
            Assert.That(((IComponentIndex)component1).Index, Is.EqualTo(0));
            Assert.That(((IComponentIndex)component2).Index, Is.EqualTo(1));
            
            BatchDummySystem.Unregister(component1, UpdateTiming.Update);
            
            Assert.That(((IComponentIndex)component1).Index, Is.EqualTo(-1));
            Assert.That(((IComponentIndex)component2).Index, Is.EqualTo(0));
        }

        [Test]
        public void オブジェクトの登録を解除したコンポーネントの登録を解除()
        {
            var obj1 = new GameObject();
            var component1 = obj1.AddComponent<BatchDummyComponent>();
            
            BatchDummySystem.Register(component1, UpdateTiming.Update);
            BatchDummySystem.Unregister(component1, UpdateTiming.Update);
            BatchDummySystem.Unregister(component1, UpdateTiming.Update);
        }
        
        [Test]
        public void オブジェクトの登録を全て解除()
        {
            var obj1 = new GameObject();
            var component1 = obj1.AddComponent<BatchDummyComponent>();

            var obj2 = new GameObject();
            var component2 = obj2.AddComponent<BatchDummyComponent>();
            
            var system = BatchDummySystem.GetInstance(UpdateTiming.Update);
            
            BatchDummySystem.Register(component1, UpdateTiming.Update);
            BatchDummySystem.Register(component2, UpdateTiming.Update);
            
            BatchDummySystem.Unregister(component1, UpdateTiming.Update);
            BatchDummySystem.Unregister(component2, UpdateTiming.Update);

            Assert.That(((IComponentIndex)component1).Index, Is.EqualTo(-1));
            Assert.That(((IComponentIndex)component2).Index, Is.EqualTo(-1));
        }
    }
}
