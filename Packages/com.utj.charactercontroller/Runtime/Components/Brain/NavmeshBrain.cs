using Unity.TinyCharacterController.Interfaces.Components;
using UnityEngine;
using UnityEngine.AI;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Utility;

namespace Unity.TinyCharacterController.Brain
{
    /// <summary>
    /// This brain operates using Navmesh.
    /// The height and width of the Agent are determined by <see cref="CharacterSettings.Height"/>.Height and <see cref="CharacterSettings.Radius"/>.
    /// To function properly, a <see cref="UnityEngine.AI.NavMeshAgent"/> is required.
    /// </summary>
    [AddComponentMenu(MenuList.MenuBrain + "Navmesh Brain")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    [DefaultExecutionOrder(Order.UpdateBrain)]
    [RequireComponent(typeof(CharacterSettings))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.NavmeshBrain")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Brain.NavmeshBrain")]
    public class NavmeshBrain : BrainBase, 
        ICharacterSettingUpdateReceiver
    {
        private NavMeshAgent _agent;
        private EarlyUpdateBrainBase _earlyUpdate;
        private Vector3 _position;
        private Quaternion _rotation;

        private void Reset()
        {
            var agent = GetComponent<NavMeshAgent>();
            agent.avoidancePriority = 0;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            agent.autoTraverseOffMeshLink = false;
            agent.autoBraking = false;
            agent.autoRepath = false;
            agent.baseOffset = -0.05f;
            ((ICharacterSettingUpdateReceiver)this).OnUpdateSettings(GetComponent<CharacterSettings>());
        }

        private void Awake()
        {
            Initialize();
            TryGetComponent(out _agent);
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

        protected override void ApplyPosition(in Vector3 totalVelocity, float deltaTime)
        {
            if (_agent.enabled)
                _agent.Move(totalVelocity * deltaTime);
            else
                CachedTransform.position += totalVelocity * deltaTime;
        }


        protected override void ApplyRotation(in Quaternion rotation)
        {
            CachedTransform.rotation = rotation;
        }

        protected override void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            position = CachedTransform.position;
            rotation = CachedTransform.rotation;
        }

        public override UpdateTiming Timing => UpdateTiming.Update;


        void ICharacterSettingUpdateReceiver.OnUpdateSettings(CharacterSettings settings)
        {
            TryGetComponent(out NavMeshAgent agent);

            agent.radius = settings.Radius;
            agent.height = settings.Height;
        }

        protected override void SetPositionDirectly(in Vector3 position)
        {
            if( _agent.enabled )
                _agent.Warp(position);
            else
                CachedTransform.position = position;
        }

        protected override void SetRotationDirectly(in Quaternion rotation)
        {
            CachedTransform.rotation = rotation;
        }

        protected override void MovePosition(in Vector3 newPosition)
        {
            var deltaPosition = newPosition - Position;
            _agent.Move(deltaPosition);
        }
    }
}
