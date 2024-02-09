namespace Unity.TinyCharacterController.Interfaces.Components
{
    /// <summary>
    /// Interface that provides callbacks to interrupt Animator processing.
    /// It allows checking the timing of component execution and interrupting at any desired timing.
    /// </summary>
    public interface IAnimationModifierUpdate 
    {
        /// <summary>
        /// Callback to interrupt after Animator's Transform calculation.
        /// </summary>
        void OnUpdate();
    }
}