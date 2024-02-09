using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using UnityEngine;
using Unity.TinyCharacterController;
using Unity.TinyCharacterController.Control;

namespace Unity.TinyCharacterController.Smb
{
    /// <summary>
    /// Component to control AnimatorMoveControl from the StateMachine side.
    /// </summary>
    public class AnimatorMoveBehaviour : StateMachineBehaviour
    {
        /// <summary>
        /// Setting the duration during which AnimatorMoveControl's priority is active.
        /// </summary>
        [SerializeField, HideInInspector, Range(0, 1)] [Tooltip("Duration to raise the priority of AnimationMove")]
        private float _start = 0, _end = 1;

        [HideInInspector, SerializeField] private bool _copyed = false;

        [SerializeField, MinMax(0, 1)] private Vector2 _range;

        private void OnValidate()
        {
            if (_copyed == false)
            {
                _range.x = _start;
                _range.y = _end;
                _copyed = true;
            }
        }

        /// <summary>
        /// Determines whether to consider ground detection when moving the character.
        /// </summary>
        [SerializeField] [Tooltip("Determines whether to use ground detection when moving the character with AnimationMove. For example, set to False when moving up and down like on a ladder.")]
        private bool _useGroundNormal = true;

        public bool UseGroundNormal => _useGroundNormal;

        /// <summary>
        /// Process character movement as a Warp instead of using Control.
        /// </summary>
        [SerializeField] 
        private bool _isFixedPosition = false;

        public bool IsFixedPosition => _isFixedPosition;

        public bool IsInRange(AnimatorStateInfo info)
        {
            var normalizedTime = info.loop ? info.normalizedTime % 1 : info.normalizedTime;

            return _range.x < normalizedTime && _range.y > normalizedTime;
        }
    }
}
