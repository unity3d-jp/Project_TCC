namespace Unity.TinyCharacterController.Interfaces.Core
{
    /// <summary>
    /// Interface for updating the character's orientation.
    /// Updates the character's orientation using the orientation with the highest priority.
    /// </summary>
    public interface ITurn : IPriority<ITurn>
    {
        /// <summary>
        /// Orientation update speed.
        /// </summary>
        int TurnSpeed { get; }
        
        /// <summary>
        /// The final angle the character will face.
        /// </summary>
        float YawAngle { get; }
    }
}