using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTJ
{
    public class PathUtil
    {
        public static string NormalizePath(string path)
        {
            path = path.Trim();
            if (Path.IsPathRooted(path))
            {
                path = Path.GetFullPath(path);
            }
            return path.Replace('\\', '/');
        }

        public static string CombinePath(string parent, string child)
        {
            return NormalizePath(Path.Combine(parent, child));
        }

        public static string CombinePaths(IEnumerable<string> paths)
        {
            var combinedPath = "";
            foreach (var path in paths)
            {
                combinedPath = Path.Combine(combinedPath, path);
            }
            return NormalizePath(combinedPath);
        }

        // OS上のディレクトリーをプロジェクト内のAssets/で始まるディレクトリーに変換
        public static string SystemPathToAssetPath(string inSystemPath)
        {
            // Assetsより一つ上のフォルダーを取得
            var projectPath = Application.dataPath;
            var projectURI = new System.Uri(projectPath);
            var fullURI = new System.Uri(inSystemPath);
            var relativePath = projectURI.MakeRelativeUri(fullURI).ToString();
            relativePath = relativePath.Replace('\\', '/');
            return relativePath;
        }

        public static string AssetPathToSystemPath(string assetPath)
        {
            const string AssetDirectory = "Assets";

            var rootDir = Path.GetDirectoryName(Application.dataPath);
            if (!assetPath.ToLowerInvariant().StartsWith(AssetDirectory.ToLowerInvariant()))
            {
                assetPath = CombinePath(AssetDirectory, assetPath);
            }
            var finalPath = CombinePath(rootDir, assetPath);
#if UNITY_EDITOR_WIN
            finalPath = finalPath.Replace('/', '\\');
#endif
            return finalPath;
        }

        // Resourcesディレクトリーが入ってるパスを見つけて、その後のパスを返す
        // Resourcesが入っていない場合は空の文字列を返す
        public static string PathToResourcePath(string sourcePath)
        {
            const string ResourcesDirectory = "resources/";
            const string ResourcesSubdirectory = "/" + ResourcesDirectory;

            sourcePath = sourcePath.Replace('\\', '/').Trim();
            var lowerPath = sourcePath.ToLowerInvariant();
            if (lowerPath.StartsWith(ResourcesDirectory))
            {
                return sourcePath.Substring(ResourcesDirectory.Length);
            }

            var resourcesDirectoryIndex = lowerPath.IndexOf(ResourcesSubdirectory);
            if (resourcesDirectoryIndex != -1)
            {
                return sourcePath.Substring(resourcesDirectoryIndex + ResourcesSubdirectory.Length);
            }

            return "";
        }

        public static IEnumerable<string> GetUniquePaths(IEnumerable<string> inputPaths)
        {
            var pathMap = new Dictionary<string, string>();
            foreach (var originalPath in inputPaths)
            {
                var pathKey = originalPath.ToLowerInvariant().Replace('\\', '/');
                pathMap[pathKey] = originalPath;
            }
            return pathMap.Values;
        }
    }
}