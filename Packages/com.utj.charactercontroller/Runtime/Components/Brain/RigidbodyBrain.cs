using Unity.Mathematics;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Utility;
using UnityEngine;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Core;

namespace Unity.TinyCharacterController.Brain
{
    /// <summary>
    /// This brain operates using a <see cref="UnityEngine.Rigidbody"/>.
    /// It is a Brain that operates with a <see cref="UnityEngine.CapsuleCollider"/> and <see cref="UnityEngine.Rigidbody"/>.
    /// The character is positioned at a raised position determined by the <see cref="_stepHeight"/> value and stops at a position with a margin defined by the <see cref="_skinWidth"/> value during movement.
    /// The height and width of the <see cref="UnityEngine.CapsuleCollider"/> are determined by <see cref="CharacterSettings.Height"/> and <see cref="CharacterSettings.Radius"/>.
    /// To function properly, it requires <see cref="IGravity"/> and <see cref="IGroundContact"/> for its operations.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(Order.UpdateBrain)]
    [AddComponentMenu(MenuList.MenuBrain + "Rigidbody Brain")]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CharacterSettings))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireInterface(typeof(IGravity))]
    [RequireInterface(typeof(IGroundContact))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Brain.RigidbodyBrain")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.RigidbodyBrain")]
    public class RigidbodyBrain : BrainBase, 
        ICharacterSettingUpdateReceiver
    {
        /// <summary>
        /// Vectors along which the character can move
        /// </summary>
        [SerializeField]
        private bool3 _freezeAxis = new bool3(false, false, false);
        
        /// <summary>
        /// Width to be set between character and wall
        /// </summary>
        [SerializeField, Range(0, 1f)] 
        private float _skinWidth = 0.08f;

        /// <summary>
        /// Height of steps that the character can climb over
        /// </summary>
        [SerializeField, Range(0.01f, 1f)] 
        private float _stepHeight = 0.2f;

        // Components
        private IGroundContact _groundCheck;
        private Rigidbody _rigidbody;
        private EarlyUpdateBrainBase _earlyUpdate;
        private Vector3 _lockAxis = Vector3.one;

        // Field
        private static readonly RaycastHit[] RaycastHits = new RaycastHit[5];

        /// <summary>
        /// FreezeAxis content in Vector3
        /// </summary>
        public Vector3 LockAxis
        {
            get => _lockAxis;
            set => SetFreezeAxis(value.x > 0.5f, value.y > 0.5f, value.z > 0.5f);
        }

        /// <summary>
        /// Setting of axes along which the character can move.
        /// </summary>
        public bool3 FreezeAxis => _freezeAxis;

        /// <summary>
        /// Update freeze position.
        /// This setting is reflected in <see cref="_rigidbody"/>.
        /// </summary>
        /// <param name="x">lock x axis</param>
        /// <param name="y">lock y axis</param>
        /// <param name="z">lock z axis</param>
        public void SetFreezeAxis(bool x, bool y, bool z)
        {
            // Update values used in the inspector
            _freezeAxis.x = x;
            _freezeAxis.y = y;
            _freezeAxis.z = z;
            
            // Update values used in calculations
            _lockAxis.x = x ? 0 : 1;
            _lockAxis.y = y ? 0 : 1;
            _lockAxis.z = z ? 0 : 1;
            
            // Update Rigidbody behavior based on RigidbodyBrain settings
            var initialConstraints = RigidbodyConstraints.FreezeRotation;
            if(_freezeAxis.x)
                initialConstraints |= RigidbodyConstraints.FreezePositionX;
            if(_freezeAxis.y)
                initialConstraints |= RigidbodyConstraints.FreezePositionY;
            if(_freezeAxis.z)
                initialConstraints |= RigidbodyConstraints.FreezePositionZ; 
            _rigidbody.constraints = initialConstraints;
        }
        
        
        private void Reset()
        {
            // Collect components used in calculations.
            // Do not use caching as this method might be called outside of runtime.
            TryGetComponent(out Rigidbody rig);
            TryGetComponent(out CharacterSettings settings);
            TryGetComponent(out CapsuleCollider col);
            
            // Update Rigidbody properties
            rig.constraints = RigidbodyConstraints.FreezeRotation;
            rig.useGravity = false;
            rig.drag = 5;
            rig.mass = settings.Mass;

            // Update collider shape
            UpdateColliderSettings(col, settings);
        }

        private void OnValidate()
        {
            // Collect components used in calculations.
            // Do not use caching as this method might be called outside of runtime.
            TryGetComponent(out CharacterSettings settings);
            TryGetComponent(out CapsuleCollider col);
            TryGetComponent(out _rigidbody);

            // Update settings outside of components
            UpdateColliderSettings( col, settings);
            
            // Update the axis
            SetFreezeAxis(_freezeAxis.x, _freezeAxis.y, _freezeAxis.z);
        }

        private void Awake()
        {
            // Collect components
            GatherComponents();
            
            Initialize();
            
            // Add a component that aggregates pre-processing
            // The initial status is set to disabled to account for the possibility of the Brain being disabled.
            _earlyUpdate = gameObject.AddComponent<EarlyFixedUpdateBrain>();
            _earlyUpdate.enabled = false; 
            
            // Update FreezeAxis
            SetFreezeAxis(_freezeAxis.x, _freezeAxis.y, _freezeAxis.z);
        }

        private void OnEnable()
        {
            // Activate TCC-related components
            _earlyUpdate.enabled = true;
        }

        private void OnDisable()
        {
            // Deactivate TCC-related components
            _earlyUpdate.enabled = false;
        }

        private void FixedUpdate()
        {
            UpdateBrain();
        }

        private void Update()
        {
            // Apply Transform from rigidbody.　If this process is not included, the character will leave
            // an afterimage depending on the timing of the warp.
            // Due to calculation order issues, it must be run after Update and before LateUpdate.
            CachedTransform.position = _rigidbody.position;
            CachedTransform.rotation = _rigidbody.rotation;
        }

        /// <summary>
        /// Get components.
        /// </summary>
        private void GatherComponents()
        {
            TryGetComponent(out _groundCheck);
            TryGetComponent(out _rigidbody);
        }

        /// <summary>
        /// Update collider settings based on <see cref="settings"/>.
        /// </summary>
        /// <param name="caps">Capsule collider size</param>
        /// <param name="settings">Character settings</param>
        private static void UpdateColliderSettings(CapsuleCollider caps, CharacterSettings settings)
        {
            caps.radius = settings.Radius;
            caps.height = settings.Height;
            caps.center = new Vector3(0, settings.Height / 2 , 0);
        }


        /// <summary>
        /// Get the character's head position based on the starting frame's coordinates.
        /// Used for capsule calculations.
        /// </summary>
        private Vector3 HeadPosition => Position + new Vector3(0, Settings.Height - Settings.Radius , 0);

        /// <summary>
        /// Get the character's foot position based on the starting frame's coordinates.
        /// Used for capsule calculations.
        /// </summary>
        private Vector3 FootPosition => Position + new Vector3(0,  Settings.Radius , 0);

        /// <summary>
        /// Set the final character position.
        /// </summary>
        /// <param name="total">Final movement vector</param>
        /// <param name="deltaTime">Delta time</param>
        protected override void ApplyPosition(in Vector3 total, float deltaTime)
        {
            // var totalVelocity = WallCorrection(total);
            // totalVelocity = Vector3.Scale(_lockAxis, totalVelocity) ;
            
            var totalVelocity = Vector3.Scale(_lockAxis, total) ;
            
            // Get the movement vector on the XZ plane
            var horizontal = totalVelocity;
            horizontal.y = 0;
            
            var lastPosition = Position;

            // Overcome obstacles based on FootStep
            lastPosition += CalculateFootStep( lastPosition, horizontal, deltaTime);
            
            // If grounded, adjust position to adhere to the ground
            lastPosition += FitGround(totalVelocity, lastPosition);
            
            // If there's a wall in the movement direction, adjust the position to avoid penetration
            lastPosition += CalculateSkinWidth(lastPosition, horizontal);

            // Apply position.

            // _rigidbody.MovePosition( lastPosition );
            _rigidbody.position = lastPosition;
            _rigidbody.velocity = totalVelocity ;
        }


        /// <summary>
        /// Update the final character orientation.
        /// </summary>
        /// <param name="rotation">Character orientation</param>
        protected override void ApplyRotation(in Quaternion rotation)
        {
            _rigidbody.MoveRotation(rotation);
        }

        /// <summary>
        /// Calculate the character's allowable movement width based on the value of <see cref="_skinWidth"/>.
        /// </summary>
        /// <param name="position">Starting coordinate for calculations</param>
        /// <param name="direction">Direction of assessment</param>
        /// <returns>Correction offset</returns>
        private Vector3 CalculateSkinWidth(in Vector3 position, in Vector3 direction)
        {
            if (direction == Vector3.zero)
                return Vector3.zero;
            
            var footPosition = position + new Vector3(0, Settings.Radius, 0);
            var headPosition = position + new Vector3(0, Settings.Height - Settings.Radius, 0);
            var offset = new Vector3(0, _stepHeight, 0);
            
            // Check the destination and consider it as terrain if there is terrain
            var count = Physics.CapsuleCastNonAlloc(
                footPosition + offset, headPosition, Settings.Radius, direction.normalized,
                RaycastHits, _skinWidth, Settings.EnvironmentLayer,
                QueryTriggerInteraction.Ignore);

            // No contact, no correction needed
            if (count == 0)
                return Vector3.zero;

            // Get the farthest position hit, excluding your own Collider
            // Return Vector3.zero if there is no Collider
            if (GetFarthestHit(RaycastHits, count, out var hit))
            {
                // Move the character away from the reflection position by the amount of SkinWidth
                // Use LockAxis settings for the reflection
                var inverseVelocity = hit.normal * (_skinWidth - hit.distance);
                
                // Check _lockAxis and correct the non-movable direction
                inverseVelocity = Vector3.Scale(_lockAxis, inverseVelocity);
                return inverseVelocity;
            }
            else
            {
                // No correction needed
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Get the farthest Raycast hit.
        /// </summary>
        /// <param name="raycastHits">List of RayCasts to check</param>
        /// <param name="count">Number of RayCasts</param>
        /// <param name="result"></param>
        private bool GetFarthestHit(in RaycastHit[] raycastHits, int count, out RaycastHit result)
        {
            // Get the smallest assessment distance considering multiple hits
            var distance = 0f;
            result = default(RaycastHit);
            var hasHit = false;

            // Get the largest coordinates
            for (var i = 0; i < count; i++)
            {
                var hit = raycastHits[i];
                if (hit.distance < distance || Settings.IsOwnCollider(raycastHits[i].collider))
                    continue;

                distance = hit.distance;
                result = raycastHits[i];
                hasHit = true;
            }

            return hasHit;
        }

        /// <summary>
        /// If grounded and falling, adjust the position to fit the ground.
        /// </summary>
        /// <param name="velocity">Vector</param>
        /// <param name="position">Current coordinate</param>
        /// <returns>Correction offset</returns>
        private Vector3 FitGround(in Vector3 velocity, in Vector3 position)
        {
            // If grounded and falling, adjust to the position on the ground
            if (_groundCheck.IsFirmlyOnGround && velocity.y < 0)
            {
                return new Vector3(0, -_groundCheck.DistanceFromGround, 0);
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Overcome obstacles based on the value of <see cref="_stepHeight"/>.
        /// </summary>
        /// <param name="position">Current coordinate</param>
        /// <param name="velocity">Parallel vector</param>
        /// <param name="deltaTime">Delta time</param>
        /// <returns>Correction offset</returns>
        private Vector3 CalculateFootStep(in Vector3 position, in Vector3 velocity, float deltaTime)
        {
            // Cast a ray from the position at the radius of the character to detect steps.
            var distance = velocity.magnitude * deltaTime + Settings.Radius;
            var origin = position + velocity.normalized * distance;
            var ray = new Ray(origin + new Vector3(0, _stepHeight, 0), Vector3.down);
            if (Physics.Raycast(ray, out var hit, _stepHeight, 
                    Settings.EnvironmentLayer, QueryTriggerInteraction.Ignore) == false)
            {
                return Vector3.zero;
            }

            // Compare the current slope with the height of the step,
            // and adjust the position if the step is steeper than the slope.
            var offset = _stepHeight - hit.distance;
            var groundAngle = Vector3.Angle(Vector3.up, _groundCheck.GroundSurfaceNormal);
            var targetAngle = Mathf.Atan(offset / Settings.Radius) * Mathf.Rad2Deg;

            return targetAngle > groundAngle ? new Vector3(0, offset, 0) : Vector3.zero;
        }
        
        /// <summary>
        /// Cache the coordinates.
        /// </summary>
        /// <param name="position">Coordinate</param>
        /// <param name="rotation">Rotation</param>
        protected override void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            position = _rigidbody.position;
            rotation = _rigidbody.rotation;
        }
        
        /// <summary>
        /// Component update timing.
        /// </summary>
        public override UpdateTiming Timing => UpdateTiming.FixedUpdate;
        
        /// <summary>
        /// If there is an obstacle at the destination,
        /// the direction of movement is corrected.
        /// </summary>
        /// <param name="velocity">current direction</param>
        /// <returns>Corrected direction</returns>
        private Vector3 WallCorrection(Vector3 velocity)
        {
            // If not moving, exit the process
            if (velocity == Vector3.zero)
                return velocity;

            var deltaTime = Time.fixedDeltaTime;
            var moveDirection = velocity.normalized;
            var speed = velocity.magnitude;
            
            // Distance that Raycast assesses
            // Character width and play + movement distance
            var distance = (Settings.Radius + _skinWidth) + (speed * deltaTime) ;

            // Adjust position to avoid stepping on foot detection
            var stepHeight = new Vector3(0, _stepHeight, 0);
            var isHit = Physics.CapsuleCast(FootPosition + stepHeight, HeadPosition, Settings.Radius,
                moveDirection, out var hit, distance, Settings.EnvironmentLayer, QueryTriggerInteraction.Ignore);
            
            // Correct the direction
            var result = isHit ? Vector3.ProjectOnPlane(velocity, hit.normal) : velocity;
            
            return result;
        }

        /// <summary>
        /// Called when CharacterSettings values change.
        /// </summary>
        /// <param name="settings">Updated Settings</param>
        void ICharacterSettingUpdateReceiver.OnUpdateSettings(CharacterSettings settings)
        {
            var capsuleCollider = GetComponent<CapsuleCollider>();
            var rig = GetComponent<Rigidbody>();

            // Update values
            UpdateColliderSettings(capsuleCollider, settings);
            rig.mass = settings.Mass;
        }

        /// <summary>
        /// Update the position of the RigidbodyBrain.
        /// Unlike Warp, it is immediately reflected and affected by Control and Effect.
        /// </summary>
        /// <param name="position">new position</param>
        /// <returns>can warp</returns>
        protected override void SetPositionDirectly(in Vector3 position)
        {
            // Update the Rigidbody's coordinates
            _rigidbody.MovePosition(position);
        }

        /// <summary>
        /// Update the orientation of the RigidbodyBrain.
        /// Unlike Warp, it is immediately reflected and affected by Control and Effect.
        /// </summary>
        /// <param name="rotation">New orientation</param>
        protected override void SetRotationDirectly(in Quaternion rotation)
        {
            // Update rigidbody coordinates
            _rigidbody.rotation = rotation;
        }

        protected override void MovePosition(in Vector3 newPosition)
        {
            var position = BrainUtility.LimitAxis(Position, newPosition, _freezeAxis);

            _rigidbody.Move(position, Rotation);
        }
    }
}
