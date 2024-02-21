using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Utility;
using UnityEngine;

namespace Unity.TinyCharacterController.Brain
{
    /// <summary>
    /// This brain operates using <see cref="UnityEngine.Transform"/>.
    /// It is a Brain that operates with <see cref="UnityEngine.Transform"/>.
    /// There are no movement restrictions based on collider contacts.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(Order.UpdateBrain)]
    [AddComponentMenu(MenuList.MenuBrain + "Transform Brain")]
    [RequireComponent(typeof(CharacterSettings))]   
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.TransformBrain")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Brain.TransformBrain")]
    public class TransformBrain : BrainBase
    {
        private EarlyUpdateBrainBase _earlyUpdate;

        private void Awake()
        {
            Initialize();
            _earlyUpdate = gameObject.AddComponent<EarlyUpdateBrain>();
        }

        private void OnEnable()
        {
            _earlyUpdate.enabled = true;
        }

        private void OnDisable()
        {
            _earlyUpdate.enabled = false;
        }

        private void Update()
        {
            UpdateBrain();
        }

        public override UpdateTiming Timing => UpdateTiming.Update;
        
        protected override void ApplyPosition(in Vector3 totalVelocity, float deltaTime)
        {
            CachedTransform.position += totalVelocity * Time.deltaTime;
        }

        protected override void SetPositionDirectly(in Vector3 position)
        {
            CachedTransform.position = position;
        }

        protected override void SetRotationDirectly(in Quaternion rotation)
        {
            CachedTransform.rotation = rotation;
        }

        protected override void MovePosition(in Vector3 newPosition)
        {
            SetPositionDirectly(newPosition);
        }

        protected override void ApplyRotation(in Quaternion rotation)
        {
            CachedTransform.rotation = rotation;
        }
        
        protected override void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            CachedTransform.GetPositionAndRotation(out position, out rotation);
        }
    }
}
