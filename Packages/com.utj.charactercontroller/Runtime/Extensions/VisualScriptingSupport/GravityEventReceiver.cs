using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController;
using Unity.TinyCharacterController.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.VisualScripting
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.VisualScripting.GravityEventReceiver")]   

    public class GravityEventReceiver : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<IGravityEvent>().OnLanding.AddListener(OnLanding);
        }

        private void OnDestroy()
        {
            GetComponent<IGravityEvent>().OnLanding.RemoveListener(OnLanding);
        }

        private void OnLanding(float value)
        {
            EventBus.Trigger(EventNames.GravityOnLanding, gameObject, value);
        }

        private void OnLeaving()
        {
            EventBus.Trigger(EventNames.GravityOnLeaving, gameObject);
        }
    }
}


