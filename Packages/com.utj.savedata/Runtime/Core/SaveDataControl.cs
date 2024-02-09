using Unity.SaveData.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.SaveData
{
    /// <summary>
    /// Controls the save and load operations for game data using <see cref="DataContainerManager"/>,
    /// <see cref="Unity.SaveData.SaveLoadManager"/>, and <see cref="FileManagerBase"/>.
    /// </summary>
    [RenamedFrom("DataStore.SaveDataControl")]
    public class SaveDataControl : MonoBehaviour
    {
        /// <summary>
        /// Name of the save data.
        /// </summary>
        [SerializeField]
        private string _fileName;

        /// <summary>
        /// Method for saving and loading.
        /// If null, <see cref="Unity.SaveData.SaveLoadManager"/> will be automatically generated and assigned.
        /// </summary>
        public SaveLoadManagerBase SaveLoadManager;

        /// <summary>
        /// Management of file paths and saving processes.
        /// </summary>
        public FileManagerBase FileManager;

        private readonly DataContainerManager _dataContainerManager = new ();

        /// <summary>
        /// Gets the file name for saving data. Uses the GameObject's name if _fileName is not set.
        /// </summary>
        private string FileName => string.IsNullOrEmpty(_fileName) ? name : _fileName;

        /// <summary>
        /// Initializes the control components for save data operations.
        /// </summary>
        private void Awake()
        {
            _dataContainerManager.Initialize(gameObject);

            if (FileManager == null)
                FileManager = Unity.SaveData.FileManager.Instance;

            
            if (SaveLoadManager == null)
                SaveLoadManager = Unity.SaveData.SaveLoadManager.Instance;
        }

        /// <summary>
        /// Resets the filename to the name of the GameObject when the component is reset in the editor.
        /// </summary>
        private void Reset()
        {
            _fileName = name;
        }

        /// <summary>
        /// Retrieves a data container component by ID.
        /// </summary>
        /// <param name="id">The ID of the data container.</param>
        /// <returns>The component that matches the specified ID.</returns>
        public Component GetContainer(string id)
        {
            return GetContainer(new PropertyName(id));
        }

        /// <summary>
        /// Retrieves a data container of type T by ID.
        /// </summary>
        /// <typeparam name="T">The type of data container to retrieve.</typeparam>
        /// <param name="id">The ID of the data container.</param>
        /// <returns>The data container of type T that matches the specified ID.</returns>
        public T GetContainerT<T>(PropertyName id) where T : IDataContainer
        {
            return (T)_dataContainerManager.GetContainer(id);
        }

        /// <summary>
        /// Retrieves a data container component by ID.
        /// </summary>
        /// <param name="id">The ID of the data container.</param>
        /// <returns>The component that matches the specified ID.</returns>
        public Component GetContainer(PropertyName id)
        {
            return _dataContainerManager.GetContainer(id) as Component;
        }

        /// <summary>
        /// Checks if a save data file exists.
        /// </summary>
        /// <param name="folder">The folder to check within.</param>
        /// <returns>True if the file exists, otherwise false.</returns>
        [RenamedFrom("Exists")]
        public bool IsExists([RenamedFrom("saveDataName")] string folder)
        {
            return FileManager.IsExists(folder, FileName);
        }

        /// <summary>
        /// Removes the save data file.
        /// </summary>
        /// <param name="folder">The folder containing the save data file to remove.</param>
        public void Remove([RenamedFrom("saveDataName")] string folder)
        {
            FileManager.Remove(folder, FileName);
        }

        /// <summary>
        /// Saves the current game state to a file.
        /// </summary>
        /// <param name="folder">The folder to save the file in.</param>
        public void Save(string folder)
        {
            var saveData = _dataContainerManager.CreateSaveData();
            SaveLoadManager.Save(folder, FileName, saveData, FileManager);
        }

        /// <summary>
        /// Loads the game state from a file.
        /// </summary>
        /// <param name="folder">The folder to load the file from.</param>
        public void Load(string folder)
        {
            if (FileManager.IsExists(folder, FileName) == false)
                return;

            var saveData = SaveLoadManager.Load(folder, FileName, FileManager);
            _dataContainerManager.OverwriteData(saveData);
        }
    }
}