using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Utility;
using Unity.TinyCharacterController.Interfaces;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    [DefaultExecutionOrder(Order.EarlyUpdateBrain)]
    [AddComponentMenu("")]
    public class EarlyUpdateBrain : EarlyUpdateBrainBase
    {
        private void Update() => OnUpdate();
    }
}
