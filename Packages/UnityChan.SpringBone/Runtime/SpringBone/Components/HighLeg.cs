using UnityEngine;

namespace UTJ
{
    public class HighLeg : MonoBehaviour
    {
        [System.Serializable]
        public class TransformPair
        {
            public Transform start;
            public Transform end;

            public Vector3 GetDirection()
            {
                return (end.position - start.position).normalized;
            }
        }

        public Transform hipPivot;
        public TransformPair[] legs;

        // private

        private void LateUpdate()
        {
            var forwardVector = hipPivot.forward;
            var upVector = hipPivot.up;
            var legCount = legs.Length;
            var upDot = -1f;
            for (int legIndex = 0; legIndex < legCount; legIndex++)
            {
                var legDirection = legs[legIndex].GetDirection();
                var forwardDot = Vector3.Dot(legDirection, forwardVector);
                if (forwardDot >= 0f)
                {
                    var dotProduct = Vector3.Dot(legDirection, upVector);
                    upDot = Mathf.Max(dotProduct, upDot);
                }
            }

            var remainder = Mathf.Sqrt(Mathf.Clamp01(1f - upDot * upDot));
            var targetDirection = upVector * upDot + forwardVector * remainder;
            var target = transform.position + targetDirection;

            transform.LookAt(target, (upDot > -0.5f) ? upVector : forwardVector);
        }
    }
}