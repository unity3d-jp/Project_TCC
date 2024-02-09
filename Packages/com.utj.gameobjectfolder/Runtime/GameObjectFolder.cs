using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Utility
{
    /// <summary>
    /// Unfolds the hierarchical structure of GameObjects below the object where this component is attached.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [Unity.VisualScripting.RenamedFrom("GameDevUtility.GameObjectFolder")]
    public class GameObjectFolder : MonoBehaviour
    {
#if UNITY_EDITOR
        /// <summary>
        /// UI comment.
        /// </summary>
        [FormerlySerializedAs("comment")]
        [SerializeField]
        [Multiline(15)]
        private string _comment ;

        /// <summary>
        /// Color of the Hierarchy displayed in the Inspector.
        /// </summary>
        [FormerlySerializedAs("MenuColor")]
        [FormerlySerializedAs("color")] 
        [SerializeField]
        [ColorUsage(false)]
        private Color _menuColor = Color.white;

        
        /// <summary>
        /// Display objects below the GameObjectFolder.
        /// </summary>
        [FormerlySerializedAs("isVisible")] 
        [SerializeField] 
        private bool _isVisible = false;
        
        private void Reset()
        {
            // Update the object's position when it is created.
            var trs = transform;
            trs.position = Vector3.zero;
            trs.hideFlags = HideFlags.HideInInspector;
        }

        public List<GameObject> ChildObjects = new List<GameObject>();
#endif
    }
}