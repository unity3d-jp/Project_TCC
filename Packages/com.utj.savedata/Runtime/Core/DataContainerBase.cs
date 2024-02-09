using System;
using System.Collections;
using System.Collections.Generic;
using Unity.SaveData.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.SaveData.Core
{
    /// <summary>
    /// Components for storing data.
    /// Saves the information stored in the component by <see cref="SaveDataControl"/>.
    /// </summary>
    /// <typeparam name="T">Stored data type</typeparam>
    public abstract class DataContainerBase<T> : MonoBehaviour, IDataContainer
    {
        [Tooltip("ID for identify data.")]
        [SerializeField, NoEmptyString] 
        private PropertyName _id;

        [SerializeField] 
        private T _value;

        PropertyName IDataContainer.Id => _id;

        private void Reset()
        {
            // If the component is not registered, SaveDataControl is automatically registered.
            var control = GetComponentInParent<SaveDataControl>();
            if (control == null)
            {
                gameObject.AddComponent<SaveDataControl>();
            }
        }


        /// <summary>
        /// Value
        /// </summary>
        public T Value
        {
            get => _value;
            set => _value = value;
        }
    }
}