using UTJ.GameObjectExtensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class AutoSpringBoneSetup
    {
        public class AutoSetupParameters
        {
            public bool createPivots;

            public AutoSetupParameters()
            {
                createPivots = true;
            }
        }

        public static string FindCharacterBoneListPath(GameObject rootObject)
        {
            const string BoneListFilePattern = "*_BoneList.txt";

            var modelDirectory = GetModelDirectoryFromGameObject(rootObject);
            if (modelDirectory.Length > 0)
            {
                var filePaths = DirectoryUtil.GetFiles(modelDirectory, BoneListFilePattern);
                if (filePaths.Length > 0)
                {
                    return filePaths[0];
                }
            }
            return "";
        }

        public static IEnumerable<GameObject> GetBonesDesignatedForDynamics(GameObject rootObject)
        {
            // First try to find bone list from character's model data folder
            var boneListPath = FindCharacterBoneListPath(rootObject);
            if (boneListPath.Length > 0)
            {
                var boneListText = FileUtil.ReadAllText(boneListPath);
                var boneNames = GetDynamicsBoneNamesFromBoneList(boneListText).ToArray();
                var dynamicsBones = new List<GameObject>(boneNames.Count());
                var allNodes = rootObject.GetComponentsInChildren<Transform>(true);
                foreach (var boneName in boneNames)
                {
                    var transform = allNodes.FirstOrDefault(item => item.name == boneName);
                    if (transform != null)
                    {
                        dynamicsBones.Add(transform.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("ダイナミクス骨が見つかりません: " + boneName);
                    }
                }
                return dynamicsBones;
            }
            return new List<GameObject>(0);
        }

        public static IEnumerable<string> GetDynamicsBoneNamesFromBoneList(string boneListText)
        {
            const int DynamicsBoneColumn = 3;
            char[] EntrySeparators = { ',' };

            var sourceLines = boneListText.Split("\r\n".ToCharArray());
            var boneNames = new List<string>(sourceLines.Length);
            foreach (var sourceLine in sourceLines)
            {
                var line = sourceLine.Trim();
                if (line.Length > 0 && !line.StartsWith("//"))
                {
                    var entries = line.Split(EntrySeparators, System.StringSplitOptions.None);
                    if (entries.Length > DynamicsBoneColumn)
                    {
                        var boneName = entries[DynamicsBoneColumn].Trim();
                        if (boneName.Length > 0)
                        {
                            boneNames.Add(boneName);
                        }
                    }
                }
            }
            return boneNames;
        }

        public static SpringManager PerformSetup
        (
            GameObject rootObject, 
            IEnumerable<GameObject> newSpringBones,
            AutoSetupParameters parameters
        )
        {
            SpringBoneSetup.DestroySpringManagersAndBones(rootObject);
            SpringColliderSetup.DestroySpringColliders(rootObject);

            var springBones = new List<SpringBone>();
            foreach (var boneParent in newSpringBones.Where(boneParent => boneParent.transform.childCount > 0))
            {
                springBones.Add(boneParent.AddComponent<SpringBone>());
            }

            var manager = rootObject.AddComponent<SpringManager>();
            SpringBoneSetup.FindAndAssignSpringBones(manager, true);

            if (parameters.createPivots)
            {
                foreach (var springBone in springBones)
                {
                    SpringBoneSetup.CreateSpringPivotNode(springBone);
                }
            }

            return manager;
        }

        public static void UpdateSpringManagerFromBoneList(SpringManager springManager)
        {
            var rootObject = springManager.gameObject;
            var designatedSpringBones = GetBonesDesignatedForDynamics(rootObject);
            var currentSpringBones = rootObject.GetComponentsInChildren<SpringBone>(true);

            var skinBones = GameObjectUtil.GetAllBones(rootObject);
            var springBonesToDestroy = currentSpringBones
                .Where(bone => !designatedSpringBones.Contains(bone.gameObject));
            foreach (var boneToDestroy in springBonesToDestroy)
            {
                var pivot = boneToDestroy.pivotNode;
                if (pivot != null
                    && SpringBoneSetup.IsPivotProbablySafeToDestroy(pivot, skinBones))
                {
                    SpringBoneSetup.DestroyUnityObject(pivot.gameObject);
                }
                SpringBoneSetup.DestroyUnityObject(boneToDestroy);
            }

            var objectsToAddBonesTo = designatedSpringBones
                .Where(bone => bone.GetComponent<SpringBone>() == null);
            foreach (var newBoneOwner in objectsToAddBonesTo)
            {
                var newSpringBone = newBoneOwner.AddComponent<SpringBone>();
                SpringBoneSetup.CreateSpringPivotNode(newSpringBone);
            }
            SpringBoneSetup.FindAndAssignSpringBones(springManager, true);
        }

        // private
        
        private static bool IsProbablyFacialObject(Transform transform)
        {
            string[] boneNameTokensToFind = { "head", "neck" };
            var transformToCheck = transform;
            while (transformToCheck != null)
            {
                var lowerName = transformToCheck.name.ToLowerInvariant();
                if (boneNameTokensToFind.Any(item => lowerName.Contains(item)))
                {
                    return true;
                }
                transformToCheck = transformToCheck.parent;
            }
            return false;
        }

        private static string GetModelDirectoryFromGameObject(GameObject rootObject)
        {
            // Find a mesh?
            var meshPaths = rootObject.GetComponentsInChildren<SkinnedMeshRenderer>(true)
                .Where(renderer => renderer.sharedMesh != null
                    && !IsProbablyFacialObject(renderer.transform))
                .Select(renderer => AssetDatabase.GetAssetPath(renderer.sharedMesh))
                .Where(path => path.Length > 0);
            if (meshPaths.Any())
            {
                var modelDirectory = System.IO.Path.GetDirectoryName(meshPaths.First());
                return PathUtil.AssetPathToSystemPath(modelDirectory);
            }
            
            // Find a texture and go up from there
            var renderers = rootObject.GetComponentsInChildren<Renderer>(true)
                .Where(renderer => !IsProbablyFacialObject(renderer.transform));
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                var texturePaths = materials
                    .Where(material => material.mainTexture != null)
                    .Select(material => AssetDatabase.GetAssetPath(material.mainTexture));
                if (texturePaths.Any())
                {
                    var textureDirectory = System.IO.Path.GetDirectoryName(texturePaths.First());
                    var modelDirectory = System.IO.Path.GetDirectoryName(textureDirectory);
                    return PathUtil.AssetPathToSystemPath(modelDirectory);
                }
            }

            return "";
        }
    }
}