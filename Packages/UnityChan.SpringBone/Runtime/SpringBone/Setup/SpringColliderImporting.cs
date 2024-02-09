using UTJ.GameObjectExtensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    public static partial class SpringColliderSerialization
    {
        public class ParsedColliderSetup
        {
            public IEnumerable<DynamicsSetup.ParseMessage> Errors { get; set; }

            public static ParsedColliderSetup ReadColliderSetupFromText(GameObject colliderRoot, string recordText)
            {
                List<TextRecordParsing.Record> rawColliderRecords = null;
                List<TextRecordParsing.Record> rawDynamicsNullRecords = null;
                List<TextRecordParsing.Record> rawComponentRecords = null;
                try
                {
                    var sourceRecords = TextRecordParsing.ParseRecordsFromText(recordText);
                    TextRecordParsing.Record versionRecord = null;
                    DynamicsSetup.GetVersionFromSetupRecords(sourceRecords, out versionRecord);
                    rawColliderRecords = TextRecordParsing.GetSectionRecords(sourceRecords, "Colliders");
                    // Only try to read collider records from the root section if there is no version record (old collider-only CSV)
                    if (versionRecord == null)
                    {
                        if (rawColliderRecords == null || rawColliderRecords.Count == 0)
                        {
                            rawColliderRecords = TextRecordParsing.GetSectionRecords(sourceRecords, null);
                        }
                    }
                    rawDynamicsNullRecords = TextRecordParsing.GetSectionRecords(sourceRecords, "DynamicsNulls");
                    rawComponentRecords = TextRecordParsing.GetSectionRecords(sourceRecords, "Components");
                }
                catch (System.Exception exception)
                {
                    Debug.LogError("SpringColliderSetup: 元のテキストデータを読み込めませんでした！\n\n" + exception.ToString());
                    return null;
                }

                var errors = new List<DynamicsSetup.ParseMessage>();
                var colliderRecords = SerializeColliderRecords(rawColliderRecords, errors);
                var dynamicsNullRecords = SerializeTransformRecords(rawDynamicsNullRecords, errors);

                var validParentNames = colliderRoot.GetComponentsInChildren<Transform>(true).Select(item => item.name).ToList();
                var validDynamicsNullRecords = new List<TransformSerializer>();
                VerifyTransformRecords(dynamicsNullRecords, validParentNames, validDynamicsNullRecords, errors);

                // Colliders get added after DynamicsNulls, so they can be added as children to them
                validParentNames.AddRange(validDynamicsNullRecords.Select(item => item.name));
                var validColliderRecords = new List<IColliderSerializer>();
                VerifyColliderRecords(colliderRecords, colliderRoot, validParentNames, validColliderRecords, errors);

                // Todo: Verify Component records

                return new ParsedColliderSetup
                {
                    colliderRecords = validColliderRecords,
                    dynamicsNullRecords = validDynamicsNullRecords,
                    componentRecords = rawComponentRecords,
                    Errors = errors
                };
            }

            public void BuildObjects(GameObject colliderRoot)
            {
                SpringColliderSetup.DestroySpringColliders(colliderRoot);
                var allChildren = colliderRoot.BuildNameToComponentMap<Transform>(true);
                BuildDynamicsNulls(allChildren, dynamicsNullRecords);

                // New objects may have been created by SetupDynamicNulls, so retrieve all the children again
                allChildren = colliderRoot.BuildNameToComponentMap<Transform>(true);
                foreach (var record in colliderRecords)
                {
                    BuildColliderFromRecord(allChildren, record);
                }

                allChildren = colliderRoot.BuildNameToComponentMap<Transform>(true);
                BuildComponents(allChildren, componentRecords);
            }

            public IEnumerable<string> GetColliderNames()
            {
                return colliderRecords.Select(item => item.GetBaseInfo().transform.name);
            }

            // private

            private IEnumerable<IColliderSerializer> colliderRecords;
            private IEnumerable<TransformSerializer> dynamicsNullRecords;
            private List<TextRecordParsing.Record> componentRecords;
        }

        // private

        private const string SphereColliderToken = "sp";
        private const string CapsuleColliderToken = "cp";
        private const string PanelColliderToken = "pa";

        private static Transform CreateNewGameObject(Transform parent, string name)
        {
            var newObject = new GameObject(name);
            if (newObject == null)
            {
                Debug.LogError("新しいオブジェクトを作成できませんでした: " + name);
                return null;
            }
            newObject.transform.parent = parent;
            return newObject.transform;
        }

        private static Transform GetChildByName(Transform parent, string name)
        {
            return Enumerable.Range(0, parent.childCount)
                .Select(index => parent.GetChild(index))
                .Where(child => child.name == name)
                .FirstOrDefault();
        }

        private static T TryToFindComponent<T>(GameObject gameObject, string name) where T : Component
        {
            T component = default(T);
            if (name.Length > 0)
            {
                component = GameObjectUtil.FindChildComponentByName<T>(
                    gameObject.transform.root.gameObject, name);
            }
            return component;
        }

        // Serialized classes
#pragma warning disable 0649

        private class TransformSerializer
        {
            public string name;
            public string parentName;
            public Vector3 position;
            public Vector3 eulerAngles;
            public Vector3 scale;
        }

        private class ColliderSerializerBaseInfo
        {
            public TransformSerializer transform;
            public string colliderType;
        }

        private interface IColliderSerializer
        {
            ColliderSerializerBaseInfo GetBaseInfo();
            Component BuildColliderComponent(GameObject gameObject);
            string GetLinkedRendererName();
        }

        private class SphereColliderSerializer : IColliderSerializer
        {
            public ColliderSerializerBaseInfo baseInfo;
            public float radius;
            public string linkedRenderer;

            public ColliderSerializerBaseInfo GetBaseInfo() { return baseInfo; }

            public Component BuildColliderComponent(GameObject gameObject)
            {
                var collider = gameObject.AddComponent<SpringSphereCollider>();
                collider.radius = radius;
                if (!string.IsNullOrEmpty(linkedRenderer))
                {
                    collider.linkedRenderer = TryToFindComponent<Renderer>(gameObject, linkedRenderer);
                }
                return collider;
            }

            public string GetLinkedRendererName()
            {
                return linkedRenderer;
            }
        }

        private class CapsuleColliderSerializer : IColliderSerializer
        {
            public ColliderSerializerBaseInfo baseInfo;
            public float radius;
            public float height;
            public string linkedRenderer;

            public ColliderSerializerBaseInfo GetBaseInfo() { return baseInfo; }

            public Component BuildColliderComponent(GameObject gameObject)
            {
                var collider = gameObject.AddComponent<SpringCapsuleCollider>();
                collider.radius = radius;
                collider.height = height;
                if (!string.IsNullOrEmpty(linkedRenderer))
                {
                    collider.linkedRenderer = TryToFindComponent<Renderer>(gameObject, linkedRenderer);
                }
                return collider;
            }

            public string GetLinkedRendererName()
            {
                return linkedRenderer;
            }
        }

        private class PanelColliderSerializer : IColliderSerializer
        {
            public ColliderSerializerBaseInfo baseInfo;
            public float width;
            public float height;
            public string linkedRenderer;

            public ColliderSerializerBaseInfo GetBaseInfo() { return baseInfo; }

            public Component BuildColliderComponent(GameObject gameObject)
            {
                var collider = gameObject.AddComponent<SpringPanelCollider>();
                collider.width = width;
                collider.height = height;
                if (!string.IsNullOrEmpty(linkedRenderer))
                {
                    collider.linkedRenderer = TryToFindComponent<Renderer>(gameObject, linkedRenderer);
                }
                return collider;
            }

            public string GetLinkedRendererName()
            {
                return linkedRenderer;
            }
        }

#pragma warning restore 0649

        // Parsing and verification

        private static IEnumerable<IColliderSerializer> SerializeColliderRecords
        (
            IEnumerable<TextRecordParsing.Record> sourceRecords,
            List<DynamicsSetup.ParseMessage> errorRecords
        )
        {
            var serializerClasses = new Dictionary<string, System.Type>
            {
                { SphereColliderToken, typeof(SphereColliderSerializer) },
                { CapsuleColliderToken, typeof(CapsuleColliderSerializer) },
                { PanelColliderToken, typeof(PanelColliderSerializer) },
            };

            var colliderSerializers = new List<IColliderSerializer>(sourceRecords.Count());
            foreach (var sourceRecord in sourceRecords)
            {
                IColliderSerializer newColliderInfo = null;
                DynamicsSetup.ParseMessage error = null;
                var baseInfo = DynamicsSetup.SerializeObjectFromStrings<ColliderSerializerBaseInfo>(sourceRecord.Items, null, ref error);
                if (baseInfo != null)
                {
                    System.Type serializerType;
                    if (serializerClasses.TryGetValue(baseInfo.colliderType, out serializerType))
                    {
                        newColliderInfo = DynamicsSetup.SerializeObjectFromStrings(
                            serializerType, sourceRecord.Items, "linkedRenderer", ref error)
                            as IColliderSerializer;
                    }
                    else
                    {
                        error = new DynamicsSetup.ParseMessage("Invalid collider type: " + baseInfo.colliderType, sourceRecord.Items);
                    }
                }

                if (newColliderInfo != null)
                {
                    colliderSerializers.Add(newColliderInfo);
                }
                else
                {
                    errorRecords.Add(error);
                }
            }
            return colliderSerializers;
        }

        private static IEnumerable<TransformSerializer> SerializeTransformRecords
        (
            IEnumerable<TextRecordParsing.Record> sourceRecords,
            List<DynamicsSetup.ParseMessage> errorRecords
        )
        {
            var transformRecords = new List<TransformSerializer>(sourceRecords.Count());
            foreach (var sourceRecord in sourceRecords)
            {
                DynamicsSetup.ParseMessage error = null;
                var transformRecord = DynamicsSetup.SerializeObjectFromStrings<TransformSerializer>(sourceRecord.Items, null, ref error);
                if (transformRecord != null)
                {
                    transformRecords.Add(transformRecord);
                }
                else
                {
                    errorRecords.Add(error);
                }
            }
            return transformRecords;
        }

        private static bool VerifyTransformRecord
        (
            TransformSerializer transformSerializer,
            IEnumerable<string> validParentNames,
            out DynamicsSetup.ParseMessage error
        )
        {
            error = null;
            var objectName = transformSerializer.name;
            if (objectName.Length == 0)
            {
                // Todo: Need more details...
                error = new DynamicsSetup.ParseMessage("コライダー名が指定されていないものがあります");
            }

            var parentName = transformSerializer.parentName;
            if (parentName.Length == 0)
            {
                error = new DynamicsSetup.ParseMessage(objectName + " : 親名が指定されていません");
            }
            else if (!validParentNames.Contains(parentName))
            {
                error = new DynamicsSetup.ParseMessage(objectName + " : 親が見つかりません: " + parentName);
            }

            return error == null;
        }

        private static bool VerifyTransformRecords
        (
            IEnumerable<TransformSerializer> sourceRecords,
            IEnumerable<string> validParentNames,
            List<TransformSerializer> validRecords,
            List<DynamicsSetup.ParseMessage> errors
        )
        {
            var newValidRecords = new List<TransformSerializer>(sourceRecords.Count());
            foreach (var sourceRecord in sourceRecords)
            {
                DynamicsSetup.ParseMessage error = null;
                var isValidRecord = VerifyTransformRecord(sourceRecord, validParentNames, out error);
                if (isValidRecord)
                {
                    if (newValidRecords.Any(item => item.name == sourceRecord.name))
                    {
                        error = new DynamicsSetup.ParseMessage(sourceRecord.name + " : 名前が重複します");
                        isValidRecord = false;
                    }
                }

                if (isValidRecord)
                {
                    newValidRecords.Add(sourceRecord);
                }
                else
                {
                    errors.Add(error);
                }
            }
            validRecords.AddRange(newValidRecords);
            return sourceRecords.Count() == newValidRecords.Count;
        }

        private static bool VerifyColliderRecords
        (
            IEnumerable<IColliderSerializer> colliderRecords,
            GameObject rootObject,
            IEnumerable<string> validParentNames,
            List<IColliderSerializer> validRecords,
            List<DynamicsSetup.ParseMessage> errors
        )
        {
            var newValidRecords = new List<IColliderSerializer>(colliderRecords.Count());
            foreach (var sourceRecord in colliderRecords)
            {
                DynamicsSetup.ParseMessage error = null;
                var objectName = sourceRecord.GetBaseInfo().transform.name;
                var isValidRecord = true;
                if (!VerifyTransformRecord(sourceRecord.GetBaseInfo().transform, validParentNames, out error))
                {
                    isValidRecord = false;
                }

                var linkedRendererName = sourceRecord.GetLinkedRendererName();
                if (!string.IsNullOrEmpty(linkedRendererName)
                    && rootObject.FindChildComponentByName<Renderer>(linkedRendererName) == null)
                {
                    error = new DynamicsSetup.ParseMessage(objectName + " : linkedRendererが見つかりません: " + linkedRendererName);
                    isValidRecord = false;
                }

                if (newValidRecords.Any(item => item.GetBaseInfo().transform.name == objectName))
                {
                    error = new DynamicsSetup.ParseMessage(objectName + " : 名前が重複します");
                    isValidRecord = false;
                }

                if (isValidRecord)
                {
                    newValidRecords.Add(sourceRecord);
                }
                else
                {
                    errors.Add(error);
                }
            }
            validRecords.AddRange(newValidRecords);
            return colliderRecords.Count() == newValidRecords.Count;
        }

        // Building

        private static GameObject BuildTransformFromRecord
        (
            Dictionary<string, Transform> objectMap,
            TransformSerializer serializer
        )
        {
            Transform parent = null;
            if (!objectMap.TryGetValue(serializer.parentName, out parent))
            {
                Debug.LogError("親が見つかりませんでした: " + serializer.parentName);
                return null;
            }

            var objectTransform = GetChildByName(parent, serializer.name);
            if (objectTransform == null)
            {
                objectTransform = CreateNewGameObject(parent, serializer.name);
                if (objectTransform == null)
                {
                    return null;
                }
            }

            // Don't change the transform if it belongs to the base skeleton
            //var skeletonDefinition = SkeletonDefinition.LoadDefaultSkeleton();
            //if (!skeletonDefinition.AllBones.Contains(serializer.name))
            //{
            objectTransform.localScale = serializer.scale;
            objectTransform.localEulerAngles = serializer.eulerAngles;
            objectTransform.localPosition = serializer.position;
            //}
            return objectTransform.gameObject;
        }

        private static bool BuildColliderFromRecord
        (
            Dictionary<string, Transform> objectMap,
            IColliderSerializer colliderSerializer
        )
        {
            var gameObject = BuildTransformFromRecord(objectMap, colliderSerializer.GetBaseInfo().transform);
            var succeeded = gameObject != null;
            if (succeeded)
            {
                colliderSerializer.BuildColliderComponent(gameObject);
            }
            return succeeded;
        }

        // DynamicsNulls

        private static void BuildDynamicsNulls
        (
            Dictionary<string, Transform> objectMap,
            IEnumerable<TransformSerializer> records
        )
        {
            // Remove excess DynamicsNulls
            foreach (var transform in objectMap.Values)
            {
                var dynamicsNulls = transform.gameObject.GetComponents<DynamicsNull>();
                for (int index = 1; index < dynamicsNulls.Length; ++index)
                {
                    Object.DestroyImmediate(dynamicsNulls[index]);
                }
            }

            foreach (var entry in records)
            {
                var newObject = BuildTransformFromRecord(objectMap, entry);
                if (newObject != null && newObject.GetComponent<DynamicsNull>() == null)
                {
                    newObject.AddComponent<DynamicsNull>();
                }
            }
        }

        private static void BuildComponents
        (
            Dictionary<string, Transform> objectMap,
            IEnumerable<TextRecordParsing.Record> records
        )
        {
            var componentDefiners = SpringSetupComponentDefiners.GetComponentDefiners();
            foreach (var entry in records)
            {
                Transform transform = null;
                if (objectMap.TryGetValue(entry.GetString(0), out transform))
                {
                    var gameObject = transform.gameObject;
                    var currentQueue = new Queue<string>(entry.Items.Skip(1));
                    foreach (var definer in componentDefiners)
                    {
                        definer.BuildFromDefinition(gameObject, currentQueue);
                    }
                }
            }
        }
    }
}