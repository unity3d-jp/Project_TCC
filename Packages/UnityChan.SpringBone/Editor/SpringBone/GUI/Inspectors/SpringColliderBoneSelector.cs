using UTJ.GameObjectExtensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class SpringColliderBoneSelector
    {
        public static SpringColliderBoneSelector Create<T>
        (
            Object[] targets,
            System.Func<SpringBone, IEnumerable<T>, bool> containmentCheck
        ) where T : class
        {
            var affectedSpringBones = FindSpringBonesUsingCollider<T>(targets, containmentCheck);
            var totalBoneCount = affectedSpringBones.Sum(item => item.bones.Length);
            return new SpringColliderBoneSelector()
            {
                affectedBoneGroups = affectedSpringBones,
                totalBoneCount = totalBoneCount,
                springBoneTitle = string.Format("Spring Bones : {0}", totalBoneCount)
            };
        }

        public void ShowInspector()
        {
            if (totalBoneCount == 0) { return; }

            const string SelectAllLabel = "全選択";

            SpringBoneGUIStyles.ReacquireStyles();

            EditorGUILayout.Space();
            GUILayout.BeginVertical("box");
            GUILayout.Label(springBoneTitle, SpringBoneGUIStyles.HeaderLabelStyle);
            EditorGUILayout.Space();

            var groupCount = affectedBoneGroups.Length;
            if (groupCount >= 2)
            {
                if (GUILayout.Button(SelectAllLabel, SpringBoneGUIStyles.ButtonStyle))
                {
                    Selection.objects = affectedBoneGroups
                        .SelectMany(group => group.bones)
                        .Select(bone => bone.gameObject)
                        .ToArray();
                }
                EditorGUILayout.Space();
            }

            for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
            {
                var bones = affectedBoneGroups[groupIndex].bones;
                var boneCount = bones.Length;

                GUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUILayout.ObjectField("Manager", affectedBoneGroups[groupIndex].manager, typeof(SpringManager), true);
                GUILayout.Label(affectedBoneGroups[groupIndex].boneCountLabel);
                if (boneCount >= 2 && GUILayout.Button(SelectAllLabel))
                {
                    Selection.objects = bones.Select(bone => bone.gameObject).ToArray();
                }
                EditorGUILayout.Space();

                for (int boneIndex = 0; boneIndex < boneCount; boneIndex++)
                {
                    EditorGUILayout.ObjectField(bones[boneIndex], typeof(SpringBone), true);
                }
                EditorGUILayout.Space();
                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // private

        private SpringColliderBoneSelector() { }

        private SpringManagerBoneGroup[] affectedBoneGroups;
        private int totalBoneCount;
        private string springBoneTitle;

        private struct SpringManagerBoneGroup
        {
            public SpringManager manager;
            public SpringBone[] bones;
            public string boneCountLabel;
        }

        private static SpringManagerBoneGroup[] FindSpringBonesUsingCollider<T>
        (
            Object[] components,
            System.Func<SpringBone, IEnumerable<T>, bool> containmentCheck
        ) where T : class
        {
            var colliders = components
                .Select(component => component as T)
                .Where(component => component != null)
                .ToArray();

            var allSpringBones = GameObjectUtil.FindComponentsOfType<SpringBone>();
            var matchingSpringBones = allSpringBones
                .Where(springBone => containmentCheck(springBone, colliders))
                .Distinct()
                .ToArray();
            System.Array.Sort(matchingSpringBones, (a, b) => string.Compare(a.name, b.name));

            // Now arrange by manager
            var allManagers = GameObjectUtil.FindComponentsOfType<SpringManager>().ToArray();
            System.Array.Sort(allManagers, (a, b) => string.Compare(a.name, b.name));
            var boneGroups = new List<SpringManagerBoneGroup>();
            foreach (var manager in allManagers)
            {
                var bonesForManager = matchingSpringBones
                    .Where(bone => manager.springBones.Contains(bone))
                    .ToArray();
                if (bonesForManager.Any())
                {
                    boneGroups.Add(new SpringManagerBoneGroup
                    {
                        manager = manager,
                        bones = bonesForManager,
                        boneCountLabel = string.Format("Bones: {0}", bonesForManager.Length)
                    });
                }
            }

            // Now find the bones with no manager
            var bonesWithNoManager = matchingSpringBones
                .Where(bone => !allManagers.Any(manager => manager.springBones.Contains(bone)))
                .ToArray();
            if (bonesWithNoManager.Any())
            {
                boneGroups.Add(new SpringManagerBoneGroup
                {
                    manager = null,
                    bones = bonesWithNoManager,
                    boneCountLabel = string.Format("Bones: {0}", bonesWithNoManager.Length)
                });
            }

            return boneGroups.ToArray();
        }
    }
}