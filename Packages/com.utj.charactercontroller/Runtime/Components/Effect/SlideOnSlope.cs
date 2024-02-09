// using System.Collections;
// using System.Collections.Generic;
// using TinyCharacterController.Interfaces;
// using TinyCharacterController.Utility;
// using UnityEngine;
//
// namespace TinyCharacterController
// {
//     /// <summary>
//     /// 坂道に乗ったときに滑る挙動を追加するコンポーネント。
//     /// 坂道を滑っている間はプライオリティを上げて移動制御をオーバーライドすることも可能。
//     /// 坂道の最後で停止する場合はMeshColliderでConvexを使用するか、BoxColliderのようなプリミティブなコライダーを使用する。
//     /// </summary>
//     [AddComponentMenu(MenuList.MenuEffect + "Slide On Slope")]
//     [DisallowMultipleComponent]
//     [RequestInterface(typeof(IGravity))]
//     [RequestInterface(typeof(IGroundCheck))]
//     public class SlideOnSlope : MonoBehaviour, 
//         IExternalVelocity, 
//         ICharacterMove, 
//         ICharacterTurn, 
//         IEarlyUpdateComponent
//     {
//         [Header("滑る坂道の設定")]
//         [Tooltip("坂道と判定する角度")]
//         [SerializeField] 
//         private float _slideAngle = 45;
//         
//         [Tooltip("坂道を滑る速度の倍率")]
//         [SerializeField] 
//         private float _slideSpeed = 1f;
//
//         [Header("滑っているときの設定")] 
//         [SerializeField] [Range(0, 100)]
//         private int _acceleration = 50;
//
//         [SerializeField, Range(0, 100)] 
//         private int _brake = 10;
//
//         [Tooltip("坂道を滑る時の移動プライオリティ。滑っていない場合は内部的にプライオリティは0になる。")]
//         [SerializeField] 
//         private int _movePriorityWhenSliding = 10;
//
//         [Tooltip("坂道を滑る時の回転プライオリティ。滑っていない場合は内部的にプライオリティは0になる。")] 
//         [SerializeField]
//         private int _rotationPriorityWhenSliding = 10;
//
//         [Tooltip("滑っている方向を向く速度。RotationPriorityが高い場合に有効")]
//         [SerializeField]
//         [Range(-1, 50)]
//         private int _rotationSpeed = 30;
//
//         
//         /// <summary>
//         /// 滑る坂道の角度
//         /// </summary>
//         public float SlideAngle { get => _slideAngle; set => _slideAngle = value; }
//
//         /// <summary>
//         /// 現在滑って移動しているかの判定
//         /// </summary>
//         public bool IsSliding { get; private set; }
//
//         float ICharacterMove.Speed => 0;
//         int IPriority<ICharacterMove>.Priority => IsSliding ? _movePriorityWhenSliding : 0;
//         Vector3 IExternalVelocity.Velocity => _velocity;
//         Vector3 ICharacterMove.MoveVelocity => Vector3.zero;
//         int IPriority<ICharacterTurn>.Priority => IsSliding ? _rotationPriorityWhenSliding : 0;
//         float ICharacterTurn.YawAngle => _yawAngle;        
//         int ICharacterTurn.TurnSpeed => _rotationSpeed;
//         
//         private IGroundCheck _groundCheck;
//         private IGravity _gravity;
//         private Vector3 _velocity;
//         private int _rotationPriority;
//         private float _yawAngle;
//
//         private void Awake()
//         {
//             TryGetComponent(out _groundCheck);
//             TryGetComponent(out _gravity);
//         }
//
//         private void OnDisable()
//         {
//             _velocity = Vector3.zero;
//             IsSliding = false;
//         }
//
//         void IEarlyUpdateComponent.OnUpdate(float deltaTime)
//         {
//             if (enabled == false)
//                 return;
//             
//             IsSliding = false;
//             if (_groundCheck.IsGroundedStrictly && _gravity.FallSpeed <= 0)
//             {
//                 var ray = new Ray(_groundCheck.ContactPoint + Vector3.up, Vector3.down);
//                 var isRayCastHit = _groundCheck.GroundCollider.Raycast(ray, out var hit, 1.2f);
//                 var normal = isRayCastHit ? hit.normal : Vector3.up;
//                 IsSliding = Vector3.Angle(Vector3.up, normal) > _slideAngle;
//                 
//                 if (IsSliding)
//                 {
//
//                     var cross = Vector3.Cross(normal, Vector3.up);
//                     var rotation = Quaternion.FromToRotation(Vector3.up, Vector3.Cross(cross, normal));
//                     var targetVelocity = rotation * Physics.gravity * _slideSpeed;
//                     ;
//                     var scaledTargetVelocity = Vector3.Scale(new Vector3(1, 0, 1), targetVelocity);
//
//                     if( scaledTargetVelocity != Vector3.zero)
//                         _yawAngle = Vector3.SignedAngle(Vector3.forward, targetVelocity, Vector3.up);
//                     _velocity =  Vector3.Lerp(_velocity, targetVelocity, deltaTime * _acceleration);
//                     IsSliding = true;
//                 }
//             }
//
//             if( IsSliding == false)
//             {
//                 _velocity = Vector3.Lerp(_velocity, Vector3.zero, deltaTime * _brake);
//                 IsSliding = false;
//             }
//             
//         }
//
//         int IEarlyUpdateComponent.Order => Order.Effect;
//     }
// }
