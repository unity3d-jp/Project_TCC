using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    public static partial class SpringBoneSerialization
    {
        public class ExportSettings
        {
            public ExportSettings()
            {
                ExportSpringBones = true;
                ExportCollision = true;
            }

            public bool ExportSpringBones { get; set; }
            public bool ExportCollision { get; set; }
        }

        public static string BuildDynamicsSetupString(GameObject rootObject, ExportSettings exportSettings = null)
        {
            if (exportSettings == null)
            {
                exportSettings = new ExportSettings();
            }

            const int CurrentVersion = 4;

            var builder = new System.Text.StringBuilder();
            builder.Append("version,");
            builder.Append(CurrentVersion);
            builder.AppendLine();

            if (exportSettings.ExportSpringBones)
            {
                builder.Append(BuildSpringBoneSetupString(rootObject));
            }

            if (exportSettings.ExportCollision)
            {
                builder.Append(SpringColliderSerialization.BuildCollisionSetupString(rootObject));
            }

            return builder.ToString();
        }

        // private

        private static AngleLimitSerializer AngleLimitsToSerializer(AngleLimits sourceLimits)
        {
            return new AngleLimitSerializer
            {
                enabled = sourceLimits.active,
                min = sourceLimits.min,
                max = sourceLimits.max
            };
        }

        private static SpringBoneBaseSerializer SpringBoneToBaseSerializer(SpringBone sourceBone)
        {
            return new SpringBoneBaseSerializer
            {
                boneName = sourceBone.name,
                radius = sourceBone.radius,
                stiffness = sourceBone.stiffnessForce,
                drag = sourceBone.dragForce,
                springForce = sourceBone.springForce,
                windInfluence = sourceBone.windInfluence,
                pivotName = (sourceBone.pivotNode != null) ? sourceBone.pivotNode.name : "",
                yAngleLimits = AngleLimitsToSerializer(sourceBone.yAngleLimits),
                zAngleLimits = AngleLimitsToSerializer(sourceBone.zAngleLimits),
                angularStiffness = sourceBone.angularStiffness,
                lengthLimits = sourceBone.lengthLimitTargets
                    .Where(item => item != null)
                    .Select(item => new LengthLimitSerializer
                    {
                        objectName = item.name,
                        ratio = 1.01f
                    })
                    .ToArray()
            };
        }

        private static PivotSerializer PivotToSerializer(Transform sourcePivot)
        {
            return new PivotSerializer
            {
                name = sourcePivot.name,
                parentName = (sourcePivot.parent != null) ? sourcePivot.parent.name : "",
                eulerAngles = sourcePivot.localEulerAngles
            };
        }

        private static void AppendSpringBones(CSVBuilder builder, IEnumerable<SpringBone> springBones)
        {
            if (!springBones.Any()) { return; }

            string[] springBoneHeaderRow = {
                "// bone",
                "radius",
                "stiffnessForce",
                "dragForce",
                "springForce x",
                "springForce y",
                "springForce z",
                "wind influence",
                "pivot node",
                "use y angle limit",
                "y angle min",
                "y angle max",
                "use z angle limit",
                "z angle min",
                "z angle max",
                "angle stiffness",
                "length limit count",
                "length limit target",
                "length limit ratio x N",
                "collider x N"
            };

            builder.AppendLine();
            builder.AppendLine("[SpringBones]");
            builder.AppendLine(springBoneHeaderRow);
            foreach (var bone in springBones)
            {
                var boneSerializer = SpringBoneToBaseSerializer(bone);
                builder.Append(boneSerializer);

                var colliderNames = new List<string>();
                colliderNames.AddRange(bone.sphereColliders.Where(item => item != null).Select(item => item.name));
                colliderNames.AddRange(bone.capsuleColliders.Where(item => item != null).Select(item => item.name));
                colliderNames.AddRange(bone.panelColliders.Where(item => item != null).Select(item => item.name));
                builder.AppendLine(colliderNames.Distinct());
            }
        }

        private static void AppendPivots(CSVBuilder builder, IEnumerable<SpringBone> springBones)
        {
            var pivotSerializers = springBones
                .Where(bone => bone.pivotNode != null
                    && bone.pivotNode != bone.transform.parent
                    && bone.pivotNode.parent != null)
                .Select(bone => bone.pivotNode)
                .Distinct()
                .Select(pivot => PivotToSerializer(pivot));

            if (!pivotSerializers.Any()) { return; }

            string[] pivotHeader = { "// PivotName", "ParentName", "local rotation x", "y", "z" };

            builder.AppendLine();
            builder.AppendLine("[Pivots]");
            builder.AppendLine(pivotHeader);
            foreach (var pivot in pivotSerializers)
            {
                builder.Append(pivot);
                builder.AppendLine();
            }
        }

        private static string BuildSpringBoneSetupString(GameObject rootObject)
        {
            var builder = new CSVBuilder();
            var springBones = rootObject.GetComponentsInChildren<SpringBone>(true);
            AppendSpringBones(builder, springBones);
            AppendPivots(builder, springBones);
            return builder.ToString();
        }
    }
}