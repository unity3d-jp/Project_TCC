using System;
using System.Collections.Generic;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Subsystem;
using Unity.TinyCharacterController.Utility;
using UnityEngine.Pool;

namespace Unity.TinyCharacterController.Manager
{
    /// <summary>
    /// System for calculating the lifetime of Pooled Game Objects.
    /// </summary>
    public class PooledGameObjectSystem : SystemBase<PooledGameObject, PooledGameObjectSystem>, IEarlyUpdate
    {
        int ISystemBase.Order => 0;

        private void OnDestroy()
        {
            UnregisterAllComponents();
        }

        void IEarlyUpdate.OnUpdate()
        {
            var willReleaseObjects = ListPool<PooledGameObject>.Get();

            // Collect objects to be released, trigger events, and then release them.
            GetReleaseObjects(ref willReleaseObjects);
            InvokeEvents(willReleaseObjects);
            ReleasePooledObjects(willReleaseObjects);

            ListPool<PooledGameObject>.Release(willReleaseObjects);
        }

        /// <summary>
        /// Release the elements.
        /// </summary>
        /// <param name="willReleaseObjects">List of objects to be released.</param>
        private static void ReleasePooledObjects(in List<PooledGameObject> willReleaseObjects)
        {
            foreach (var obj in willReleaseObjects) obj.Release();
        }

        /// <summary>
        /// Invoke events associated with PooledGameObject.
        /// </summary>
        /// <param name="willReleaseObjects">List of objects to be released.</param>
        private static void InvokeEvents(in List<PooledGameObject> willReleaseObjects)
        {
            foreach (var obj in willReleaseObjects) obj.OnReleaseByLifeTime.Invoke();
        }

        /// <summary>
        /// Collect a list of objects to be released.
        /// </summary>
        /// <param name="willReleaseObjects">List of objects to be released.</param>
        private void GetReleaseObjects(ref List<PooledGameObject> willReleaseObjects)
        {
            foreach (var comp in Components)
            {
                if (comp.IsPlaying == false)
                    continue;

                willReleaseObjects.Add(comp);
            }
        }
    }
}
