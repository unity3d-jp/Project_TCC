using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Interfaces;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    public abstract class EarlyUpdateBrainBase : MonoBehaviour
    {
        private readonly List<IEarlyUpdateComponent> _updates = new();
        
        private void Awake()
        {
            GetComponents(_updates);
            _updates.Sort((a,b) => a.Order - b.Order);
        }

        protected void OnUpdate()
        {
            // If executed at the timing of FixedUpdate, deltaTime returns the value of FixedUpdate.
            var deltaTime = Time.deltaTime;
            
            foreach (var update in _updates)
            {
                update.OnUpdate(deltaTime);
            }
        }
    }
}