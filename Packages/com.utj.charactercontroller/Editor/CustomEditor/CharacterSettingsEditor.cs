using System;
using Unity.TinyCharacterController;
using Unity.TinyCharacterControllerEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TinyCharacterControllerEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CharacterSettings))]
    public class CharacterSettingsEditor : UnityEditor.Editor
    {
        private readonly TccCondition _condition = new ();
        private GUIContent _title;

        private void Awake()
        {
            _title = new GUIContent("Condition Check");
            OnUpdate();
        }
        
        public override bool HasPreviewGUI() => _condition.HasErrorMessage;

        public override GUIContent GetPreviewTitle() => _title;

        public override VisualElement CreatePreview(VisualElement previewWindow)
        {
            OnUpdate();
            
            previewWindow.Clear();

            var scrollView = new ScrollView();
            foreach (var message in _condition.ErrorMessages)
            {
                var box = new HelpBox(message, HelpBoxMessageType.Warning);
                scrollView.Add(box);
            }
            
            previewWindow.Add(scrollView);
            return previewWindow;
        }

        /// <summary>
        /// Update components
        /// </summary>
        private void OnUpdate()
        {
            if (Application.isPlaying)
                return;

            _condition.GatherErrorMessage((Component)target);
        }
    }
}