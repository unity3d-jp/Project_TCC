using Unity.TinyCharacterController.Interfaces.Components;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace TinyCharacterControllerEditor.Overlays
{
    /// <summary>
    /// Editor tools for registering TCC-related Overlays.
    /// Registers <see cref="ControlPriorityOverlay"/>, <see cref="VelocityOverlay"/>,
    /// and <see cref="EffectOverlay"/>.
    /// </summary>
    [EditorTool("Control Priority", typeof(IBrain))]
    public class TccEditorTool : EditorTool
    {
        private ControlPriorityOverlay _priorityOverlay;
        private VelocityOverlay _velocityOverlay;
        private EffectOverlay _effectOverlay;

        
        private void Initialize()
        {
            var obj = ((MonoBehaviour)target).gameObject;
            _effectOverlay = new EffectOverlay(obj);
            _priorityOverlay = new ControlPriorityOverlay(obj);
            _velocityOverlay = new VelocityOverlay(obj.GetComponent<IBrain>());
        }

        private void OnEnable()
        {
            Initialize();

            SceneView.AddOverlayToActiveView(_priorityOverlay);
            SceneView.AddOverlayToActiveView(_velocityOverlay);
            SceneView.AddOverlayToActiveView(_effectOverlay);

        }

        private void OnDisable()
        {
            SceneView.RemoveOverlayFromActiveView(_priorityOverlay);
            SceneView.RemoveOverlayFromActiveView(_velocityOverlay);
            SceneView.RemoveOverlayFromActiveView(_effectOverlay);
        }

        public override bool IsAvailable() => false; // Do not display the editor tool icon
    }
}