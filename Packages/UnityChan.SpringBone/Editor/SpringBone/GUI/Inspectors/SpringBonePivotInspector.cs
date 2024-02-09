using UTJ.GameObjectExtensions;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    [CustomEditor(typeof(SpringBonePivot))]
    [CanEditMultipleObjects]
    public class SpringBonePivotInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            InitializeData();

            if (GUILayout.Button("ボーンを選択", SpringBoneGUIStyles.ButtonStyle))
            {
                Selection.objects = bones.Select(bone => bone.gameObject).ToArray();
            }

            base.OnInspectorGUI();

            var managerCount = managers.Length;
            for (int managerIndex = 0; managerIndex < managerCount; managerIndex++)
            {
                EditorGUILayout.ObjectField("Manager", managers[managerIndex], typeof(SpringManager), true);
            }

            var boneCount = bones.Length;
            for (int boneIndex = 0; boneIndex < boneCount; boneIndex++)
            {
                EditorGUILayout.ObjectField("Bone", bones[boneIndex], typeof(SpringBone), true);
            }
        }

        private SpringManager[] managers;
        private SpringBone[] bones;

        private void InitializeData()
        {
            if (managers != null && managers.Length > 0) { return; }

            managers = targets
                .Select(target => target as Component)
                .Where(target => target != null)
                .Select(target => target.GetComponentInParent<SpringManager>())
                .Where(manager => manager != null)
                .Distinct()
                .ToArray();

            var pivots = targets
                .Where(target => target is Component)
                .Select(target => ((Component)target).transform)
                .ToArray();

            bones = GameObjectUtil.FindComponentsOfType<SpringBone>()
                .Where(bone => pivots.Contains(bone.pivotNode))
                .ToArray();
        }
    }
}