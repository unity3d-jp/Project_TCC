using System;
using UnityEditor;
using UnityEngine.AddressableAssets;

namespace  Unity.SceneManagement
{
    
    [Serializable]
    public class AssetReferenceScene : AssetReferenceT<SceneReference>
    {
        /// <summary>
        /// Constructs a new reference to a GameObject.
        /// </summary>
        /// <param name="guid">The object guid.</param>
        public AssetReferenceScene(string guid) : base(guid)
        {
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            return typeof(SceneAsset).IsAssignableFrom(type);
#else
            return false;
#endif
        }

#if UNITY_EDITOR
        public new SceneAsset editorAsset
        {
            get
            {
                if (CachedAsset != null || string.IsNullOrEmpty(AssetGUID))
                    return CachedAsset as SceneAsset;

                var assetPath = AssetDatabase.GUIDToAssetPath(AssetGUID);
                var main = AssetDatabase.LoadMainAssetAtPath(assetPath) as SceneAsset;
                if (main != null)
                    CachedAsset = main;
                return main;
            }
        }
#endif
    }

    [Serializable]
    public class SceneReference : UnityEngine.Object
    {
    }

}
