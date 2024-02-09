using System.Collections;
using System.Collections.Generic;
using Unity.SaveData.Core;
using UnityEngine;

namespace GameDataSave.Sample
{
    public class CustomDataContainer : DataContainerBase<PlayerData>
    {

    }

    [System.Serializable]
    public struct PlayerData
    {
        public int Power;
        public int Hp;
    }
}
