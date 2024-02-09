using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.TinyCharacterController.Modifier
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class RootMotionReceiver : MonoBehaviour
    {
        public Vector3 Velocity { get; private set; }
        public Quaternion Rotation { get; private set; }

        private Animator _animator;

        void Awake()
        {
            TryGetComponent(out _animator);
        }

        void OnAnimatorMove()
        {
            Velocity = _animator.velocity;
            Rotation = _animator.deltaRotation;
        }
    }
}
