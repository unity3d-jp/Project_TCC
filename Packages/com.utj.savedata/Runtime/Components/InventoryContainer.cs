using System;
using System.Collections.Generic;
using System.Linq;
using Unity.SaveData.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.SaveData
{
    /// <summary>
    /// Manages the inventory of items for a character or entity.
    /// </summary>
    [RenamedFrom("DataStore.InventoryContainer")]
    public class InventoryContainer : MonoBehaviour, IDataContainer
    {
        /// <summary>
        /// List of items currently in the inventory.
        /// </summary>
        [SerializeField] [InventoryProperty] private List<ItemData> _items;

        /// <summary>
        /// Provides the names of all items in the inventory, sorted alphabetically.
        /// </summary>
        public IEnumerable<string> ItemNames => _items.Select(c => c.Name).OrderBy(c => c);

        /// <summary>
        /// Ensures the existence of SaveDataControl on the parent game object.
        /// </summary>
        private void Reset()
        {
            var saveDataControl = GetComponentInParent<SaveDataControl>();
            if (saveDataControl == null)
                gameObject.AddComponent<SaveDataControl>();
        }

        public PropertyName Id => new("Inventory");

        /// <summary>
        /// Checks if an item exists in the inventory by name.
        /// </summary>
        /// <param name="itemName">The name of the item to check.</param>
        /// <returns>True if the item exists, otherwise false.</returns>
        public bool ExistsItem(string itemName)
        {
            var property = new PropertyName(itemName);
            return _items.Exists(c => c.Id == property);
        }

        /// <summary>
        /// Gets the count of a specific item in the inventory by name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <returns>The count of the item.</returns>
        public int GetItemCount(string itemName)
        {
            var property = new PropertyName(itemName);
            return GetItemCount(property);
        }


        /// <summary>
        /// Adds a specified number of items to the inventory.
        /// </summary>
        /// <param name="itemName">The name of the item to add.</param>
        /// <param name="count">The number of items to add.</param>
        public void AddItem(string itemName, int count)
        {
            var item = GetOrAddItem(itemName);
            item.Count += count;
        }

        /// <summary>
        /// Sets the count of a specific item in the inventory.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="count">The new count of the item.</param>
        public void SetItemCount(string itemName, int count)
        {
            if (count <= 0)
            {
                if (ExistsItem(itemName))
                    _items.Remove(GetItem(itemName));
            }
            else
            {
                var item = GetOrAddItem(itemName);
                item.Count = count;
            }
        }

        /// <summary>
        /// Subtracts a specified number of items from the inventory.
        /// </summary>
        /// <param name="itemName">The name of the item to subtract.</param>
        /// <param name="count">The number of items to subtract.</param>
        public void SubtractItem(string itemName, int count)
        {
            var item = GetItem(itemName);
            if (item == null)
                return;

            item.Count -= count;
            if (item.Count <= 0)
                _items.Remove(item);
        }


        /// <summary>
        /// Retrieves an item from the inventory by name.
        /// </summary>
        /// <param name="itemName">The name of the item to retrieve.</param>
        /// <returns>The item if found; otherwise, null.</returns>
        private ItemData GetItem(string itemName)
        {
            var id = new PropertyName(itemName);
            var item = _items.Find(c => c.Id == id);
            return item;
        }
        
        private int GetItemCount(PropertyName id)
        {
            var item = _items.Find(c => c.Id == id);
            var hasItem = item != null;

            return hasItem ? item.Count : 0;
        }

        /// <summary>
        /// Gets an existing item or adds a new one if it doesn't exist.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <returns>The item data.</returns>
        private ItemData GetOrAddItem(string itemName)
        {
            var item = GetItem(itemName);
            if (item == null)
            {
                item = new ItemData { Name = itemName };
                _items.Add(item);
            }

            return item;
        }

        /// <summary>
        /// Represents the data for an item in the inventory.
        /// </summary>
        [Serializable]
        public class ItemData
        {
            /// <summary>
            /// The name of the item.
            /// </summary>
            [SerializeField] 
            private string _name;

            /// <summary>
            /// The count of the item.
            /// </summary>
            public int Count;

            /// <summary>
            /// The unique identifier for the item.
            /// </summary>
            [NonSerialized]
            private PropertyName _id;

            /// <summary>
            /// Indicates whether the ID has been initialized.
            /// </summary>
            [NonSerialized]
            private bool _applyNameToId ;

            /// <summary>
            /// Gets or sets the name of the item. Setting the name also resets the item's ID.
            /// </summary>
            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    _applyNameToId = false;
                }
            }

            /// <summary>
            /// Gets the item's unique identifier, initializing it if necessary.
            /// </summary>
            public PropertyName Id
            {
                get
                {
                    if (_applyNameToId )
                        return _id;
                    
                    _id = new PropertyName(Name);
                    _applyNameToId = true;
                    return _id;
                }
            }
        }
    }
}