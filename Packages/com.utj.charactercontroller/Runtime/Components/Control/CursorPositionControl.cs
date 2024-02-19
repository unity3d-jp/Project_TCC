using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// This component updates the character's orientation based on the cursor position. <br/>
    /// 
    /// If the component has high priority, the character will look in the direction of the cursor.
    /// The coordinates at which the character gazes are calculated based on <see cref="LookTargetPoint"/>.
    /// If you want to use side view instead of top-down, change <see cref="_planeAxis"/>.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(CursorPositionControl))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RenamedFrom("TinyCharacterController.CursorPositionControl")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Control.CursorPositionControl")]
    public class CursorPositionControl : 
        MonoBehaviour, 
        ITurn,
        IUpdateComponent
    {
        /// <summary>
        /// Maximum distance of cursor.
        /// Used to limit the range of movement of the camera.
        /// For example, if you want the camera to follow the cursor position.
        /// </summary>
        [Header("Cursor behavior settings")]
        [Tooltip("Maximum distance of cursor")]
        [SerializeField] 
        private float _maxDistance = 3;
        
        /// <summary>
        /// Offset to compensate for orientation to the cursor.
        /// For (0,0,0), orientation from the root of the model.
        /// Adjust to gun height if you want to calculate orientation based on gun height.
        /// </summary>
        [SerializeField] 
        private Vector3 _originOffset = Vector3.zero;


        /// <summary>
        /// Direction of the judgment plane.
        /// Set (0, 1, 0) for a top viewpoint or (0, 0, 1) for a side viewpoint.
        /// <see cref="PlaneAxis"/>
        /// </summary>
        [Header("Plane settings")]
        [SerializeField] 
        private Vector3 _planeAxis = Vector3.up;

        [SerializeField] private float _planeOffset;

        /// <summary>
        /// Rotation Priority. Face the cursor direction when it is higher than the priority of other components.
        /// </summary>
        [FormerlySerializedAs("_turnPriority")]
        [Header("Character orientation control")]
        public int TurnPriority = 1;

        /// <summary>
        /// Speed at which the cursor is oriented. If the value is -1, the orientation is fixed.
        /// </summary>
        [SerializeField, Range(-1, 100)]
        private int _turnSpeed = 30;
        
        int IPriority<ITurn>.Priority => TurnPriority;

        /// <summary>
        /// Turn Speed
        /// </summary>
        public int TurnSpeed { get => _turnSpeed; set => _turnSpeed = value; }

        float ITurn.YawAngle => YawRotation.eulerAngles.y;

        /// <summary>
        /// CursorPosition centered on the character and limited to MaxDistance distance.
        /// Avoid the problem of the cursor leaving indefinitely in cases where the camera is tracking the cursor.
        /// </summary>
        public Vector3 LimitedPosition { get; private set; }

        /// <summary>
        /// World position of cursor
        /// </summary>
        public Vector3 CursorPosition { get; private set; }
        
        /// <summary>
        /// Rotation to cursor (horizontal rotation)
        /// </summary>
        public Quaternion YawRotation { get; private set; }

        /// <summary>
        /// Rotation to cursor (vertical rotation)
        /// </summary>
        public Quaternion PitchRotation { get; private set; }
        
        /// <summary>
        /// Direction of the judgment plane.
        /// <see cref="_planeAxis"/>
        /// </summary>
        public Vector3 PlaneAxis {
            get => _planeAxis;
            set => _planeAxis = value;
        }

        /// <summary>
        /// Face the direction of the screen coordinates
        /// Note that this is not immediately reflected. The value is reflected at the time of Update.
        /// </summary>
        /// <param name="screenPosition">look position</param>
        public void LookTargetPoint(Vector2 screenPosition)
        {
            _mousePosition = screenPosition;
        }

        private Vector2 _mousePosition;
        private ITransform _transform;
        private CharacterSettings _characterSettings;

        private void Awake()
        {
            TryGetComponent(out _transform);
            TryGetComponent(out _characterSettings);

            LimitedPosition = transform.position;
            YawRotation = Quaternion.AngleAxis(0, Vector3.up);
        }

        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            var plane = new Plane(_planeAxis, _transform.Position+ _planeAxis * _planeOffset );
            var ray = _characterSettings.CameraMain.ScreenPointToRay(_mousePosition);

            // If the cursor position does not make contact with the plane, the process is aborted.
            if (!plane.Raycast(ray, out var distance)) 
                return;
            
            // Get cursor position
            var contactPosition = ray.GetPoint(distance) + _originOffset;
            var position = _transform.Position;
            CursorPosition = contactPosition ;
            LimitedPosition = (Vector3.Distance(position, contactPosition) > _maxDistance) ?
                Vector3.MoveTowards(position, contactPosition, _maxDistance) : contactPosition;
            
            // Pre-calculate character orientation
            var deltaPosition = LimitedPosition - position;
            YawRotation = Quaternion.LookRotation(Vector3.Scale(deltaPosition, new Vector3(1, 0, 1)), Vector3.up);
            PitchRotation = Quaternion.LookRotation(Vector3.Scale(deltaPosition, new Vector3(1, 1, 0)), Vector3.up);
        }

        int IUpdateComponent.Order => Order.Control;
        
        
        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            var size = new Vector3(1, 0, 1);

            GizmoDrawUtility.DrawCube(position, size, Color.green, 0.2f);

            if (Application.isPlaying == false)
                return;

            var boxSize = Vector3.one * 0.2f;
            Gizmos.color = Color.yellow;
            GizmoDrawUtility.DrawCube(CursorPosition, boxSize, Color.yellow);

            GizmoDrawUtility.DrawCube(LimitedPosition, boxSize, Color.white);
            Gizmos.DrawLine(position, LimitedPosition);

            Gizmos.DrawWireCube(LimitedPosition, Vector3.one * 0.2f);
            Gizmos.color = Color.white;
            Gizmos.DrawCube(LimitedPosition, boxSize);
        }
    }
}
