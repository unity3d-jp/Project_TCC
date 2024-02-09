using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Interfaces.Utility;
using Unity.TinyCharacterController.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// Retrieve the object located at the center of the screen.
    /// Continuously check every frame as long as the component is enabled, and obtain information such as the target object and the distance to the target.
    /// If the object is visible on the screen but not visible from the character's point of view, prioritize the character's perspective.<br/>
    ///
    /// This component performs detection every frame. If you want to stop the detection, please disable the component.
    /// </summary>
    [AddComponentMenu(MenuList.MenuCheck + nameof(AimCheck))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.AimCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.AimCheck")]
    public class AimCheck : MonoBehaviour, 
        IComponentCondition, 
        IEarlyUpdateComponent, 
        IAimCheck
    {
        private static readonly RaycastHit[] Hits = new RaycastHit[30];
        
        /// <summary>
        /// The visibility check includes the character's perspective. If it is false, it only performs the check from the camera's perspective.
        /// </summary>
        [FormerlySerializedAs("_isUseAimParallax")] 
        public bool IsUseAimParallax = true;

        /// <summary>
        /// The starting point from which the ray is casted by AimCheck.
        /// </summary>
        [Tooltip("The starting point from which the ray is casted by AimCheck."), SerializeField, RequestField]
        private Transform _origin;

        /// <summary>
        /// he target point on the screen. (0.5, 0.5) corresponds to the center of the screen.
        /// </summary>
        [FormerlySerializedAs("_aimViewportPoint")] [Tooltip("The target point on the screen. (0.5, 0.5) corresponds to the center of the screen.")][SerializeField] 
        private Vector2 _aimTargetViewportPoint = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// In addition to the terrain detection in CharacterSettings, it also includes the contactable layers.
        /// For example, if Default is set in CharacterSettings, Enemy and Player can be added to AdditionalHitMask.
        /// </summary>
        [Tooltip("In addition to the terrain detection in CharacterSettings, it also includes the contactable layers. For example, if Default is set in CharacterSettings, Enemy and Player can be added to AdditionalHitMask."), SerializeField]
        private LayerMask _additionalHitMask;

        /// <summary>
        /// The maximum distance that can be detected by AimCheck.
        /// </summary>
        [Tooltip("The maximum distance that can be detected by AimCheck."), SerializeField]
        private int _maxRange = 30;

        /// <summary>
        /// The maximum distance that can be detected by AimCheck.
        /// </summary>
        public int MaxRange
        {
            get => _maxRange;
            set => _maxRange = value;
        }
        
        /// <summary>
        /// Invoke Unity Event when the object ahead of the viewpoint changes.
        /// </summary>
        [FormerlySerializedAs("onChangeAimedGameObject")] 
        public UnityEvent OnChangeAimedGameObject;
        
        public bool IsChangeAImedObject { get; private set; }

        private CharacterSettings _characterSettings;
        
        /// <summary>
        /// Determines whether there is a collider that can be recognized by AimCheck in the center of the screen.
        /// </summary>
        public bool IsHit { get; private set; } = false;
        
        /// <summary>
        /// The starting point from which AimCheck casts a ray.
        /// </summary>
        public Vector3 Origin
        {
            get => _origin.position;
            set => _origin.position = value;
        }

        /// <summary>
        /// The point on viewport that the character aims at.
        /// Aiming for the center of the screen is (0.5, 0.5).
        /// </summary>
        public Vector2 AimTargetViewportPoint
        {
            get => _aimTargetViewportPoint;
            set => _aimTargetViewportPoint = value;
        }

        /// <summary>
        /// The point on the screen that the character aims at.
        /// Aiming for the center of the screen is (Screen.width / 2, Screen.height / 2).
        /// </summary>
        public Vector2 AimTargetScreenPoint
        {
            get => new Vector2(_aimTargetViewportPoint.x * Screen.width, _aimTargetViewportPoint.y * Screen.height);
            set
            {
                _aimTargetViewportPoint.x = value.x / Screen.width;
                _aimTargetViewportPoint.y = value.y / Screen.height;
            }
        }
        
        /// <summary>
        ///The direction in which the ray is casted by AimCheck.
        /// </summary>
        public Vector3 Direction { get; private set; }
        
        /// <summary>
        /// The contact point when <see cref="IsHit">IsHit</see>  is true.
        /// If <see cref="IsHit">IsHit</see> is false, it returns the recalculated position of the line of sight.
        /// </summary>
        public Vector3 Point { get; private set; }
        
        /// <summary>
        /// Distance from <see cref="Point"/>Point<see/>
        /// </summary>
        public float Distance { get; private set; }
        
        /// <summary>
        /// The normal of the contact surface.
        /// </summary>
        public Vector3 Normal { get; private set; }
        
        /// <summary>
        /// The GameObject with a Collider located at the AimCheck's line of sight. If IsHit is false, it is null.
        /// </summary>
        public GameObject AimGameObject { get; private set; }
        
        /// <summary>
        /// The previously seen GameObject.
        /// </summary>
        public GameObject PreAimGameObject { get; private set; }

        /// <summary>
        /// The screen coordinates of the contact point.
        /// This can be used, for example, when IsParallax is true and the character's line of sight is obstructed,
        /// to display UI at the obstruction position.
        /// </summary>
        public Vector2 ScreenPosition => RectTransformUtility.WorldToScreenPoint(_characterSettings.CameraMain, Point);

        public bool HitCheck(Vector2 center, out Vector3 point, out Vector3 normal, out float distance,
            out GameObject hitGameObject, out Vector3 direction)
        {
            using var profiler = new ProfilerScope("AimCheck.HitCheck");

            // It casts a ray from center towards AimTargetViewportPoint of the screen.
            var isHitFromCamera = RayCastCheckFromCenterView(
                new Vector3(center.x, center.y, 1),
                out var hitPoint, out var hitDistance, 
                out var hitNormal, out var aimDirection,  out var col);

            if (IsUseAimParallax)
            {
                // It casts a ray from the Origin of the character towards the contact object.
                var isHitFromCharacter = RayCastCheckFromOrigin(hitPoint, out var hitFromPoint, out direction, out distance);
            
                point = isHitFromCharacter ? hitFromPoint.point : hitPoint;
                normal = isHitFromCharacter ? hitFromPoint.normal : direction;
                distance = isHitFromCharacter ? hitFromPoint.distance : hitDistance;
                hitGameObject = isHitFromCharacter ? hitFromPoint.collider.gameObject : null;

                return isHitFromCharacter;
            }
            else
            {
                // It uses only the detection from the center of the camera.
                point = hitPoint;
                distance = Vector3.Distance(_origin.position, hitPoint);
                normal = hitNormal;
                hitGameObject = isHitFromCamera ? col.gameObject : null;
                direction = aimDirection;
                return isHitFromCamera;
            }
        }
        
        /// <summary>
        /// This API immediately performs the detection of the object in AimCheck's line of sight.
        /// This process is normally automatically executed by the component.
        /// The result of calling this API directly is not stored in the component.
        /// </summary>
        /// <param name="point">contact point</param>
        /// <param name="normal">surface normal</param>
        /// <param name="distance">distance from contact point</param>
        /// <param name="hitGameObject">contact object</param>
        /// <param name="direction">direction to reach the contact point</param>
        /// <returns>接触の有無</returns>
        public bool HitCheck(out Vector3 point, out Vector3 normal, out float distance, out GameObject hitGameObject, out Vector3 direction)
        {
            return HitCheck(_aimTargetViewportPoint, 
                out point, out normal, out distance, out hitGameObject, out direction);
        }

        private LayerMask CombinedLayerMask =>  _characterSettings.EnvironmentLayer | _additionalHitMask;


        private void Awake()
        {
            TryGetComponent(out _characterSettings);
        }
        
        private void OnDisable()
        {
            Direction = Vector3.zero;
            Normal = Vector3.zero;
            Point = Vector3.zero;
            Distance = 0;
            IsHit = false;
            PreAimGameObject = null;
            AimGameObject = null;
            IsChangeAImedObject = false;
        }
        
        void IEarlyUpdateComponent.OnUpdate(float deltaTime)
        {
            if (enabled == false)
                return;
            
            using var profiler = new ProfilerScope(nameof(AimCheck));

            
            // The visibility check determines the presence of objects in front of the line of sight
            // and stores the result of the check, including Direction, Normal, Point, and Distance, in properties.
            IsHit = HitCheck(out var point, out var normal, out var distance, 
                out var aimingGameObject, out var direction);
            
            Direction = direction.normalized;
            Normal = normal;
            Point = point;
            Distance = distance;
            
            // If the object in front of the host has changed from the previous frame,
            // update the AimGameObject and call the callback.
            IsChangeAImedObject = AimGameObject != aimingGameObject;
            if( IsChangeAImedObject )
            {
                using var invokeEvent = new ProfilerScope("AimCheck.OnChangeAimedGameObject");

                PreAimGameObject = AimGameObject;
                AimGameObject = aimingGameObject;

                OnChangeAimedGameObject?.Invoke();
            }
        }

        int IEarlyUpdateComponent.Order => Order.Check;
        
        private bool RayCastCheckFromCenterView(in Vector3 aimTarget, out Vector3 hitPoint, out float distance, out Vector3 normal, out Vector3 direction, out Collider col)
        {
            var centerRay = _characterSettings.CameraMain.ViewportPointToRay(aimTarget);

            var count = Physics.RaycastNonAlloc(centerRay, Hits, _maxRange, CombinedLayerMask, QueryTriggerInteraction.Ignore);
            var isHit = _characterSettings.ClosestHit(Hits, count, _maxRange, out var hit);
            
            hitPoint = isHit ? hit.point : centerRay.GetPoint(_maxRange);
            distance = isHit ? hit.distance : _maxRange;
            normal = isHit ? Vector3.zero : hit.normal;
            col = isHit ? hit.collider : null;
            direction = centerRay.direction;
            
            return isHit;
        }

        private bool RayCastCheckFromOrigin(Vector3 hitPosition, out RaycastHit hit, out Vector3 direction, out float distance)
        {
            var deltaPosition = hitPosition - Origin;
            distance = deltaPosition.magnitude;
            direction = deltaPosition.normalized;

            var characterRay = new Ray(Origin, direction.normalized);
            
            var count = Physics.RaycastNonAlloc(characterRay, Hits, _maxRange, CombinedLayerMask, QueryTriggerInteraction.Ignore);
            var isHit = _characterSettings.ClosestHit(Hits, count, _maxRange, out hit);

            return isHit;
        }


        void IComponentCondition.OnConditionCheck(List<string> messageList)
        {
#if UNITY_EDITOR            
            if( _origin == null)
                messageList.Add( $"{nameof(AimCheck)} : Origin is not set. ({nameof(AimCheck)})");
#endif            
        }

        
        
        private void OnDrawGizmosSelected()
        {
            if( enabled == false)
                return;
            
            if (_origin == null && Application.isPlaying == false)
                return;

            Gizmos.DrawLine(Origin, Point);
            Gizmos.DrawRay(Point, Normal);
            GizmoDrawUtility.DrawSphere(Origin, 0.1f, Color.yellow, 0.1f);
            GizmoDrawUtility.DrawSphere(Point, 0.1f, Color.yellow, 0.1f);
        }
    }
}
