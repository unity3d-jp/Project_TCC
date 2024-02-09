using UnityEngine;

namespace UTJ
{
    public class WindVolume : ForceProvider
    {
        [Range(0f, 1f)]
        public float weight = 0f;
        public float strength = 100f;
        public float amplitude = 1f;
        public float period = 1.5f;
        public float spinPeriod = 1.5f;
        public float peakDistance = 0.2f;

        public override Vector3 GetForceOnBone(SpringBone springBone)
        {
            var fullWeight = weight * strength;
            if ((fullWeight <= 0.0001f)
                | (period <= 0.001f))
            {
                return Vector3.zero;
            }

            var localPosition = transform.InverseTransformPoint(springBone.transform.position);
            var positionalFactor = GetPositionalFactor(localPosition.x, localPosition.z);
            var offsetMultiplier = Mathf.Sin(timeFactor + positionalFactor);
            var forceAtPosition = fullWeight * (transform.forward + offsetMultiplier * offsetVector).normalized;
            return springBone.windInfluence * forceAtPosition;
        }

        // private

        private const float PI2 = Mathf.PI * 2f;

        private float positionalMultiplier;
        private float currentTime;
        private float timeFactor;
        private float spinTime;
        private Vector3 offsetVector;

        private float GetPositionalFactor(float x, float y)
        {
            return Mathf.Sin(positionalMultiplier * x) + Mathf.Cos(positionalMultiplier * y);
        }

        private float AddPeriodically(float currentValue, float deltaValue, float period)
        {
            var newValue = currentValue + deltaValue;
            while (newValue >= period)
            {
                newValue -= period;
            }
            return newValue;
        }

        private void Update()
        {
            if (weight <= 0.0001f
                || period <= 0.001f)
            {
                return;
            }

            peakDistance = Mathf.Max(peakDistance, 0.0001f);
            positionalMultiplier = PI2 / peakDistance;

            var deltaTime = Time.deltaTime;
            currentTime = AddPeriodically(currentTime, deltaTime, period);
            timeFactor = currentTime * PI2 / period;

            offsetVector = transform.up;
            if (Mathf.Abs(spinPeriod) > 0.001f)
            {
                spinTime = AddPeriodically(spinTime, deltaTime, spinPeriod);
                var spinTimeFactor = spinTime * PI2 / spinPeriod;
                offsetVector = Mathf.Cos(spinTimeFactor) * transform.right 
                    + Mathf.Sin(spinTimeFactor) * transform.up;
            }
            offsetVector = amplitude * offsetVector;
        }

        private void OnDrawGizmos()
        {
            var origin = transform.position;
            var strengthMultiplier = Mathf.Clamp(strength, 0.1f, 1f);
            var destination = origin + strengthMultiplier * transform.forward;
            var offsets = new Vector3[]
            {
                Vector3.zero,
                0.02f * transform.up,
                -0.02f * transform.up
            };

            foreach (var offset in offsets)
            {
                GizmoUtil.DrawArrow(origin + offset, destination + offset, Color.gray, 0.1f);
            }
        }
    }
}