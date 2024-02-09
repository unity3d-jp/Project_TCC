using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.ScenarioImporter.Test
{
    public class ScenarioDummyComponent : MonoBehaviour
    {
        [SerializeField, Multiline(60)] 
        private string _textData;

        public string TextData => _textData;
    }
}
