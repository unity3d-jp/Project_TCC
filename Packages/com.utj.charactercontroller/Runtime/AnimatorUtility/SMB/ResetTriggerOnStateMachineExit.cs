using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.TinyCharacterController.Smb
{
    public class ResetTriggerOnStateMachineExit : StateMachineBehaviour
    {
        [SerializeField] string _triggerName;

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            animator.ResetTrigger(_triggerName);
        }
    }
}