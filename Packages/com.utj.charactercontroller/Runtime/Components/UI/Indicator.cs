using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Unity.TinyCharacterController.Manager;

namespace Unity.TinyCharacterController.UI
{
    /// <summary>
    /// This component that provides utility for visually tracking a specific target on the screen within games and
    /// applications. This class is primarily designed to be attached to UI elements and displays an icon
    /// representing the position of a target object, adjusting the icon's position and state based on whether
    /// the target is visible on the screen or off-screen. It also offers the capability to trigger custom events
    /// in response to changes in the target's visibility. The Indicator class can be used effectively to indicate
    /// the positions of players or objects within a game, especially in 2D or 3D environments.
    /// </summary>
    // [RenamedFrom("Indicator")]
    [DisallowMultipleComponent]
    [AddComponentMenu(MenuList.Ui + nameof(Indicator))]
    [RenamedFrom("TinyCharacterController.Indicator.Indicator")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.UI.Indicator")]
    [RequireComponent(typeof(RectTransform))]
    public class Indicator : ComponentBase
    {
        /// <summary>
        ///     The timing to update the component.
        ///     Set to Update if the character moves during Update timing, or FixedUpdate if it moves during Physics timing.
        /// </summary>
        [SerializeField] private UpdateTiming _updateTiming;

        /// <summary>
        ///     The Transform to track.
        ///     If not set, the Indicator will stop functioning.
        /// </summary>
        [Header("Target to Track")] [SerializeField] private Transform _target;

        /// <summary>
        ///     The offset at which the Indicator is displayed.
        /// </summary>
        public Vector3 Offset;

        /// <summary>
        ///     If true, the Indicator will adjust its position to stay within the screen boundaries.
        /// </summary>
        [Header("Tracking Range")][SerializeField] private bool _isLimitIconRange;

        /// <summary>
        ///     If <see cref="IsLimitIconRange"/> is true, sets the position to adjust the UI.
        ///     For example, setting it to 0.9 will adjust it at 90% of the screen's position.
        /// </summary>
        [SerializeField] private float _bounds = 0.9f;

        /// <summary>
        ///     Event called when the UI is either inside or outside the screen.
        /// </summary>
        public UnityEvent<bool> OnValueChanged = new();

        /// <summary>
        ///     The icon to use when the target is visible.
        /// </summary>
        [Header("Behavior When Off-Screen")] [SerializeField]
        private Transform _onScreenIcon;

        /// <summary>
        ///     The icon to use when the target is not visible.
        /// </summary>
        [SerializeField] private Transform _offScreenIcon;

        /// <summary>
        ///     When <see cref="IsTargetVisible"/> is false, the icon will rotate towards the target.
        /// </summary>
        [SerializeField] private bool _isTurnOffScreenIcon;

        private RectTransform _transform;
        private bool _isInitialized = false;

        /// <summary>
        ///     The setting for the bounds.
        /// </summary>
        public float Bounds => _bounds;

        /// <summary>
        ///     If true, the Indicator will adjust its position to stay within the screen boundaries.
        /// </summary>
        public bool IsLimitIconRange => _isLimitIconRange;

        /// <summary>
        ///     The target is visible on the screen.
        /// </summary>
        public bool IsTargetVisible { get; private set; }

        /// <summary>
        ///     Specify the target's Transform.
        ///     Setting it to Null will stop the Indicator's operation.
        /// </summary>
        [AllowsNull]
        public Transform Target
        {
            get => _target;
            set
            {
                var hasTarget = value != null;
                var isChangedTarget = _target != value;

                _target = value;

                // If the target being tracked by an already active Indicator changes, re-register the unit once.
                if (isChangedTarget && ((IComponentIndex)this).IsRegistered && hasTarget )
                {
                    IndicatorSystem.Unregister(this, _updateTiming);
                    IndicatorSystem.Register(this, _updateTiming);
                }

                enabled = hasTarget;
            }
        }

        /// <summary>
        ///     Update timing for the target.
        /// </summary>
        public UpdateTiming FollowTargetUpdateTiming
        {
            get => _updateTiming;
            set
            {
                // Exit the process if there is no change.
                if (_updateTiming == value)
                    return;

                // If the component is active, unregister it from the system and re-assign it to another timing.
                if (((IComponentIndex)this).IsRegistered)
                {
                    var preTiming = _updateTiming;
                    var newTiming = _updateTiming == UpdateTiming.Update
                        ? UpdateTiming.Update
                        : UpdateTiming.FixedUpdate;

                    IndicatorSystem.Unregister(this, preTiming);
                    IndicatorSystem.Register(this, newTiming);
                }

                _updateTiming = value;
            }
        }

        /// <summary>
        ///     If true, the icon will rotate when it's off-screen.
        /// </summary>
        public bool IsTurnOffscreenIcon => _isTurnOffScreenIcon;

        private void Awake()
        {
            TryGetComponent(out _transform);


        }

        private void OnEnable()
        {
            // If there is no target, immediately stop processing.
            if (InitializeCheck() && _target == null)
            {
                enabled = false;
                return;
            }

            // Register the component.
            IndicatorSystem.Register(this, _updateTiming);

            // Switch between on-screen and off-screen icons.
            UpdateIconDisplay();
        }

        /// <summary>
        /// 動作するためのチェック
        /// </summary>
        /// <returns>動作可能ならばTrue</returns>
        private bool InitializeCheck()
        {
            if (_isInitialized == false)
            {
                var canvas = GetComponentInParent<Canvas>();
                if (canvas == null)
                {
                    Debug.LogError("UI requires Canvas.", gameObject);
                    return false;
                }
                
                if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    Debug.LogError("Canvas is not a ScreenSpaceOverlay.", gameObject);
                    return false;
                }

            }
            _isInitialized = true;
            return _isInitialized;
        }

        private void OnDisable()
        {
            // Unregister the component.
            IndicatorSystem.Unregister(this, _updateTiming);
        }

        private void OnDrawGizmosSelected()
        {
            // Stop processing if there is no target.
            if (_target == null)
                return;

            // Represent a sphere at the target's position including the offset.
            var center = _target.position + Offset;
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(center, 0.5f);

            // Represent a filled sphere.
            Gizmos.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.4f);
            Gizmos.DrawSphere(center, 0.5f);
        }

