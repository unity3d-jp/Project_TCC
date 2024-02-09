using UnityEngine;

namespace UTJ
{
    public class ForceVolume : ForceProvider 
    {
        public float strength = 0.01f;

        public override Vector3 GetForceOnBone(SpringBone springBone)
        {
            return strength * transform.forward;
        }

        // private

        private void OnDrawGizmos()
        {
            const float DrawScale = 10f;
            var origin = transform.position;
            var destination = origin + strength * DrawScale * transform.forward;
            GizmoUtil.DrawArrow(origin, destination, Color.gray, 0.1f);
        }
    }
}