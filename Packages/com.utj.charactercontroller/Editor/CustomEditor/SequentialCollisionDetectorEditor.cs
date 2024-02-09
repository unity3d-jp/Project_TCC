using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Utility;
using UnityEditor;
using UnityEngine;

namespace TinyCharacterControllerEditor
{
    /// <summary>
    /// Editor extension for SequentialCollisionDetector.
    /// To avoid problems during EditMultipleObjects, the editor extension without content avoids simultaneous editing of components.
    /// /</summary>
    [CustomEditor(typeof(SequentialCollisionDetector))]
    public class SequentialCollisionDetectorEditor : UnityEditor.Editor
    {
        
    }
}
