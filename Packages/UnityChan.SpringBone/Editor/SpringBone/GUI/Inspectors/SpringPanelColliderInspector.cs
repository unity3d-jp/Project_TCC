using System.Linq;
using UnityEditor;

namespace UTJ
{
    [CustomEditor(typeof(SpringPanelCollider))]
    [CanEditMultipleObjects]
    public class SpringPanelColliderInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (boneSelector == null)
            {
                boneSelector = SpringColliderBoneSelector.Create<SpringPanelCollider>(
                    targets, (bone, colliders) => bone.panelColliders.Any(collider => colliders.Contains(collider)));
            }
            boneSelector.ShowInspector();
        }

        // private

        private SpringColliderBoneSelector boneSelector;
    }
}