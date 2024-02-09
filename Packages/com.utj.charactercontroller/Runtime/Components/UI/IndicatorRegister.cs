using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.UI;
using UnityEngine;
using Unity.TinyCharacterController.Manager;

namespace Unity.TinyCharacterController.Utility
{
    /// <summary>
    /// A component for easily adding indicators to characters.
    /// Generates and registers the <see cref="_ui"/> object automatically when the component becomes active.
    /// Objects are created using the functionality of <see cref="GameObjectPool"/>.
    /// Therefore, the UI to be generated should have a <see cref="PooledGameObject" /> and be registered in the <see cref="GameObjectPool"/>.
    /// </summary>
    [DefaultExecutionOrder(Order.IndicatorRegister)]
    [AddComponentMenu(MenuList.Ui + nameof(IndicatorRegister))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Utility.IndicatorRegister")]
    public class IndicatorRegister : MonoBehaviour
    {
        /// <summary>
        /// The UI to generate.
        /// </summary>
        [SerializeField] private PooledGameObject _ui;

        private PooledGameObject _current;
        private IGameObjectPool _pool;
        private bool _hasInstance = false;

        /// <summary>
        /// The generated UI.
        /// </summary>
        public GameObject Instance { get; private set; }

        /// <summary>
        /// True if an instance of the UI has been generated.
        /// </summary>
        public bool HasInstance => _hasInstance && _current != null;

        private void OnEnable()
        {
            // Retrieve the UI from GameObjectPool and register it as a pooledObject.
            if (_pool == null)
                _pool = GameObjectPoolManager.GetPool(_ui);
            
            Instance = _pool.Get().GameObject;
            Instance.TryGetComponent(out _current);

            // If there's an Indicator component, register itself with the component.
            if (Instance.TryGetComponent(out Indicator indicator))
            {
                indicator.Target = transform;

                if (TryGetComponent(out BrainBase brain))
                    indicator.FollowTargetUpdateTiming = brain.Timing;
            }

            // If there's a UIPin component, register itself with the component.
            if (Instance.TryGetComponent(out IndicatorPin pin))
            {
                pin.WorldPosition = transform.position;
            }

            _hasInstance = true;

            // Since GameObjects may not automatically become active, activate it.
            Instance.SetActive(true);
        }

        private void OnDisable()
        {
            // Release the acquired instance.
            if( _current != null )
                _current.Release();
            _hasInstance = false;
            _current = null;
        }
    }
}
