using UnityEngine;

namespace Unity.TinyCharacterController.Utility
{
    /// <summary>
    /// Creates a ScriptableObject as a singleton.
    /// </summary>
    /// <typeparam name="T">The class to be treated as a singleton</typeparam>
    public class Singleton<T> : ScriptableObject where T : Singleton<T>
    {
        private static T _instance;

        /// <summary>
        /// Returns True if the object has been created.
        /// Note: When accessing the component at a time when it might be destroyed, like OnDestroy, always check for the existence of the object.
        /// </summary>
        protected static bool IsCreated => _instance != null;
        
        /// <summary>
        /// Get the instance. If the instance has not been created, it will be instantiated and registered.
        /// </summary>
        protected static T Instance
        {
            get
            {
                if (_instance != null) 
                    return _instance;
                
                // create instance and register callback.
                // ScriptableObject is not automatically destroyed in EnterPlayMode,
                // so it is necessary to detect the end of the application and destroy the component.
                _instance = CreateInstance<T>();
                Application.quitting += _instance.OnQuit;

                return _instance;
            }
        }

        /// <summary>
        /// Callback for when the game is quitting.
        /// </summary>
        private void OnQuit()
        {
            Application.quitting -= OnQuit;
            Destroy(this);
        }
    }
}