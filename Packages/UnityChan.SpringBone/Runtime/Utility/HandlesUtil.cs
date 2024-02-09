#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class HandlesUtil
    {
        public static void DrawArrow(Vector3 origin, Vector3 destination, Color color, float headRatio = 0.05f)
        {
            Handles.color = color;
            Handles.DrawLine(origin, destination);

            var direction = destination - origin;
            var headStart = destination - headRatio * direction;
            var sideDirection = 0.5f * headRatio * (new Vector3(direction.y, direction.z, direction.x));
            Handles.DrawLine(destination, headStart + sideDirection);
            Handles.DrawLine(destination, headStart - sideDirection);
        }

        public static void DrawTransform
        (
            Transform transform, 
            float drawScale, 
            float headRatio = 0.05f, 
            float brightness = 1f
        )
        {
            if (transform != null)
            {
                DrawTransform(
                    transform.position, transform.right, transform.up, transform.forward, drawScale, headRatio, brightness);
            }
        }

        public static void DrawTransform
        (
            Vector3 origin,
            Transform orientation,
            float drawScale,
            float headRatio = 0.05f,
            float brightness = 1f
        )
        {
            if (orientation != null)
            {
                DrawTransform(origin, orientation.right, orientation.up, orientation.forward, drawScale, headRatio, brightness);
            }
        }

        public static void DrawTransform
        (
            Vector3 origin,
            Vector3 right,
            Vector3 up,
            Vector3 forward,
            float drawScale,
            float headRatio = 0.05f,
            float brightness = 1f
        )
        {
            var xColor = brightness * Color.red;
            var yColor = brightness * Color.green;
            var zColor = brightness * new Color(0f, 0.9f, 1f);
            xColor.a = 1f;
            yColor.a = 1f;
            zColor.a = 1f;

            DrawArrow(origin, origin + drawScale * right, xColor, headRatio);
            DrawArrow(origin, origin + drawScale * up, yColor, headRatio);
            DrawArrow(origin, origin + drawScale * forward, zColor, headRatio);
        }
    }
}

#endif