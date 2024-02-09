using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Manager;
using Unity.TinyCharacterController.Smb;
using Unity.TinyCharacterController.Utility;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// This component monitors changes in the stored information and extracts added or removed elements.
    /// If there is a change in the keys, it issues a callback.
    ///
    /// Adding or removing keys is expected to be performed from AnimationModifierBehaviour.
    /// </summary>
    [AddComponentMenu(MenuList.MenuCheck + nameof(AnimatorModifierCheck))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RequireComponent(typeof(CharacterSettings))]    
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.AnimationModifierCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.AnimationModifierCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.AnimatorModifierCheck")]
    public class AnimatorModifierCheck : MonoBehaviour, 
        IAnimationModifierUpdate
    {
        /// <summary>
        /// Specify the update timing.
        /// If the Animator is operating in Physics, specify FixedUpdate; otherwise, specify Update.
        /// </summary>
        [SerializeField] private UpdateMode _updateMode = UpdateMode.Update;
        
        /// <summary>
        /// Callback when a key is added or removed.
        /// </summary>
        public UnityEvent OnChangeKey;

        /// <summary>
        /// Determines if any key was added or removed during the frame.
        /// </summary>
        public bool ChangeKey { get; private set; }
        
        /// <summary>
        ///  Check if a key is held.
        /// </summary>
        /// <param name="key">check key</param>
        /// <returns>True if has key.</returns>
        public bool HasKey(PropertyName key) => _currentKeyList.Contains(key);

        /// <summary>
        ///  Check if a key is held.
        /// </summary>
        /// <param name="key">check key</param>
        /// <returns>True if has key.</returns>
        public bool HasKey(string key) => HasKey(new PropertyName(key));
        
        /// <summary>
        /// Check if a key was removed during this frame.
        /// Keys are automatically removed if they are not added in the same frame.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>True if removed during this frame.</returns>
        public bool IsRemoved(PropertyName key) => _removedKeyList.Contains(key);
        
        /// <summary>
        /// Check if a key was removed during this frame.
        /// Keys are automatically removed if they are not added in the same frame.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>True if removed during this frame.</returns>
        public bool IsRemoved(string key) => IsRemoved(new PropertyName(key));
        
        /// <summary>
        /// Check if a key was added during this frame.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>True if added during this frame.</returns>
        public bool IsAdded(PropertyName key) => _addedKeyList.Contains(key);
        
        /// <summary>
        /// Check if a key was added during this frame.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>True if added during this frame.</returns>
        public bool IsAdded(string key) => IsAdded(new PropertyName(key));
        
        /// <summary>
        /// A list of currently active keys.
        /// </summary>
        public List<PropertyName> CurrentKeys => _currentKeyList;

        private readonly List<PropertyName> _keyList = new ();
        private readonly List<PropertyName> _currentKeyList = new ();
        private readonly List<PropertyName> _removedKeyList = new ();
        private readonly List<PropertyName> _addedKeyList = new ();

        private Animator _animator;
        private readonly Dictionary<int, List<AnimatorModifierBehaviour>> _behaviours = new();
        
        private void Awake()
        {
            TryGetComponent(out _animator);
            AnimationModifierSystem.Add(this, _updateMode == UpdateMode.FixedUpdate);
        }

        private void OnDestroy()
        {
            AnimationModifierSystem.Remove(this, _updateMode == UpdateMode.FixedUpdate);
        }
        
        private void CopyKeyList()
        {
            _currentKeyList.Clear();
            _currentKeyList.AddRange(_keyList);
        }
        
        void IAnimationModifierUpdate.OnUpdate()
        {
            // Delete all key information to reset the content.
            _removedKeyList.Clear();
            _addedKeyList.Clear();
            _keyList.Clear();
            ChangeKey = false;
            
            AddCurrentKeyList(_animator.GetCurrentAnimatorStateInfo(0));
            AddCurrentKeyList(_animator.GetNextAnimatorStateInfo(0));

            // Compare the previous frame's key information with the current frame's key information,
            // and if there are any keys that were not present in the previous frame, add them to the AddedKeyList.
            foreach (var key in _keyList)
            {
                if (_currentKeyList.Contains(key)) 
                    continue;
                
                ChangeKey = true;
                _addedKeyList.Add(key);
            }

            // If a key from the previous frame is not included in the current key list when compared,
            // it will be added to the RemoveKeyList.
            foreach (var key in _currentKeyList)
            {
                if (_keyList.Contains(key)) 
                    continue;
                
                ChangeKey = true;
                _removedKeyList.Add(key);
            }

            // If a key is added or removed, update the CurrentList and call OnChangeKey.
            if (ChangeKey)
            {
                CopyKeyList();
                OnChangeKey?.Invoke();
            }
        }

        private void AddCurrentKeyList(AnimatorStateInfo stateInfo)
        {
            var hash = stateInfo.fullPathHash;

            if (_behaviours.ContainsKey(hash) == false)
                CacheBehaviour(hash);

            // Register key list.
            foreach (var behaviour in _behaviours[hash])
            {
                if( behaviour.IsInRange(stateInfo))
                    _keyList.Add(behaviour.Key);
            }
        }

        private void CacheBehaviour(int hash)
        {
            var behaviours = new List<AnimatorModifierBehaviour>();
            foreach (var behaviour in _animator.GetBehaviours(hash, 0))
            {
                if (behaviour is AnimatorModifierBehaviour animationModifierBehaviour)
                    behaviours.Add(animationModifierBehaviour);
            }
            _behaviours.Add(hash, behaviours);
        }

        private enum UpdateMode
        {
            Update = 0,
            FixedUpdate = 1
        }
    }
}
