using Unity.TinyCharacterController.Attributes;
using UnityEngine;

namespace Unity.TinyCharacterController.Smb
{
    /// <summary>
    /// Register keys in the Animator's states.
    /// </summary>
    [SharedBetweenAnimators]
    public class AnimatorModifierBehaviour : StateMachineBehaviour
    {
        /// <summary>
        /// The key to register.
        /// </summary>
        [SerializeField]
        private PropertyName _key;

        /// <summary>
        /// The range where the key is active.
        /// </summary>
        [SerializeField, MinMax(0, 1)]
        private Vector2 _range = new (0, 1);
        
        /// <summary>
        /// String representation of key information.
        /// For debugging purposes.
        /// </summary>
        public string KeyName => _key.ToString().Split(':')[0];

        /// <summary>
        /// Key information.
        /// </summary>
        public PropertyName Key => _key;

        /// <summary>
        /// Returns true if the specified state is within the range.
        /// </summary>
        /// <param name="stateInfo"></param>
        /// <returns></returns>
        public bool IsInRange(in AnimatorStateInfo stateInfo)
        {
            if (stateInfo is { loop: false, normalizedTime: > 1 })
                return false;
        
            var normalizeTime = stateInfo.normalizedTime % 1;
            var inRange = normalizeTime > _range.x && normalizeTime < _range.y;

            return inRange;
        }
    }
}