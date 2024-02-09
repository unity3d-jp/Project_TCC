using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Check;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace TinyCharacterControllerEditor
{
    [CustomEditor(typeof(SightCheck))]
    public class SightCheckEditor : UnityEditor.Editor
    {
        private static readonly Color FindSomeTarget = new Color(1, 0, 0, 0.3f), NotFound = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        
        private void OnSceneGUI()
        {
            foreach (var o in targets)
            {
                if (o is SightCheck == false)
                    continue;
                
                var sightCheck = (SightCheck)o;
                var color = sightCheck.IsInsightAnyTarget ? FindSomeTarget : NotFound;
                var so = new SerializedObject(sightCheck);
                var eye = so.FindProperty("_headTransform").objectReferenceValue as Transform;
                if( eye == null)
                    continue;

                Handles.color = color;
                var invAngle = Quaternion.AngleAxis(sightCheck.Angle * 0.5f, Vector3.down);
                Handles.DrawSolidArc(eye.position, Vector3.up, invAngle * eye.forward, sightCheck.Angle, sightCheck.Range);
            }            
        }
    }
}
