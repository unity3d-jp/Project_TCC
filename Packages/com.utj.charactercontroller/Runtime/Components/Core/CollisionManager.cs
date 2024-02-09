using System.Collections.Generic;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    internal class CollisionManager
    {
        private readonly List<Collider> _ownColliders = new();        // Own collider (collider to be ignored when determining movement)

        public void Initialize(GameObject obj)
        {
            obj.GetComponents(_ownColliders);

            IgnoreChildColliders(obj, true);
        }
        
        /// <summary>
        /// To avoid contact between the Collider set on the parent object and the child Collider,
        /// To prevent interference with the child Collider.
        /// Without this process, the character could rise infinitely in contact with its own Collider.
        /// </summary>
        public void IgnoreChildColliders(GameObject obj, bool isIgnore)
        {
            var childColliders = obj.GetComponentsInChildren<Collider>();
            foreach (var ownCollider in _ownColliders)
            {
                foreach (var col in childColliders)
                {
                    if( _ownColliders.Contains(col))
                        continue;
                
                    Physics.IgnoreCollision(col, ownCollider, isIgnore);
                }
            }
        }
    }
}