using System.Collections.Generic;

namespace Unity.TinyCharacterController.Interfaces.Core
{
    /// <summary>
    /// Interface for updating components.
    /// To execute this component, an object inheriting from <see cref="TinyCharacterController.Core.BrainBase"/> must exist on the same object.
    /// </summary>
    public interface IUpdateComponent 
    {
        /// <summary>
        /// Component update process.
        /// </summary>
        /// <param name="deltaTime"></param>
        void OnUpdate(float deltaTime);

        /// <summary>
        /// Order of component updates.
        /// </summary>
        int Order { get; }
    }
}