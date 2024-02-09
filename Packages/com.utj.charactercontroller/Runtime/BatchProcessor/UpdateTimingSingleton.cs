using UnityEngine;
using UnityEngine.Playables;

namespace Unity.TinyCharacterController.Core
{
    /// <summary>
    /// Timing of updates.
    /// </summary>
    public enum UpdateTiming : int
    {
        Update = 0,
        FixedUpdate = 1,
        LateUpdate = 2,
    }
    
    /// <summary>
    /// </summary>
    /// <typeparam name="TSystem">The class that becomes a singleton.</typeparam>
    public abstract class UpdateTimingSingleton<TSystem> : ScriptableObject
        where TSystem : UpdateTimingSingleton<TSystem>
    {
        /// <summary>
        /// To support FixedUpdate and Update, it holds multiple instances.
        /// It should have the same size as <see cref="UpdateTiming"/>.
        /// </summary>
        private static readonly TSystem[] Instance = new TSystem[3];

        /// <summary>
        /// Check if an object exists.
        /// Unlike GetInstance, this does not create an instance.
        /// </summary>
        /// <param name="timing">Timing of update</param>
        /// <returns>True if an instance has been created</returns>
        public static bool IsCreated(UpdateTiming timing) => Instance[(int)timing] != null;
        
        
        /// <summary>
        /// Get the instance. If the instance does not exist, it will be created.
        /// </summary>
        /// <param name="timing">Timing of update</param>
        /// <returns>The instance</returns>
        public static TSystem GetInstance(UpdateTiming timing)
        {
            var index = (int)timing;
            if (IsCreated(timing))
                return Instance[index];

            var instance = CreateInstance<TSystem>();
            instance.Timing = timing;
            instance.OnCreate(timing);
            Application.quitting += instance.OnQuit;

            Instance[index] = instance;

            return instance;
        }
        
        /// <summary>
        /// Execution timing.
        /// </summary>
        public UpdateTiming Timing { get; private set; }

        /// <summary>
        /// Callback called when an instance is created.
        /// This is used to avoid interference with subclasses when implemented in Awake.
        /// It is called after Awake.
        /// </summary>
        protected virtual void OnCreate(UpdateTiming timing) { }

        /// <summary>
        /// Destroy the instance when the application is quitting.
        /// This is to handle EnterPlayMode.
        /// </summary>
        private void OnQuit()
        {
            Application.quitting -= OnQuit;
            
            DestroyImmediate(this);
        }
    }
}
