using UnityEngine;

namespace Unity.SaveData
{
    public abstract class SaveLoadManagerBase : ScriptableObject
    {
        public abstract void Save(string path, string fileName, SaveData saveData, FileManagerBase fileManager);
        
        public abstract SaveData Load(string path, string fileName, FileManagerBase fileManager);
    }
}