using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.SceneManagement
{
    /// <summary>
    /// Manages scene loading and unloading operations in a queue for SceneLoaderManager
    /// </summary>
    internal class SceneLoaderTaskQueueManager : IDisposable
    {
        /// <summary>
        /// Commands for scenes associated with the specified AssetReferenceScene
        /// </summary>
        private readonly Dictionary<AssetReferenceScene, SceneLoadTask> _sceneLoadTasks = new();

        /// <summary>
        /// Retrieves an action. Adds it if it doesn't exist.
        /// </summary>
        /// <param name="scene">SceneReference</param>
        /// <returns>Action</returns>
        public SceneLoadTask GetOrCreateSceneLoadTask(AssetReferenceScene scene)
        {
            if (_sceneLoadTasks.TryGetValue(scene, out var act))
                return act;

            act = new SceneLoadTask();
            _sceneLoadTasks.Add(scene, act);
            return act;
        }

        /// <summary>
        /// Releases an action
        /// </summary>
        public void Dispose()
        {
            foreach(var queue in _sceneLoadTasks)
                queue.Value.Dispose();
            _sceneLoadTasks.Clear();
        }
    }
    
    /// <summary>
    /// Collects actions and executes them sequentially.
    /// Expected actions are only of two types: "Load" and "Unload".
    /// If commands are duplicated, the command is canceled.
    /// (For example, if it's "1. Load", "2. Unload", "3. Load", then "2. Unload" and "3. Load" are canceled)
    /// </summary>
    internal class SceneLoadTask : IDisposable
    {
        private readonly Queue<Action> _actionQueue = new();
        private bool _isQueueProcessing = false;
        private Task _task;
        
        /// <summary>
        /// Stores an action in the queue.
        /// Executes immediately if there are no actions in the queue.
        /// </summary>
        /// <param name="action">Action to add</param>
        public void EnqueueAction(Action action)
        {
            _actionQueue.Enqueue(action);

            if (_actionQueue.Count > 1)
                _actionQueue.Clear();
            
            if (_isQueueProcessing) 
                return;
            
            _isQueueProcessing = true;
            ProcessQueue();
        }

        /// <summary>
        /// If there is a queue, starts the action registered in the queue.
        /// Stops the process if there isn't.
        /// This method is intended to be called at the end of an action.
        /// </summary>
        public void ExecuteNextAction()
        {
#if UNITY_EDITOR
            // Interrupts the process when the game playback is stopped
            if (UnityEditor.EditorApplication.isPlaying == false)
            {
                Dispose();
                return;
            }
#endif
            
            if (_actionQueue.Count > 0)
            {
                _task = ProcessQueueNextFrame();
            }
            else
            {
                _isQueueProcessing = false;
            }
        }

        /// <summary>
        /// Executes an action with a one-frame delay.
        /// To address cases where Addressables are not released after scene unloading
        /// </summary>
        private async Task ProcessQueueNextFrame()
        {
            await Awaitable.NextFrameAsync();
            ProcessQueue();
        }

        /// <summary>
        /// Executes an action immediately
        /// </summary>
        private void ProcessQueue()
        {
            var action = _actionQueue.Dequeue();
            action?.Invoke();
        }

        public void Dispose()
        {
            _task?.Dispose();
        }
    }
}
