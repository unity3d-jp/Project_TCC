namespace Unity.TinyCharacterController.Interfaces.Utility
{
    public interface IEditorProcess
    {
        /// <summary>
        /// Process to execute before building.
        /// </summary>
        void Execute();
    
        /// <summary>
        /// Determines whether to remove the component after the build.
        /// </summary>
        bool RemoveComponentAfterBuild { get; }
    }    
}