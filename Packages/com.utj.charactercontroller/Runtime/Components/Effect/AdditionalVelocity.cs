using Unity.VisualScripting;
using UnityEngine;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;

namespace Unity.TinyCharacterController.Effect
{
    /// <summary>
    /// A component that sets a custom acceleration for a character.
    /// The acceleration is set externally and is not changed by the component.
    /// The acceleration set here is reflected in the character by the Brain.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(MenuList.MenuEffect + nameof(AdditionalVelocity))]
    [RequireComponent(typeof(CharacterSettings))]
    [RenamedFrom("TinyCharacterController.AdditionalVelocity")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Effect.AdditionalVelocity")]
    public class AdditionalVelocity : MonoBehaviour, IEffect
    {
        [SerializeField] private Vector3 _velocity;

        /// <summary>
        /// A velocity.
        /// </summary>
        public Vector3 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }
        
        /// <summary>
        /// Speed to move
        /// </summary>
        public float Speed => Velocity.magnitude;

        /// <summary>
        /// Reset velocity.
        /// </summary>
        public void ResetVelocity()
        {
            Velocity = Vector3.zero;
        }

        private void OnDrawGizmosSelected()
        {
            var startPosition = transform.position;
            var endPosition = startPosition + Velocity;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(startPosition, Velocity);
            Gizmos.DrawSphere(endPosition, 0.1f);
        }
    }
}