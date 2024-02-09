using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Interfaces.Utility;
using Unity.TinyCharacterController.Settings;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// This component manages changes in the camera's perspective.
    /// It updates the direction of the Transform specified by CameraRoot using RotateCamera,
    /// limiting changes in the vertical direction to the angle specified by CameraPitch.
    /// The speed of movement of the camera's perspective is corrected by CameraUserSettings.
    /// When TurnPriority is high, the character will face the direction specified by the camera.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(TpsCameraControl))]
    [DisallowMultipleComponent]
    [RenamedFrom("TinyCharacterController.CameraControl")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Control.TpsCameraControl")]
    public class TpsCameraControl : MonoBehaviour, 
        ITurn, 
        ICameraUpdate, 
        IComponentCondition,
        IUpdateComponent
    {
        /// <summary>
        /// DPI of the assumed screen, for resolution-independent behavior.
        /// </summary>
        private const float TargetDPI = 1f / 800f;
        
        /// <summary>
        /// A Transform to be updated by UpdateCamera.
        /// </summary>
        [RequestField]
        [SerializeField]
        private Transform _cameraRoot;

        /// <summary>
        /// Maximum elevation angle of CameraRoot
        /// </summary>
        [Header("Camera Settings")]         
        [SerializeField, MinMax(-90, 90)]
        private Vector2 _cameraPitch = new Vector2(-40, 40);

        [SerializeField]
        [RequestField]
        private CameraUserSettings _userSettings;

        /// <summary>
        /// Turn priority
        /// </summary>
        [FormerlySerializedAs("_characterTurnPriority")]
        [Header("Character Settings")]
        public int TurnPriority;

        /// <summary>
        /// Turn Speed.
        /// </summary>
        [SerializeField, Range(-1, 50)]
        private int _characterTurnSpeed = -1;
        
        private float _yaw, _pitch;             // Euler values for horizontal and vertical rotation
        private Vector3 _offset;                // Offsetting the initial positions of the character and camera
        private Quaternion _targetRotation;     // The value of the final rotation to face.
        private Vector2 _inputLook;             // Requested camera change value. Mouse delta position.
        private Transform _transform;

        /// <summary>
        /// User settings for running the camera.
        /// </summary>
        public CameraUserSettings UserSettings
        {
            get => _userSettings;
            set => _userSettings = value;
        }
        
        /// <summary>
        /// Priority of turning character.
        /// If this component has the highest priority, the character will face the camera.
        /// </summary>
        int IPriority<ITurn>.Priority => TurnPriority;

        /// <summary>
        /// Turn speed of character.
        /// If this value is -1, the character immediately changes direction.
        /// </summary>
        public int TurnSpeed { get => _characterTurnSpeed; set => _characterTurnSpeed = value; }

        /// <summary>
        /// Yaw Angle.
        /// </summary>
        public float YawAngle => _yaw;

        /// <summary>
        /// Pitch Angle.
        /// </summary>
        public float PitchAngle => _pitch;

        /// <summary>
        /// Horizontal rotation of the camera.
        /// </summary>
        public Quaternion YawRotation => Quaternion.AngleAxis(_yaw, Vector3.up);
        
        /// <summary>
        /// Vertical rotation of the camera.
        /// </summary>
        public Quaternion PitchRotation => Quaternion.AngleAxis(_pitch, Vector3.right);

        /// <summary>
        /// Camera Rotation.
        /// </summary>
        public Quaternion Rotation => _targetRotation;
        
        /// <summary>
        /// Forces the camera to point in the specified direction.
        /// CameraRoot is also updated immediately.
        /// </summary>
        /// <param name="rotation">Direction</param>
        public void ForceUpdateRotation(Quaternion rotation)
        {
            var euler = rotation.eulerAngles;
            _yaw = Mathf.DeltaAngle(0, euler.y);
            _pitch = Mathf.DeltaAngle(0, euler.x);
            _yaw = ClampAngle(_yaw, float.MinValue, float.MaxValue);
            _pitch = ClampAngle(_pitch, _cameraPitch.x, _cameraPitch.y);
            _targetRotation = Quaternion.Euler(_pitch, _yaw, 0);
            
            UpdateCameraRotation();
        }        

        /// <summary>
        /// Update camera orientation.
        /// CameraRoot is updated at the time of Update brain.
        /// </summary>
        /// <param name="inputLook">mouse delta position.</param>
        public void RotateCamera(Vector2 inputLook)
        {
            _inputLook = inputLook;
        }

        
        private void Awake()
        {
            TryGetComponent(out _transform);
            _targetRotation = Quaternion.identity;
            UserSettings = Instantiate(_userSettings);
            _offset = _cameraRoot.position - transform.position;
            
            // Move the camera to the root object.
            // To prevent character orientation changes from interfering with camera orientation changes.
            // if (_cameraRoot != null) 
            //     _cameraRoot.SetParent(null);
        }


        void ICameraUpdate.OnUpdate(float deltaTime)
        {
            UpdateCameraRotation();
        }
        
        /// <summary>
        /// Update Camera Root.
        /// </summary>
        private void UpdateCameraRotation()
        {
            _cameraRoot.rotation = Rotation;
            // _cameraRoot.SetPositionAndRotation(_transform.position + _offset, Rotation);
        }

        private void UpdateTargetRotation(float sensitive)
        {
            // Set the assumed resolution so that the behavior does not change depending on the screen resolution.
            var screenSize = 1980f / Screen.width;
            var input = _inputLook *  screenSize;

            // Update Yaw and Pitch. 
            _yaw += input.x * TargetDPI * (UserSettings.InverseX ? -1 : 1) * sensitive;
            _pitch += input.y * TargetDPI * (UserSettings.InverseY ? -1 : 1) * sensitive;

            // Limit angle.
            _yaw = ClampAngle(_yaw, float.MinValue, float.MaxValue);
            _pitch = ClampAngle(_pitch, _cameraPitch.x, _cameraPitch.y);

            // Calculate final angle
            _targetRotation = Quaternion.Euler(_pitch, _yaw, 0);
            
            // Disable input
            _inputLook = Vector2.zero;
        }

        /// <summary>
        /// Limit angle.
        /// Loop if the angle is greater than 360 degrees or less than -360 degrees.
        /// </summary>
        /// <param name="angle">Current Angle</param>
        /// <param name="min">min angle</param>
        /// <param name="max">max angle</param>
        /// <returns>clamp or looped angle.</returns>
        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }

        void IComponentCondition.OnConditionCheck(List<string> messageList)
        {                
            if( _cameraRoot == null)
                messageList.Add($"{nameof(TpsCameraControl)} : CameraRoot is not set. ({nameof(TpsCameraControl)})");
            if( _userSettings == null)
                messageList.Add($"{nameof(TpsCameraControl)} : UserSettings is not set  ({nameof(TpsCameraControl)})");
        }

        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            using var profiler = new ProfilerScope(nameof(TpsCameraControl));

            var sensitive = Mathf.Pow(UserSettings.MouseSensitivity * 0.05f, 2);
            UpdateTargetRotation( sensitive);
        }

        int IUpdateComponent.Order => Order.Control;
    }
}
