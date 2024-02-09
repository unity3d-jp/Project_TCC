// using System.Collections;
// using System.Collections.Generic;
// using TinyCharacterController.Interfaces;
// using UnityEngine;
//
// namespace TinyCharacterController
// {
//     public class FootIkRig : MonoBehaviour, IIkRig
//     {
//         private Animator _animator;
//         private CharacterSettings _settings;
//         private Transform _leftToes, _rightToes;
//         private IGroundCheck _groundCheck;
//         private static RaycastHit[] _colliders = new RaycastHit[5];
//         private float _leftFootWeight, _rightFootWeight;
//         private bool _contactLeftFoot, _contactRightFoot;
//         [SerializeField] private LayerMask _hitLayer;
//
//         [SerializeField] private Transform _leftFootHint, _rightFootHint;
//         [SerializeField] private float _footOffset;
//         [SerializeField] private float _speed = 5;
//
//         public float Weight => 1f;
//
//         void IIkRig.Initialize(Animator animator)
//         {
//             _animator = animator;
//             animator.TryGetComponent(out _settings);
//             animator.TryGetComponent(out _groundCheck);
//             _leftToes = _animator.GetBoneTransform(HumanBodyBones.LeftToes);
//             _rightToes = _animator.GetBoneTransform(HumanBodyBones.RightToes);
//
//         }
//
//         void IIkRig.OnPreProcess(float deltaTime)
//         {
//             _leftFootWeight = Mathf.Clamp01(_leftFootWeight + Time.deltaTime * (_contactLeftFoot ? 1 : -1) * _speed);
//             _rightFootWeight = Mathf.Clamp01(_rightFootWeight + Time.deltaTime * (_contactRightFoot ? 1 : -1) * _speed);
//         }
//         
//         void IIkRig.OnIkProcess()
//         {
//             _contactLeftFoot = CalculateFootIK(AvatarIKGoal.LeftFoot);
//             _contactRightFoot = CalculateFootIK(AvatarIKGoal.RightFoot);
//             
//             _animator.SetIKHintPosition( AvatarIKHint.LeftKnee, _leftFootHint.position );
//             _animator.SetIKHintPosition( AvatarIKHint.RightKnee, _rightFootHint.position );
//             _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _leftFootWeight);
//             _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _rightFootWeight);
//             _animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, _leftFootWeight);
//             _animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, _rightFootWeight);
//             _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _leftFootWeight);
//             _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _rightFootWeight);
//         }
//
//         bool CalculateFootIK(AvatarIKGoal goal)
//         {
//             var distance = 0.3f;
//             var upVector = new Vector3(0, distance, 0);
//             var position = _animator.GetIKPosition(goal);
//             var ray = new Ray(position + upVector , Vector3.down);
//             
//             Debug.DrawRay(ray.origin, ray.direction * (distance + _footOffset), Color.green, 0.3f);
//             
//             if (Physics.SphereCast(ray, 0.1f, out var hit,distance + _footOffset,  _hitLayer, QueryTriggerInteraction.Ignore) &&
//                 hit.point.y > position.y)
//             {
//                 var footPosition = hit.point;
//                 footPosition.y += _footOffset;
//                 var footRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * _animator.rootRotation;
//                 _animator.SetIKPosition(goal, footPosition);
//                 _animator.SetIKRotation(goal, footRotation);
//                 return true;
//             }
//
//             return false;
//         }
//
//         bool IIkRig.IsValid => true;
//     }
// }
