using System;
using System.IO;
using UnityEngine;

namespace Unity.SaveData
{
    /// <summary>
    /// Implements file control for saving and loading data.
    /// </summary>
    public class FileManager : FileManagerBase
    {

        private static FileManager _fileManager;
        public static FileManager Instance
        {
            get
            {
                if (_fileManager != null)
                    return _fileManager;

                _fileManager = CreateInstance<FileManager>();
                
                return _fileManager;
            }
        }
        
        public override void Write(string path, string fileName, string json)
        {
            var fullPath = GetPath(path, fileName);

#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerPrefs.SetString(fullPath, json);
            PlayerPrefs.Save();
#else
            var directory = Path.GetDirectoryName(fullPath);
            if (directory == null)
                throw new Exception("Directory can't create.");

            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            File.WriteAllText(fullPath, json);
#endif
        }

        public override string Read(string path, string fileName)
        {
            var fullPath = GetPath(path, fileName);
            
#if UNITY_WEBGL && !UNITY_EDITOR
            return PlayerPrefs.GetString(fullPath);
#else
            return File.ReadAllText(fullPath);
#endif
        }

        public override void Remove(string path, string fileName)
        {
            var fullPath = GetPath(path, fileName);

#if UNITY_WEBGL && !UNITY_EDITOR
            if (PlayerPrefs.HasKey(fullPath))
                PlayerPrefs.DeleteKey(fullPath);
#else

            if (File.Exists(fullPath))
                File.Delete(fullPath);
#endif
        }

        public override bool IsExists(string path, string fileName)
        {
            var fullPath = GetPath(path, fileName);

#if UNITY_WEBGL && !UNITY_EDITOR
            return PlayerPrefs.HasKey(fullPath);
#else
            return File.Exists(fullPath);
#endif
        }

        /// <summary>
        /// Generates the full path for a given file.
        /// </summary>
        /// <param name="path">The path segment.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>The full path to the file.</returns>
        public override string GetPath(string path, string fileName)
        {
            const string saveFolder = "SaveData";
#if UNITY_EDITOR
            var folderPath = $"{saveFolder}/{path}";
#elif UNITY_ANDROID || UNITY_IOS
            var folderPath = $"{Application.persistentDataPath}/{saveFolder}/{path}";
#else
            var folderPath = $"{saveFolder}/{path}";
#endif
            return $"{folderPath}/{fileName}.json";
        }
    }
}