namespace Unity.TinyCharacterController.Interfaces.Core
{
    /// <summary>
    ///  This interface defines the lifecycle callbacks for a component with respect to its priority status.
    /// </summary>
    /// <typeparam name="T">ITurn or IMove</typeparam>
    public interface IPriorityLifecycle<T>
    {
        /// <summary>
        /// Callback called during regular updates while having the highest priority.
        /// </summary>
        /// <param name="deltaTime">Elapsed time</param>
        void OnUpdateWithHighestPriority(float deltaTime);

        /// <summary>
        /// Callback called when the highest priority is acquired.
        /// </summary>
        void OnAcquireHighestPriority();

        /// <summary>
        /// Callback called when the highest priority is lost.
        /// </summary>
        void OnLoseHighestPriority();
    }

}