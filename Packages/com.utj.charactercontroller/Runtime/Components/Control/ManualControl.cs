using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// Components that directly set the movement or orientation.
    /// Sets the direction of movement and orientation, and uses those values to move the character.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(ManualControl))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RenamedFrom("TinyCharacterController.ManualControl")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Control.ManualControl")]
    public class ManualControl : MonoBehaviour , 
        ITurn, 
        IMove
    {
        /// <summary>
        /// Move priority.
        /// If this value is higher than the other components,
        /// the character moves by the <see cref="MoveVelocity"/> value.
        /// </summary>
        [FormerlySerializedAs("movePriority")]
        [Header("Move Settings")]
        public int MovePriority;
        
        /// <summary>
        /// Movement vector of the character.
        /// </summary>
        [FormerlySerializedAs("moveVelocity")] 
        public Vector3 MoveVelocity;

        /// <summary>
        /// Turn Priority.
        /// If this value is higher than the other values,
        /// the character turns toward <see cref="TurnAngle"/>.
        /// </summary>
        [FormerlySerializedAs("turnPriority")]
        [Header("Turn Settings")]
        public int TurnPriority;
        
        /// <summary>
        /// Sets the speed at which the direction is changed.
        /// Basic is 0 to 30, negative is not complementary and changes instantly.
        /// </summary>
        [FormerlySerializedAs("turnSpeed")]
        [Range(-1, 50)]
        public int TurnSpeed = 35;

        /// <summary>
        /// The direction in which the character faces. World Coordinates
        /// </summary>
        [FormerlySerializedAs("turnAngle")]
        [SerializeField]
        private float _turnAngle;
        
        private Quaternion _yawRotation;

        /// <summary>
        /// The direction in which the character faces. World Coordinates
        /// </summary>
        public float TurnAngle
        {
            get => _turnAngle;
            set
            {
                _turnAngle = value;
                _yawRotation = Quaternion.AngleAxis(_turnAngle, Vector3.up);
            }
        }

        /// <summary>
        /// The direction in which the character faces. World Coordinates.
        /// Ignore the Y axis.
        /// </summary>
        public Vector3 TurnDirection
        {
            get => _yawRotation * Vector3.forward;
            set
            {
                _yawRotation = Quaternion.LookRotation(value, Vector3.up);
                _turnAngle = Vector3.SignedAngle(Vector3.forward, value, Vector3.up);
            }
        }

        /// <summary>
        /// Character rotation.
        /// Ignore the Y axis.
        /// </summary>
        public Quaternion TurnRotation
        {
            get => _yawRotation;
            set
            {
                _yawRotation = value;
                _turnAngle = Vector3.SignedAngle(Vector3.forward, value * Vector3.forward, Vector3.up);
            }
        }
        
        int IPriority<ITurn>.Priority => TurnPriority;

        int ITurn.TurnSpeed => TurnSpeed;

        float ITurn.YawAngle => _turnAngle;

        int IPriority<IMove>.Priority => MovePriority;

        Vector3 IMove.MoveVelocity => MoveVelocity;

        private void OnValidate()
        {
            MovePriority = Mathf.Max(0, MovePriority);
            TurnPriority = Mathf.Max(0, TurnPriority);
            TurnSpeed = Mathf.Max(-1, TurnSpeed);

            TurnAngle = _turnAngle;
        }
    }
}