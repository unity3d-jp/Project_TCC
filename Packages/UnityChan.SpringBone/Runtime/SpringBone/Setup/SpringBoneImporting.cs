using UTJ.GameObjectExtensions;
using UTJ.StringQueueExtensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    public static partial class SpringBoneSerialization
    {
        public class ParsedSpringBoneSetup
        {
            public IEnumerable<DynamicsSetup.ParseMessage> Errors { get; set; }

            public static ParsedSpringBoneSetup ReadSpringBoneSetupFromText
            (
                GameObject springBoneRoot,
                GameObject colliderRoot,
                string recordText,
                IEnumerable<string> inputValidColliderNames
            )
            {
                List<TextRecordParsing.Record> rawSpringBoneRecords = null;
                List<TextRecordParsing.Record> rawPivotRecords = null;
                try
                {
                    var sourceRecords = TextRecordParsing.ParseRecordsFromText(recordText);
                    TextRecordParsing.Record versionRecord = null;
                    DynamicsSetup.GetVersionFromSetupRecords(sourceRecords, out versionRecord);
                    rawSpringBoneRecords = TextRecordParsing.GetSectionRecords(sourceRecords, "SpringBones");
                    if (rawSpringBoneRecords == null || rawSpringBoneRecords.Count == 0)
                    {
                        rawSpringBoneRecords = TextRecordParsing.GetSectionRecords(sourceRecords, null)
                            .Where(item => item != versionRecord)
                            .ToList();
                    }
                    rawPivotRecords = TextRecordParsing.GetSectionRecords(sourceRecords, "Pivots");
                }
                catch (System.Exception exception)
                {
                    Debug.LogError("SpringBoneSetup: 元のテキストデータを読み込めませんでした！\n\n" + exception.ToString());
                    return null;
                }

                var errors = new List<DynamicsSetup.ParseMessage>();
                var pivotRecords = SerializePivotRecords(rawPivotRecords, errors);
                var springBoneRecords = SerializeSpringBoneRecords(rawSpringBoneRecords, errors);

                var validObjectNames = springBoneRoot.GetComponentsInChildren<Transform>(true)
                    .Select(item => item.name).Distinct().ToList();
                var validPivotRecords = new List<PivotSerializer>();
                VerifyPivotRecords(pivotRecords, validObjectNames, validPivotRecords, errors);

                var validPivotNames = new List<string>(validObjectNames);
                validPivotNames.AddRange(validPivotRecords.Select(record => record.name));

                var validColliderNames = new List<string>();
                var colliderTypes = SpringColliderSetup.GetColliderTypes();
                validColliderNames.AddRange(colliderTypes
                    .SelectMany(type => colliderRoot.GetComponentsInChildren(type, true))
                    .Select(item => item.name));
                if (inputValidColliderNames != null) { validColliderNames.AddRange(inputValidColliderNames); }

                var validSpringBoneRecords = new List<SpringBoneSerializer>();
                bool hasMissingColliders;
                VerifySpringBoneRecords(
                    springBoneRecords,
                    validObjectNames,
                    validPivotNames,
                    validColliderNames,
                    validSpringBoneRecords,
                    out hasMissingColliders,
                    errors);

                if (hasMissingColliders)
                {
                    Debug.LogWarning("スプリングボーンセットアップ：一部のコライダーが見つかりません");
                }

                return new ParsedSpringBoneSetup
                {
                    pivotRecords = validPivotRecords,
                    springBoneRecords = validSpringBoneRecords,
                    Errors = errors
                };
            }

            public void BuildObjects(GameObject springBoneRoot, GameObject colliderRoot, IEnumerable<string> requiredBones)
            {
                var managerProperties = PersistentSpringManagerProperties.Create(
                    springBoneRoot.GetComponentInChildren<SpringManager>());
                SpringBoneSetup.DestroySpringManagersAndBones(springBoneRoot);

                if (requiredBones != null)
                {
                    FilterBoneRecordsByRequiredBonesAndCreateUnrecordedBones(springBoneRoot, requiredBones);
                }

                var initialTransforms = GameObjectUtil.BuildNameToComponentMap<Transform>(springBoneRoot, true);
                foreach (var record in pivotRecords)
                {
                    BuildPivotFromSerializer(initialTransforms, record);
                }

                var setupMaps = SpringBoneSetupMaps.Build(springBoneRoot, colliderRoot);
                foreach (var record in springBoneRecords)
                {
                    BuildSpringBoneFromSerializer(setupMaps, record);
                }

                var springManager = springBoneRoot.AddComponent<SpringManager>();
                if (managerProperties != null)
                {
                    managerProperties.ApplyTo(springManager);
                }
                SpringBoneSetup.FindAndAssignSpringBones(springManager);
            }

            // private

            private IEnumerable<PivotSerializer> pivotRecords;
            private IEnumerable<SpringBoneSerializer> springBoneRecords;

            private void FilterBoneRecordsByRequiredBonesAndCreateUnrecordedBones
            (
                GameObject springBoneRoot,
                IEnumerable<string> requiredBones
            )
            {
                var boneRecordsToUse = springBoneRecords
                    .Where(record => requiredBones.Contains(record.baseData.boneName));
                var recordedBoneNames = boneRecordsToUse.Select(record => record.baseData.boneName);

                var bonesWithoutRecords = requiredBones
                    .Except(recordedBoneNames)
                    .Select(boneName => GameObjectUtil.FindChildByName(springBoneRoot, boneName))
                    .Where(item => item != null)
                    .Select(item => item.gameObject);
                foreach (var bone in bonesWithoutRecords)
                {
                    var springBone = bone.AddComponent<SpringBone>();
                    SpringBoneSetup.CreateSpringPivotNode(springBone);
                }

                // Report the skipped bone records so the user knows
                var skippedBoneRecords = springBoneRecords.Except(boneRecordsToUse);
                foreach (var skippedRecord in skippedBoneRecords)
                {
                    Debug.LogWarning(skippedRecord.baseData.boneName
                        + "\nボーンリストにないので作成しません");
                }

                springBoneRecords = boneRecordsToUse;
            }
        }
 
        // private

        // todo: Add to CSV
        private class PersistentSpringManagerProperties
        {
            public static PersistentSpringManagerProperties Create(SpringManager sourceManager)
            {
                if (sourceManager == null) { return null; }

                var properties = new PersistentSpringManagerProperties
                {
                    automaticUpdates = sourceManager.automaticUpdates,
                    simulationFrameRate = sourceManager.simulationFrameRate,
                    dynamicRatio = sourceManager.dynamicRatio,
                    gravity = sourceManager.gravity,
                    collideWithGround = sourceManager.collideWithGround,
                    groundHeight = sourceManager.groundHeight,
                    bounce = sourceManager.bounce,
                    friction = sourceManager.friction
                };
                return properties;
            }

            public void ApplyTo(SpringManager targetManager)
            {
                targetManager.automaticUpdates = automaticUpdates;
                targetManager.simulationFrameRate = simulationFrameRate;
                targetManager.dynamicRatio = dynamicRatio;
                targetManager.gravity = gravity;
                targetManager.collideWithGround = collideWithGround;
                targetManager.groundHeight = groundHeight;
                targetManager.bounce = bounce;
                targetManager.friction = friction;
            }

            private bool automaticUpdates;
            private int simulationFrameRate;
            private float dynamicRatio;
            private Vector3 gravity;
            private bool collideWithGround;
            private float groundHeight;
            private float bounce;
            private float friction;
        }

        private class SpringBoneSetupMaps
        {
            public Dictionary<string, Transform> allChildren;
            public Dictionary<string, SpringSphereCollider> sphereColliders;
            public Dictionary<string, SpringCapsuleCollider> capsuleColliders;
            public Dictionary<string, SpringPanelCollider> panelColliders;

            public static SpringBoneSetupMaps Build(GameObject springBoneRoot, GameObject colliderRoot)
            {
                return new SpringBoneSetupMaps
                {
                    allChildren = GameObjectUtil.BuildNameToComponentMap<Transform>(springBoneRoot, true),
                    sphereColliders = GameObjectUtil.BuildNameToComponentMap<SpringSphereCollider>(colliderRoot, true),
                    capsuleColliders = GameObjectUtil.BuildNameToComponentMap<SpringCapsuleCollider>(colliderRoot, true),
                    panelColliders = GameObjectUtil.BuildNameToComponentMap<SpringPanelCollider>(colliderRoot, true),
                };
            }
        }

        // Serialized classes
#pragma warning disable 0649

        private class PivotSerializer
        {
            public string name;
            public string parentName;
            public Vector3 eulerAngles;
        }

        private class AngleLimitSerializer
        {
            public bool enabled;
            public float min;
            public float max;
        }

        private class LengthLimitSerializer
        {
            public string objectName;
            public float ratio;
        }

        private class SpringBoneBaseSerializer
        {
            public string boneName;
            public float radius;
            public float stiffness;
            public float drag;
            public Vector3 springForce;
            public float windInfluence;
            public string pivotName;
            public AngleLimitSerializer yAngleLimits;
            public AngleLimitSerializer zAngleLimits;
            public float angularStiffness;
            public LengthLimitSerializer[] lengthLimits;
        }

        private class SpringBoneSerializer
        {
            public SpringBoneBaseSerializer baseData;
            public string[] colliderNames;
        }

#pragma warning restore 0649

        // Object serialization

        private static IEnumerable<PivotSerializer> SerializePivotRecords
        (
            IEnumerable<TextRecordParsing.Record> sourceRecords,
            List<DynamicsSetup.ParseMessage> errorRecords
        )
        {
            var validRecords = new List<PivotSerializer>(sourceRecords.Count());
            foreach (var sourceRecord in sourceRecords)
            {
                DynamicsSetup.ParseMessage error = null;
                var newRecord = DynamicsSetup.SerializeObjectFromStrings<PivotSerializer>(sourceRecord.Items, null, ref error);
                if (newRecord != null)
                {
                    validRecords.Add(newRecord);
                }
                else
                {
                    errorRecords.Add(error);
                }
            }
            return validRecords;
        }

        private static IEnumerable<SpringBoneSerializer> SerializeSpringBoneRecords
        (
            IEnumerable<TextRecordParsing.Record> sourceRecords,
            List<DynamicsSetup.ParseMessage> errorRecords
        )
        {
            var validRecords = new List<SpringBoneSerializer>(sourceRecords.Count());
            foreach (var sourceRecord in sourceRecords)
            {
                var itemQueue = sourceRecord.ToQueue();
                SpringBoneBaseSerializer newBaseRecord = null;
                DynamicsSetup.ParseMessage error = null;
                try
                {
                    newBaseRecord = itemQueue.DequeueObject<SpringBoneBaseSerializer>();
                }
                catch (System.Exception exception)
                {
                    error = new DynamicsSetup.ParseMessage("Error building SpringBoneBaseSerializer", sourceRecord.Items, exception.ToString());
                }

                if (newBaseRecord != null)
                {
                    // The rest of the queue should be collider names
                    var colliderNames = new List<string>(itemQueue).Where(item => item.Length > 0);
                    var newRecord = new SpringBoneSerializer
                    {
                        baseData = newBaseRecord,
                        colliderNames = colliderNames.ToArray()
                    };
                    validRecords.Add(newRecord);
                }
                else
                {
                    errorRecords.Add(error);
                }
            }
            return validRecords;
        }

        // Verification

        private static bool VerifyPivotRecords
        (
            IEnumerable<PivotSerializer> sourceRecords,
            IEnumerable<string> validParentNames,
            List<PivotSerializer> validRecords,
            List<DynamicsSetup.ParseMessage> errors
        )
        {
            var newValidRecords = new List<PivotSerializer>(sourceRecords.Count());
            foreach (var sourceRecord in sourceRecords)
            {
                DynamicsSetup.ParseMessage error = null;
                if (sourceRecord.name.Length == 0)
                {
                    // Todo: Need more details...
                    error = new DynamicsSetup.ParseMessage("名前が指定されていない基点オブジェクトがあります");
                }

                var parentName = sourceRecord.parentName;
                if (parentName.Length == 0)
                {
                    error = new DynamicsSetup.ParseMessage(sourceRecord.name + " : 親名が指定されていません");
                }
                else if (!validParentNames.Contains(parentName))
                {
                    error = new DynamicsSetup.ParseMessage(sourceRecord.name + " : 親が見つかりません: " + parentName);
                }

                if (error == null)
                {
                    newValidRecords.Add(sourceRecord);
                }
                else
                {
                    errors.Add(error);
                }
            }
            validRecords.AddRange(newValidRecords);
            return sourceRecords.Count() == newValidRecords.Count();
        }

        private static bool VerifySpringBoneRecords
        (
            IEnumerable<SpringBoneSerializer> sourceRecords,
            IEnumerable<string> validBoneNames,
            IEnumerable<string> validPivotNames,
            IEnumerable<string> validColliderNames,
            List<SpringBoneSerializer> validRecords,
            out bool hasMissingColliders,
            List<DynamicsSetup.ParseMessage> errors
        )
        {
            hasMissingColliders = false;
            var newValidRecords = new List<SpringBoneSerializer>(sourceRecords.Count());
            foreach (var sourceRecord in sourceRecords)
            {
                DynamicsSetup.ParseMessage error = null;
                var baseData = sourceRecord.baseData;
                if (baseData.boneName.Length == 0)
                {
                    // Todo: Need more details...
                    error = new DynamicsSetup.ParseMessage("名前が指定されていない基点オブジェクトがあります");
                }
                else if (!validBoneNames.Contains(baseData.boneName))
                {
                    error = new DynamicsSetup.ParseMessage(baseData.boneName + " : オブジェくトが見つかりません");
                }

                var pivotName = baseData.pivotName;
                if (pivotName.Length == 0)
                {
                    error = new DynamicsSetup.ParseMessage(baseData.boneName + " : 基点名が指定されていません");
                }
                else if (!validPivotNames.Contains(pivotName))
                {
                    error = new DynamicsSetup.ParseMessage(baseData.boneName + " : 基点オブジェクトが見つかりません: " + pivotName);
                }

                var missingColliders = sourceRecord.colliderNames
                    .Where(name => !validColliderNames.Contains(name));
                if (missingColliders.Any())
                {
                    // Missing colliders are just a warning
                    hasMissingColliders = true;
                    Debug.LogWarning(
                        baseData.boneName + " : コライダーが見つかりません:\n"
                        + string.Join(" ", missingColliders.ToArray()));
                }

                if (error == null)
                {
                    newValidRecords.Add(sourceRecord);
                }
                else
                {
                    errors.Add(error);
                }
            }
            validRecords.AddRange(newValidRecords);
            return sourceRecords.Count() == newValidRecords.Count();
        }

        // Object construction

        private static AngleLimits BuildAngleLimitsFromSerializer(AngleLimitSerializer serializer)
        {
            return new AngleLimits
            {
                active = serializer.enabled,
                min = serializer.min,
                max = serializer.max
            };
        }

        private static Transform FindChildByName(Transform parent, string name)
        {
            for (var childIndex = 0; childIndex < parent.childCount; ++childIndex)
            {
                var child = parent.GetChild(childIndex);
                if (child.name.ToLowerInvariant() == name.ToLowerInvariant())
                {
                    return child;
                }
            }
            return null;
        }

        private static bool BuildPivotFromSerializer
        (
            Dictionary<string, Transform> transforms,
            PivotSerializer serializer
        )
        {
            Transform parent;
            var parentExists = transforms.TryGetValue(serializer.parentName, out parent);
            if (parentExists)
            {
                var pivot = FindChildByName(parent, serializer.name);
                if (pivot == null)
                {
                    var pivotGameObject = new GameObject(serializer.name, typeof(SpringBonePivot));
                    pivot = pivotGameObject.transform;
                    pivot.parent = parent;
                }
                pivot.localScale = Vector3.one;
                pivot.localEulerAngles = serializer.eulerAngles;
                pivot.localPosition = Vector3.zero;
            }
            return parentExists;
        }

        private static bool BuildSpringBoneFromSerializer
        (
            SpringBoneSetupMaps setupMaps,
            SpringBoneSerializer serializer
        )
        {
            var baseData = serializer.baseData;
            Transform childBone = null;
            if (!setupMaps.allChildren.TryGetValue(baseData.boneName, out childBone))
            {
                Debug.LogError("ボーンが見つかりません: " + baseData.boneName);
                return false;
            }

            var springBone = childBone.gameObject.AddComponent<SpringBone>();
            springBone.stiffnessForce = baseData.stiffness;
            springBone.dragForce = baseData.drag;
            springBone.springForce = baseData.springForce;
            springBone.windInfluence = baseData.windInfluence;
            springBone.angularStiffness = baseData.angularStiffness;
            springBone.yAngleLimits = BuildAngleLimitsFromSerializer(baseData.yAngleLimits);
            springBone.zAngleLimits = BuildAngleLimitsFromSerializer(baseData.zAngleLimits);
            springBone.radius = baseData.radius;

            // Pivot node
            var pivotNodeName = baseData.pivotName;
            Transform pivotNode = null;
            if (pivotNodeName.Length > 0)
            {
                if (!setupMaps.allChildren.TryGetValue(pivotNodeName, out pivotNode))
                {
                    Debug.LogError("Pivotオブジェクトが見つかりません: " + pivotNodeName);
                    pivotNode = null;
                }
            }
            if (pivotNode == null)
            {
                pivotNode = springBone.transform.parent ?? springBone.transform;
            }
            else
            {
                var skinBones = GameObjectUtil.GetAllBones(springBone.transform.root.gameObject);
                if (pivotNode.GetComponent<SpringBonePivot>()
                    && SpringBoneSetup.IsPivotProbablySafeToDestroy(pivotNode, skinBones))
                {
                    pivotNode.position = springBone.transform.position;
                }
            }
            springBone.pivotNode = pivotNode;

            springBone.lengthLimitTargets = baseData.lengthLimits
                .Where(lengthLimit => setupMaps.allChildren.ContainsKey(lengthLimit.objectName))
                .Select(lengthLimit => setupMaps.allChildren[lengthLimit.objectName])
                .ToArray();

            springBone.sphereColliders = serializer.colliderNames
                .Where(name => setupMaps.sphereColliders.ContainsKey(name))
                .Select(name => setupMaps.sphereColliders[name])
                .ToArray();

            springBone.capsuleColliders = serializer.colliderNames
                .Where(name => setupMaps.capsuleColliders.ContainsKey(name))
                .Select(name => setupMaps.capsuleColliders[name])
                .ToArray();

            springBone.panelColliders = serializer.colliderNames
                .Where(name => setupMaps.panelColliders.ContainsKey(name))
                .Select(name => setupMaps.panelColliders[name])
                .ToArray();

            return true;
        }
   }
}