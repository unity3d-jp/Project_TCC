using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// This component performs collision checks with objects above.
    /// However, the upward check is done based on height, not by Raycast, to reduce processing time.
    /// It is intended for use in games with simple terrain or no ceiling.
    /// </summary>
    [AddComponentMenu(MenuList.MenuCheck + nameof(ReachHeightCheck))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RenamedFrom("TinyCharacterController.HeightBasedHeadCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.HeightBasedOverheadDetection")]
    [RenamedFrom("Unity.TinyCharacterController.Check.HeightBasedOverheadDetection")]
    public class ReachHeightCheck : MonoBehaviour,
        IOverheadDetection
    {
        /// <summary>
        /// The height considered as the ceiling.
        /// </summary>
        [FormerlySerializedAs("Height")]
        public float RoofHeight = 100;

        bool IOverheadDetection.IsObjectOverhead => true;

        /// <summary>
        /// The object considered to be in contact during upward detection.
        /// </summary>
        public GameObject RoofObject = null;

        private ITransform _transform;
        private CharacterSettings _settings;

        private void Awake()
        {
            TryGetComponent(out _transform);
            TryGetComponent(out _settings);
        }

        /// <summary>
        /// Returns true if the specified position has been reached.
        /// </summary>
        public bool IsHeadContact => RoofHeight < _transform.Position.y + _settings.Height;

        GameObject IOverheadDetection.ContactedObject => RoofObject;

        /// <summary>
        /// The position of the ceiling.
        /// </summary>
        public Vector3 ContactPoint => ((IOverheadDetection)this).IsHeadContact ?
                        _transform.Position + new Vector3(0, _settings.Height, 0) :
                        Vector3.Scale(_transform.Position, new Vector3(1, 0, 1)) + new Vector3(0, RoofHeight, 0);

        private void OnDrawGizmosSelected()
        {
            // Draw the roof
            var color = Color.yellow;
            color.a = 0.4f;
            Gizmos.color = color;
            var flatScale = new Vector3(1, 0, 1);
            var pos = Application.isPlaying ?
                ((IOverheadDetection)this).ContactPoint :
                Vector3.Scale(transform.position, flatScale) + new Vector3(0, RoofHeight, 0);
            Gizmos.DrawCube(pos, new Vector3(5, 0, 5));
        }
    }
}
