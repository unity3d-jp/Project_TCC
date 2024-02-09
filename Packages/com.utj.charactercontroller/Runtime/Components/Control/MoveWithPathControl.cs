// using TinyCharacterController.Interfaces;
// using UnityEngine;
// using UnityEngine.Assertions;
// using UnityEngine.Events;
//
// namespace TinyCharacterController
// {
//     /// <summary>
//     /// 
//     /// </summary>
//     public class MoveWithPathControl : MonoBehaviour, ICharacterMove, ICharacterTurn
//     {
//         [SerializeField]
//         private int _priority;
//
//         [SerializeField, Range(0, 50)]
//         private int _turnSpeed;
//
//         [Range(1f, 30f)]
//         [SerializeField] private float _supplementarySpeed = 1;
//
//         [SerializeField] 
//         private float _moveSpeed;
//
//         /// <summary>
//         /// 
//         /// </summary>
//         public UnityEvent OnComplete;
//         
//         /// <summary>
//         /// 
//         /// </summary>
//         public bool IsComplete { get; private set; }
//         
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <param name="axis"></param>
//         public void Move(float axis)
//         {
//             _moveInput = axis;
//         }
//
//         public int Priority
//         {
//             get => _priority;
//             set => _priority = value;
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <param name="path"></param>
//         /// <param name="direction"></param>
//         public void SetPath(MovePath path, MoveDirection direction)
//         {
//             _currentPath = path;
//             IsComplete = false;
//
//             if (path == null)
//                 return;
//
//             _currentPoint = direction == MoveDirection.StartToEnd ? 0 : path.Distance - path.StepSize * 2;
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         public MoveDirection MoveAxis
//         {
//             get
//             {
//                 if (_moveInput > 0)
//                     return MoveDirection.StartToEnd;
//                 else if (_moveInput < 0)
//                     return MoveDirection.EndToStart;
//                 else
//                     return MoveDirection.None;
//             }
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         public enum MoveDirection
//         {
//             EndToStart,
//             StartToEnd,
//             None
//         }
//
//         private Transform _transform;
//         private float _currentPoint;
//         private MovePath _currentPath;
//         private float _speed;
//         private Vector3 _moveVelocity;
//         private Quaternion _yawRotation;
//         private float _moveInput = 0;        
//         
//         int ICharacterMove.MovePriority => _currentPath ? _priority : 0;
//         Vector3 ICharacterMove.MoveVelocity => _currentPath ? _moveVelocity : Vector3.zero;
//         int ICharacterTurn.TurnPriority => _currentPath ? _priority : 0;
//         float ICharacterMove.Speed => _moveVelocity.magnitude;
//         int ICharacterTurn.TurnSpeed => _turnSpeed;
//         Quaternion ICharacterTurn.YawRotation => _yawRotation;
//
//         private void Awake()
//         {
//             TryGetComponent(out _transform);
//         }
//         
//         private void FixedUpdate()
//         {
//             if (_currentPath == null)
//                 return;
//
//             _yawRotation = Quaternion.AngleAxis(_currentPath.transform.eulerAngles.y, Vector3.up);
//             _currentPoint = _currentPath.ClosestPoint(_transform.position);
//             _currentPoint = _currentPath.ClosestPoint(_currentPoint);
//             
//             if (_currentPath.CheckRange(_currentPoint) == false)
//             {
//                 IsComplete = true;
//                 OnComplete?.Invoke();
//                 return;
//             }
//                         
//             var point = _currentPath.Evaluate(_currentPoint);
//             var delta = point - _transform.position;
//             _moveVelocity = delta / (Time.fixedDeltaTime * _supplementarySpeed) ;
//         }
//
//
//         private void OnDrawGizmosSelected()
//         {
//             if (_currentPath == null)
//                 return;
//             
//             Gizmos.DrawCube(_currentPath.Evaluate(_currentPoint), Vector3.one * 0.3f);
//         }
//     }
// }