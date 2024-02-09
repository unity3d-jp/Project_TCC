using Unity.TinyCharacterController;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using UnityEngine;

namespace Unity.TinyCharacterController.Check
{
    [AddComponentMenu(MenuList.MenuCheck + nameof(GroundHeightCheck))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    public class GroundHeightCheck : MonoBehaviour,
        IGroundContact
    {
        /// <summary>
        /// 地面の高さ
        /// </summary>
        public float GroundHeight;

        /// <summary>
        /// 接地したと判断される遊びの高さ。
        /// </summary>
        [SerializeField] 
        private float _toleranceHeight;
        
        private ITransform _transform;

        private void Awake()
        {
            TryGetComponent(out _transform);
        }

        public bool IsOnGround => _transform.Position.y <= GroundHeight +_toleranceHeight;

        public bool IsFirmlyOnGround => _transform.Position.y <= GroundHeight;

        public float DistanceFromGround => _transform.Position.y - GroundHeight;

        Vector3 IGroundContact.GroundSurfaceNormal => Vector3.up;

        public Vector3 GroundContactPoint => new (_transform.Position.x, GroundHeight, _transform.Position.z );

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = GetGizmosColor();
            
            // 地面を描画
            var position = transform.position;
            var groundPos = new Vector3(position.x, GroundHeight, position.z);
            Gizmos.DrawCube(groundPos, new Vector3(5, 0, 5));
            return;

            Color GetGizmosColor()
            {
                if (Application.isPlaying == false)
                    return Color.yellow;

                if (((IGroundContact)this).IsFirmlyOnGround)
                    return Color.red;
                if( ((IGroundContact)this).IsOnGround)
                    return Color.magenta;
                return Color.yellow;
            }
        }
    }
}
