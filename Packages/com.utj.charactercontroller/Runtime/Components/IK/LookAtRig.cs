using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Ik
{
    [DisallowMultipleComponent]
    [AddComponentMenu(MenuList.Ik + nameof(LookAtRig))]
    [RenamedFrom("TinyCharacterController.LookAtRig")]
    [RenamedFrom("TinyCharacterController.Ik.LookAtRig")]
    public class LookAtRig : MonoBehaviour, IIkRig
    {
        /// <summary>
        /// If true, the character looks at the target.
        /// </summary>
        [FormerlySerializedAs("isWork")] 
        [RenamedFrom("isWork")]
        public bool IsWork ;

        /// <summary>
        /// The object to look at.
        /// </summary>
        [AllowsNull]
        [RenamedFrom("target")]
        [FormerlySerializedAs("target")]
        public Transform Target;
        
        /// <summary>
        /// Time to switch the look-at effect on/off.
        /// </summary>
        [FormerlySerializedAs("transitionTime")] 
        [RenamedFrom("transitionTime")]
        public float TransitionTime = 1;

        [Range(0, 1)] 
        [FormerlySerializedAs("bodyWeight")] 
        [RenamedFrom("bodyWeight")]
        public float BodyWeight = 0.12f;
        
        [Range(0, 1)] 
        [FormerlySerializedAs("headWeight")] 
        [RenamedFrom("headWeight")]
        public float HeadWeight = 0.3f;
        
        [Range(0, 1)] 
        [FormerlySerializedAs("eyeWeight")]
        [RenamedFrom("eyeWeight")]
        public float EyeWeight = 0.3f;
        
        [Range(0, 1)]
        [FormerlySerializedAs("clampAngle")]
        [RenamedFrom("clampAngle")]
        public float ClampAngle = 0.5f;

        private bool _isValid;
        private float _weight;
        private Animator _animator;


        bool IIkRig.IsValid => Target != null;
        float IIkRig.Weight => _weight;

        public void Initialize(Animator animator)
        {
            _animator = animator;
        }

        void IIkRig.OnPreProcess(float deltaTime)
        {
            var speed = deltaTime / TransitionTime;
            _weight = isActiveAndEnabled && IsWork ? _weight + speed : _weight - speed;
            _weight = Mathf.Clamp01(_weight);
        }
        
        void IIkRig.OnIkProcess(Vector3 offset)
        {
            var clamp = 1 - ClampAngle;

            _animator.SetLookAtPosition(Target.position + offset);
            _animator.SetLookAtWeight(_weight, BodyWeight, HeadWeight, EyeWeight, clamp );
        }
    }
}
