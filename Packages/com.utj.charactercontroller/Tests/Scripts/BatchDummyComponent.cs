using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.TinyCharacterController.Test
{
    public class BatchDummyComponent : ComponentBase
    {
        
        

    }

    public class BatchDummySystem : SystemBase<BatchDummyComponent, BatchDummySystem>
    {
        public int Count => Components.Count;

        public UnityEvent OnDeleteInstance = new();

        private void OnDestroy()
        {
            UnregisterAllComponents();
            OnDeleteInstance.Invoke();
        }
    }
}
