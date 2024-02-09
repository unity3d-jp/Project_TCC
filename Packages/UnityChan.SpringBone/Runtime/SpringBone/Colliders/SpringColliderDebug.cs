#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace UTJ
{
    public class SpringColliderDebug
    {
        public void ClearCollisions()
        {
            collisions.Clear();
        }

        public void RecordCollision
        (
            Vector3 position,
            Vector3 normal,
            float radius, 
            SpringBone.CollisionStatus collisionStatus
        )
        {
            Profiler.BeginSample("SpringColliderDebug.RecordCollision");
            var newCollision = new Collision
            {
                position = position,
                normal = normal,
                radius = radius,
                collisionStatus = collisionStatus
            };
            collisions.Add(newCollision);
            Profiler.EndSample();
        }

        public void DrawGizmosAndClear()
        {
            var collisionCount = collisions.Count;
            for (int collisionIndex = 0; collisionIndex < collisionCount; collisionIndex++)
            {
                var collision = collisions[collisionIndex];
                Handles.color = (collision.collisionStatus == SpringBone.CollisionStatus.HeadIsEmbedded) ?
                    Color.red : Color.yellow;
                Handles.DrawWireDisc(collision.position, collision.normal, collision.radius);
                HandlesUtil.DrawArrow(
                    collision.position, collision.position + collision.normal * 0.1f, Handles.color, 0.1f);
            }
            ClearCollisions();
        }

        // private

        private class Collision
        {
            public Vector3 position;
            public Vector3 normal;
            public float radius;
            public SpringBone.CollisionStatus collisionStatus;
        }

        private List<Collision> collisions = new List<Collision>();
    }
}

#endif