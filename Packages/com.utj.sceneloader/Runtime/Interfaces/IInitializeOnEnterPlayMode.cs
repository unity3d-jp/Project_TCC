namespace Unity.SceneManagement
{
    /// <summary>
    /// Interface to perform processing in EnterPlayMode.
    /// <see cref="SceneInitialization"/>
    /// </summary>
    internal interface IInitializeOnEnterPlayMode
    {
        void OnEnterPlayMode();
    }
}