using Unity.Mathematics;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Utility;
using UnityEngine;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Core;

namespace Unity.TinyCharacterController.Brain
{
    /// <summary>
    /// This brain operates using <see cref="UnityEngine.CharacterController"/>.
    /// The height and width of the Agent are determined by <see cref="CharacterSettings.Height"/> and <see cref="CharacterSettings.Radius"/>.
    /// </summary>
    [AddComponentMenu(MenuList.MenuBrain + "CharacterBrain")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(CharacterSettings))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.CharacterBrain")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Brain.CharacterBrain")]
    [DefaultExecutionOrder(Order.UpdateBrain)]
    public class CharacterBrain : BrainBase,  
        ICharacterSettingUpdateReceiver
    {

        /// <summary>
        /// Setting of axes along which the character can move.
        /// </summary>
        [SerializeField]
        private bool3 _freezeAxis = new bool3(false, false, false);

        /// <summary>
        /// Possible to push when colliding with a Rigidbody.
        /// </summary>
        public bool Pushable = true;

        [SerializeField, ReadOnlyOnRuntime] 
        private bool _detectCollisions = true;
        
        /// <summary>
        /// FreezeAxis content in Vector3
        /// </summary>
        public Vector3 LockAxis
        {
            get => _lockAxis;
            set => SetFreezeAxis(value.x < 0.5f, value.y < 0.5f, value.z < 0.5f);
        }

        /// <summary>
        /// Setting of axes along which the character can move.
        /// </summary>
        public bool3 FreezeAxis => _freezeAxis;

        /// <summary>
        /// Update freeze position.
        /// </summary>
        /// <param name="x">lock x axis</param>
        /// <param name="y">lock y axis</param>
        /// <param name="z">lock z axis</param>
        public void SetFreezeAxis(bool x, bool y, bool z)
        {
            _lockAxis.x = x ? 0 : 1;
            _lockAxis.y = y ? 0 : 1;
            _lockAxis.z = z ? 0 : 1;
            _freezeAxis.x = x;
            _freezeAxis.y = y;
            _freezeAxis.z = z;
        }

        /// <summary>
        /// Reference to the component for moving the character.
        /// </summary>
        private CharacterController _controller;
        
        /// <summary>
        /// Component for performing pre-calculations.
        /// Required to toggle ON/OFF the component from within.
        /// </summary>
        private EarlyUpdateBrainBase _earlyUpdate;

        private Vector3 _lockAxis = Vector3.one;
        private static readonly Collider[] Colliders = new Collider[5];
        private IGroundContact _groundCheck;
        private bool _hasGroundCheck;

        private void Reset()
        {
            // Update settings such as CharacterController.
            var settings = GetComponent<CharacterSettings>();
            ((ICharacterSettingUpdateReceiver)this).OnUpdateSettings(settings);
        }

        private void Awake()
        {
            // Initialize the parent class.
            Initialize();

            // Collect components.
            GatherComponents();

            // Apply LockAxis.
            SetFreezeAxis(_freezeAxis.x, _freezeAxis.y, _freezeAxis.z);

            _controller.detectCollisions = _detectCollisions;
        }

        /// <summary>
        /// Gather and add a list of components used by <see cref="CharacterBrain"/>.
        /// </summary>
        private void GatherComponents()
        {
            TryGetComponent(out CachedTransform);
            TryGetComponent(out _controller);
            _hasGroundCheck = TryGetComponent(out _groundCheck);
            
            _earlyUpdate = gameObject.AddComponent<EarlyUpdateBrain>();
            
            // Disable the component at the start to prevent it from running immediately.
            _earlyUpdate.enabled = false;
        }

        private void OnEnable()
        {
            // Activate the component for pre-calculation.
            _earlyUpdate.enabled = true;
        }

        private void OnDisable()
        {
            // Deactivate the component for pre-calculation.
            _earlyUpdate.enabled = false;
        }

        private void Update()
        {
            UpdateBrain();
        }


        private void OnValidate()
        {
            // Update the settings to match the Inspector for changes during gameplay.
            SetFreezeAxis(_freezeAxis.x, _freezeAxis.y, _freezeAxis.z);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // If push is disabled, do not perform pushing when colliding.
            if (Pushable == false)
                return;

            // push other character brain.
            if (hit.collider.TryGetComponent(out CharacterBrain brain))
                brain.PushedOtherController(hit.moveDirection * TotalVelocity.magnitude , Settings.Mass);

            // On contact with an object, if the object is operating with a rigidbody, push it out.
            var body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
                return;
            var pushDir = hit.moveDirection;
            body.AddForce(pushDir * Settings.Mass, ForceMode.Force);
        }

        /// <summary>
        /// Push other character brain.
        /// </summary>
        /// <param name="direction">the direction of push</param>
        /// <param name="mass">The character mass</param>
        private void PushedOtherController(Vector3 direction, float mass)
        {
            var max = Mathf.Max(Settings.Mass, mass);
            var rate = (mass / Settings.Mass ) / max ;

            var velocity = Vector3.Scale(direction, new Vector3(1, 0f, 1))  * rate * Time.deltaTime;
            velocity.y = 0.001f; // avoid character controller bugs.

            _controller.Move( velocity);
        }

        /// <summary>
        /// Update the character's position.
        /// </summary>
        /// <param name="total">The final position.</param>
        /// <param name="deltaTime">Delta time.</param>
        protected override void ApplyPosition(in Vector3 total, float deltaTime)
        {
            var totalVelocity = Vector3.Scale(_lockAxis, total);

            
            var velocity = totalVelocity * deltaTime;

            // If GroundCheck is present, correct the position to fit the ground.
            if (_hasGroundCheck && _groundCheck.IsFirmlyOnGround && totalVelocity.y <= 0)
            {
                var distance = _groundCheck.DistanceFromGround;
                velocity -= new Vector3(0, distance, 0);
            }
            
            // If CharacterController is enabled, move within the context of CharacterController.
            // Otherwise, move the character with Transform.
            if (_controller.enabled)
                _controller.Move(velocity);
            else
                CachedTransform.position += velocity;

            CachedTransform.position = BrainUtility.LimitAxis(
                Position, CachedTransform.position, _freezeAxis);
        }
        

        /// <summary>
        /// Apply the character's rotation.
        /// </summary>
        /// <param name="rotation">The final rotation.</param>
        protected override void ApplyRotation(in Quaternion rotation)
        {
            CachedTransform.rotation = rotation;
        }

        /// <summary>
        /// Get the initial position and rotation.
        /// </summary>
        protected override void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            CachedTransform.GetPositionAndRotation(out position, out rotation);
        }

        /// <summary>
        /// Update timing. CharacterBrain updates during the Update phase.
        /// </summary>
        public override UpdateTiming Timing => UpdateTiming.Update;

        /// <summary>
        /// Callback when CharacterSettings is updated.
        /// </summary>
        /// <param name="settings">CharacterSettings.</param>
        void ICharacterSettingUpdateReceiver.OnUpdateSettings(CharacterSettings settings)
        {
            // If the Controller is not set, retrieve it.
            if (_controller == null) TryGetComponent(out _controller);

            // Get the height, center point, and width.
            _controller.height = settings.Height - _controller.skinWidth * 2;
            _controller.center = new Vector3(0, settings.Height * 0.5f + _controller.skinWidth, 0);
            _controller.radius = settings.Radius;
        }

        /// <summary>
        /// Update the position.
        /// In this process, the position is updated immediately and is not affected by other Control or Effect.
        /// </summary>
        /// <param name="position">The new position.</param>
        /// <returns>True if the movement is possible.</returns>
        protected override void SetPositionDirectly(in Vector3 position)
        {
            // It is necessary to temporarily stop CharacterController in order to update the coordinates.
            if (_controller.enabled)
            {
                _controller.enabled = false;
                CachedTransform.position = position;
                _controller.enabled = true;
            }
            else
            {
                // Update the position.
                CachedTransform.position = position;
            }
        }

        /// <summary>
        /// Update the character's rotation.
        /// </summary>
        /// <param name="rotation">The new rotation.</param>
        protected override void SetRotationDirectly(in Quaternion rotation)
        {
            // Rotation = rotation;
            CachedTransform.rotation = rotation;
        }

        protected override void MovePosition(in Vector3 newPosition)
        {
            var position = BrainUtility.LimitAxis(Position, newPosition, _freezeAxis);

            if (_controller.enabled)
            {
                // Since CharacterController is enabled, it cannot be moved by Transform.
                _controller.Move(position - CachedTransform.position);
            }
            else
            {
                CachedTransform.position = position;
            }
        }
    }
}
