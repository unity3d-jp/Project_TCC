// using System;
// using System.Collections;
// using System.Collections.Generic;
// using TinyCharacterController.Utility;
// using UnityEngine;
// using UnityEngine.Events;
//
// namespace TinyCharacterController
// {
//     /// <summary>
//     /// オブジェクトの接触を検知する。
//     /// 接触中のオブジェクト
//     /// </summary>
//     [DefaultExecutionOrder(Order.Check)]
//     public class ContactEventCheck : MonoBehaviour, IUpdateComponent
//     {
//
//         /// <summary>
//         /// 
//         /// </summary>
//         public UnityEvent OnChangeValue = new();
//
//         /// <summary>
//         /// 
//         /// </summary>
//         public bool IsContainCollider => _colliders.Count > 0;
//
//         /// <summary>
//         /// 
//         /// </summary>
//         public Collider ContactCollider => _colliders[0];
//
//         private void OnTriggerEnter(Collider other)
//         {
//             foreach (var targetTag in _targetTags)
//             {
//                 if (other.CompareTag(targetTag) == false) 
//                     continue;
//                 
//                 _addColliders.Enqueue(other);
//                 return;
//             }
//         }
//
//         [ReadOnlyOnRuntime] [SerializeField] 
//         private string[] _targetTags;
//
//         private Queue<Collider> _addColliders = new();
//         private Queue<Collider> _removeColliders = new();
//         private readonly List<Collider> _colliders = new();
//
//         private void OnTriggerExit(Collider other)
//         {
//             foreach (var targetTag in _targetTags)
//             {
//                 if (other.CompareTag(targetTag) == false) 
//                     continue;
//                 
//                 _removeColliders.Enqueue(other);
//                 return;
//             }
//         }
//
//         void IUpdateComponent.OnUpdate(float deltaTime)
//         {
//             var isChanged = (_addColliders.Count + _removeColliders.Count) > 0;
//             
//             while (_removeColliders.TryDequeue(out var removeCollider))
//                 _colliders.Remove(removeCollider);
//             
//             _colliders.AddRange(_addColliders);
//             
//             _addColliders.Clear();
//             _removeColliders.Clear();
//             
//             if( isChanged )
//                 OnChangeValue?.Invoke();
//         }
//
//         int IUpdateComponent.Order => Order.Check;
//     }
// }
