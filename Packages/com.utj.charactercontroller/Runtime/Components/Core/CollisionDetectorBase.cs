using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Unity.TinyCharacterController.Components.Utility
{
    public class CollisionDetectorBase : ComponentBase
    {
        /// <summary>
        ///     Execution timing.
        /// </summary>
        [FormerlySerializedAs("_timing")] [Header("Update Settings")] [SerializeField] [ReadOnlyOnRuntime]
        protected UpdateTiming Timing = UpdateTiming.Update;
        
        /// <summary>
        /// Gets or sets the owner of the collision detection.
        /// Components do not collide with the Collider held by <see cref="_owner"/>.
        /// </summary>
        [Header("Hit Settings")] 
        [SerializeField] 
        private CharacterSettings _owner;

        /// <summary>
        ///     Layer to perform collision detection against.
        /// </summary>
        [FormerlySerializedAs("_cacheTargetType")] 
        [SerializeField] [ReadOnlyOnRuntime]
        protected CachingTarget CacheTargetType = CachingTarget.Collider;

        [FormerlySerializedAs("_hitLayer")] [SerializeField] [ReadOnlyOnRuntime]
        protected LayerMask HitLayer;

        /// <summary>
        ///     Tags to collide with.
        /// </summary>
        [FormerlySerializedAs("_hitTags")] [SerializeField] [ReadOnlyOnRuntime] [NonReorderable] [TagSelector]
        protected string[] HitTags;


        /// <summary>
        ///     Event called when a collision is detected.
        ///     Contains the same contents as <see cref="HitCollidersInThisFrame" /> as arguments.
        /// </summary>
        [FormerlySerializedAs("OnHitCollider")] [Header("Callback")] 
        public UnityEvent<List<GameObject>> OnHitObjects;
        
        /// <summary>
        ///     List of collided colliders.
        ///     The indices correspond to those in <see cref="HitObjects" />.
        /// </summary>
        protected readonly List<Collider> HitColliders = new();

        /// <summary>
        ///     Reset the component.
        /// </summary>
        protected virtual void Reset()
        {
            HitLayer = (LayerMask)(1 << LayerMask.NameToLayer("Default"));
            _owner = GetComponentInParent<CharacterSettings>();
        }

        /// <summary>
        ///     List of colliders hit during the current frame.
        /// </summary>
        public readonly List<Collider> HitCollidersInThisFrame = new();


        /// <summary>
        ///     List of colliders hit while the component is active.
        /// </summary>
        public readonly List<GameObject> HitObjects = new();

        /// <summary>
        ///     List of GameObjects hit during the current frame.
        ///     The type of objects stored may vary depending on the value of <see cref="CacheTargetType" />.
        /// </summary>
        [RenamedFrom("HitObjectInThisFrame")]
        public List<GameObject> HitObjectsInThisFrame { get; } = new();


        /// <summary>
        ///     True if there is contact with colliders in the current frame.
        ///     This information is updated at the beginning of the frame when the component is active.
        /// </summary>
        public bool IsHitInThisFrame => HitCollidersInThisFrame.Count > 0;

        /// <summary>
        ///     True if there is contact with colliders.
        ///     This information is updated when colliders become active.
        /// </summary>
        public bool IsHit => HitColliders.Count > 0;

        /// <summary>
        ///     Clear the cache of collision detection.
        /// </summary>
        public void ClearHitObjectsCache()
        {
            InitializeBufferOfCollidedCollision();
        }

        
        /// <summary>
        /// Gets or sets the owner of the collision detection.
        /// Components do not collide with the Collider held by <see cref="Owner"/>.
        /// </summary>
        [AllowsNull]
        public CharacterSettings Owner
        {
            get => _owner;
            set => _owner = value;
        }


        /// <summary>
        /// Initializes a list of colliders that this component has collided with.
        /// </summary>
        protected void InitializeBufferOfCollidedCollision()
        {
            HitColliders.Clear();
            HitObjects.Clear();
            HitObjectsInThisFrame.Clear();
            HitCollidersInThisFrame.Clear();
        }

        /// <summary>
        ///     Retrieves the Collider associated with the object contained in <see cref="HitObjects" />.
        /// </summary>
        /// <param name="obj">The previously encountered GameObject.</param>
        /// <returns>The Collider used when <see cref="obj" /> was detected, or null if it doesn't exist.</returns>
        public Collider GetColliderForGameObject(GameObject obj)
        {
            // If obj exists within HitObjects, return the corresponding Collider.
            // Otherwise, return null.
            var index = HitObjects.IndexOf(obj);
            return index == -1 ? null : HitCollidersInThisFrame[index];
        }


        /// <summary>
        ///     Get the cached object based on the <see cref="CacheTargetType" />.
        ///     If it's <see cref="CachingTarget.RootObject" />, it returns the root object of the collision target, typically
        ///     assumed to be a character.
        ///     If it's <see cref="CachingTarget.Rigidbody" />, it returns the object with a Rigidbody. If no Rigidbody is present,
        ///     it returns the Collider.
        ///     If it's <see cref="CachingTarget.Collider" />, it returns the collided Collider.
        /// </summary>
        /// <param name="hit">Hit Collider</param>
        /// <returns>target game object</returns>
        protected GameObject GetHitObject(Collider hit)
        {
            switch (CacheTargetType)
            {
                case CachingTarget.RootObject:
                    // Returns the root object of the collision target.
                    return hit.transform.root.gameObject;
                case CachingTarget.Rigidbody:
                {
                    // If Rigidbody is null, return the current Collider.
                    var attachedRigidbody = hit.attachedRigidbody;
                    return attachedRigidbody != null ? attachedRigidbody.gameObject : hit.gameObject;
                }
                case CachingTarget.CharacterController:
                {
                    // If the parent object of the collided target has a CharacterController, return the object with the CharacterController attached.
                    var controller = hit.transform.GetComponentInParent<CharacterController>();
                    return controller != null ? controller.gameObject : hit.gameObject;
                }
                case CachingTarget.RigidbodyOrCharacterController:
                {
                    // If the parent object has a Rigidbody, return the object with the Rigidbody attached.
                    var attachedRigidbody = hit.attachedRigidbody;
                    if (attachedRigidbody != null)
                        return attachedRigidbody.gameObject;

                    // If the parent object has a CharacterController, return the object with the CharacterController attached.
                    var controller = hit.transform.GetComponentInParent<CharacterController>();
                    if (controller != null)
                        return controller.gameObject;

                    // If none of the above conditions are met, return the current Collider.
                    return hit.gameObject;
                }
                default:
                    // Returns the GameObject of the collided Collider.
                    return hit.gameObject;
            }
        }


        /// <summary>
        ///     Enumeration of caching target types.
        ///     Caches objects under specific conditions to avoid multiple hits.
        /// </summary>
        protected enum CachingTarget
        {
            RootObject = 0,                     // Cache the root object
            Collider = 1,                       // Cache colliders
            Rigidbody = 2,                      // Cache objects with Rigidbody
            CharacterController = 3,            // cache object with character controller.
            RigidbodyOrCharacterController = 4  // Rigidbody or Character controller.
        }
    }
}