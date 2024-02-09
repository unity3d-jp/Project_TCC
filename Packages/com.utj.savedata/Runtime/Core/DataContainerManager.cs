using System.Collections.Generic;
using Unity.SaveData.Core;
using UnityEngine;

namespace Unity.SaveData
{
    /// <summary>
    /// Manages data containers for serialization.
    /// </summary>
    public class DataContainerManager
    {
        /// <summary>
        /// Gets the list of data containers.
        /// </summary>
        private List<IDataContainer> Containers { get; } = new();

        /// <summary>
        /// Initializes the data container manager and collects data containers from the given game object.
        /// </summary>
        /// <param name="owner">The game object to collect data containers from.</param>
        public void Initialize(GameObject owner)
        {
            owner.GetComponentsInChildren(Containers);
        }

        /// <summary>
        /// Retrieves a data container by ID.
        /// </summary>
        /// <param name="id">The ID of the data container to retrieve.</param>
        /// <returns>The data container with the specified ID.</returns>
        public IDataContainer GetContainer(PropertyName id)
        {
            return Containers.Find(c => c.Id == id);
        }

        /// <summary>
        /// Creates a SaveData object from the current data containers.
        /// </summary>
        /// <returns>A new SaveData object.</returns>
        public SaveData CreateSaveData()
        {
            var saveData = new SaveData
            {
                Ids = new PropertyName[Containers.Count],
                Jsons = new string[Containers.Count]
            };

            for (var index = 0; index < Containers.Count; index++)
            {
                var component = Containers[index];

                if (PropertyName.IsNullOrEmpty(component.Id))
                    Debug.LogError($"{component}'s id is empty", (Component)component);

                saveData.Ids[index] = component.Id;
                saveData.Jsons[index] = JsonUtility.ToJson(component);
            }

            return saveData;
        }

        /// <summary>
        /// Overwrites the data in containers with the provided SaveData.
        /// </summary>
        /// <param name="saveData">The SaveData to overwrite with.</param>
        public void OverwriteData(SaveData saveData)
        {
            for (var i = 0; i < saveData.Ids.Length; i++)
            {
                var id = saveData.Ids[i];
                var index = Containers.FindIndex(container => container.Id == id);
                if (index == -1)
                    continue;

                JsonUtility.FromJsonOverwrite(saveData.Jsons[i], Containers[index]);
            }
        }
    }
}