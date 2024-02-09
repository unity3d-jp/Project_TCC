using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    internal class UpdateComponentManager
    {
        private readonly List<IUpdateComponent> _updates = new();     // List of components to be updated at runtime

        public void Initialize(GameObject obj)
        {
            obj.GetComponents(_updates);
            _updates.Sort((a,b) => a.Order - b.Order);
        }
        
        public void Process(float deltaTime)
        {
            using var _ = new ProfilerScope("Component Update");
            foreach (var update in _updates)
            {
                update.OnUpdate(deltaTime);
            }
        }
    }
}