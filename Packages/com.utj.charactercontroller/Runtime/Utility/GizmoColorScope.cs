using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.Utility
{


    public static class GizmoDrawUtility
    {
        private static Mesh _capsuleMesh;

        private static Mesh CapsuleMesh
        {
            get
            {
                if (_capsuleMesh != null)
                    return _capsuleMesh;
                
                _capsuleMesh = Resources.GetBuiltinResource<Mesh>("New-Capsule.fbx");
                return _capsuleMesh;
            }
        }
        
        private readonly struct GizmoColorScope : IDisposable
        {
            private readonly Color _cachedColor;
            public GizmoColorScope(Color color, float alpha)
            {
                _cachedColor = Gizmos.color;
                color.a = alpha;
                Gizmos.color = color;
            }

            void IDisposable.Dispose()
            {
                Gizmos.color = _cachedColor;
            }
        }
        
        public static void DrawCollider(in Collider col, in Color color, float alpha = 0.4f)
        {
            var trs = col.transform;
            var position = trs.position;
            var rotation = trs.rotation;
            var scale = trs.localScale;

            switch (col)
            {
                case MeshCollider meshCollider:
                    DrawMesh(meshCollider.sharedMesh, position, rotation, scale, color, alpha);
                    break;
                case BoxCollider boxCollider:
                    DrawCube(boxCollider.center, boxCollider.size, position, rotation, scale, color, alpha);
                    break;
                case SphereCollider sphereCollider:
                    DrawSphere(position, sphereCollider.radius * scale.magnitude * 0.5f, color, alpha);
                    break;
                case CapsuleCollider:
                case CharacterController:
                    DrawMesh(CapsuleMesh, position, rotation, scale, color, alpha);
                    break;
            }
        }

        public static void DrawMesh(Mesh mesh, in Vector3 position, in Quaternion rotation, in Vector3 scale, in Color color, float alpha = 0.4f)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireMesh(mesh, position, rotation, scale);

            if ((alpha > 0) == false) 
                return;
            
            using (new GizmoColorScope(color, alpha))
                Gizmos.DrawMesh(mesh, position, rotation, scale);
        }

        public static void DrawCube(in Vector3 center, in Vector3 size, in Color color, float alpha = 0.4f)
        {
            DrawCube(center, size, Vector3.zero, quaternion.identity, Vector3.one, color, alpha);
        }

        public static void DrawCube(in Vector3 center, in Vector3 size, in Vector3 position, in Quaternion rotation, in Vector3 scale, in Color color, float alpha = 0.4f)
        {
            
            var cache = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(center, size);
            
            if ((alpha > 0) == false) 
                return;

            using (new GizmoColorScope(color, alpha))
                Gizmos.DrawCube(center, size);
            Gizmos.matrix = cache;
        }

        public static void DrawSphere(in Vector3 position, in float radius, in Color color, float alpha = 0.4f)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(position, radius);
            
            if ((alpha > 0) == false) 
                return;

            using (new GizmoColorScope(color, alpha))
                Gizmos.DrawSphere(position, radius);
        }

    }
}
