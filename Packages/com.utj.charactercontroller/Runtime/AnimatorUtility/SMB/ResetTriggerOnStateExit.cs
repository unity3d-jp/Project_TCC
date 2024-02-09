using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.TinyCharacterController.Smb
{
    public class ResetTriggerOnStateExit : StateMachineBehaviour
    {
        [SerializeField] string _triggerName;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger(_triggerName);
        }
    }
    
}

