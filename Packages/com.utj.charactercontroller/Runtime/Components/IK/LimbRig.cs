using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Ik
{
    [AddComponentMenu(MenuList.Ik + "LimbRig")]
    [RenamedFrom("TinyCharacterController.LimbRig")]
    [RenamedFrom("TinyCharacterController.Ik.LimbRig")]
    public class LimbRig : MonoBehaviour, IIkRig
    {
        [FormerlySerializedAs("isWorking")] 
        [RenamedFrom("isWorking")]
        public bool IsWorking = true;

        [SerializeField] private AvatarIKGoal _ikGoal;
        [SerializeField] private AvatarIKHint _ikHint;
        [SerializeField] private  Transform _target;
        [SerializeField] private  Transform _hint;
        
        [FormerlySerializedAs("transitionToEnable")] 
        [RenamedFrom("transitionToEnable")]
        public float TransitionToEnable;
        
        [FormerlySerializedAs("transitionToDisable")] 
        [RenamedFrom("transitionToDisable")]
        public float TransitionToDisable = 0.3f;
        
        private float _weight;
        private Animator _animator;

        void IIkRig.OnIkProcess(Vector3 offset)
        {
            _animator.SetIKPosition(_ikGoal, _target.position + offset);
            _animator.SetIKRotation(_ikGoal, _target.rotation);
            _animator.SetIKHintPosition(_ikHint, _hint.position + offset);
            
            _animator.SetIKHintPositionWeight(_ikHint, _weight);
            _animator.SetIKPositionWeight(_ikGoal, _weight);
            _animator.SetIKRotationWeight(_ikGoal, _weight);
        }

        bool IIkRig.IsValid => _target != null && _hint != null ;

        float IIkRig.Weight => _weight;

        void IIkRig.Initialize(Animator animator)
        {
            _animator = animator;
        }

        void IIkRig.OnPreProcess(float deltaTime)
        {
            _weight = isActiveAndEnabled && IsWorking ? 
                _weight + deltaTime / TransitionToEnable : 
                _weight - deltaTime / TransitionToDisable;
            _weight = Mathf.Clamp01(_weight);
        }
    }
}
