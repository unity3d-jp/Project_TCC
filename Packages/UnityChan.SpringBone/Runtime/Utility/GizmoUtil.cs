using UnityEngine;

namespace UTJ
{
    public class GizmoUtil
    {
        public static void DrawArrow(Vector3 origin, Vector3 destination, Color color, float headRatio = 0.05f)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(origin, destination);

            var direction = destination - origin;
            var headStart = destination - headRatio * direction;
            var sideDirection = 0.5f * headRatio * (new Vector3(direction.y, direction.z, direction.x));
            Gizmos.DrawLine(destination, headStart + sideDirection);
            Gizmos.DrawLine(destination, headStart - sideDirection);
        }

        public static void DrawTransform(Transform transform, float drawScale, float headRatio = 0.05f)
        {
            if (transform != null)
            {
                DrawTransform(
                    transform.position, transform.right, transform.up, transform.forward, drawScale, headRatio);
            }
        }

        public static void DrawTransform
        (
            Vector3 origin, 
            Transform orientation, 
            float drawScale, 
            float headRatio = 0.05f
        )
        {
            if (orientation != null)
            {
                DrawTransform(origin, orientation.right, orientation.up, orientation.forward, drawScale, headRatio);
            }
        }

        public static void DrawTransform
        (
            Vector3 origin,
            Vector3 right,
            Vector3 up,
            Vector3 forward,
            float drawScale,
            float headRatio = 0.05f
        )
        {
            var xColor = Color.red;
            var yColor = Color.green;
            var zColor = new Color(0f, 0.9f, 1f);

            DrawArrow(origin, origin + drawScale * right, xColor, headRatio);
            DrawArrow(origin, origin + drawScale * up, yColor, headRatio);
            DrawArrow(origin, origin + drawScale * forward, zColor, headRatio);
        }
    }
}