        private void OnValidate()
        {
            if (((IComponentIndex)this).IsRegistered && IndicatorSystem.IsCreated(_updateTiming))
                IndicatorSystem.GetInstance(_updateTiming)
                    .SetIndicatorData(this, Bounds, Offset, IsTurnOffscreenIcon, IsLimitIconRange);
        }

        /// <summary>
        ///     Update the settings that the Indicator displays.
        /// </summary>
        /// <param name="iconAngle">The angle of the icon.</param>
        /// <param name="isTargetVisible">The UI is displayed outside the screen.</param>
        public void OnUpdate(float iconAngle, bool isTargetVisible)
        {
            // Update properties.
            var isChangeVisible = IsTargetVisible != isTargetVisible;
            IsTargetVisible = isTargetVisible;

            // Apply transform.
            if (_offScreenIcon != null)
                _offScreenIcon.localRotation = Quaternion.AngleAxis(iconAngle, Vector3.forward);;

            // If the display status has changed, update the icon and trigger the event.
            if (isChangeVisible)
            {
                UpdateIconDisplay();
                OnValueChanged.Invoke(IsTargetVisible);
            }
        }

        /// <summary>
        ///     Toggle the visibility of the icon based on <see cref="IsTargetVisible"/>.
        ///     It won't work if <see cref="_isLimitIconRange"/> is false.
        /// </summary>
        private void UpdateIconDisplay()
        {
            if (_isLimitIconRange == false)
                return;

            // Switch the icon.
            if (_onScreenIcon != null)
                _onScreenIcon.gameObject.SetActive(IsTargetVisible);
            if (_offScreenIcon != null)
                _offScreenIcon.gameObject.SetActive(!IsTargetVisible);
        }
    }
}
