using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    internal class CameraManager
    {
        public void Initialize(GameObject obj)
        {
            obj.GetComponents(_cameraUpdates);
        }
        
        private readonly List<ICameraUpdate> _cameraUpdates = new();  // List of Camera Control Components

        public void Process(float deltaTime)
        {
            using var _ = new ProfilerScope("Camera Update");
            
            // No limitation by priority.
            // The final orientation is determined by Cinemachine.
            foreach (var cameraUpdate in _cameraUpdates)
            {
                cameraUpdate.OnUpdate(deltaTime);
            }
        }
    }
}