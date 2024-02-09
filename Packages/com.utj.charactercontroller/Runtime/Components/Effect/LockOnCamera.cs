// using System;
// using TinyCharacterController.Interfaces;
// using TinyCharacterController.Utility;
// using UnityEngine;
//
// namespace TinyCharacterController
// {
//     [AddComponentMenu(MenuList.MenuEffect + "LockOnCamera")]
//     [DisallowMultipleComponent]
//     [DefaultExecutionOrder(Order.LookOnCameraControl)]
//     public class LockOnCamera : MonoBehaviour, ICharacterTurn, ICameraUpdate
//     {
//
//         [SerializeField] private Transform _cameraRoot;
//         [SerializeField] private Transform _target;
//         [SerializeField] private float _distance = 5;
//
//         public Transform Target { get => _target; set => _target = value; }
//
//         public Vector3 Offset;
//         public Vector3 Shoulder = new Vector3(3, 0, -5);
//         public Vector3 TargetOffset;
//
//         private Vector3 TargetPosition => _target.position + TargetOffset;
//
//         [SerializeField]
//         private int _priority;
//
//         [SerializeField]
//         [Range(-1, 50)]
//         private int _rotationSpeed;
//
//         [SerializeField]
//         private float _minClamp, _maxClamp;
//
//         public int TurnPriority { get => _priority; set => _priority = value; }
//
//         int ICharacterTurn.TurnSpeed => _rotationSpeed;
//
//         public Quaternion YawRotation { get; private set; }
//
//         private Transform _transform;
//
//         private void Awake()
//         {
//             TryGetComponent(out _transform);
//         }
//
//         void ICameraUpdate.OnUpdate()
//         {
//             var transformPosition = _transform.position;
//             var delta = TargetPosition - transformPosition;
//             var yaw = Vector3.Scale(delta, new Vector3(1, 0, 1));
//             var yawRotation = Quaternion.LookRotation(yaw, Vector3.up);
//             var rotation = Quaternion.LookRotation(delta, Vector3.up);
//
//             // カメラの移動範囲を限定
//             rotation.eulerAngles = new Vector3(
//                 Mathf.Clamp(rotation.eulerAngles.x, _minClamp * Mathf.Rad2Deg, _maxClamp * Mathf.Rad2Deg),
//                 rotation.eulerAngles.y,
//                 rotation.eulerAngles.z
//             );
//
//             _cameraRoot.SetPositionAndRotation(transformPosition + rotation * Shoulder * _distance + Offset, rotation);
//
//             YawRotation = yawRotation;
//         }
//     }
// }
