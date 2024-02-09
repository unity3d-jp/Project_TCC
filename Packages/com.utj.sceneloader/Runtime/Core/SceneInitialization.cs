using System.Collections.Generic;
using UnityEditor;

namespace Unity.SceneManagement
{
    /// <summary>
    /// Automatically initialize registered SceneLoader-related classes
    /// </summary>
    internal class SceneInitialization
    {
#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode()
        {
            Instance.Initialize();
        }
#endif
        
        public static SceneInitialization Instance { get; } = new();
        
        private readonly List<IInitializeOnEnterPlayMode> _initializes = new();

        public void Register(IInitializeOnEnterPlayMode initializer)
        {
            _initializes.Add(initializer);
        }

        private void Initialize()
        {
            foreach( var obj in _initializes)
                obj.OnEnterPlayMode();
        }
    }
}