using UnityEngine;

namespace Unity.SaveData
{
    public abstract class FileManagerBase : ScriptableObject
    {
        public abstract string GetPath(string path, string fileName);
        
        public abstract void Write(string path, string fileName, string json);

        public abstract string Read(string path, string fileName);

        public abstract void Remove(string path, string fileName);

        public abstract bool IsExists(string path, string fileName);
    }
}