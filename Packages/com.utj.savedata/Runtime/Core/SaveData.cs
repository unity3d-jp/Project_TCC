using System;
using UnityEngine;

namespace Unity.SaveData
{
    /// <summary>
    /// Represents save data with IDs and JSON strings.
    /// </summary>
    [Serializable]
    public struct SaveData
    {
        public PropertyName[] Ids;
        public string[] Jsons;
    }
}