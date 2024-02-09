using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Components;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Smb
{
    /// <summary>
    /// This component is used to control the matching of a character's animation to a specific target
    /// within an animation state, with the ability to control the transition and behavior as the animation progresses. 
    /// </summary>
    public class MatchTargetBehaviour : StateMachineBehaviour
    {
        [FormerlySerializedAs("_key")] 
        [SerializeField] private PropertyName _id;

        [Header("Settings")]
        [SerializeField]
        [MinMax(0f, 1f)] private Vector2 _minMax;

        [SerializeField] private AnimationCurve _moveCurve;
        [SerializeField] private AnimationCurve _turnCurve;


        private ILinearInterpolationMove _linearInterpolation;
        private bool _gatheredComponents = false;
        private bool _inRange = false;
        private bool _isProcessedTransition = false;
        private bool _isStopped = false;

        private void Reset()
        {
            _moveCurve = AnimationCurve.Linear(0, 0, 1, 1);
            _turnCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GatherComponent(animator);

            _inRange = false;
            _isProcessedTransition = false;
            _isStopped = false;
        }

        /// <summary>
        /// Gather the components
        /// </summary>
        /// <param name="animator">The GameObject to gather components from</param>
        private void GatherComponent(in Animator animator)
        {
            if (_gatheredComponents)
                return;

            animator.TryGetComponent(out _linearInterpolation);
            _gatheredComponents = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            // If the transition has already been processed, move to the target position in the playback point.
            if (_isProcessedTransition  && _isStopped == false)
                _linearInterpolation.Stop(_id);
        }

        private bool IsInRange(float normalizedTime)
        {
            var time = normalizedTime % 1;
            return  time > _minMax.x && time < _minMax.y;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_isStopped)
                return;
            
            // stateInfo.normalizedTime can exceed 1 in looped animations, so normalize it to the range of 0-1.
            var normalized = stateInfo.loop ? stateInfo.normalizedTime % 1 : stateInfo.normalizedTime;

            var preInRange = _inRange;
            _inRange = IsInRange(normalized);

            if (preInRange == false && _inRange)
            {
                // When entering the range for the first time, start the MatchTarget.
                _isProcessedTransition = true;
                _linearInterpolation.Play(_id);
            }

            if (_isProcessedTransition == false && normalized > _minMax.y)
            {
                _isProcessedTransition = true;
                _linearInterpolation.FitTargetWithoutPlay(_id);
                return;
            }

            if (preInRange && _inRange == false )
            {
                // Since the range has been exited, stop the process.
                _linearInterpolation.Stop(_id);
                _isStopped = true;
                return;
            }

            // Inside the range, progress the transition.
            if (_inRange)
            {
                var amount = Remap(normalized, _minMax.x, _minMax.y);
                var moveAmount = _moveCurve.Evaluate(amount);
                var turnAmount = _turnCurve.Evaluate(amount);
                _linearInterpolation.SetNormalizedTime(moveAmount, turnAmount);
            }
        }

        /// <summary>
        /// Normalize the value of <see cref="value"/> from the range of `from` to the range of `to`.
        /// </summary>
        /// <param name="value">The target value</param>
        /// <param name="fromMin">Original minimum value</param>
        /// <param name="fromMax">Original maximum value</param>
        /// <param name="toMin">Transformed minimum value</param>
        /// <param name="toMax">Transformed maximum value</param>
        /// <returns>Remap value</returns>
        private static float Remap(float value, float fromMin, float fromMax, float toMin = 0, float toMax = 1)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }
    }
}
