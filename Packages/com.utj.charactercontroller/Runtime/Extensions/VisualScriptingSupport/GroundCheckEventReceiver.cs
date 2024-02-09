using System;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.VisualScripting
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.VisualScripting.GroundCheckEventReceiver")] 
    public class GroundCheckEventReceiver : MonoBehaviour
    {
        private IGroundObject _groundCheck;

        private void Awake()
        {
            TryGetComponent(out _groundCheck);
            _groundCheck.OnChangeGroundObject.AddListener(OnChangeGround);
        }

        private void OnDestroy()
        {
            _groundCheck.OnChangeGroundObject.RemoveListener(OnChangeGround);
        }

        private void OnChangeGround(GameObject obj)
        {
            EventBus.Trigger(EventNames.GroundCheckOnChangeGround,  gameObject,new GroundObject{ Obj = obj});
        }
    }
}