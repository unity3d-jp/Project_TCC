namespace Unity.TinyCharacterController.Interfaces.Core
{
    /// <summary>
    /// Interface for batch processing of components.
    /// Provides access to the ID of a component during batch processing and whether it is registered.
    /// </summary>
    public interface IComponentIndex
    {
        /// <summary>
        /// Index of the component during batch processing.
        /// </summary>
        int Index { get; set; }
        
        /// <summary>
        /// True if the component is active during batch processing.
        /// </summary>
        bool IsRegistered { get; }
    }
}