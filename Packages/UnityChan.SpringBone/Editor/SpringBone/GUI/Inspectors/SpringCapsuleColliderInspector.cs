using System.Linq;
using UnityEditor;

namespace UTJ
{
    [CustomEditor(typeof(SpringCapsuleCollider))]
    [CanEditMultipleObjects]
    public class SpringCapsuleColliderInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (boneSelector == null)
            {
                boneSelector = SpringColliderBoneSelector.Create<SpringCapsuleCollider>(
                    targets, (bone, colliders) => bone.capsuleColliders.Any(collider => colliders.Contains(collider)));
            }
            boneSelector.ShowInspector();
        }

        // private

        private SpringColliderBoneSelector boneSelector;
    }
}