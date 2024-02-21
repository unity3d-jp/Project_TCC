using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    /// <summary>
    /// The TinyCharacterController is the fundamental component that drives the character.
    /// It gathers information from <see cref="IMove"/> and <see cref="ITurn"/> components within the same object and applies them to the character.
    /// If there is a need for special processing for the highest priority, <see cref="IMove"/> and <see cref="ITurn"/> are used.
    /// The specific method for reflecting position and rotation is determined by the inherited component. For example,
    /// if it's <see cref="TinyCharacterController.Brain.TransformBrain"/>, it uses <see cref="UnityEngine.Transform.position"/>,
    /// while <see cref="TinyCharacterController.Brain.RigidbodyBrain"/> uses <see cref="UnityEngine.Rigidbody.position"/>.
    /// Forcibly updating the position is done using <see cref="IWarp"/> or <see cref="Warp(UnityEngine.Vector3)"/>.
    /// The Position and Rotation of the Transform are cached and reused.
    /// Components that inherit from this component should add <see cref="IEarlyUpdateComponent"/>.
    /// This allows users to insert their own processing between the character's pre-computation (Check, Effect) and character reflection (Brain).
    /// </summary>
    public abstract class BrainBase : MonoBehaviour, 
        IWarp, IBrain,
        IEarlyUpdateComponent, ITransform
    {
        private readonly MoveManager _moveManager = new();
        private readonly TurnManager _turnManager = new();
        private readonly EffectManager _effectManager = new();
        private readonly UpdateComponentManager _updateComponentManager = new();
        private readonly CameraManager _cameraManager = new();
        private readonly CollisionManager _collisionManager = new();
        private readonly WarpManager _warpManager = new();
        
        protected CharacterSettings Settings;
        protected Transform CachedTransform;

        /// <summary>
        /// Warp the character.
        /// </summary>
        /// <param name="position">New position.</param>
        /// <param name="direction">New rotation. If Vector3.zero, maintain current orientation.</param>
        public void Warp(Vector3 position, Vector3 direction)
        {
            // If Direction is vector3.zero, maintain current orientation
            var rotation = direction != Vector3.zero 
                ? Quaternion.LookRotation(direction) : CachedTransform.rotation;
            _warpManager.SetPositionAndRotation(position, rotation);
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// Warp the character.
        /// </summary>
        /// <param name="position">New position</param>
        /// <param name="rotation">new rotation.</param>
        public void Warp(Vector3 position, Quaternion rotation)
        {
            _warpManager.SetPositionAndRotation(position, rotation);
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// Update position.
        /// Movement by warp takes precedence over <see cref="IMove"/>.
        /// </summary>
        /// <param name="position">New position.</param>
        [RenamedFrom("SetPosition")]
        public void Warp(Vector3 position)
        {
            _warpManager.SetPosition(position);
            Position = position;
        }

        /// <summary>
        /// Update rotation.
        /// Movement by warp takes precedence over <see cref="ITurn"/>.
        /// </summary>
        /// <param name="rotation">new rotation.</param>
        [RenamedFrom("SetRotation")]
        public void Warp(Quaternion rotation)
        {
            _warpManager.SetRotation(rotation);
            Rotation = rotation;
        }

        /// <summary>
        /// Warp to take into account the movement of the midpoint.
        /// Warp behavior depends on Brain.
        /// </summary>
        /// <param name="position">New coordinates</param>
        public void Move(Vector3 position)
        {
            _warpManager.Move(position);
            Position = position;
        } 

        protected void Initialize()
        {
            var go = gameObject;
            _cameraManager.Initialize(go);
            _updateComponentManager.Initialize(go);
            _moveManager.Initialize(go);
            _turnManager.Initialize(go, this);
            _effectManager.Initialize(go);
            _collisionManager.Initialize(go);
            
            // gather all components.
            TryGetComponent(out CachedTransform);
            TryGetComponent(out Settings);
        }
        
        public abstract UpdateTiming Timing { get; }
        
        /// <summary>
        /// Current character movement speed by Control.
        /// If there is no <see cref="IMove"/> greater than 0, the value is 0.
        /// </summary>
        [RenamedFrom("Speed")]
        public float CurrentSpeed => _moveManager.CurrentSpeed;

        /// <summary>
        /// Rotation speed of the character by Rotation.
        ///  If there is no <see cref="ITurn"/> greater than 0, the value is 0.
        /// </summary>
        public int TurnSpeed => _turnManager.CurrentTurn?.TurnSpeed ?? 0;

        /// <summary>
        /// Character orientation in world space.
        /// </summary>
        public float YawAngle => _turnManager.TargetYawAngle;

        /// <summary>
        /// Local vector for the direction the character is facing
        /// </summary>
        public Vector3 LocalVelocity => Quaternion.Inverse(CachedTransform.rotation) * ControlVelocity;

        /// <summary>
        /// Movement vector of the character in world space.
        /// </summary>
        [RenamedFrom("ControllableVelocity")]
        public Vector3 ControlVelocity => _moveManager.Velocity;

        /// <summary>
        /// Additional movement vectors that are added.
        /// For example, gravity or impact.
        /// </summary>
        [RenamedFrom("AdditionalVelocity")]
        public Vector3 EffectVelocity => _effectManager.Velocity;
        
        /// <summary>
        /// The vector that is the sum of the character's movement vector and the additive movement vector.
        /// </summary>
        public Vector3 TotalVelocity { get; private set; }

        /// <summary>
        /// Difference between the current direction and the target direction
        /// </summary>
        public float DeltaTurnAngle => _turnManager.DeltaTurnAngle;

        /// <summary>
        /// Update information in Brain.<br/>
        /// </summary>
        protected void UpdateBrain()
        {
            if (HasCameraCheck() == false)
                return;

            // If executed at the timing of FixedUpdate, deltaTime returns the value of FixedUpdate.
            var deltaTime = Time.deltaTime;

            // Update coordinates
            _updateComponentManager.Process(deltaTime);

            // update highestPriority
            _moveManager.UpdateHighestMoveControl(deltaTime);
            _turnManager.UpdateHighestTurnControl(deltaTime);

            // update velocity and angle.
            _effectManager.CalculateVelocity();
            _moveManager.CalculateVelocity();
            _turnManager.CalculateAngle(deltaTime);

            TotalVelocity = _moveManager.Velocity + _effectManager.Velocity;

            // Update the position.
            if (_warpManager.WarpedPosition)
            {
                if( _warpManager.IsMove)
                    MovePosition(_warpManager.Position);
                else
                    SetPositionDirectly(_warpManager.Position);

                _effectManager.ResetVelocity();
            }else {
                ApplyPosition(TotalVelocity, deltaTime);
            }

            // Update the direction.
            if (_warpManager.WarpedRotation)
                SetRotationDirectly(_warpManager.Rotation);
            else if( _turnManager.HasHighestPriority)
                ApplyRotation(Quaternion.AngleAxis(_turnManager.NextYawAngle, Vector3.up));

            // Update the camera's position and direction.
            // To prevent jitter, process the camera after updating the character's position.
            _cameraManager.Process(deltaTime);
            
            _warpManager.ResetWarp();
        }

        private bool HasCameraCheck()
        {
            if (Settings.HasCamera == false)
            {
                Debug.LogWarning("Camera not found", gameObject);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply final position
        /// </summary>
        /// <param name="totalVelocity">Current acceleration</param>
        /// <param name="deltaTime">Delta time</param>
        protected abstract void ApplyPosition(in Vector3 totalVelocity, float deltaTime);

        /// <summary>
        /// apply rotation.
        /// </summary>
        /// <param name="rotation">set rotation</param>
        protected abstract void ApplyRotation(in Quaternion rotation);

        /// <summary>
        /// Move the position of the character to the <see cref="newPosition"/>.
        /// Unlike <see cref="Warp(UnityEngine.Vector3)"/>, it is affected by other vectors.
        /// </summary>
        /// <param name="newPosition">new position</param>
        /// <returns>move success</returns>
        protected abstract void SetPositionDirectly(in Vector3 newPosition);

        /// <summary>
        /// Turn the character to the <see cref="newRotation"/>.
        /// Unlink <see cref="Warp(UnityEngine.Quaternion)"/>, it is affected by other turn.
        /// </summary>
        /// <param name="newRotation">new rotation.</param>
        protected abstract void SetRotationDirectly(in Quaternion newRotation);

        /// <summary>
        /// Move the character to the <see cref="newPosition"/>.
        /// Unlink <see cref="Warp(UnityEngine.Vector3)"/>, it is affected by other move.
        /// </summary>
        /// <param name="newPosition">new position</param>
        protected abstract void MovePosition(in Vector3 newPosition);
        
        /// <summary>
        /// Cache Position and Rotation for each inherited Brain.
        /// </summary>
        protected abstract void GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
        
        
        IMove IBrain.CurrentMove => _moveManager.CurrentMove;
        ITurn IBrain.CurrentTurn => _turnManager.CurrentTurn;
        
        
        // ----
        // todo : Separate the following code as CachedTRS class
        // Normally, the caching and updating of coordinates should be separated in a form like CachedPositionAndRotation,
        // but due to time constraints, we will integrate them.
        
        protected Quaternion Rotation;
        protected Vector3 Position;
        
        void IEarlyUpdateComponent.OnUpdate(float deltaTime)
        {
            GetPositionAndRotation(out Position, out Rotation);
        }

        int IEarlyUpdateComponent.Order => Order.PrepareEarlyUpdate;

        Vector3 ITransform.Position
        {
            get => Position;
            set
            {
                SetPositionDirectly(value);
                Position = value;
            }
        }

        Quaternion ITransform.Rotation
        {
            get => Rotation;
            set
            {
                SetRotationDirectly(value);
                Rotation = value;
            }
        }
    }
}