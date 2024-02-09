using System;
using Unity.VisualScripting;
using UnityEngine;
using Unity.TinyCharacterController.Check;

namespace Unity.TinyCharacterController.VisualScripting
{
    [DisallowMultipleComponent]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.VisualScripting.AnimationModifierEventReceiver")]   
    public class AnimationModifierEventReceiver : MonoBehaviour
    {
        private AnimatorModifierCheck _component;
        
        private void Awake()
        {
            TryGetComponent(out _component);
            _component.OnChangeKey.AddListener(OnChangeKey);
        }

        private void OnDestroy()
        {
            _component.OnChangeKey.RemoveListener(OnChangeKey);
        }

        private void OnChangeKey()
        {
            EventBus.Trigger(EventNames.AnimationModifierCheckOnChanged, gameObject, _component);
        }
    }
}