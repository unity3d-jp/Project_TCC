#if PACKAGE_INPUT_SYSTEM_EXISTS

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.TinyCharacterController.Extensions
{
    [DefaultExecutionOrder(-1000)]
    public class SwitchInputUpdateTiming : MonoBehaviour
    {
        [SerializeField] private InputSettings.UpdateMode _updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
        private void Awake()
        {
            InputSystem.settings.updateMode = _updateMode;
        }
    }
}



#endif