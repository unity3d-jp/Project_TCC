using UnityEngine;

namespace Unity.SaveData.Core
{
    /// <summary>
    /// ID to identify the data container.
    /// </summary>
    public interface IDataContainer
    {
        /// <summary>
        /// ID to identify the object.
        /// Use PropertyName to speed up lookup.
        /// </summary>
        PropertyName Id { get; }
    }
}