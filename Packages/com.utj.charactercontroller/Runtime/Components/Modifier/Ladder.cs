using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.Modifier
{
    /// <inheritdoc />
    /// <summary>
    /// Ladder configuration.
    /// Used to control movement with MoveLadderControl.
    /// </summary>
    [AddComponentMenu(MenuList.Gimmick + "Ladder")]
    [DisallowMultipleComponent]
    [RenamedFrom("Ladder")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Modifier.Ladder")]
    public class Ladder : MonoBehaviour
    {
        [Header("Ladder Length")] [Tooltip("Starting point of the ladder")] [SerializeField]
        private Vector3 _startOffset = new Vector3(0, 0, 0);

        [SerializeField] private Vector3 _characterOffset = new Vector3(0, 0, 0);

        [SerializeField] [Tooltip("Ladder height")] private float _length;

        [Tooltip("Number of steps")] [SerializeField] private int _stepCount = 5;


        [Header("Climbing and Descending Points on the Ladder")] [SerializeField]
        private Vector3 _topStartPosition;

        [SerializeField] private Vector3 _bottomStartPosition;

        /// <summary>
        /// Get the closest position on the ladder's step.
        /// </summary>
        /// <param name="position">Position to find the closest step to.</param>
        /// <returns>Closest step position.</returns>
        public Vector3 CloseStepPosition(Vector3 position)
        {
            var point = ClosePoint(position);
            var closestStepPoint = CloseStepPoint(point);

            return GetPosition(closestStepPoint);
        }

        /// <summary>
        /// Get the closest position on the ladder's step.
        /// </summary>
        /// <param name="point">Distance to move.</param>
        /// <returns>Step position.</returns>
        public Vector3 CloseStepPosition(float point)
        {
            var closestStepPoint = CloseStepPoint(point);

            return GetPosition(closestStepPoint);
        }

        /// <summary>
        /// Get the closest position on the path.
        /// </summary>
        /// <param name="position">Position to find the closest point to.</param>
        /// <returns>Closest point on the path.</returns>
        public Vector3 ClosePosition(Vector3 position)
        {
            var distance = ClosePoint(position);
            return GetPosition(distance);
        }

        /// <summary>
        /// Get the closest step point for a given distance.
        /// </summary>
        /// <param name="point">Distance to move.</param>
        /// <returns>Adjusted distance based on steps.</returns>
        public float CloseStepPoint(float point)
        {
            var stepSize = GetStepSize();
            var minDistance = Length;
            var closestStep = 0f;

            for (var i = 0; i < _stepCount; i++)
            {
                var stepPoint = stepSize * i;
                var distance = Mathf.Abs(point - stepPoint);

                if (distance > minDistance)
                    continue;

                minDistance = distance;
                closestStep = i;
            }

            return stepSize * closestStep;
        }

        /// <summary>
        /// Get the closest step point for a given position.
        /// </summary>
        /// <param name="position">Position to find the closest step to.</param>
        /// <returns>Adjusted distance based on steps.</returns>
        public float CloseStepPoint(Vector3 position)
        {
            var point = ClosePoint(position);
            return CloseStepPoint(point);
        }

        /// <summary>
        /// Get the distance along the path for a given position.
        /// </summary>
        /// <param name="position">Position to measure distance from.</param>
        /// <returns>Distance along the path.</returns>
        public float ClosePoint(Vector3 position)
        {
            var offset = _transform.rotation * _startOffset;
            var endLocalPosition = offset + Vector3.up * _length;
            var line = endLocalPosition - offset;
            var length = line.magnitude;
            var direction = line.normalized;

            var delta = position - _transform.position - offset;
            var dot = Vector3.Dot(delta, direction);
            var clampedDot = Mathf.Clamp(dot, 0f, length);
            return clampedDot;
        }

        /// <summary>
        /// Get the bottom position of the ladder.
        /// </summary>
        public Vector3 BottomPosition => _transform.rotation * _startOffset + _transform.position;

        /// <summary>
        /// Get the length of the ladder.
        /// </summary>
        public float Length => _length;

        /// <summary>
        /// Get the top position of the ladder.
        /// </summary>
        public Vector3 TopPosition => _transform.rotation * _startOffset + (Vector3.up * _length) + _transform.position;

        /// <summary>
        /// Get the top start position.
        /// </summary>
        public Vector3 TopStartPosition => _transform.rotation * _topStartPosition + (Vector3.up * _length) + _transform.position;

        /// <summary>
        /// Get the bottom start position.
        /// </summary>
        public Vector3 BottomStartPosition => _transform.rotation * _bottomStartPosition + _transform.position;

        /// <summary>
        /// Get the position based on distance along the ladder.
        /// </summary>
        /// <param name="distance">Distance along the ladder.</param>
        /// <returns>Position on the ladder.</returns>
        public Vector3 GetPosition(float distance)
        {
            return Vector3.Lerp(BottomPosition, TopPosition, distance / Length) +
                   _transform.rotation * _characterOffset;
        }

        private Transform _transform;
        private bool _initialized = false;

        private void Awake()
        {
            BindComponents();
        }

        private void OnDrawGizmosSelected()
        {
            BindComponents();

            Gizmos.color = Color.red;
            var flat = new Vector3(0.1f, 0.1f, 0.1f);
            Gizmos.DrawCube(BottomPosition, flat);
            Gizmos.DrawCube(TopPosition, flat);
            Gizmos.DrawLine(BottomPosition, TopPosition);

            var stepSize = GetStepSize();
            Gizmos.color = Color.yellow;
            for (var i = 1; i < _stepCount; i++)
            {
                var position = CloseStepPosition(stepSize * i) - _characterOffset;
                Gizmos.DrawCube(position, flat);
            }

            Gizmos.DrawCube(BottomStartPosition, Vector3.one * 0.3f);
            Gizmos.DrawCube(TopStartPosition, Vector3.one * 0.3f);
        }

        public float GetStepSize() => Length / _stepCount;

        private void BindComponents()
        {
            if (_initialized == true)
                return;

            TryGetComponent(out _transform);
        }
    }
}
