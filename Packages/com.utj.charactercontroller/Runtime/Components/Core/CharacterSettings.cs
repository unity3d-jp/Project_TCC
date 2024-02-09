using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.VisualScripting;
using UnityEngine;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Utility;

namespace Unity.TinyCharacterController
{
    /// <summary>
    ///     Gets or sets the configuration required for the <see cref="IBrain" /> and TCC components' behavior.
    /// </summary>
    [SelectionBase]
    [AddComponentMenu(MenuList.MenuBrain + "Character Settings")]
    [RequireInterface(typeof(IBrain))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.CharacterSettings")]
    public class CharacterSettings : MonoBehaviour
    {
        /// <summary>
        ///     Minimum height.
        ///     There may be issues if it's set to 0.
        /// </summary>
        private const float MinHeight = 0.1f;

        /// <summary>
        ///     Minimum radius.
        ///     There may be issues if it's set to 0.
        /// </summary>
        private const float MinRadius = 0.1f;

        /// <summary>
        ///     Minimum mass.
        ///     There may be issues if it's set to 0.
        /// </summary>
        private const float MinMass = 0.001f;

        /// <summary>
        ///     Layer for recognizing terrain colliders.
        /// </summary>
        [Header("Environment Settings")] [SerializeField] [Tooltip("Layer for recognizing terrain colliders")]
        private LayerMask _environmentLayer;

        /// <summary>
        ///     Force applied when colliding with other rigid bodies.
        /// </summary>
        [Header("Character Settings")]
        [SerializeField]
        [Tooltip("Force applied when colliding with other rigid bodies")]
        private float _mass = 1;

        /// <summary>
        ///     Character's height.
        /// </summary>
        [SerializeField] [Tooltip("Character's height")]
        private float _height = 1.4f;

        /// <summary>
        ///     Character's width.
        /// </summary>
        [SerializeField] [Tooltip("Character's width")]
        private float _radius = 0.5f;

        /// <summary>
        ///     List to store Collider components under GameObject.
        /// </summary>
        private readonly List<Collider> _hierarchyColliders = new();

        /// <summary>
        ///     Cached camera.
        /// </summary>
        private Camera _camera;

        /// <summary>
        ///     Cached value of the camera's Transform.
        /// </summary>
        private Transform _cameraTransform;

        /// <summary>
        ///     Returns true if a camera is set.
        /// </summary>
        public bool HasCamera => _camera != null;

        /// <summary>
        ///     Character's width.
        /// </summary>
        public float Radius
        {
            get => _radius;
            set
            {
                // Restrict the width to the minimum value.
                var newValue = Mathf.Max(value, MinRadius);

                // Do nothing if the width hasn't changed.
                if (Mathf.Approximately(_radius, newValue))
                    return;

                _radius = newValue;

                // Update settings of other components.
                UpdateSettings();
            }
        }

        /// <summary>
        ///     Character's height.
        /// </summary>
        public float Height
        {
            get => _height;
            set
            {
                // Restrict the height to the minimum value.
                var newValue = Mathf.Max(value, MinHeight);

                // Do nothing if the height hasn't changed.
                if (Mathf.Approximately(_height, newValue))
                    return;

                _height = newValue;

                // Update settings of other components.
                UpdateSettings();
            }
        }

        /// <summary>
        ///     Layer for recognizing terrain colliders.
        /// </summary>
        public LayerMask EnvironmentLayer => _environmentLayer;

        /// <summary>
        ///     Gets character's camera information.
        ///     Uses Camera.Main if no camera is set.
        /// </summary>
        public Camera CameraMain
        {
            get
            {
                // Return the cached camera if it's already registered.
                if (_camera != null)
                    return _camera;

                ApplyMainCameraTransform();
                return _camera;
            }

            set
            {
                // Update the camera and _cameraTransform.
                _camera = value;
                _cameraTransform = _camera != null ? _camera.transform : null;
            }
        }

        /// <summary>
        ///     Gets the Y-axis rotation of the camera.
        ///     Returns <see cref="Quaternion.identity" /> if no camera exists.
        /// </summary>
        public Quaternion CameraYawRotation => HasCamera
            ? Quaternion.AngleAxis(CameraTransform.eulerAngles.y, Vector3.up)
            : Quaternion.identity;

        /// <summary>
        ///     MainCamera's Transform.
        /// </summary>
        public Transform CameraTransform
        {
            get
            {
                // Get the camera's Transform if already registered.
                if (_cameraTransform != null)
                    return _cameraTransform;

                ApplyMainCameraTransform();

                return _cameraTransform;
            }
        }

        /// <summary>
        ///     Sets the value of mass.
        ///     Used when colliding with or being affected by objects.
        /// </summary>
        public float Mass
        {
            get => _mass;
            set => _mass = value;
        }

        private void Awake()
        {
            // Get a list of components.
            GatherOwnColliders();

            // Update the camera's Transform.
            ApplyMainCameraTransform();
        }

        /// <summary>
        ///     Callback to initialize components when attaching them.
        /// </summary>
        private void Reset()
        {
            // Initialize the layer.
            _environmentLayer = 1 << LayerMask.NameToLayer("Default");
        }

        /// <summary>
        ///     Callback when the component's values change.
        /// </summary>
        private void OnValidate()
        {
            // Ensure values don't go below the minimum.
            _height = Mathf.Max(MinHeight, _height);
            _radius = Mathf.Max(MinRadius, _radius);
            _mass = Mathf.Max(MinMass, _mass);

            UpdateSettings();
        }

        /// <summary>
        ///     Updates the list of colliders owned by the character.
        /// </summary>
        public void GatherOwnColliders()
        {
            _hierarchyColliders.Clear();
            _hierarchyColliders.AddRange(GetComponentsInChildren<Collider>());
        }

        /// <summary>
        ///     Checks whether a collider belongs to the character.
        /// </summary>
        /// <param name="col">The collider to check</param>
        /// <returns>True if the collider belongs to the character</returns>
        public bool IsOwnCollider(Collider col)
        {
            return _hierarchyColliders.Contains(col);
        }

        /// <summary>
        ///     Retrieves the closest RaycastHit excluding the character's own colliders.
        /// </summary>
        /// <param name="hits">List of Raycasts to check</param>
        /// <param name="count">Number of Raycasts</param>
        /// <param name="maxDistance">Maximum distance to check</param>
        /// <param name="closestHit">The closest RaycastHit</param>
        /// <returns>True if any Raycast hit is found</returns>
        public bool ClosestHit(RaycastHit[] hits, int count, float maxDistance, out RaycastHit closestHit)
        {
            var min = maxDistance;
            closestHit = default;
            var isHit = false;

            for (var i = 0; i < count; i++)
            {
                var hit = hits[i];

                // Skip if the current Raycast's distance is greater than the current minimum,
                // or if it belongs to the character's collider list, or if it's null.
                if (hit.distance > min || IsOwnCollider(hit.collider) || hit.collider == null)
                    continue;

                // Update the closest Raycast.
                min = hit.distance;
                closestHit = hit;

                // Set to true if at least one closest Raycast is found.
                isHit = true;
            }

            return isHit;
        }

        /// <summary>
        ///     Updates <see cref="Camera.main" /> settings for <see cref="_camera" /> and <see cref="_cameraTransform" />.
        /// </summary>
        private void ApplyMainCameraTransform()
        {
            // Get objects with the MainCamera tag.
            _camera = Camera.main;

            // Update the CameraTransform if a camera is acquired.
            if (_camera != null && _cameraTransform == null)
                _cameraTransform = _camera.transform;
        }

        /// <summary>
        ///     Converts player input direction to world space direction on the screen.
        /// </summary>
        /// <param name="input">
        ///     Player's input. x-axis represents left-right on the screen, y-axis represents forward-backward on
        ///     the screen.
        /// </param>
        /// <returns>Movement direction</returns>
        public Vector3 PlayerInputToWorldSpaceDirection(Vector2 input)
        {
            return CameraYawRotation * new Vector3(input.x, 0, input.y);
        }

        /// <summary>
        ///     Updates components with <see cref="ICharacterSettingUpdateReceiver" />.
        /// </summary>
        private void UpdateSettings()
        {
            var controls = ListPool<ICharacterSettingUpdateReceiver>.New();

            GetComponents(controls);
            foreach (var control in controls)
                control.OnUpdateSettings(this);

            ListPool<ICharacterSettingUpdateReceiver>.Free(controls);
        }
    }
}