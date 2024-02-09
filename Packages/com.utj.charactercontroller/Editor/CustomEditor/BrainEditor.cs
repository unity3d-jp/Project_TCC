using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEditor;
using UnityEngine;

namespace TinyCharacterControllerEditor
{
    [CustomEditor(typeof(BrainBase), true)]
    public class BrainEditor : UnityEditor.Editor
    {
        // private static readonly List<ICharacterMove> _moves = new();
        // private static readonly List<ICharacterTurn> _turns = new();
        // private static MenuType _menuType = MenuType.Priority;
        // private static GUIContent _previewTitle = new GUIContent("Brain");
        // public override bool HasPreviewGUI() => Application.isPlaying;
        //
        // enum MenuType
        // {
        //     Priority,
        //     Velocity
        // }
        //
        //
        // public override GUIContent GetPreviewTitle() => _previewTitle;

        // public override void OnPreviewSettings()
        // {
        //     _menuType = (MenuType)EditorGUILayout.EnumPopup(_menuType);
        // }

        // public override bool RequiresConstantRepaint() => true;
        //
        // public override void OnPreviewGUI(Rect r, GUIStyle background)
        // {
        //     void DrawPriority<T>(in Rect rect1, IPriority<T> comp) where T :  class, IPriority<T>
        //     {
        //         var size = 30;
        //         var width = rect1.width - size;
        //         var left = rect1;
        //         left.width = width;
        //         var right = left;
        //         right.x = width;
        //         right.width = size;
        //         EditorGUI.LabelField(left, $"{comp.GetType().Name} : ");
        //         using (new EditorGUI.DisabledScope(true))
        //         {
        //             EditorGUI.IntField(right, comp.Priority);
        //         }
        //     }
        //
        //     var brain = (BrainBase)target;
        //
        //     var bold = EditorStyles.boldLabel;
        //     var rect = r;
        //     rect.height = 15;
        //
        //     if (_menuType == MenuType.Priority)
        //     {
        //         brain.gameObject.GetComponents(_moves);
        //         brain.gameObject.GetComponents(_turns);
        //     
        //         _moves.Sort((x,y) => y.Priority - x.Priority);
        //         _turns.Sort((x,y) => y.Priority - x.Priority);
        //
        //         EditorGUI.LabelField(rect, "Move", bold);
        //         rect.y += 15;
        //         foreach (var comp in _moves)
        //         {
        //             DrawPriority(rect, comp);
        //             rect.y += 15;
        //         }
        //         rect.y += 15;
        //
        //         EditorGUI.LabelField(rect, "Turn", bold);
        //         rect.y += 15;
        //         foreach (var comp in _turns)
        //         {
        //             DrawPriority(rect, comp);
        //             rect.y += 15;
        //         }
        //     }
        //
        //     if (_menuType == MenuType.Velocity)
        //     {
        //         EditorGUI.LabelField(rect, "Move", bold);
        //         rect.y += 15;
        //         EditorGUI.LabelField(rect, $"Total   : {brain.TotalVelocity}");
        //         rect.y += 15;
        //         EditorGUI.LabelField(rect, $"Control : {brain.ControllableVelocity}");
        //         rect.y += 15;
        //         EditorGUI.LabelField(rect, $"Effect  : {brain.AdditionalVelocity}");
        //         rect.y += 30;
        //
        //         EditorGUI.LabelField(rect, "Turn", bold);
        //         rect.y += 15;
        //         EditorGUI.LabelField(rect, $"TurnSpeed : {brain.YawAngle}");
        //         rect.y += 15;
        //         EditorGUI.LabelField(rect, $"TurnSpeed : {brain.TurnSpeed}");
        //         rect.y += 30;
        //     }
        // }
    }
}
