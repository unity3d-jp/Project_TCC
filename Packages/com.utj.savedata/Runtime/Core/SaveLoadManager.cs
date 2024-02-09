using UnityEngine;

namespace Unity.SaveData
{
    /// <summary>
    /// Manages saving and loading of data.
    /// </summary>
    public class SaveLoadManager : SaveLoadManagerBase
    {
        private static SaveLoadManager _instance;

        public static SaveLoadManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = CreateInstance<SaveLoadManager>();
                return _instance;
            }
        }

        /// <summary>
        /// Saves data to a specified location.
        /// </summary>
        /// <param name="path">The path to save the data to.</param>
        /// <param name="fileName">The name of the file to save the data as.</param>
        /// <param name="saveData">The data to save.</param>
        /// <param name="fileManager">file writer</param>
        public override void Save(string path, string fileName, SaveData saveData, FileManagerBase fileManager)
        {
            var json = JsonUtility.ToJson(saveData);
            fileManager.Write(path, fileName, json);
        }

        /// <summary>
        /// Loads data from a specified location.
        /// </summary>
        /// <param name="path">The path to load the data from.</param>
        /// <param name="fileName">The name of the file to load the data from.</param>
        /// <param name="fileManager"></param>
        /// <returns>The loaded data.</returns>
        public override SaveData Load (string path, string fileName, FileManagerBase fileManager)
        {
            var json = fileManager.Read(path, fileName);
            return JsonUtility.FromJson<SaveData>(json);
        }
    }
}