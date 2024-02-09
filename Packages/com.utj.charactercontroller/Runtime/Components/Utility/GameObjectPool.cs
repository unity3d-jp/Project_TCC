using System;
using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Components;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Unity.TinyCharacterController.Manager;

namespace Unity.TinyCharacterController.Utility
{
    /// <summary>
    ///     Component for reusing GameObjects.
    ///     Retrieve objects with <see cref="Get()" />, and return them with <see cref="Release" />, reducing object creation
    ///     costs.
    ///     Components should be prepared for each Prefab. It is possible to reference Prefabs from
    ///     <see cref="GameObjectPoolManager" />.
    ///     Instances of GameObjectPool are created in the scene where GameObjectPool belongs.
    ///     If <see cref="_isActiveOnGet" /> is False, objects will not become active automatically.
    /// </summary>
    /// <seealso cref="GameObjectPoolManager"/>
    /// <seealso cref="PooledGameObject"/>
    [DefaultExecutionOrder(Order.GameObjectPool)]
    [AddComponentMenu(MenuList.Utility + nameof(GameObjectPool))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Utility.GameObjectPool")]
    public class GameObjectPool : ComponentBase,
        IGameObjectPool,
        IEquatable<GameObjectPool>
    {
        /// <summary>
        ///     The Prefab to create instances from.
        /// </summary>
        [Tooltip("Prefab to be created")] [RequestField] [SerializeField] [ReadOnlyOnRuntime]
        private PooledGameObject _prefab;

        /// <summary>
        ///     Automatically activate objects when retrieved.
        /// </summary>
        [SerializeField] [ReadOnlyOnRuntime] private bool _isActiveOnGet = true;
        
        /// <summary>
        ///     The parent object to create instances under.
        /// </summary>
        [Tooltip("Specify the parent of the object to be created. " +
                 "For example, if you want to place UI objects under a Canvas, you can do so by setting this item. " +
                 "However, it is recommended not to set it in unnecessary cases due to transformation calculation issues.")]
        [SerializeField]
        [ReadOnlyOnRuntime]
        private Transform _parent;

        /// <summary>
        ///     Number of objects to create at awake.
        /// </summary>
        [SerializeField] [ReadOnlyOnRuntime] private int _prewarmCount;

#if UNITY_EDITOR
        [SerializeField, ReadOnlyOnRuntime] private bool _hideSpawnObject;
#endif
        
        /// <summary>
        /// Stores a list of objects created by <see cref="Get(TinyCharacterController.Utility.PooledGameObject)"/>.
        /// This list is used to destroy unused objects with methods like <see cref="DestroyAllInstance"/> and <see cref="DestroyUnusedObjects"/>.
        /// NOTE: Use the component, not the interface, to know that the component has been destroyed from GameObject deletion.
        /// </summary>
        private readonly List<PooledGameObject> _createdObjects = new();

        /// <summary>
        /// Stores instances that were created with <see cref="Get(TinyCharacterController.Utility.PooledGameObject)"/> and subsequently released with <see cref="Release(TinyCharacterController.Interfaces.Components.IPooledObject)"/>.
        /// When you call <see cref="Get(TinyCharacterController.Utility.PooledGameObject)"/> again, the elements are retrieved from this list.
        /// NOTE: Use the component, not the interface, to know that the component has been destroyed from GameObject deletion.
        /// </summary>
        private readonly Stack<PooledGameObject> _free = new();

        private int _cachedPrefabInstanceID;
        private bool _hasParent;
        private bool _hasRigidbody;
        private bool _isInitialize;
        private int _siblingIndex;

        private void Awake()
        {
            if (GameObjectPoolManager.Register(this) == false)
            {
                Debug.LogError($"{_prefab.name} already registered prefab.", gameObject);
                enabled = false;
                return;
            }

            if( _isInitialize == false)
                Initialize();
        }

        private void Start()
        {
            _siblingIndex = transform.root.GetSiblingIndex();

#if UNITY_EDITOR
            if( _hasParent == false)
                foreach (var t in _createdObjects)
                    t.transform.SetSiblingIndex(_siblingIndex);
#endif
        }

        private void OnDestroy()
        {
            // Unregister from the manager.
            GameObjectPoolManager.Unregister(this);

            // Destroy all created objects.
            DestroyAllInstance();
        }

        private void OnValidate()
        {
            _prewarmCount = Mathf.Max(0, _prewarmCount);
        }

        /// <summary>
        ///     Compare the registered Prefab with other GameObjectPools.
        /// </summary>
        /// <param name="other">Another GameObjectPool</param>
        /// <returns>True if the same Prefab is registered</returns>
        bool IEquatable<GameObjectPool>.Equals(GameObjectPool other)
        {
            return other != null &&
                   other.gameObject.scene == gameObject.scene &&
                   other.PrefabID == PrefabID;
        }

        /// <summary>
        ///     The instance ID of the Prefab.
        /// </summary>
        public int PrefabID
        {
            get
            {
                if (_isInitialize == false)
                    Initialize();
                return _cachedPrefabInstanceID;
            }
        }

        /// <summary>
        /// reference owner scene.
        /// </summary>
        Scene IGameObjectPool.Scene => gameObject.scene;

        /// <summary>
        /// release instance.
        /// </summary>
        /// <param name="obj">release object</param>
        void IGameObjectPool.Release(IPooledObject obj)
        {
            if (obj == null)
                return;

            if (obj.IsUsed == false)
            {
                Debug.LogError("Already release instance.");
                return;
            }

            if (obj.GameObject != null)
            {
                obj.GameObject.SetActive(false);
#if UNITY_EDITOR
                if( _hideSpawnObject)
                    obj.GameObject.hideFlags = HideFlags.HideInHierarchy;
#endif

                
                _free.Push((PooledGameObject)obj);
                obj.OnRelease();
            }
        }

        /// <summary>
        /// Retrieve a recycled instance from the pool. If no instances are available, a new one is created.
        /// </summary>
        /// <param name="prefab">Prefab to duplicate</param>
        /// <returns>Available instance</returns>
        public static GameObject Get(PooledGameObject prefab)
        {
            var pool = GameObjectPoolManager.GetPool(prefab);
            return pool.Get().GameObject;
        }

        /// <summary>
        /// Retrieve a recycled instance from the specified scene's GameObjectPool. If no instances are available, a new one is created.
        /// </summary>
        /// <param name="prefab">Prefab to duplicate</param>
        /// <param name="scene">Scene to search</param>
        /// <returns>Available instance</returns>
        public static GameObject Get(PooledGameObject prefab, Scene scene)
        {
            var pool = GameObjectPoolManager.GetPool(prefab, scene);
            return pool.Get().GameObject;
        }

        /// <summary>
        /// Retrieve a recycled instance from the pool. If no instances are available, a new one is created.
        /// </summary>
        /// <param name="prefab">Prefab to duplicate</param>
        /// <param name="position">New instance's position</param>
        /// <param name="rotation">New instance's rotation</param>
        /// <returns>New instance</returns>
        public static GameObject Get(PooledGameObject prefab, Vector3 position, Quaternion rotation)
        {
            var pool = GameObjectPoolManager.GetPool(prefab);
            return pool.Get(position, rotation).GameObject;
        }

        /// <summary>
        /// Retrieve a recycled instance from the specified scene's GameObjectPool. If no instances are available, a new one is created.
        /// </summary>
        /// <param name="prefab">Prefab to duplicate</param>
        /// <param name="scene">Scene to search</param>
        /// <param name="position">New instance's position</param>
        /// <param name="rotation">New instance's rotation</param>
        /// <returns>Available instance</returns>
        public static GameObject Get(PooledGameObject prefab, Scene scene, Vector3 position, Quaternion rotation)
        {
            var pool = GameObjectPoolManager.GetPool(prefab, scene);
            return pool.Get(position, rotation).GameObject;
        }

        /// <summary>
        /// Retrieve a recycled instance from the specified scene's GameObjectPool. If no instances are available, a new one is created.
        /// </summary>
        /// <param name="position">New instance's position</param>
        /// <param name="rotation">New instance's rotation</param>
        /// <returns>Available instance</returns>
        IPooledObject IGameObjectPool.Get(in Vector3 position, in Quaternion rotation) =>
            GetNewInstance(position, rotation);

        /// <summary>
        /// Retrieve a recycled instance from the specified scene's GameObjectPool. If no instances are available, a new one is created.
        /// </summary>
        /// <returns>Available instance</returns>
        IPooledObject IGameObjectPool.Get() => GetNewInstance();
        
        /// <summary>
        ///     Get a GameObject from the object pool.
        /// </summary>
        /// <returns>The cached GameObject</returns>
        public GameObject Get() => GetNewInstance().gameObject;

        /// <summary>
        ///     Get a GameObject from the object pool and set its position and rotation.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="rotation">The rotation</param>
        /// <returns>The cached GameObject</returns>
        public GameObject Get(Vector3 position, Quaternion rotation)
            => GetNewInstance(position, rotation).gameObject;
        
        /// <summary>
        ///     Release the acquired object.
        /// </summary>
        /// <param name="obj">The object to release</param>
        public static void Release(GameObject obj)
        {
            if (obj.TryGetComponent(out IPooledObject pooledGameObject))
                pooledGameObject.Release();
        }

        /// <summary>
        ///     Release the acquired object.
        /// </summary>
        /// <param name="obj">The object to release</param>
        public static void Release(IPooledObject obj)
        {
            obj.Release();
        }

        /// <summary>
        ///     Destroy unused instances.
        /// </summary>
        public void DestroyUnusedObjects()
        {
            while (_free.TryPop(out var instance)) Destroy(instance.gameObject);
            _free.Clear();
        }

        /// <summary>
        ///     Create an instance and register it in <see cref="_createdObjects" />.
        /// </summary>
        /// <returns>The new instance</returns>
        private PooledGameObject CreateInstance()
        {
            // Create an instance.
            // If there is no parent object, create it in the same scene.
            var newInstance = _hasParent ? Instantiate(_prefab, _parent) : Instantiate(_prefab);
            if (_hasParent == false)
            {
                SceneManager.MoveGameObjectToScene(newInstance.gameObject, gameObject.scene);
                newInstance.transform.SetSiblingIndex(_siblingIndex + 1);
            }

            // Register the object in the list of created objects and initialize it.
            _createdObjects.Add(newInstance);
            if (newInstance is IPooledObject pooledObject)
                pooledObject.Initialize(this, _hasRigidbody);

#if UNITY_EDITOR
            if (_hideSpawnObject)
                newInstance.gameObject.hideFlags = HideFlags.HideInHierarchy;

            newInstance.gameObject.name = $"{_prefab.name}({newInstance.GetInstanceID()})";
#endif
            
            return newInstance;
        }

        /// <summary>
        ///     Release all registered instances.
        /// </summary>
        public void ReleaseAllInstance()
        {
            foreach (var instance in _createdObjects)
            {
                if (_free.Contains(instance) == false)
                {
                    Release(instance);
                }
            }
        }

        /// <summary>
        ///     Destroy all registered instances.
        /// </summary>
        private void DestroyAllInstance()
        {
            foreach (var obj in _createdObjects)
            {
                // Instances created by the component may be discarded by the scene side first.
                if (obj != null)
                    Destroy(obj.gameObject);
            }

            // Clear the component references.
            _createdObjects.Clear();
            _free.Clear();
        }

        /// <summary>
        ///     Generate objects in advance and cache them.
        /// </summary>
        private void PrewarmProcess()
        {
            for (var i = 0; i < _prewarmCount; i++)
                ((IPooledObject)CreateInstance()).OnGet();

            foreach (var pooledObject in _createdObjects)
                Release(pooledObject);
        }

        /// <summary>
        ///     Initialize the component.
        /// </summary>
        private void Initialize()
        {
            
            // _isActiveOnGet means that the user needs to activate the instance.
            // If the Prefab is active and the setting to activate the object when creating is False, the first instance will be forcibly activated.
            // Therefore, if the Prefab is active and IsActiveOnGet is False, consistency cannot be maintained.
            Assert.IsFalse(_prefab.gameObject.activeSelf && _isActiveOnGet == false,
                $"{_prefab.name} is Active. Make it inactive to be consistent with _isActiveOnGet.");

            _cachedPrefabInstanceID = ((IPooledObject)_prefab).InstanceId;
            _hasRigidbody = _prefab.TryGetComponent(out Rigidbody _);
            _isInitialize = true;
            _hasParent = _parent != null;

            // PreWarm processing.
            PrewarmProcess();
        }

        /// <summary>
        /// Retrieves a new instance from the pooled instances. If there are no available instances, a new instance is created.
        /// Initializes the instance and activates it if necessary.
        /// </summary>
        /// <returns>Available instance</returns>
        private PooledGameObject GetNewInstance()
        {
            var instance = GetPooledObjectOrNewInstance();
            ((IPooledObject)instance).OnGet();
            ActivateObjectIfNecessary(instance);

            return instance;
        }

        /// <summary>
        /// Retrieves a new instance from the pooled instances. If there are no available instances, a new instance is created.
        /// Initializes and updates the position and rotation of the instance before it becomes active, and activates it if necessary.
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="rotation">Rotation</param>
        /// <returns>Available instance</returns>
        private PooledGameObject GetNewInstance(Vector3 position, Quaternion rotation)
        {
            var instance = GetPooledObjectOrNewInstance();
            instance.transform.SetPositionAndRotation(position, rotation);

            ((IPooledObject)instance).OnGet();
            ActivateObjectIfNecessary(instance);

            return instance;
        }

        /// <summary>
        /// Retrieves a new instance from the pooled instances.
        /// </summary>
        /// <returns>New instance</returns>
        private PooledGameObject GetPooledObjectOrNewInstance()
        {
            // If there are free objects, retrieve from there.
            if (_free.TryPop(out var instance) == false)
            {
                instance = CreateInstance();
            }

#if UNITY_EDITOR
            instance.gameObject.hideFlags = HideFlags.None;     
#endif            
            return instance;
        }

        /// <summary>
        /// Activates the object if necessary.
        /// </summary>
        /// <param name="instance">Object to process</param>
        private void ActivateObjectIfNecessary(in PooledGameObject instance)
        {
            if (_isActiveOnGet)
                instance.gameObject.SetActive(true);
        }
    }
}