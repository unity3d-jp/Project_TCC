namespace Unity.TinyCharacterController.Interfaces.Core
{
    /// <summary>
    /// Interface for executing components at the EarlyUpdateBrain timing.
    /// If the Brain operates in FixedUpdate, it operates before FixedUpdate.
    /// if it operates in Update, it operates before Update.
    /// </summary>
    public interface IEarlyUpdateComponent
    {
        /// <summary>
        /// Component update process.
        /// </summary>
        /// <param name="deltaTime"></param>
        void OnUpdate(float deltaTime);

        /// <summary>
        /// Component update order.
        /// </summary>
        int Order { get; }
    }
}