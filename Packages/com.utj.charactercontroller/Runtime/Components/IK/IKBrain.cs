using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.Ik
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(Order.UpdateIK)]
    [AddComponentMenu(MenuList.Ik + nameof(IKBrain))]
    [RenamedFrom("TinyCharacterController.IKBrain")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Ik.IKBrain")]
    public class IKBrain : MonoBehaviour
    {
        private readonly List<IIkRig> _additionalRigs = new ();
        private readonly List<IIkRig> _rigs = new();
        private Animator _animator;

        [SerializeField]
        private GameObject additionalRigTargets;


        public Vector3 animatorOffset;

        private void Awake()
        {
            TryGetComponent(out _animator);
        }

        
        public GameObject AdditionalRig
        {
            get => additionalRigTargets;
            set
            {
                additionalRigTargets = value;
                GatherController(value, _additionalRigs);
            }
        }
        
        private void Start()
        {
            GatherController(gameObject,_rigs);
            GatherController(additionalRigTargets, _additionalRigs);
        }

        public void Rebind()
        {
            
        }

        private void Update()
        {
            foreach( var rig in _rigs)
                rig.OnPreProcess(Time.deltaTime);
            
            foreach( var rig in _additionalRigs)
                rig.OnPreProcess(Time.deltaTime);
        }

        private void GatherController(GameObject obj, List<IIkRig> rigs)
        {
            if(obj == null)
                return;
            
            rigs.Clear();
            obj.GetComponents(rigs);
            
            foreach (var rig in rigs)
                rig.Initialize(_animator);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            _animator.bodyPosition += animatorOffset;

            foreach (var rig in _rigs)
            {
                if( rig.IsValid)
                    rig.OnIkProcess(animatorOffset);
            }
            
            foreach (var rig in _additionalRigs)
            {
                if( rig.IsValid)
                    rig.OnIkProcess(animatorOffset);
            }
        }
    }
}
